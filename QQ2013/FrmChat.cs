using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CCWin;
using CCWin.SkinControl;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace CC2013
{
    public partial class FrmChat : CCSkinMain
    {
        private string destinationIP = string.Empty;
        private string destinationName = string.Empty;
        private string destinationID = string.Empty;
        private string receiveMsg = string.Empty;
        public string Cuser = string.Empty;
        public string CuserIP = string.Empty;

        public string filePath = string.Empty;
        public Socket socketTCPListen;
        public Socket socketReceiveFile;
        public IPEndPoint ipEP;
        byte[] Buff = new byte[1024000];

        const int WM_COPYDATA = 0x004A;//文本类型参数

        private bool isTextBoxNotEmpty = true; //判断输入文本框是否为空

        private ToolStripDropDown emotionDropDown = new ToolStripDropDown();

        //RtfRichTextBox richtxtChat = new RtfRichTextBox();

        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;//用户定义数据
            public int cbData;//数据大小
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;//指向数据的指针
        }//消息中传递的结构体

        public FrmChat()
        {
            InitializeComponent();
        }

        public FrmChat(string ip, string name, string id, string mesg)
        {
            destinationIP = ip;
            destinationName = name;
            destinationID = id;
            receiveMsg = mesg;
            InitializeComponent();
        }

        //窗口加载时
        private void FrmChat_Load(object sender, EventArgs e)
        {
            //txtSMsg.Focus();
            //Cuser = UserLogin.UserItem.NicName.Trim();

            //ClassBoardCast cBC = new ClassBoardCast();
            //cBC.GetLocalIP();
            //CuserIP = cBC.localIP;

            //if (receiveMsg != string.Empty)
            //{
            //    displayMessage(receiveMsg);
            //}

            //lblChatName.Text = string.IsNullOrEmpty(UserLogin.UserItem.DisplayName) ? UserLogin.UserItem.NicName : UserLogin.UserItem.DisplayName;
            //lblChatQm.Text = UserLogin.UserItem.PersonalMsg;
        }

        //发送消息方法
        private void sentMessage()
        {
            //if (this.txtSMsg.Text == "")
            //{
            //    this.txtSMsg.Text = "输入消息不能为空...";
            //    this.txtSMsg.BackColor = Color.OldLace;
            //    this.isTextBoxNotEmpty = false;
            //    this.txtSMsg.ReadOnly = true;
            //}
            if (isTextBoxNotEmpty)
            {
                try
                {
                    //发送到对方的信息框中
                    string sendMessageInfo = ":MESG:" + Cuser + "|" + System.Environment.UserName + "|" +
                        CuserIP + "|";// +this.txtSMsg.Rtf;
                    byte[] buff = Encoding.Default.GetBytes(sendMessageInfo);
                    ClassSendMsg cSendMsg = new ClassSendMsg(destinationIP, buff);
                    cSendMsg.SendMessage();
                    //向自己的显示框中显示发送信息
                    //this.txtRMsg.AppendTextAsRtf(Cuser + "  " + DateTime.Now.ToLongTimeString() + "\r\n",
                    // new Font(this.Font, FontStyle.Regular), RtfRichTextBox.RtfColor.Green);
                    //this.txtRMsg.AppendTextAsRtf("   ");
                    //this.txtRMsg.AppendRtf(this.txtSMsg.Rtf);
                    ////this.txtRMsg.AppendTextAsRtf("\n");
                    //this.txtRMsg.Select(txtRMsg.Text.Length, 0);
                    //this.txtRMsg.ScrollToCaret();
                    ////清空输入框
                    //this.txtSMsg.Text = string.Empty;
                    //this.txtSMsg.Focus();
                }
                catch
                {
                    //this.txtRMsg.AppendText(DateTime.Now.ToLongTimeString() + " 发送消息失败！" + "\r\n");
                }
            }
        }

        //接收传递的消息
        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_COPYDATA:
                    COPYDATASTRUCT mystr = new COPYDATASTRUCT();
                    Type mytype = mystr.GetType();
                    mystr = (COPYDATASTRUCT)m.GetLParam(mytype);
                    receiveMsg = mystr.lpData;

                    displayMessage(receiveMsg);
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        //显示消息
        private void displayMessage(string msg)
        {
            try
            {
                if (msg.Length > 6 && msg.Substring(0, 6) == "【发送文件】")
                {
                    //this.txtRMsg.AppendTextAsRtf(destinationName + "  " + DateTime.Now.ToLongTimeString() + "\r\n",
                    //    new Font(this.Font, FontStyle.Regular), RtfRichTextBox.RtfColor.Blue);

                    //this.txtRMsg.SelectionColor = Color.Red;
                    //this.txtRMsg.AppendText(receiveMsg + "\n");
                    ////this.txtRMsg.AppendTextAsRtf("\n");
                    //this.txtRMsg.ForeColor = Color.Black;
                    //this.txtRMsg.Select(txtRMsg.Text.Length, 0);
                    //this.txtRMsg.ScrollToCaret();

                    this.filePath = msg.Substring(6);
                    this.labFileInfo.Text = destinationName + " 向你发送文件";
                    this.labFileInfo.Visible = true;
                    this.linkLableAccept.Visible = true;
                    this.linkLabelRefuse.Visible = true;
                }
                else if (msg.Length > 6 && msg.Substring(0, 6) == "【发送信息】")
                {
                    //this.txtRMsg.AppendTextAsRtf(destinationName + " " + DateTime.Now.ToLongTimeString() + "\r\n",
                    //   new Font(this.Font,FontStyle.Regular), RtfRichTextBox.RtfColor.Blue);
                    //this.txtRMsg.SelectionColor = Color.Red;
                    //this.txtRMsg.AppendText(receiveMsg + "\n");
                    ////this.txtRMsg.AppendTextAsRtf("\n");
                    //this.txtRMsg.ForeColor = Color.Black;
                    //this.txtRMsg.Select(txtRMsg.Text.Length, 0);
                    //this.txtRMsg.ScrollToCaret();
                }
                else if (msg.Length > 6 && msg.Substring(0, 6) == "【发送震动】")
                {
                    //this.txtRMsg.AppendTextAsRtf(destinationName + "  " + DateTime.Now.ToLongTimeString() + "\r\n",
                    //new Font(this.Font, FontStyle.Regular), RtfRichTextBox.RtfColor.Blue);

                    //this.txtRMsg.SelectionColor = Color.Red;
                    //this.txtRMsg.AppendTextAsRtf("   ");
                    //this.txtRMsg.AppendText(destinationName + "给您发送了窗口抖动。\n");
                    ////this.txtRMsg.AppendTextAsRtf("\n");
                    //this.txtRMsg.ForeColor = Color.Black;
                    //this.txtRMsg.Select(txtRMsg.Text.Length, 0);
                    //this.txtRMsg.ScrollToCaret();
                    Vibration();
                }
                else
                {
                    //this.txtRMsg.AppendTextAsRtf(destinationName + "  " + DateTime.Now.ToLongTimeString() + "\r\n",
                    //    new Font(this.Font, FontStyle.Regular), RtfRichTextBox.RtfColor.Blue);
                    //this.txtRMsg.AppendTextAsRtf("   ");
                    //this.txtRMsg.AppendRtf(receiveMsg);
                    ////this.txtRMsg.AppendTextAsRtf("\n");
                    //this.txtRMsg.Select(txtRMsg.Text.Length, 0);
                    //this.txtRMsg.ScrollToCaret();

                }
            }
            catch
            {
            }
        }

        //拒绝接收文件按钮
        private void linkLabelRefuse_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.labFileInfo.Visible = false;
            this.linkLableAccept.Visible = false;
            this.linkLabelRefuse.Visible = false;

            string sendMsg = ":MESG:" + Cuser + "|" + System.Environment.UserName + "|" +
                    CuserIP + "|" + "【发送信息】对方拒绝接收";
            byte[] buff = Encoding.Default.GetBytes(sendMsg);

            ClassSendMsg cSendFileInfo = new ClassSendMsg(destinationIP, buff);
            cSendFileInfo.SendMessage();
        }

        //接受文件的按钮
        private void linkLableAccept_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                this.linkLabelRefuse.Enabled = false;
                this.linkLableAccept.Enabled = false;

                string aToReceive = ":ACEP:" + Cuser + "|" + System.Environment.UserName + "|" +
                    CuserIP + "|" + filePath + "|";
                byte[] buff = Encoding.Default.GetBytes(aToReceive);

                string[] realFileName = filePath.Split('\\');
                string filename = realFileName[realFileName.Length - 1].ToString();
                int len;

                //同意接收文件，发送同意请求，并打开TCP监听
                TCPListen();

                ClassSendMsg cReadyToReceive = new ClassSendMsg(destinationIP, buff);
                cReadyToReceive.SendMessage();

                socketReceiveFile = socketTCPListen.Accept();

                SaveFileDialog SFD = new SaveFileDialog();
                SFD.OverwritePrompt = true;
                SFD.RestoreDirectory = true;
                SFD.Filter = "所有文件(*.*)|*.*";
                SFD.InitialDirectory = "C:\\Documents and Settings\\" + System.Environment.UserName + "\\桌面\\";
                SFD.FileName = filename;

                if ((len = socketReceiveFile.Receive(Buff)) != 0)
                {
                    if (SFD.ShowDialog() == DialogResult.OK)
                    {
                        FileStream FS = new FileStream(SFD.FileName, FileMode.OpenOrCreate, FileAccess.Write);
                        FS.Write(Buff, 0, len);
                        while ((len = socketReceiveFile.Receive(Buff)) != 0)
                        {
                            FS.Write(Buff, 0, len);
                        }
                        FS.Flush();
                        FS.Close();
                        //this.txtRMsg.SelectionColor = Color.Red;
                        //this.txtRMsg.AppendText("【接收完成】文件已保存" + "\r\n");
                        //this.txtRMsg.ForeColor = Color.Black;
                    }
                }

                string sendMessageInfo = ":MESG:" + Cuser + "|" + System.Environment.UserName + "|" +
                    CuserIP + "|" + "【发送信息】文件已发送成功";
                byte[] buffReply = Encoding.Default.GetBytes(sendMessageInfo);

                ClassSendMsg cSendMsg = new ClassSendMsg(destinationIP, buffReply);
                cSendMsg.SendMessage();

                socketTCPListen.Close();
                socketReceiveFile.Close();

                this.linkLabelRefuse.Enabled = true;
                this.linkLableAccept.Enabled = true;
                this.labFileInfo.Visible = false;
                this.linkLabelRefuse.Visible = false;
                this.linkLableAccept.Visible = false;
            }
            catch
            {
            }
            finally
            {
                socketTCPListen.Close();
                socketReceiveFile.Close();
            }
        }

        //同意接收文件，发送同意请求，并打开TCP监听
        public void TCPListen()
        {
            socketTCPListen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ipEP = new IPEndPoint(IPAddress.Parse(CuserIP), 8001);
            socketTCPListen.Bind(ipEP);
            socketTCPListen.Listen(1024);
        }

        //当发送消息为空时，显示提示信息，鼠标单击此区域，恢复可输入状态
        private void txtSMsg_MouseClick(object sender, MouseEventArgs e)
        {
            if (!isTextBoxNotEmpty)
            {
                //this.txtSMsg.Text = "";
                //this.txtSMsg.BackColor = Color.White;
                //isTextBoxNotEmpty = true;
                //this.txtSMsg.ReadOnly = false;
            }
        }

        //悬浮至好友Q名时
        private void lblChatName_MouseEnter(object sender, EventArgs e)
        {
            lblChatName.Font = new Font("微软雅黑", 14F, FontStyle.Underline);
        }

        //离开好友Q名时
        private void lblChatName_MouseLeave(object sender, EventArgs e)
        {
            lblChatName.Font = new Font("微软雅黑", 14F);
        }

        //发送
        private void btnSend_Click(object sender, EventArgs e)
        {
            sentMessage();
        }

        //关闭
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //渐变层
        private void FrmChat_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            SolidBrush sb = new SolidBrush(Color.FromArgb(100, 255, 255, 255));
            g.FillRectangle(sb, new Rectangle(new Point(1, 91), new Size(Width - 2, Height - 91)));
        }

        //显示字体对话框
        private void toolFont_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == fontDialog1.ShowDialog())
            {
                //this.txtSMsg.Font = fontDialog1.Font;
                //this.txtSMsg.ForeColor = fontDialog1.Color;
            }
        }

        //发送文件
        private void ToolFile_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog Dlg = new OpenFileDialog();
                FileInfo FI;
                Dlg.Filter = "所有文件(*.*)|*.*";
                Dlg.CheckFileExists = true;
                Dlg.InitialDirectory = "C:\\Documents and Settings\\" + System.Environment.UserName + "\\桌面\\";

                if (Dlg.ShowDialog() == DialogResult.OK)
                {
                    FI = new FileInfo(Dlg.FileName);
                    string sendMsg = ":DATA:" + Cuser + "|" + System.Environment.UserName + "|" +
                        CuserIP + "|" + Dlg.FileName + "|" + FI.Length + "|";

                    byte[] buff = Encoding.Default.GetBytes(sendMsg);

                    ClassSendMsg cSendFileInfo = new ClassSendMsg(destinationIP, buff);
                    cSendFileInfo.SendMessage();

                    //this.txtRMsg.AppendTextAsRtf(Cuser + "  " + DateTime.Now.ToLongTimeString() + "\r\n",
                    //    new Font(this.Font, FontStyle.Regular), RtfRichTextBox.RtfColor.Green);

                    //this.txtRMsg.SelectionColor = Color.Red;
                    //this.txtRMsg.AppendText("【发送文件】" + Dlg.FileName + "\r\n");
                    //this.txtRMsg.ForeColor = Color.Black;
                    //this.txtRMsg.Select(txtRMsg.Text.Length, 0);
                    //this.txtRMsg.ScrollToCaret();

                }
            }
            catch
            {
                MessageBox.Show("文件发送失败！" + "\r\n");
            }
        }

        //震动方法
        private void Vibration()
        {
            Point pOld = this.Location;//原来的位置
            int radius = 3;//半径
            for (int n = 0; n < 3; n++) //旋转圈数
            {
                //右半圆逆时针
                for (int i = -radius; i <= radius; i++)
                {
                    int x = Convert.ToInt32(Math.Sqrt(radius * radius - i * i));
                    int y = -i;

                    this.Location = new Point(pOld.X + x, pOld.Y + y);
                    Thread.Sleep(10);
                }
                //左半圆逆时针
                for (int j = radius; j >= -radius; j--)
                {
                    int x = -Convert.ToInt32(Math.Sqrt(radius * radius - j * j));
                    int y = -j;

                    this.Location = new Point(pOld.X + x, pOld.Y + y);
                    Thread.Sleep(10);
                }
            }
            //抖动完成，恢复原来位置
            this.Location = pOld;
        }

        //震动
        private void toolVibration_Click(object sender, EventArgs e)
        {
            string sendMessageInfo = ":MESG:" + Cuser + "|" + System.Environment.UserName + "|" +
                       CuserIP + "|" + "【发送震动】您发送了一个窗口抖动。\r\n";
            byte[] buff = Encoding.Default.GetBytes(sendMessageInfo);

            ClassSendMsg cSendMsg = new ClassSendMsg(destinationIP, buff);
            cSendMsg.SendMessage();

            //this.txtRMsg.AppendTextAsRtf(Cuser + "  " + DateTime.Now.ToLongTimeString() + "\r\n",
            //    new Font(this.Font, FontStyle.Regular), RtfRichTextBox.RtfColor.Green);
            //this.txtRMsg.SelectionColor = Color.Red;
            //this.txtRMsg.AppendTextAsRtf("   ");
            //this.txtRMsg.AppendText("您发送了一个窗口抖动。\r\n");
            //this.txtRMsg.ForeColor = Color.Black;
            //this.txtRMsg.Select(txtRMsg.Text.Length, 0);
            //this.txtRMsg.ScrollToCaret();
            //this.txtSMsg.Text = string.Empty;
            //this.txtSMsg.Focus();
            Vibration();
        }

        //截图方法
        private FrmCapture m_frmCapture;
        private void StartCapture()
        {
            //if (m_frmCapture == null || m_frmCapture.IsDisposed)
            //    m_frmCapture = new FrmCapture(txtSMsg);
            //m_frmCapture.IsCaptureCursor = false;
            //m_frmCapture.Show();
        }

        //截图按钮
        private void toolPrintScreen_ButtonClick(object sender, EventArgs e)
        {
            this.StartCapture();
        }

        //表情
        private void toolCountenance_Click(object sender, EventArgs e)
        {
            FrmCountenance frm = new FrmCountenance(new Point((this.Left + skToolMenu.Left + 30) - (464/2), this.Top + skToolMenu.Top - 316));
            frm.Show();
        }

        //窗体调用重绘时
        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            //txtRMsg.Invalidate();
            //txtSMsg.Invalidate();
            base.OnInvalidated(e);
        }
    }
}
