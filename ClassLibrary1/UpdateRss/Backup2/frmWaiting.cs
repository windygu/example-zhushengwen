using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

///���ܣ�������ʱ������ʱ��ʾ�ĵȴ����壬��ǰδ��
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
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