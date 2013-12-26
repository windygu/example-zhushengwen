using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CCWin;

namespace CC2013
{
    public partial class FrmCountenance : CCSkinMain
    {
        private Point point;
        public FrmCountenance(Point point)
        {
            InitializeComponent();
            this.point = point;
        }

        //窗口加载时
        private void FrmCountenance_Load(object sender, EventArgs e)
        {
            //初始化窗口出现位置
            this.Location = point;
        }
    }
}
