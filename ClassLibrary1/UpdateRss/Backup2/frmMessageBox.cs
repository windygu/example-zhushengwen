using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;

namespace SoukeyNetget
{
    public partial class frmMessageBox : Form
    {
        //窗体关闭延迟时间，默认8秒
        private int DelayTime = 8000;
        private string m_Title;

        private ResourceManager rm;

        public frmMessageBox()
        {
            InitializeComponent();

            rm = new ResourceManager("SoukeyNetget.Resources.globalUI", Assembly.GetExecutingAssembly());
        }

        public void MessageBox(string Mess, string Title, MessageBoxButtons but, MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Question :
                    m_Title = rm.GetString("MessageboxQuaere");
                    
                    //启动定时器
                    this.timer1.Enabled = true;
                    this.labDelay.Visible = true;

                    break;
                case MessageBoxIcon.Error :
                    m_Title = rm.GetString("MessageboxError");
                    break;
                case MessageBoxIcon.Information :
                    m_Title = rm.GetString("MessageboxInfo");
                    break ;
                default :
                    m_Title = rm.GetString("MessageboxInfo");
                    break ;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (DelayTime == 0 || DelayTime < 0)
            {
                this.DialogResult = DialogResult.Yes;
                this.Close();
            }
            else
                DelayTime = DelayTime - 1000;

            string s = DelayTime.ToString().Substring(0, 1) + " " + rm.GetString("Label21");
            this.Text = m_Title + " " + rm.GetString ("Info81") + s;
            this.labDelay.Text = rm.GetString ("Info81") + s + " " + rm.GetString("Info82");
        }

        private void frmMessageBox_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }
    }
}