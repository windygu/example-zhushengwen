namespace JrIntercepter.Net 
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;

    public class ClientChatter
    {
        internal static int _cbClientReadBuffer = 0x2000;
        private int iBodySeekProgress;
        private int iEntityBodyOffset;
        private HTTPRequestHeaders m_headers;
        private MemoryStream m_requestData;
        private Session m_session;
        private string m_sHostFromURI;
        public ClientPipe ClientPipe;

        internal ClientChatter(Session oSession)
        {
            this.m_session = oSession;
        }

        internal ClientChatter(Session oSession, string sData)
        {
            this.m_session = oSession;
            this.Headers = Parser.ParseRequest(sData);
            if ((this.Headers != null) && ("CONNECT" == this.m_headers.HTTPMethod))
            {
                this.m_session.isTunnel = true;
            }
        }

        private long calculateExpectedEntityTransferSize()
        {
            long result = 0L;
            if (this.m_headers.ExistsAndEquals("Transfer-encoding", "chunked"))
            {
                long num2;
                long num3;
                if (!Utilities.IsChunkedBodyComplete(this.m_session, this.m_requestData, (long) this.iEntityBodyOffset, out num3, out num2))
                {
                    throw new InvalidDataException("Bad request: Chunked Body was incomplete.");
                }
                if (num2 < this.iEntityBodyOffset)
                {
                    throw new InvalidDataException("Chunked Request Body was malformed. Entity ends before it starts!");
                }
                return (num2 - this.iEntityBodyOffset);
            }
            if (long.TryParse(this.m_headers["Content-Length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result) && (result > -1L))
            {
                return result;
            }
            return result;
        }

        private void freeRequestData()
        {
            if (this.m_requestData != null)
            {
                this.m_requestData.Dispose();
                this.m_requestData = null;
            }
        }

        public void FailSession(int iError, string sErrorStatusText, string sErrorBody)
        {
            this.m_session.SetBitFlag(SessionFlags.ResponseGeneratedByJI, true);
            if ((iError >= 400) && (sErrorBody.Length < 0x200))
            {
                sErrorBody = sErrorBody.PadRight(0x200, ' ');
            }
            this.m_session.ResponseBodyBytes = Encoding.UTF8.GetBytes(sErrorBody);
            this.m_session.Response.Headers = new HTTPResponseHeaders(Config.HeaderEncoding);
            this.m_session.Response.Headers.HTTPResponseCode = iError;
            this.m_session.Response.Headers.HTTPResponseStatus = iError.ToString() + " " + sErrorStatusText;
            this.m_session.Response.Headers.Add("Content-Type", "text/html; charset=UTF-8");
            this.m_session.Response.Headers.Add("Connection", "close");
            this.m_session.Response.Headers.Add("Timestamp", DateTime.Now.ToString("HH:mm:ss.fff"));
            this.m_session.State = SessionStates.Aborted;
            this.m_session.ReturnResponse();
        }
       
        private bool HeadersAvailable()
        {
            if (this.m_requestData.Length >= 0x10L)
            {
                HTTPHeaderParseWarnings warnings;
                byte[] arrData = this.m_requestData.GetBuffer();
                long length = this.m_requestData.Length;
                if (Parser.FindEndOfHeaders(arrData, ref this.iBodySeekProgress, length, out warnings))
                {
                    this.iEntityBodyOffset = this.iBodySeekProgress + 1;
                    return true;
                }
            }
            return false;
        }

        private bool isRequestComplete()
        {
            if (this.m_headers == null)
            {
                if (!this.HeadersAvailable())
                {
                    return false;
                }
                if (!this.ParseRequestForHeaders())
                {
                    string str;
                    if (this.m_requestData != null)
                    {
                        str = Utilities.ByteArrayToHexView(this.m_requestData.GetBuffer(), 0x18, (int) Math.Min(this.m_requestData.Length, 0x800L));
                    }
                    else
                    {
                        str = "{JrIntercepter:no data}";
                    }
                    if (this.m_headers == null)
                    {
                        this.m_headers = new HTTPRequestHeaders();
                        this.m_headers.HTTPMethod = "BAD";
                        this.m_headers["Host"] = "BAD-REQUEST";
                        this.m_headers.RequestPath = "/BAD_REQUEST";
                    }
                    this.FailSession(400, "JrIntercepter - Bad Request", "[JrIntercepter] Request Header parsing failed. Request was:\n" + str);
                    return true;
                }
                this.m_session.AssignID();     
            }
            if (this.m_headers.ExistsAndEquals("Transfer-encoding", "chunked"))
            {
                long num;
                long num2;
                return Utilities.IsChunkedBodyComplete(this.m_session, this.m_requestData, (long) this.iEntityBodyOffset, out num2, out num);
            }
            if (this.m_headers.Exists("Content-Length"))
            {
                long result = 0L;
                try
                {
                    if (!long.TryParse(this.m_headers["Content-Length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result) || (result < 0L))
                    {
                        this.FailSession(400, "JrIntercepter - Bad Request", "[JrIntercepter] Request Content-Length header parsing failed.\nContent-Length: " + this.m_headers["Content-Length"]);
                        return true;
                    }
                    return (this.m_requestData.Length >= (this.iEntityBodyOffset + result));
                }
                catch
                {
                    this.FailSession(400, "JrIntercepter - Bad Request", "[JrIntercepter] Unknown error: Check content length header?");
                    return false;
                }
            }
            return true;
        }

        private bool ParseRequestForHeaders()
        {
            int num;
            int num2;
            int num3;
            if ((this.m_requestData == null) || (this.iEntityBodyOffset < 4))
            {
                return false;
            }
            this.m_headers = new HTTPRequestHeaders(Config.HeaderEncoding);
            byte[] arrRequest = this.m_requestData.GetBuffer();
            Parser.CrackRequestLine(arrRequest, out num2, out num3, out num);
            if ((num2 < 1) || (num3 < 1))
            {
                return false;
            }
            this.m_headers.HTTPMethod = Encoding.ASCII.GetString(arrRequest, 0, num2 - 1).ToUpper();
            this.m_headers.HTTPVersion = Encoding.ASCII.GetString(arrRequest, (num2 + num3) + 1, ((num - num3) - num2) - 2).Trim().ToUpper();
            int num4 = 0;
            if (arrRequest[num2] != 0x2f)
            {
                if (((num3 > 7) && (arrRequest[num2 + 4] == 0x3a)) && ((arrRequest[num2 + 5] == 0x2f) && (arrRequest[num2 + 6] == 0x2f)))
                {
                    this.m_headers.UriScheme = Encoding.ASCII.GetString(arrRequest, num2, 4);
                    num4 = num2 + 6;
                    num2 += 7;
                    num3 -= 7;
                }
                else if (((num3 > 8) && (arrRequest[num2 + 5] == 0x3a)) && ((arrRequest[num2 + 6] == 0x2f) && (arrRequest[num2 + 7] == 0x2f)))
                {
                    this.m_headers.UriScheme = Encoding.ASCII.GetString(arrRequest, num2, 5);
                    num4 = num2 + 7;
                    num2 += 8;
                    num3 -= 8;
                }
                else if (((num3 > 6) && (arrRequest[num2 + 3] == 0x3a)) && ((arrRequest[num2 + 4] == 0x2f) && (arrRequest[num2 + 5] == 0x2f)))
                {
                    this.m_headers.UriScheme = Encoding.ASCII.GetString(arrRequest, num2, 3);
                    num4 = num2 + 5;
                    num2 += 6;
                    num3 -= 6;
                }
            }
            if (num4 == 0)
            {
                if ((this.ClientPipe != null) && this.ClientPipe.IsSecured)
                {
                    this.m_headers.UriScheme = "https";
                }
                else
                {
                    this.m_headers.UriScheme = "http";
                }
            }
            if (num4 > 0)
            {
                while (((num3 > 0) && (arrRequest[num2] != 0x2f)) && (arrRequest[num2] != 0x3f))
                {
                    num2++;
                    num3--;
                }
                if (num3 == 0)
                {
                    num2 = num4;
                    num3 = 1;
                }
                int index = num4 + 1;
                int count = num2 - index;
                if (count > 0)
                {
                    this.m_sHostFromURI = Config.HeaderEncoding.GetString(arrRequest, index, count);
                    if ((this.m_headers.UriScheme == "ftp") && this.m_sHostFromURI.Contains("@"))
                    {
                        int length = this.m_sHostFromURI.LastIndexOf("@") + 1;
                        this.m_headers._uriUserInfo = this.m_sHostFromURI.Substring(0, length);
                        this.m_sHostFromURI = this.m_sHostFromURI.Substring(length);
                    }
                }
            }
            byte[] dst = new byte[num3];
            Buffer.BlockCopy(arrRequest, num2, dst, 0, num3);
            this.m_headers.RawPath = dst;
            string str = Config.HeaderEncoding.GetString(arrRequest, num, this.iEntityBodyOffset - num).Trim();
            arrRequest = null;
            if (str.Length >= 1)
            {
                string[] sHeaderLines = str.Replace("\r\n", "\n").Split(new char[] { '\n' });
                string sErrors = string.Empty;
                if (!Parser.ParseNVPHeaders(this.m_headers, sHeaderLines, 0, ref sErrors))
                {
                }
            }
            return true;
        }

        internal bool ReadRequest()
        {
            if (this.m_requestData != null)
            {
                return false;
            }
            if (this.ClientPipe == null)
            {
                return false;
            }
            this.m_requestData = new MemoryStream(0x1000);
            this.ClientPipe.IncrementUse(0);
            this.m_session.SetBitFlag(SessionFlags.ClientPipeReused, this.ClientPipe.UseCount > 1);
            this.ClientPipe.SetReceiveTimeout();
            int iMaxByteCount = 0;
            bool flag = false;
            bool flag2 = false;
            byte[] arrBuffer = new byte[_cbClientReadBuffer];
            do
            {
                try
                {
                    iMaxByteCount = this.ClientPipe.Receive(arrBuffer);
                }
                catch (Exception exception)
                {
                    flag = true;
                }
                if (iMaxByteCount <= 0)
                {
                    flag2 = true;
                }
                else
                {
                    if (this.m_requestData.Length == 0L)
                    {
                        this.m_session.Timers.ClientBeginRequest = DateTime.Now;
                        int index = 0;
                        while ((index < iMaxByteCount) && ((arrBuffer[index] == 13) || (arrBuffer[index] == 10)))
                        {
                            index++;
                        }
                        this.m_requestData.Write(arrBuffer, index, iMaxByteCount - index);
                    }
                    else
                    {
                        this.m_requestData.Write(arrBuffer, 0, iMaxByteCount);
                    }
                }
            }
            while ((!flag2 && !flag) && !this.isRequestComplete());
            arrBuffer = null;
            if (flag || (this.m_requestData.Length == 0L))
            {
                if ((this.ClientPipe.UseCount < 2) || (this.ClientPipe.IsSecured && (this.ClientPipe.UseCount < 3)))
                {
                }
                this.freeRequestData();
                return false;
            }
            if ((this.m_headers == null) || (this.m_session.State >= SessionStates.Done))
            {
                this.freeRequestData();
                return false;
            }
            if ("CONNECT" == this.m_headers.HTTPMethod)
            {
                this.m_session.isTunnel = true;
                this.m_sHostFromURI = this.m_session.PathAndQuery;
            }
            if (this.m_sHostFromURI != null)
            {
                if (this.m_headers.Exists("Host") 
                    && !Utilities.areOriginsEquivalent(
                        this.m_sHostFromURI, 
                        this.m_headers["Host"], 
                        (this.m_session.isTunnel || this.m_session.IsHTTPS) ? 0x1bb : (this.m_session.IsFTP ? 0x15 : 80)))
                {
                    this.m_session.Flags["X-Original-Host"] = this.m_headers["Host"];
                    this.m_headers["Host"] = this.m_sHostFromURI;
                }
                else if (!this.m_headers.Exists("Host"))
                {
                    if (this.m_headers.HTTPVersion.Equals("HTTP/1.1", StringComparison.OrdinalIgnoreCase))
                    {
                        this.m_session.Flags["X-Original-Host"] = string.Empty;
                    }
                    this.m_headers["Host"] = this.m_sHostFromURI;
                }
                this.m_sHostFromURI = null;
            }
            if (!this.m_headers.Exists("Host"))
            {
                this.freeRequestData();
                return false;
            }
            return true;
        }

       
        internal byte[] TakeEntity()
        {
            byte[] bytes;
            if (this.iEntityBodyOffset < 0)
            {
                throw new InvalidDataException("Request Entity Body Offset must not be negative");
            }
            long num = this.m_requestData.Length - this.iEntityBodyOffset;
            long num2 = this.calculateExpectedEntityTransferSize();
            if (num > num2)
            {
                try
                {
                    bytes = new byte[num - num2];
                    this.m_requestData.Position = this.iEntityBodyOffset + num2;
                    this.m_requestData.Read(bytes, 0, bytes.Length);
                }
                catch (OutOfMemoryException exception)
                {
                    bytes = Encoding.ASCII.GetBytes("JrIntercepter: Out of memory");
                    this.m_session.PoisonClientPipe();
                    return new byte[0];
                }
                this.ClientPipe.PutBackSomeBytes(bytes);
                num = num2;
            }
            if ((num != num2) && (num < num2))
            {
            }
            try
            {
                bytes = new byte[num];
                this.m_requestData.Position = this.iEntityBodyOffset;
                this.m_requestData.Read(bytes, 0, bytes.Length);
            }
            catch (OutOfMemoryException exception2)
            {
                bytes = Encoding.ASCII.GetBytes("JrIntercepter: Out of memory");
                this.m_session.PoisonClientPipe();
            }
            this.freeRequestData();
            return bytes;
        }

        public bool ClientSocketReused
        {
            get
            {
                return this.m_session.IsFlagSet(SessionFlags.ClientPipeReused);
            }
        }

        public HTTPRequestHeaders Headers
        {
            get
            {
                return this.m_headers;
            }
            set
            {
                this.m_headers = value;
            }
        }

        public string Host
        {
            get
            {
                if (this.m_headers != null)
                {
                    return this.m_headers["Host"];
                }
                return string.Empty;
            } 
  

            internal set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                if (this.m_headers != null)
                {
                    if (value.EndsWith(":80") && string.Equals(this.m_headers.UriScheme, "HTTP", StringComparison.OrdinalIgnoreCase))
                    {
                        value = value.Substring(0, value.Length - 3);
                    }
                    this.m_headers["Host"] = value;
                    if (string.Equals(this.m_headers.HTTPMethod, "CONNECT", StringComparison.OrdinalIgnoreCase))
                    {
                        this.m_headers.RequestPath = value;
                    }
                }
            }
        }   

        public string this[string sHeader]
        {
            get
            {
                if (this.m_headers != null)
                {
                    return this.m_headers[sHeader];
                }
                return string.Empty;
            }
            set
            {
                if (this.m_headers == null)
                {
                    throw new InvalidDataException("Request Headers object does not exist");
                }
                this.m_headers[sHeader] = value;
            }
        }
    }
}

