using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

///功能：当处理长时间任务时显示的等待窗体，当前未用
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace SoukeyNetget
{
    public partial class frmWaiting : Form
    {
        public frmWaiting()
        {
            InitializeComponent();
        }

        public frmWaiting(string Info)
        {
            InitializeComponent();
            this.labTxt.Text = Info;
            Application.DoEvents();
        }

      
    }
}