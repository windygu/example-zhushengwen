namespace JrIntercepter.Net 
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Text;

    public class ServerChatter
    {
        internal bool _bWasForwarded;
        internal static int _cbServerReadBuffer = 0x8000;
        private int _iBodySeekProgress;
        private long _lngLastChunkInfoOffset;
        private int iEntityBodyOffset;
        private HTTPResponseHeaders _inHeaders;
        private MemoryStream m_responseData;
        private long m_responseTotalDataCount;
        private Session m_session;
        public ServerPipe ServerPipe;

        public HTTPResponseHeaders Headers
        {
            get
            {
                return this._inHeaders;
            }
            set
            {
                if (value != null)
                {
                    this._inHeaders = value;
                }
            }
        }


        internal ServerChatter(Session oSession)
        {
            this._lngLastChunkInfoOffset = -1L;
            this.m_session = oSession;
            this.m_responseData = new MemoryStream(0x4000);
        }

        internal ServerChatter(Session oSession, string sHeaders)
        {
            this._lngLastChunkInfoOffset = -1L;
            this.m_session = oSession;
            this._inHeaders = Parser.ParseResponse(sHeaders);
        }


        public void DetachServerPipe()
        {
            if (this.ServerPipe != null)
            {
                this.ServerPipe.End();  
                this.ServerPipe = null;
            }
        }


        internal static Socket CreateConnectedSocket(IPAddress[] destIPs, int port, Session session)
        {
            Socket socket = null;
            bool flag = false;
            Stopwatch stopwatch = Stopwatch.StartNew();
            Exception exception = null;
            foreach (IPAddress address in destIPs)
            {
                try
                {
                    socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
                    {
                        NoDelay = true
                    };
                    socket.Connect(address, port);
                    session.HostIP = address.ToString();
                    session.Flags["x-hostIP"] = session.HostIP;
                    flag = true;
                    break;
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    session.Flags["x-DNS-Failover"] = session.Flags["x-DNS-Failover"] + "+1";
                }
            }
            session.Timers.ServerConnected = DateTime.Now;
            session.Timers.TCPConnectTime = (int)stopwatch.ElapsedMilliseconds;
            if (!flag)
            {
                throw exception;
            }
            return socket;
        }


        private bool ConnectToHost()
        {
            string str2;
            string str3;
            IPAddress[] addressArray;
            /*
            string sHostAndPort = this.m_session.Flags["x-overrideHost"];
            if (sHostAndPort == null)
            {
                sHostAndPort = this.m_session.host;
            }*/
            string sHostAndPort = this.m_session.Host;  

            int iPort = this.m_session.IsHTTPS ? 0x1bb : (this.m_session.IsFTP ? 0x15 : 80);
            Utilities.CrackHostAndPort(sHostAndPort, out str2, ref iPort);
            
            str3 = (this.m_session.IsHTTPS ? "HTTPS:" : "") + str2 + ":" + iPort.ToString();

            int port = iPort;
    
          
            try
            {
                addressArray = DNSResolver.GetIPAddressList(str2, true, this.m_session.Timers);
            }
            catch (Exception exception)
            {
                this.m_session.Request.FailSession(0x1f6, "JrIntercepter - DNS Lookup Failed", "JrIntercepter: DNS Lookup for " 
                    + Utilities.HtmlEncode(str2) + " failed. " + exception.Message);
                return false;
            }
            if ((port < 0) || (port > 0xffff))
            {
                this.m_session.Request.FailSession(0x1f6, "Invalid Request", "HTTP Request specified an invalid port number.");
                return false;
            }
     
            try
            {
                this.ServerPipe = new ServerPipe("ServerPipe#" + this.m_session.id.ToString());
                Socket socket = CreateConnectedSocket(addressArray, port, this.m_session);
               
                this.ServerPipe.WrapSocketInPipe(socket);
              
                return true;
            }
            catch (Exception exception2)
            {
                this.m_session.Request.FailSession(
                    0x1f6,
                    "JrIntercepter - Connection Failed",
                    "[JrIntercepter] Connection to " + Utilities.HtmlEncode(str2) + 
                    " failed.<BR>Exception Text: " + exception2.Message);
             
                return false;
            }
        }

        internal bool ResendRequest()
        {
            bool b = this.ServerPipe != null;
            if (!this.ConnectToHost())
            {
                this.m_session.SetBitFlag(SessionFlags.ServerPipeReused, b);
                return false;
            }
            try
            {
                this.ServerPipe.IncrementUse(this.m_session.id);
                this.m_session.Timers.ServerConnected = this.ServerPipe.ConnectedTime;
               
                this.m_session.SetBitFlag(SessionFlags.ServerPipeReused, this.ServerPipe.UseCount > 1);

                // 如果不是中转站，继续向其它代理发送或者不是HTTPS安全连接，将 头信息里的 Proxy-Connection 换成 Connection    
                if (!this._bWasForwarded && !this.m_session.IsHTTPS)
                {
                    this.m_session.Request.Headers.RenameHeaderItems("Proxy-Connection", "Connection");
                }

                this.m_session.Timers.SelfBeginRequest = DateTime.Now;
                
                this.ServerPipe.Send(
                    this.m_session.Request.Headers.ToByteArray(true, true, this._bWasForwarded && !this.m_session.IsHTTPS)
                );
                this.ServerPipe.Send(this.m_session.RequestBodyBytes);
            }
            catch (Exception exception)
            {
                this.m_session.Request.FailSession(0x1f8, "JrIntercepter - Send Failure", "ResendRequest() failed: " + exception.Message);
                return false;
            }
            this.m_session.Flags["x-EgressPort"] = this.ServerPipe.LocalPort.ToString();
            if (this.m_session.Flags.ContainsKey("log-drop-request-body"))
            {
                this.m_session.Flags["x-RequestBodyLength"] = (this.m_session.RequestBodyBytes != null) ? this.m_session.RequestBodyBytes.Length.ToString() : "0";
                this.m_session.RequestBodyBytes = new byte[0];
            }
            return true;
        }


        private bool isResponseBodyComplete()
        {
            if (this.m_session.HTTPMethodIs("HEAD"))
            {
                return true;
            }
            if (this.m_session.HTTPMethodIs("CONNECT") && (this._inHeaders.HTTPResponseCode == 200))
            {
                return true;
            }
            // 0XCC=204(OK 但无内容返回) 205  0X130=304(文档未作修改，使用本地缓存)  
            if (((this._inHeaders.HTTPResponseCode == 0xcc) || (this._inHeaders.HTTPResponseCode == 0xcd)) || (this._inHeaders.HTTPResponseCode == 0x130))
            {
                if (this._inHeaders.Exists("Content-Length") && ("0" != this._inHeaders["Content-Length"].Trim()))
                {
                    // 仅提示   
                    return true;
                }
                return true;
            }
            if (this._inHeaders.ExistsAndEquals("Transfer-Encoding", "chunked"))
            {
                long num;
                if (this._lngLastChunkInfoOffset < this.iEntityBodyOffset)
                {
                    this._lngLastChunkInfoOffset = this.iEntityBodyOffset;
                }
                return Utilities.IsChunkedBodyComplete(this.m_session, this.m_responseData, this._lngLastChunkInfoOffset, out this._lngLastChunkInfoOffset, out num);
            }
            if (this._inHeaders.Exists("Content-Length"))
            {
                long num2;
                if (long.TryParse(this._inHeaders["Content-Length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out num2) && (num2 >= 0L))
                {
                    return (this.m_responseTotalDataCount >= (this.iEntityBodyOffset + num2));
                }
                return true;
            }
            if (
                (
                    !this._inHeaders.ExistsAndEquals("Connection", "close") 
                    && !this._inHeaders.ExistsAndEquals("Proxy-Connection", "close")
                ) 
                && (
                    (this._inHeaders.HTTPVersion != "HTTP/1.0") 
                    || this._inHeaders.ExistsAndContains("Connection", "Keep-Alive")
               )
            )
            {
            }
            return false;
        }


        private bool HeadersAvailable()
        {
            if (this.iEntityBodyOffset <= 0)
            {
                HTTPHeaderParseWarnings warnings;
                if (this.m_responseData == null)
                {
                    return false;
                }
                if (!Parser.FindEndOfHeaders(this.m_responseData.GetBuffer(), ref this._iBodySeekProgress, this.m_responseData.Length, out warnings))
                {
                    return false;
                }
                this.iEntityBodyOffset = this._iBodySeekProgress + 1;
                switch (warnings)
                {
                    case HTTPHeaderParseWarnings.EndedWithLFLF:
                        break;

                    case HTTPHeaderParseWarnings.EndedWithLFCRLF:
                        break;
                }
            }
            return true;
        }


        private bool ParseResponseForHeaders()
        {
            if ((this.m_responseData != null) && (this.iEntityBodyOffset >= 4))
            {
                this._inHeaders = new HTTPResponseHeaders(Config.HeaderEncoding);
                byte[] bytes = this.m_responseData.GetBuffer();
                string str = Config.HeaderEncoding.GetString(bytes, 0, this.iEntityBodyOffset).Trim();
                if ((str == null) || (str.Length < 1))
                {
                    this._inHeaders = null;
                    return false;
                }
                string[] sHeaderLines = str.Replace("\r\n", "\n").Split(new char[] { '\n' });
                if (sHeaderLines.Length >= 1)
                {
                    int index = sHeaderLines[0].IndexOf(' ');
                    if (index > 0)
                    {
                        this._inHeaders.HTTPVersion = sHeaderLines[0].Substring(0, index).ToUpper();
                        sHeaderLines[0] = sHeaderLines[0].Substring(index + 1).Trim();
                        if (!this._inHeaders.HTTPVersion.StartsWith("HTTP/", StringComparison.OrdinalIgnoreCase))
                        {
                            return false;
                        }
                        this._inHeaders.HTTPResponseStatus = sHeaderLines[0];
                        bool flag = false;
                        index = sHeaderLines[0].IndexOf(' ');
                        if (index > 0)
                        {
                            flag = int.TryParse(sHeaderLines[0].Substring(0, index).Trim(), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out this._inHeaders.HTTPResponseCode);
                        }
                        else
                        {
                            flag = int.TryParse(sHeaderLines[0].Trim(), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out this._inHeaders.HTTPResponseCode);
                        }
                        if (!flag)
                        {
                           return false;
                        }
                        string sErrors = string.Empty;
                        if (!Parser.ParseNVPHeaders(this._inHeaders, sHeaderLines, 1, ref sErrors))
                        {
                        }
                        return true;
                    }
                }
            }
            return false;
        }


        private bool GetHeaders()
        {
            if (!this.HeadersAvailable())
            {
                return false;
            }
            if (!this.ParseResponseForHeaders())
            {
                string str;
                this.m_session.SetBitFlag(SessionFlags.ProtocolViolationInResponse, true);
                // this.PoisonPipe();
                if (this.m_responseData != null)
                {
                    str = "<plaintext>\n" + 
                        Utilities.ByteArrayToHexView(
                            this.m_responseData.GetBuffer(), 
                            0x18, 
                            (int)Math.Min(this.m_responseData.Length, 0x800L)
                        );
                }
                else
                {
                    str = "{JrIntercepter:no data}";
                }
                this.m_session.Request.FailSession(
                    500,
                    "JrIntercepter - Bad Response",
                    string.Format("[JrIntercepter] Response Header parsing failed.\n{0}Response Data:\n{1}", 
                    this.m_session.isFlagSet(SessionFlags.ServerPipeReused) 
                        ? "This can be caused by an illegal HTTP response earlier on this reused server socket-- for instance, a HTTP/304 response which illegally contains a body.\n" 
                        : string.Empty, 
                    str));
                return true;
            }
            if ((this._inHeaders.HTTPResponseCode <= 0x63) || (this._inHeaders.HTTPResponseCode >= 200))
            {
                return true;
            }
            if (this._inHeaders.Exists("Content-Length") && ("0" != this._inHeaders["Content-Length"].Trim()))
            {
            }
            
            this.deleteInformationalMessage();
            return this.GetHeaders();
        }

        internal bool ReadResponse()
        {
            int iMaxByteCount = 0;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            byte[] arrBuffer = new byte[_cbServerReadBuffer];
            do
            {
                try
                {
                    iMaxByteCount = this.ServerPipe.Receive(arrBuffer);
                    if (0L == this.m_session.Timers.ServerBeginResponse.Ticks)
                    {
                        this.m_session.Timers.ServerBeginResponse = DateTime.Now;
                    }
                    if (iMaxByteCount <= 0)
                    {
                        flag = true;
                    }
                    else
                    {
                        this.m_responseData.Write(arrBuffer, 0, iMaxByteCount);
                        this.m_responseTotalDataCount += iMaxByteCount;
                        if ((this._inHeaders == null) && this.GetHeaders())
                        {
                            if ((this.m_session.State == SessionStates.Aborted)
                                && this.m_session.isAnyFlagSet(SessionFlags.ProtocolViolationInResponse))
                            {
                                return false;
                            }

                        }
                    }
                }
                catch (Exception exception)
                {
                    flag2 = true;
                    if (exception is OperationCanceledException)
                    {
                        this.m_session.State = SessionStates.Aborted;
                    }
                    else if (exception is OutOfMemoryException)
                    {
                        this.m_session.State = SessionStates.Aborted;
                    }
                    else
                    {
                    }
                }
            }
            while ((!flag && !flag2) && ((this._inHeaders == null) || !this.isResponseBodyComplete()));
            // isResponseBodyComplete 是一个很关键的方法，得详细研究清楚。
   
            this.m_session.Timers.ServerDoneResponse = DateTime.Now;
           
            if ((this.m_responseTotalDataCount == 0L) && (this._inHeaders == null))
            {
                flag2 = true;
            }

            arrBuffer = null;
            if (flag2)
            {
                this.m_responseData.Dispose();
                this.m_responseData = null;
                return false;
            }
            if (this._inHeaders == null)
            {
                this.m_session.SetBitFlag(SessionFlags.ResponseStreamed, false);
                this._inHeaders = new HTTPResponseHeaders(Config.HeaderEncoding);
                this._inHeaders.HTTPVersion = "HTTP/1.0";
                this._inHeaders.HTTPResponseCode = 200;
                this._inHeaders.HTTPResponseStatus = "200 This buggy server did not return headers";
                this.iEntityBodyOffset = 0;
                return true;
            }
            return true;
        }


        private void deleteInformationalMessage()
        {
            this._inHeaders = null;
            byte[] buffer = new byte[this.m_responseData.Length - this.iEntityBodyOffset];
            this.m_responseData.Position = this.iEntityBodyOffset;
            this.m_responseData.Read(buffer, 0, buffer.Length);
            this.m_responseData.Dispose();
            this.m_responseData = new MemoryStream(buffer.Length);
            this.m_responseData.Write(buffer, 0, buffer.Length);
            this.m_responseTotalDataCount = 0L;
            this.iEntityBodyOffset = this._iBodySeekProgress = 0;
        }

        internal void FreeResponseDataBuffer()
        {
            if (this.m_responseData != null)
            {
                this.m_responseData.Dispose();
                this.m_responseData = null;
            }
        }

        internal byte[] TakeEntity()
        {
            byte[] bytes;
            try
            {
                bytes = new byte[this.m_responseData.Length - this.iEntityBodyOffset];
                this.m_responseData.Position = this.iEntityBodyOffset;
                this.m_responseData.Read(bytes, 0, bytes.Length);
            }
            catch (OutOfMemoryException exception)
            {
                bytes = Encoding.ASCII.GetBytes("JrIntercepter: Out of memory");
                // this.ReleaseServerPipe();
            }
            catch (Exception ex) {
                bytes = Encoding.ASCII.GetBytes(ex.Message);  
                // this.m_session.PoisonServerPipe();
                // this.ReleaseServerPipe();
            }  
            this.FreeResponseDataBuffer();
            return bytes;
        }


        internal void ReleaseServerPipe()
        {
            if (this.ServerPipe != null)
            {
                this.DetachServerPipe();
            }
        }


    }
}

