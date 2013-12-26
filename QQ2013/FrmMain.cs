using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using CCWin;
using CCWin.SkinControl;
using System.Runtime.InteropServices;
using CCWin.Win32;

namespace CC2013
{
    public partial class FrmMain : CCSkinMain
    {
        //变量
        private string Id;
        private Image Title;
        private int form_top_old;
        private int form_left_old;
        private int form_right_old;
        private int SW;
        private int SH;

        private ChatListSubItem UserItem;
        public FrmMain(string id, Image title, SkinButtom BtnState)
        {
            FrmMain.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            //New一个当前用户的对象
            UserItem = new ChatListSubItem(id, (ChatListSubItem.UserStatus)Convert.ToInt32(BtnState.Tag));
            UserItem.HeadImage = title;
            UserItem.PersonalMsg = lblQm.Text;
            UserLogin.UserItem = UserItem;
            //获取当前状态
            btnState.Image = BtnState.Image;
            btnState.Tag = BtnState.Tag;
            //获取当前登录帐号与头像
            this.Id = lblName.Text = id;
            this.Title = title;
            //获取屏幕宽高
            SW = Screen.PrimaryScreen.Bounds.Width;
            SH = Screen.PrimaryScreen.Bounds.Height;
        }

        //窗口加载时
        private void FormCSharpWinDemo_Load(object sender, EventArgs e)
        {
            FrmInformation frm = new FrmInformation(Id, Title);
            frm.Show();
            //获取屏幕宽高与调节最大大小
            this.MaximumSize = new Size(543, Screen.GetWorkingArea(this).Height);

            //监听消息（广播和聊天）
            ClassStartUdpThread startUdpThread = new ClassStartUdpThread(chatShow);
            Thread tStartUdpThread = new Thread(new ThreadStart(startUdpThread.StartUdpThread));
            tStartUdpThread.IsBackground = true;
            tStartUdpThread.Start();

            //第一次登录时发送广播消息，查看在线用户
            ClassBoardCast boardCast = new ClassBoardCast();
            boardCast.BoardCast();

            ////加载好友列表
            //chatShow.Items.Clear();
            //Random rnd = new Random();
            //for (int i = 0; i < 10; i++)
            //{
            //    ChatListItem item = new ChatListItem("分组 " + i);
            //    for (int j = 0; j < 10; j++)
            //    {
            //        ChatListSubItem subItem = new ChatListSubItem("NicName", "好友" + j, "我的个性签名,我的个性签名,我的个性签名");
            //        subItem.HeadImage = Image.FromFile("head/" + rnd.Next(1, 11) + ".png");
            //        subItem.ID = (i + 1) * (j + 1);
            //        subItem.Status = (ChatListSubItem.UserStatus)(j % 6);
            //        item.SubItems.AddAccordingToStatus(subItem);
            //    }
            //    item.SubItems.Sort();
            //    chatShow.Items.Add(item);
            //}
        }

