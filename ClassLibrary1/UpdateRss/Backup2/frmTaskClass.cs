using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Resources;


///���ܣ��ɼ����������Ϣ����
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget
{
    public partial class frmTaskClass : Form
    {
        public delegate void ReturnTaskClass(int TaskClassID, string TaskClassName,string TaskClassPath);

        public ReturnTaskClass RTaskClass;

        private string  DefaultPath="";

        private ResourceManager rm;

        private bool m_IsHoldClose = false;

        public frmTaskClass()
        {
            InitializeComponent();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            m_IsHoldClose = false;
            this.Dispose();
        }

        private void frmTaskClass_Load(object sender, EventArgs e)
        {
            rm = new ResourceManager("SoukeyNetget.Resources.globalUI", Assembly.GetExecutingAssembly());

            this.textBox2.Text = Program.getPrjPath() + "tasks\\";
            DefaultPath = this.textBox2.Text;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            int TaskClassID = 0;

            if (this.textBox1.Text.Trim().ToString() == "")
            {
                MessageBox.Show(rm.GetString ("Info88"), rm.GetString("MessageboxError"),MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_IsHoldClose = true;
                this.textBox1.Focus();
                return;
            }

            try
            {
                Task.cTaskClass cTClass = new Task.cTaskClass();

                TaskClassID = cTClass.AddTaskClass(this.textBox1.Text.Trim (), this.textBox2.Text);
                cTClass = null;
            }
            catch (cSoukeyException ex)
            {
                MessageBox.Show(ex.Message, rm.GetString ("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_IsHoldClose = true;
                return;
            }
            catch (System.Exception  ex)
            {
                MessageBox.Show(ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_IsHoldClose = true;
                return;
            }


            RTaskClass(TaskClassID, this.textBox1.Text, this.textBox2.Text);

            this.Dispose();
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    folderBrowserDialog1.SelectedPath = Program.getPrjPath();
        //    folderBrowserDialog1.ShowNewFolderButton = true;
        //    folderBrowserDialog1.Description = "��ѡ���������洢��Ŀ¼";
        //    if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
        //    {
        //        this.textBox2.Text= folderBrowserDialog1.SelectedPath + "\\";
        //        DefaultPath = this.textBox2.Text;

        //    }
        //}

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.textBox2.Text = DefaultPath + this.textBox1.Text;
            this.textBox2.Select(this.textBox2.Text.Length, 0);
            this.textBox2.ScrollToCaret();
        }

        private void frmTaskClass_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        private void frmTaskClass_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing)
            {
                if (m_IsHoldClose == true)
                    e.Cancel = true;
            }
        }

      
    }
}