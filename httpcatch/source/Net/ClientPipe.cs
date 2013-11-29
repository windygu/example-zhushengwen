namespace JrIntercepter.Net 
{
    using System;
    using System.Net;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Cryptography.X509Certificates;

    public class ClientPipe : BasePipe
    {
        private byte[] receivedAndPutBack;
        private int processID;
        private string processName;
        internal static int timeoutReceiveInitial = 0xea60;
        internal static int timeoutReceiveReused =0x7530;

        internal ClientPipe(Socket oSocket)
            : base(oSocket, "C")
        {
            try
            {
                oSocket.NoDelay = true;
                if (Config.MapSocketToProcess)
                {
                    this.processID = Winsock.MapLocalPortToProcessId(((IPEndPoint)oSocket.RemoteEndPoint).Port);
                    if (this.processID > 0)
                    {
                        this.processName = ProcessHelper.GetProcessName(this.processID);
                    }  
                }
            }
            catch
            {
            }
        }

        internal void PutBackSomeBytes(byte[] toPutback)
        {
            this.receivedAndPutBack = new byte[toPutback.Length];
            Buffer.BlockCopy(toPutback, 0, this.receivedAndPutBack, 0, toPutback.Length);
        }

        internal int Receive(byte[] buffer)
        {
            if (this.receivedAndPutBack == null)
            {
                return base.Receive(buffer);
            }
            int length = this.receivedAndPutBack.Length;
            Buffer.BlockCopy(this.receivedAndPutBack, 0, buffer, 0, length);
            this.receivedAndPutBack = null;
            return length;
        }


        internal void SetReceiveTimeout()
        {
            try
            {
                base.baseSocket.ReceiveTimeout = (base.UseCount < 2) ? timeoutReceiveInitial : timeoutReceiveReused;
            }
            catch
            {
            }
        }

        public override string ToString()
        {
            return string.Format(
                "[ClientPipe: {0}:{1}; UseCnt: {2}; Port: {3}; {4}]", 
                new object[] { 
                    this.processName, 
                    this.processID, 
                    base.UseCount, 
                    base.Port, 
                    base.IsSecured ? "SECURE" : "PLAINTTEXT" }
                );
        }

        public int LocalProcessID
        {
            get
            {
                return this.processID;
            }
        }

        public string LocalProcessName
        {
            get
            {
                return (this.processName ?? string.Empty);
            }
        }
    }
}