        //贴边隐藏
        int? hg = null;
        private void timHide_Tick(object sender, EventArgs e)
        {
            //隐藏窗口的方法
            form_top_old = 1 - this.Height;
            form_left_old = 1 - this.Width;
            form_right_old = SW - 1;
            //没点击移动时
            if (!this.isMouseDown)
            {
                //鼠标在窗体内时
                if (this.Bounds.Contains(Cursor.Position))
                {
                    switch (this.Aanhor)
                    {
                        //执行右移特效
                        case AnchorStyles.Left:
                            if (hg == null)
                            {
                                hg = this.Height;
                            }
                            this.Height = MaximumSize.Height;
                            this.Top = 0;
                            int b = this.Left;
                            for (int i = b; i < 0; i += 1)
                            {
                                this.Left = i;
                            }
                            this.Left = 0;
                            break;
                        //执行左移特效
                        case AnchorStyles.Right:
                            if (hg == null)
                            {
                                hg = this.Height;
                            }
                            this.Height = MaximumSize.Height;
                            this.Top = 0;
                            int c = this.Left;
                            for (int i = c; i > SW - this.Width; i -= 1)
                            {
                                this.Left = i;
                            }
                            this.Left = SW - this.Width;
                            break;
                        //执行下移特效
                        case AnchorStyles.Top:
                            int a = this.Top;
                            for (int i = a; i < 0; i += 2)
                            {
                                this.Top = i;
                            }
                            this.Top = 0;
                            break;
                    }
                    //hg不为空的话，恢复成原来高度
                    if (hg != null && this.Left > 0 && this.Left < Screen.PrimaryScreen.Bounds.Width - this.Width)
                    {
                        this.Height = (int)hg;
                        hg = null;
                    }
                }
                else
                {
                    switch (this.Aanhor)
                    {
                        //执行左移特效
                        case AnchorStyles.Left:
                            if (this.Left != form_left_old)
                            {
                                for (int i = 0; i >= form_left_old; i -= 1)
                                {
                                    this.Left = i;
                                }
                                this.Left = form_left_old;
                            }
                            break;
                        //执行右移特效
                        case AnchorStyles.Right:
                            if (this.Left != form_right_old)
                            {
                                for (int i = SW - this.Width; i <= form_right_old; i += 1)
                                {
                                    this.Left = i;
                                }
                                this.Left = form_right_old;
                            }
                            break;
                        //执行上移特效
                        case AnchorStyles.Top:
                            if (this.Top != form_top_old)
                            {
                                for (int i = 0; i >= form_top_old; i -= 2)
                                {
                                    this.Top = i;
                                }
                                this.Top = form_top_old;
                            }
                            break;
                    }
                }
            }
        }

        //窗体关闭时
        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClassBoardCast cUserQuit = new ClassBoardCast();
            cUserQuit.UserQuit();
            Application.Exit();
        }

        //双击好有时
        private void chatShow_DoubleClickSubItem(object sender, ChatListEventArgs e)
        {
            ChatListSubItem item = e.SelectSubItem;
            item.IsTwinkle = false;

            //bool isFormexist;
            string windowsName = "与 " + item.NicName + " 对话中";
            IntPtr handle = NativeMethods.FindWindow(null, windowsName);
            if (handle != IntPtr.Zero)
            {
                Form frm = (Form)Form.FromHandle(handle);
                frm.Activate();
            }
            else
            {
                //ipSend为从列表中取出，要发送的对象的IP
                string ipSend = item.IpAddress;
                string nameSend = item.DisplayName;
                string idSend = item.NicName;
                string mesSend = string.Empty;
                FrmChat fChat = new FrmChat(ipSend, nameSend, idSend, mesSend);
                //fChat.Name = lvItem.SubItems[0].Text;
                fChat.Text = "与 " + item.NicName + " 对话中";
                fChat.Show();
            }

        }

        //QQ主菜单悬浮时
        private void toolQQMenu_MouseEnter(object sender, EventArgs e)
        {
            toolQQMenu.Image = Properties.Resources.menu_btn_highlight;
        }

        //QQ主菜单离开时
        private void toolQQMenu_MouseLeave(object sender, EventArgs e)
        {
            if (!QQMenu.Visible)
            {
                toolQQMenu.Image = Properties.Resources.menu_btn_normal;
            }
        }

        //打开QQ主菜单
        private void toolQQMenu_Click(object sender, EventArgs e)
        {
            QQMenu.Show(SkToolCdTwo, new Point(3, -2), ToolStripDropDownDirection.AboveRight);
            toolQQMenu.Image = Properties.Resources.menu_btn_highlight;
            toolQQMenu.Checked = true;
        }

