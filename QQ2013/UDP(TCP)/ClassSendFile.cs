using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace CC2013
{
    class ClassSendFile
    {
        Socket socketSend;
        IPEndPoint ipSend;
        
        private string sendFilePath;
        private string desIP;

        public ClassSendFile(string sFilePath,string ip)
        {
            this.sendFilePath = sFilePath;
            this.desIP = ip;
        }

        //使用TCP协议发送文件，端口号为8001
        public void SendFile()
        {
            int len;
            byte[] buff = new byte[1024];
            try
            {
                socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ipSend = new IPEndPoint(IPAddress.Parse(desIP), 8001);
                socketSend.Connect(ipSend);

                FileStream FS = new FileStream(sendFilePath, FileMode.Open, FileAccess.Read);

                while ((len = FS.Read(buff, 0, 1024)) != 0)
                {
                    socketSend.Send(buff,0,len,SocketFlags.None);
                }
                socketSend.Close();
                FS.Close();
            }
            catch
            { 
            }
        }
    }
}
