using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using CCWin.Win32;

namespace CC2013
{
    //接受UDP消息时调用
    class ClassReceiveMsg
    {
        private string msgIP;
        private string msgFrom;
        private string msgID;
        private string msgDetail;

        [DllImport("user32.dll ", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);

        const int WM_COPYDATA = 0x004A;//文本类型参数

        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;//用户定义数据
            public int cbData;//数据大小
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;//指向数据的指针
        }//消息中传递的结构体

        public ClassReceiveMsg(string msgip,string msgfrom, string msgid,string msgdetail)
        {
            this.msgIP = msgip;
            this.msgFrom = msgfrom;
            this.msgID = msgid;
            this.msgDetail = msgdetail;
        }

        public void StartRecMsg()
        {
            //[Todo]在这里应该加一个判断，判断是否存在一个form.name=msgForm的窗口****************************
            //如果存在，将消息传到这个窗口，如果不存在，创建一个新窗口

            //找到当前已经打开的聊天窗口的句柄
            string windowsName = "与 " + msgFrom + " 对话中";
            IntPtr handle = NativeMethods.FindWindow(null, windowsName);            

            if (handle != IntPtr.Zero)
            {
                //对要发送的数据进行封装，直接发string类型，收到会出错
                byte[] sarr = Encoding.Default.GetBytes(msgDetail);
                int len = sarr.Length;
                COPYDATASTRUCT cds;
                cds.dwData = (IntPtr)100;
                cds.lpData = msgDetail;
                cds.cbData = len + 1;

                SendMessage(handle, WM_COPYDATA, 0, ref cds);
                NativeMethods.FlashWindow(handle, true);
            }
            else
            {
                FrmChat formRMsg = new FrmChat(msgIP, msgFrom,msgID, msgDetail);
                formRMsg.Text = "与 " + msgFrom + " 对话中";
                formRMsg.WindowState = FormWindowState.Minimized;
                formRMsg.ShowDialog();
                //formRMsg.Show();
                //formRMsg.WindowState = FormWindowState.Minimized;
                //IntPtr newHandle = FindWindow(null, formRMsg.Text);                
                //FlashWindow(newHandle, true);
            }
        }
    }
}
