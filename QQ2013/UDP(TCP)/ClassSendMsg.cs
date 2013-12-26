using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace CC2013
{
    class ClassSendMsg
    {
        byte[] bufSendMsg = null;

        IPEndPoint sendMsgEP = null;
        UdpClient sendMsgUdpClient = new UdpClient();

        public ClassSendMsg(string r_desIP,byte[] bufMsg)
        {
            this.sendMsgEP = new IPEndPoint(IPAddress.Parse(r_desIP), 2425);
            this.bufSendMsg = bufMsg;
        }
        //使用UDP协议发送文件，端口号为2425
        public void SendMessage()
        {
            sendMsgUdpClient.Send(bufSendMsg, bufSendMsg.Length, sendMsgEP);
        }
    }
}
