using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CCWin;
using CCWin.SkinControl;

namespace CC2013
{
    public partial class FrmUserInformation : CCSkinMain
    {
        private ChatListSubItem item;
        private Point point;
        public FrmUserInformation(ChatListSubItem Item,Point point)
        {
            InitializeComponent();
            this.Item = Item;
            this.point = point;
        }

        public ChatListSubItem Item 
        {
            get 
            { 
                return item; 
            }
            set 
            {
                if (item != value)
                {
                    item = value;
                    lblName.Text = item.NicName;
                    lblQm.Text = item.PersonalMsg;
                }
            }
        }

        //悬浮至好友Q名时
        private void lblChatName_MouseEnter(object sender, EventArgs e)
        {
            lblName.Font = new Font("微软雅黑", 16F, FontStyle.Underline);
        }

        //离开好友Q名时
        private void lblChatName_MouseLeave(object sender, EventArgs e)
        {
            lblName.Font = new Font("微软雅黑", 16F);
        }

        //窗口加载时
        private void FrmInformation_Load(object sender, EventArgs e)
        {
            //初始化窗口出现位置
            this.Location = point;
        }

        //窗体重绘时
        private void FrmUserInformation_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            SolidBrush sb = new SolidBrush(Color.FromArgb(100, 255, 255, 255));
            g.FillRectangle(sb, new Rectangle(new Point(1, Height - 103), new Size(Width - 2, 80)));
        }

        //计时器
        private bool flag = false;
        private void timShow_Tick(object sender, EventArgs e)
        {
            //鼠标不在窗体内时
            if (!this.Bounds.Contains(Cursor.Position) && flag)
            {
                this.Hide();
                flag = false;
            }
            else if (this.Bounds.Contains(Cursor.Position))
            {
                flag = true;
            }
        }
    }
}
