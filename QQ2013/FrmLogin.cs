using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CCWin;
using CCWin.SkinControl;
using CCWin.SkinClass;
using System.Runtime.InteropServices;
using System.Threading;
using CC2013.Properties;
using System.Diagnostics;

namespace CC2013
{
    public partial class FrmLogin : CCSkinMain
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        //窗口加载时
        private void FrmLogin_Load(object sender, EventArgs e)
        {
            int H = DateTime.Now.Hour;
            this.Back =
                H > 5 & H <= 11 ? Resources.morning :     //早上
                H > 11 & H <= 16 ? Resources.noon :       //中午
                H > 16 & H <= 19 ? Resources.afternoon :      //下午
                Resources.night;        //晚上
            //加载Id下拉框
            SetId();
        }

        //加载Id下拉框
        private void SetId()
        {
            Random rnd = new Random();
            for (int i = 0; i < 6; i++)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.AutoSize = false;
                item.Size = new System.Drawing.Size(182, 45);
                item.Tag = rnd.Next(1000, 10000).ToString();
                item.Text = "威廉乔克斯_汀\n" + rnd.Next(1000, 10000).ToString();
                item.Image = Image.FromFile("head/" + rnd.Next(1, 11) + ".png");
                item.Click += new EventHandler(item_Click);
                MenuId.Height += 45;
                MenuId.Items.Add(item);
            }
        }

        //状态菜单中的Item选择事件
        void item_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            txtId.Text = item.Tag.ToString();
            pnlImgTx.BackgroundImage = item.Image;
        }

        //悬浮时
        private void Control_MouseEnter(object sender, EventArgs e)
        {
            Control txt = (Control)sender;
            SkinPanel pnl = (SkinPanel)txt.Parent;
            pnl.ControlState = ControlState.Hover;
        }

        //离开时
        private void Control_MouseLeave(object sender, EventArgs e)
        {
            Control txt = (Control)sender;
            SkinPanel pnl = (SkinPanel)txt.Parent;
            pnl.ControlState = ControlState.Normal;
        }

        //托盘图标双击显示
        private void tuopan_DoubleClick(object sender, EventArgs e)
        {
            if (main != null)
            {
                main.Show();
            }
            else
            {
                this.Show();
            }
        }

        //登录事件
        FrmMain main;
        private void btnDl_Click(object sender, EventArgs e)
        {
            if (txtId.Text.Length == 0 || txtPwd.Text.Length == 0) { return; }
            btnDl.Enabled = btnDuoId.Enabled = btnSandeng.Enabled = false;
            imgLoadding.Visible = true;
            timShow.Start();
        }

        //选择状态
        private void btnState_Click(object sender, EventArgs e)
        {
            MenuState.Show(this.Left + pnlTx.Left + pnlImgTx.Left + btnState.Left, this.Top + pnlTx.Top + pnlImgTx.Top + btnState.Top + btnState.Height + 5);
        }

        //状态选择项
        private void Item_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem Item = (ToolStripMenuItem)sender;
            btnState.Image = Item.Image;
            btnState.Tag = Item.Tag;
        }

        //账号下拉框按钮事件
        Image BtnNorml;
        private void btnId_MouseDown(object sender, MouseEventArgs e)
        {
            MenuId.Show(pnlId, 1, pnlId.Height + 1);
            BtnNorml = btnId.NormlBack;
            btnId.NormlBack = btnId.DownBack;
            btnId.Enabled = false;

        }

        //账号下拉菜单关闭时
        private void MenuId_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            btnId.Enabled = true;
            btnId.NormlBack = BtnNorml;
            pnlId.ControlState = ControlState.Normal;
        }

        //软键盘
        private void txtPwd_IconClick(object sender, EventArgs e)
        {
            PassKey pass = new PassKey(this.Left + txtPwd.Left - 25, this.Top + txtPwd.Bottom, txtPwd);
            pass.Show(this);
        }

        //自动登录
        private void chkZdLogin_CheckedChanged(object sender, EventArgs e)
        {
            chkMima.Checked = chkZdLogin.Checked ? true : chkMima.Checked;
        }

        //记住密码
        private void chkMima_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkMima.Checked && chkZdLogin.Checked)
            {
                chkZdLogin.Checked = false;
            }
        }

        private void timShow_Tick(object sender, EventArgs e)
        {
            this.Hide();
            tuopan.Text = String.Format("QQ：{0}({1})", txtId.Text, txtId.Text);
            main = new FrmMain(txtId.Text, pnlImgTx.BackgroundImage, btnState);
            main.Show();
            timShow.Stop();
        }
        
        //关闭
        private void toolExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //找回密码
        private void btnMima_Click(object sender, EventArgs e)
        {
            Process.Start("https://aq.qq.com/cn2/findpsw/pc/pc_find_pwd_input_account?source_id=1003&ptlang=2052&aquin=");
        }

        //注册账号
        private void btnZc_Click(object sender, EventArgs e)
        {
            Process.Start("http://zc.qq.com/chs/index.html?from=client&ptlang=2052&regkey=&ADUIN=0&ADSESSION=0&ADTAG=CLIENT.QQ.5065_NewAccount_Btn.0&ADPUBNO=26154");
        }
    }
}
