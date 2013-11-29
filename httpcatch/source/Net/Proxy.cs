namespace JrIntercepter.Net
{
    using Microsoft.Win32;
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Windows.Forms;

    public class Proxy : IDisposable
    {
        private Socket acceptor;
      
        internal Proxy()
        {  
        }

        private void AcceptConnection(IAsyncResult ar)
        {
            try
            {
                ThreadPool.UnsafeQueueUserWorkItem(new WaitCallback(Session.CreateAndExecute), this.acceptor.EndAccept(ar));
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception)
            {
                return; 
            }
            try
            {
                this.acceptor.BeginAccept(new AsyncCallback(this.AcceptConnection), null);
            }
            catch (Exception)
            {
            }  
        }

        internal bool Start(int listenPort)
        {
            /*
            this.acceptor = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            if (Environment.OSVersion.Version.Major > 5)
            {
                this.acceptor.SetSocketOption(
                    SocketOptionLevel.IPv6, 
                    SocketOptionName.PacketInformation | SocketOptionName.KeepAlive, 0
                );
            }
            */

            try
            {
                this.acceptor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.acceptor.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), listenPort));
                this.acceptor.Listen(50); 
                this.acceptor.BeginAccept(new AsyncCallback(this.AcceptConnection), null);
            }
            catch (Exception e) {
                MessageBox.Show(e.Message);  
            }
            return true;  
        }

        internal void Stop()
        {
            if (this.acceptor != null)
            {
                try
                {
                    this.acceptor.LingerState = new LingerOption(true, 0);
                    this.acceptor.Close();
                }
                catch (Exception exception)
                {
                    Trace.WriteLine("Proxy.Stop threw an exception: " + exception.Message);
                }
            }
        }

        public int ListenPort
        {
            get
            {
                if (this.acceptor != null)
                {
                    return (this.acceptor.LocalEndPoint as IPEndPoint).Port;
                }
                return 0;
            }
        }

        public void Dispose()
        {
            this.Stop();  
        }
    }
}

