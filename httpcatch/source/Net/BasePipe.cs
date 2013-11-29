namespace JrIntercepter.Net  
{
    using System;
    using System.Net;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Threading;

    public abstract class BasePipe
    {
        protected Socket baseSocket;
        protected SslStream httpsStream;
        private int transmitDelayMS;
        protected internal string hackSessionList;
        protected internal string pipeName;
        protected internal uint UseCount;

        public BasePipe(Socket oSocket, string sName)
        {
            this.pipeName = sName;
            this.baseSocket = oSocket;
        }

        public void End()
        {
            try
            {
                if (this.httpsStream != null)
                {
                    this.httpsStream.Close();
                }
                if (this.baseSocket != null)
                {
                    this.baseSocket.Shutdown(SocketShutdown.Both);
                    this.baseSocket.Close();
                }
            }
            catch (Exception)
            {
            }
            this.baseSocket = null;
            this.httpsStream = null;
        }

        public Socket GetRawSocket()
        {
            return this.baseSocket;
        }

        internal void IncrementUse(int session)
        {
            this.transmitDelayMS = 0;
            this.UseCount++;
            this.hackSessionList = this.hackSessionList + session.ToString() + ",";
        }

        internal int Receive(byte[] buffer)
        {
            if (this.IsSecured)
            {
                return this.httpsStream.Read(buffer, 0, buffer.Length);
            }
            return this.baseSocket.Receive(buffer);

        }

        internal int Receive(byte[] buffer,int size,SocketFlags flags) {  
            return this.baseSocket.Receive(buffer, size, flags);
        }

        public void Send(byte[] bytes)
        {
            this.Send(bytes, 0, bytes.Length);
        }

        internal void Send(byte[] oBytes, int iOffset, int iCount)
        {
            if (oBytes != null)
            {
                if ((iOffset + iCount) > oBytes.LongLength)
                {
                    iCount = oBytes.Length - iOffset;
                }
                if (iCount >= 1)
                {
                    if (this.transmitDelayMS < 1)
                    {
                        if (this.IsSecured)
                        {
                            this.httpsStream.Write(oBytes, iOffset, iCount);
                        }
                        else
                        {
                            this.baseSocket.Send(oBytes, iOffset, iCount, SocketFlags.None);
                        }
                    }
                    else
                    {
                        int count = 0x400;
                        for (int i = iOffset; i < (iOffset + iCount); i += count)
                        {
                            if ((i + count) > (iOffset + iCount))
                            {
                                count = (iOffset + iCount) - i;
                            }
                            Thread.Sleep((int)(this.transmitDelayMS / 2));
                            if (this.IsSecured)
                            {
                                this.httpsStream.Write(oBytes, i, count);
                            }
                            else
                            {
                                this.baseSocket.Send(oBytes, i, count, SocketFlags.None);
                            }
                            Thread.Sleep((int)(this.transmitDelayMS / 2));
                        }
                    }
                }
            }
        }

        public IPAddress Address
        {
            get
            {
                if ((this.baseSocket != null) && (this.baseSocket.RemoteEndPoint != null))
                {
                    return (this.baseSocket.RemoteEndPoint as IPEndPoint).Address;
                }
                return new IPAddress(0L);
            }
        }

        public bool IsSecured
        {
            get
            {
                return (null != this.httpsStream);
            }
        }

        public bool Connected
        {
            get
            {
                if (this.baseSocket == null)
                {
                    return false;
                }
                return this.baseSocket.Connected;
            }
        }

        public int LocalPort
        {
            get
            {
                if ((this.baseSocket != null) && (this.baseSocket.LocalEndPoint != null))
                {
                    return (this.baseSocket.LocalEndPoint as IPEndPoint).Port;
                }
                return 0;
            }
        }

        public int Port
        {
            get
            {
                if ((this.baseSocket != null) && (this.baseSocket.RemoteEndPoint != null))
                {
                    return (this.baseSocket.RemoteEndPoint as IPEndPoint).Port;
                }
                return 0;
            }
        }

        public int TransmitDelay
        {
            get
            {
                return this.transmitDelayMS;
            }
            set
            {
                this.transmitDelayMS = value;
            }
        }
    }
}