        //QQ主菜单关闭时
        private void QQMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            toolQQMenu.Image = Properties.Resources.menu_btn_normal;
            toolQQMenu.Checked = false;
        }

        //窗体重绘时
        private void FrmMain_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            SolidBrush sb = new SolidBrush(Color.FromArgb(100, 255, 255, 255));
            g.FillRectangle(sb, new Rectangle(new Point(1, Height - 60), new Size(Width - 2, 60)));
        }

        //悬浮至头像时
        private FrmUserInformation frm;
        private void chatShow_MouseEnterHead(object sender, ChatListEventArgs e)
        {
            //窗体的TopLeft值
            int UserTop = this.Top + this.chatShow.Top + (e.MouseOnSubItem.HeadRect.Y - chatShow.chatVScroll.Value);
            int UserLeft = this.Left - 279 - 5;
            //屏幕不包括任务栏的高度
            int PH = Screen.GetWorkingArea(this).Height;
            //判断是否超过屏幕高度
            if (UserTop + 181 > PH)
            {
                UserTop = PH - 181 - 5;
            }
            //判断是否小于屏幕左边
            if (UserLeft < 0)
            {
                UserLeft = this.Right + 5;
            }
            //窗体不为空传值
            if (frm != null)
            {
                frm.Item = e.MouseOnSubItem;
                frm.Location = new Point(UserLeft, UserTop);
                frm.Show();
            }
            else  //窗体为空New一个
            {
                frm = new FrmUserInformation(e.MouseOnSubItem, new Point(UserLeft, UserTop));
                frm.Show();
            }
        }

        //离开头像时
        private void chatShow_MouseLeaveHead(object sender, ChatListEventArgs e)
        {
            Thread.Sleep(100);
            if (frm != null && !frm.Bounds.Contains(Cursor.Position))
            {
                frm.Hide();
            }
        }

        //选择状态
        private void btnState_Click(object sender, EventArgs e)
        {
            MenuState.Show(btnState, new Point(0, btnState.Height), ToolStripDropDownDirection.Right);
        }

        //状态选择项
        private void Item_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem Item = (ToolStripMenuItem)sender;
            btnState.Image = Item.Image;
            btnState.Tag = Item.Tag;
            UserItem.Status = (ChatListSubItem.UserStatus)Convert.ToInt32(btnState.Tag);
        }

        //窗体大小改变时
        private void FrmMain_SizeChanged(object sender, EventArgs e)
        {
            Size sizef = TextRenderer.MeasureText(lblName.SetStrLeng(lblName.Text, lblName.Font, lblName.Width), lblName.Font);
            btnState.Left = sizef.Width + lblName.Left + 10;
            lblLv.Left = btnState.Left + 23;
        }

        //背景更改时
        private void FrmMain_BackChanged(object sender, BackEventArgs e)
        {
            BackLayout = true;
        }

        //用户名悬浮时
        private void lblName_MouseEnter(object sender, EventArgs e)
        {
            //窗体的TopLeft值
            int UserTop = this.Top + lblName.Top;
            int UserLeft = this.Left - 279 - 5;
            //屏幕不包括任务栏的高度
            int PH = Screen.GetWorkingArea(this).Height;
            //判断是否超过屏幕高度
            if (UserTop + 181 > PH)
            {
                UserTop = PH - 181 - 5;
            }
            //判断是否小于屏幕左边
            if (UserLeft < 0)
            {
                UserLeft = this.Right + 5;
            }
            //窗体不为空传值
            if (frm != null)
            {
                frm.Item = UserItem;
                frm.Location = new Point(UserLeft, UserTop);
                frm.Show();
            }
            else  //窗体为空New一个
            {
                frm = new FrmUserInformation(UserItem, new Point(UserLeft, UserTop));
                frm.Show();
            }
        }

        //用户名离开时
        private void lblName_MouseLeave(object sender, EventArgs e)
        {
            Thread.Sleep(100);
            if (frm != null && !frm.Bounds.Contains(Cursor.Position))
            {
                frm.Hide();
            }
        }
    }
}