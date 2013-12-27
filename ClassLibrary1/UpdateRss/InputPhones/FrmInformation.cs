using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CCWin;
using CCWin.Win32;
using CCWin.Win32.Const;

namespace InputPhones
{
    public partial class FrmInformation : CCSkinMain
    {
        public FrmInformation()
        {
            InitializeComponent();
        }

        //窗口加载时
        private void FrmInformation_Load(object sender, EventArgs e)
        {
            //初始化窗口出现位置
            //Point p = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width, Screen.PrimaryScreen.WorkingArea.Height - this.Height);
            //this.PointToScreen(p);
            //this.Location = p;
            txtId.Text = StringTool.MyClass.connectionString;
            waterTextBox1.Text = Form1.rssurl;
            NativeMethods.AnimateWindow(this.Handle, 500, AW.AW_BLEND);//开始窗体动画
        }

        //倒计时三秒关闭弹出窗
        private void timShow_Tick(object sender, EventArgs e)
        {
            //鼠标不在窗体内时
            if (!this.Bounds.Contains(Cursor.Position))
            {
                this.Close();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            StringTool.MyClass.connectionString = txtId.Text;
            Close();
        }
    }
}
