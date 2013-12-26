using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace CC2013
{
    class ClassTcpConnect
    {
        private Socket socketSend;
        private IPEndPoint ipSend;

        public ClassTcpConnect(Socket sSend, IPEndPoint iSend)
        {
            this.socketSend = sSend;
            this.ipSend = iSend;
        }

        public void SocketSend()
        {
            socketSend.Connect(ipSend);
        }
    }
}
