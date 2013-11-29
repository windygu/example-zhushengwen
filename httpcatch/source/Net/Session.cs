using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace JrIntercepter.Net
{
    class Session
    {
        public bool isTunnel;
        public readonly StringDictionary Flags;

        [CodeDescription("IP Address of the server for this session.")]
        public string HostIP;

        internal bool allowClientPipeReuse; 
 
        private static int cRequests;
        private int requestID;

        // private bool allowClientPipeReuse;

        private int _localProcessID;
        [CodeDescription("Get the process ID of the application which made this request, or 0 if it cannot be determined.")]
        public int LocalProcessID
        {
            get
            {
                return this._localProcessID;
            }
        }

        private string _localProcessName;
        [CodeDescription("Get the process ID of the application which made this request, or 0 if it cannot be determined.")]
        public string LocalProcessName
        {
            get
            {   
                return this._localProcessName;
            }
        }   
          
        [CodeDescription("Contains the bytes of the request body.")]
        public byte[] RequestBodyBytes;

        [CodeDescription("Contains the bytes of the response body.")]
        public byte[] ResponseBodyBytes;

        [CodeDescription("Object representing the HTTP Request.")]
        public ClientChatter Request;
        [CodeDescription("Object representing the HTTP Response.")]
        public ServerChatter Response;

        [CodeDescription("IP Address of the client for this session.")]
        public string clientIP;
        [CodeDescription("Client port attached to JrIntercepter.")]
        public int clientPort;

        public SessionTimers Timers;


        [CodeDescription("Returns TRUE if the Session's HTTP Method is available and matches the target method.")]
        public bool HTTPMethodIs(string sTestFor)
        {
            return (((this.Request != null) && (this.Request.Headers != null)) && string.Equals(this.Request.Headers.HTTPMethod, sTestFor, StringComparison.OrdinalIgnoreCase));
        }


        [CodeDescription("Gets or sets the URL (without protocol) being requested from the server, in the form www.host.com/filepath?query.")]
        public string Url
        {
            get
            {
                if (this.HTTPMethodIs("CONNECT"))
                {
                    return this.PathAndQuery;
                }
                return (this.Host + this.PathAndQuery);
            }
            set
            {
                int length = value.IndexOfAny(new char[] { '/', '?' });
                if (length > -1)
                {
                    this.Host = value.Substring(0, length);
                    this.PathAndQuery = value.Substring(length);
                }
                else
                {
                    this.Host = value;
                    this.PathAndQuery = "/";
                }
            }
        }

        [CodeDescription("Retrieves the complete URI, including protocol/scheme, in the form http://www.host.com/filepath?query.")]
        public string FullUrl
        {
            get
            {
                if (this.Request.Headers != null)
                {
                    return string.Format("{0}://{1}", this.Request.Headers.UriScheme, this.Url);
                }
                return string.Empty;
            }
        }


        [CodeDescription("Gets/Sets the host to which this request is targeted. MAY include a trailing port#.")]
        public string Host
        {
            get
            {
                if (this.Request == null)
                {
                    return string.Empty;
                }
                return this.Request.Host;
            }
            set
            {
                if (this.Request != null)
                {
                    this.Request.Host = value;
                }
            }
        }

        [CodeDescription("Gets/Sets the hostname to which this request is targeted; does NOT include any port# but will include IPv6-literal brackets for IPv6 literals.")]
        public string HostName
        {
            get
            {
                if (((this.Request.Headers == null) || (this.Request.Host == null)) || (this.Request.Host.Length <= 0))
                {
                    return string.Empty;
                }
                int length = this.Request.Host.LastIndexOf(':');
                if ((length > -1) && (length > this.Request.Host.LastIndexOf(']')))
                {
                    return this.Request.Host.Substring(0, length);
                }
                return this.Request.Host;
            }
            set
            {
                int startIndex = value.LastIndexOf(':');
                if ((startIndex > -1) && (startIndex > value.LastIndexOf(']')))
                {
                    throw new ArgumentException("Do not specify a port when setting hostname; use host property instead.");
                }
                string str = this.HTTPMethodIs("CONNECT") ? this.PathAndQuery : this.Host;
                startIndex = str.LastIndexOf(':');
                if ((startIndex > -1) && (startIndex > str.LastIndexOf(']')))
                {
                    this.Host = value + str.Substring(startIndex);
                }
                else
                {
                    this.Host = value;
                }
            }
        }


        [CodeDescription("Returns the server port to which this request is targeted.")]
        public int Port
        {
            get
            {
                string str;
                string requestPath;
                if (this.HTTPMethodIs("CONNECT"))
                {
                    requestPath = this.Request.Headers.RequestPath;
                }
                else
                {
                    requestPath = this.Request.Host;
                }
                int iPort = this.IsHTTPS ? 0x1bb : (this.IsFTP ? 0x15 : 80);
                Utilities.CrackHostAndPort(requestPath, out str, ref iPort);
                return iPort;
            }
            set
            {
                if ((value < 0) || (value > 0xffff))
                {
                    throw new ArgumentException("A valid target port value (0-65535) must be specified.");
                }
                this.Host = this.HostName + ":" + value.ToString();
            }
        }


        [CodeDescription("When true, this session was conducted using the FTPS protocol.")]
        public bool IsFTP
        {
            get
            {
                return (((this.Request != null)
                    && (this.Request.Headers != null)) 
                    && string.Equals(this.Request.Headers.UriScheme, "FTP", StringComparison.OrdinalIgnoreCase)
                );
            }
        }

        [CodeDescription("When true, this session was conducted using the HTTPS protocol.")]
        public bool IsHTTPS
        {
            get
            {
                return (((this.Request != null) 
                    && (this.Request.Headers != null)) 
                    && string.Equals(this.Request.Headers.UriScheme, "HTTPS", StringComparison.OrdinalIgnoreCase)
                );
            }
        }


  
        private SessionStates _state;
        [CodeDescription("Enumerated state of the current session.")]
        public SessionStates State
        {
            get
            {
                return this._state;
            }
            set
            {
                SessionStates state = this._state;
                this._state = value;
                if (this._state == SessionStates.Aborted)
                {
                    // this.FinishUISession(true);
                }
                else if (
                    (
                        (state == SessionStates.HandTamperRequest) || (state == SessionStates.HandTamperResponse)
                     ) || 
                    (
                        (this.State == SessionStates.HandTamperRequest) || (this.State == SessionStates.HandTamperResponse)
                    )
                )
                {
                    // this.RefreshMyInspectors();
                }
                /*
                if (this._OnStateChanged != null)
                {
                    StateChangeEventArgs e = new StateChangeEventArgs(state, value);
                    this._OnStateChanged(this, e);
                    if (this.m_state >= SessionStates.Done)
                    {
                        this._OnStateChanged = null;
                    }
                }*/  
            }
        }

        private SessionFlags _bitFlags;  
        public SessionFlags BitFlags
        {
            get
            {
                return this._bitFlags;
            }
            internal set
            {
                this._bitFlags = value;
            }
        }



        [CodeDescription("Returns the path and query part of the URL. (For a CONNECT request, returns the host:port to be connected.)")]
        public string PathAndQuery
        {
            get
            {
                if (this.Request.Headers != null)
                {
                    return this.Request.Headers.RequestPath;
                }
                return string.Empty;
            }
            set
            {
                this.Request.Headers.RequestPath = value;
            }
        }


        public Session(ClientPipe clientPipe, ServerPipe serverPipe)
        {
            this.Timers = new SessionTimers();
            this.Timers.ClientConnected = DateTime.Now; 

            this.Flags = new StringDictionary();
            if (clientPipe != null)
            {
                this.clientIP = (clientPipe.Address == null) ? null : clientPipe.Address.ToString();
                this.clientPort = clientPipe.Port;
                this.Flags["x-clientIP"] = this.clientIP;
                this.Flags["x-clientport"] = this.clientPort.ToString();
                if (clientPipe.LocalProcessID != 0)
                {
                    this._localProcessID = clientPipe.LocalProcessID;
                    this.Flags["x-ProcessInfo"] = string.Format("{0}:{1}", clientPipe.LocalProcessName, this._localProcessID);
                    this._localProcessName = clientPipe.LocalProcessName;  
                }
            }
            this.Response = new ServerChatter(this);
            this.Request = new ClientChatter(this);
            this.Request.ClientPipe = clientPipe;
            this.Response.ServerPipe = serverPipe;
        }


        public bool IsFlagSet(SessionFlags FlagsToTest)
        {
            return (FlagsToTest == (this._bitFlags & FlagsToTest));
        }


        internal bool ReturnResponse()
        {
            bool flag = false;
            this.Timers.ClientBeginResponse = this.Timers.ClientDoneResponse = DateTime.Now;
            try
            {
                if ((this.Request.ClientPipe != null) && this.Request.ClientPipe.Connected)
                {
                    this.Request.ClientPipe.Send(this.Response.Headers.ToByteArray(true, true));
                    if(this.ResponseBodyBytes!=null)
                    this.Request.ClientPipe.Send(this.ResponseBodyBytes);
                    this.Timers.ClientDoneResponse = DateTime.Now;
                    
                    this.Request.ClientPipe.End();
                    flag = true;
                }
                else
                {
                    this.State = SessionStates.Done;
                }
            }
            catch (Exception exception)
            {
                this.State = SessionStates.Aborted;
            }
            this.Request.ClientPipe = null;
            try
            {
                // this.FinishUISession(false);
            }
            catch (Exception)
            {
            }
            
            return flag;
        }


        internal void SetBitFlag(SessionFlags FlagsToSet, bool b)
        {
            if (b)
            {
                this.BitFlags = this._bitFlags | FlagsToSet;
            }
            else
            {
                this.BitFlags = this._bitFlags & ~FlagsToSet;
            }
        }


        private void CloseSessionPipes(bool bNullThemToo)
        {
            if ((this.Request != null) && (this.Request.ClientPipe != null))
            {
                this.Request.ClientPipe.End();
                if (bNullThemToo)
                {
                    this.Request.ClientPipe = null;
                }
            }
            if ((this.Response != null) && (this.Response.ServerPipe != null))
            {
                this.Response.ServerPipe.End();
                if (bNullThemToo)
                {
                    this.Response.ServerPipe = null;
                }
            }
        }


        internal void AssignID()
        {
            this.requestID = Interlocked.Increment(ref cRequests);
        }

        public void PoisonClientPipe()
        {
            this.allowClientPipeReuse = false;
        }

        public bool ObtainRequest()
        {
            this.State = SessionStates.ReadingRequest;
            if (!this.Request.ReadRequest())
            {
                if (this.Response != null)
                {
                    this.Response.DetachServerPipe();
                }
                this.CloseSessionPipes(true);
                this.State = SessionStates.Aborted;
                return false;
            }

            this.Timers.ClientDoneRequest = DateTime.Now;   
            this.RequestBodyBytes = this.Request.TakeEntity();
          
            if (
                (
                    (this.RequestBodyBytes == null) || (this.RequestBodyBytes.LongLength < 1L)) 
                    && (
                        ("GET" != this.Request.Headers.HTTPMethod) 
                        && Utilities.HTTPMethodRequiresBody(this.Request.Headers.HTTPMethod)
                    )
                )
            {
            }
            if (this.Flags.ContainsKey("X-Original-Host"))
            {
                string str = this.Flags["X-Original-Host"];
                if (string.Empty == str)
                {
                }
                else
                {
                }
            }
            this.State = SessionStates.AutoTamperRequestBefore;
            
            if (this.State >= SessionStates.Done)
            {
                return false;
            }
            if (this.State < SessionStates.AutoTamperRequestAfter)
            {
                this.State = SessionStates.AutoTamperRequestAfter;
            }
            if (this.State >= SessionStates.Done)
            {
                return false;
            } 
            return true;
        }


        internal void Execute()
        {
            if ((this.Request == null) || (this.Response == null))
            {
                return;
            } 
 
            // 从客户端获取请求信息  
            if (!this.ObtainRequest())
            {
                return;
            }

            // 如果当前状态小于ReadingResponse  
            if (this.State < SessionStates.ReadingResponse)
            {
                // 如果端口超出范围
                if ((this.Port < 0) || (this.Port > 0xffff))
                {
                    // 提示错误  
                }

                // 将请求转发至止的服务器
                this.State = SessionStates.SendingRequest;
                if(!Intercepter.dr.ContainsKey(Request.Host))
                {

                    if (!this.Response.ResendRequest())
                    {
                        this.CloseSessionPipes(true);
                        this.State = SessionStates.Aborted;
                        return;
                    }

                    //  通知客户端新SESSION 

                    this.Timers.ServerGotRequest = DateTime.Now;
                    this.State = SessionStates.ReadingResponse;

                    if (!this.Response.ReadResponse())
                    {
                        if (this.State != SessionStates.Aborted)
                        {
                            this.Request.FailSession(0x1f8,
                                "JrIntercepter - Receive Failure", "ReadResponse() failed: The server did not return a response for this request."
                            );
                        }
                        this.CloseSessionPipes(true);
                        this.State = SessionStates.Aborted;
                    }

                    this.ResponseBodyBytes = this.Response.TakeEntity();
                    if (this.Response.ServerPipe != null)
                    {
                        this.Response.ServerPipe.End();
                    }

                    this.Flags["x-ResponseBodyTransferLength"] = (this.ResponseBodyBytes == null) ? "0" : this.ResponseBodyBytes.LongLength.ToString();
                    this.State = SessionStates.AutoTamperResponseBefore;

                    this.State = SessionStates.SendingResponse;

                    if (this.ReturnResponse())
                    {
                        this.State = SessionStates.Done;
                    }
                    else
                    {
                        this.State = SessionStates.Aborted;
                    }
                }
                else
                {
                    DirectResponse();
                }
                if (this.Request != null && this.Request.ClientPipe != null)
                {
                    // 结束客户连接
                    this.Request.ClientPipe.End();
                }

                // 释放服务请求  
                this.Response.ReleaseServerPipe();

            }
           

            // tmp();
        }

        public void DirectResponse()
        {
            int iError = 302;
            this.SetBitFlag(SessionFlags.ResponseGeneratedByJI, true);

            this.Response.Headers = new HTTPResponseHeaders(Config.HeaderEncoding);
            this.Response.Headers.HTTPResponseCode = iError;
            this.Response.Headers.HTTPResponseStatus = iError.ToString() + " Found" ;
            this.Response.Headers.Add("Location","http://"+ Intercepter.url);
            this.Response.Headers.Add("Content-Type", "text/html; charset=UTF-8");
            this.Response.Headers.Add("Connection", "Keep-Alive");
            this.Response.Headers.Add("Server", "	Apache/2.0.63 (Win32) PHP/5.2.14");
            this.Response.Headers.Add("Content-Length", "0");
            this.Response.Headers.Add("Keep-Alive", "timeout=15, max=100");
            this.Response.Headers.Add("Timestamp", DateTime.Now.ToString("HH:mm:ss.fff"));
            this.State = SessionStates.Done;
            Intercepter.UpdateSession(this);
            this.ReturnResponse();
        }
        public bool isAnyFlagSet(SessionFlags flagsToTest)
        {
            return (SessionFlags.None != (this._bitFlags & flagsToTest));
        }

        public bool isFlagSet(SessionFlags flagsToTest)
        {
            return (flagsToTest == (this._bitFlags & flagsToTest));
        }


        [CodeDescription("Returns the sequential number of this request.")]
        public int id
        {
            get
            {
                return this.requestID;
            }
        }


        public static void CreateAndExecute(object param)
        {
            ClientPipe clientPipe = new ClientPipe((Socket)param);
            Session session = new Session(clientPipe, null);
          
            session.Execute();
        }

    }
}


