namespace JrIntercepter.Net 
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Windows.Forms;

    public class ServerPipe : BasePipe
    {
        internal DateTime ConnectedTime;

        internal ServerPipe(Socket s ,string name)
            : base(s, name)
        {
        }


        internal ServerPipe(string sName)
            : base(null, sName)
        {
        }


        internal bool WrapSocketInPipe(Socket socket)
        {
            base.baseSocket = socket;
            this.ConnectedTime = DateTime.Now;  
            return true;
        }


        public void Connect(IPEndPoint p) {
            this.baseSocket.Connect(p);  
        }

       //  public void 
        public IPEndPoint RemoteEndPoint
        {
            get
            {
                if (base.baseSocket == null)
                {
                    return null;
                }
                return (base.baseSocket.RemoteEndPoint as IPEndPoint);
            }
        }   
    }
}

