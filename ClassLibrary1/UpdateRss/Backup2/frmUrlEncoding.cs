using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SoukeyNetget
{
    public partial class frmUrlEncoding : Form
    {
        public frmUrlEncoding()
        {
            InitializeComponent();
        }

        private void frmUrlEncoding_Load(object sender, EventArgs e)
        {
            this.comWebCode.Items.Add("gb2312");
            this.comWebCode.Items.Add("UTF-8");
            this.comWebCode.Items.Add("gbk");
            this.comWebCode.Items.Add("big5");

            this.comWebCode.SelectedIndex = 0;
        }

        private void frmUrlEncoding_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (this.comWebCode.SelectedItem.ToString ())
            {
                case "gb2312":
                    this.textBox2.Text = System.Web.HttpUtility.UrlEncode(this.textBox1.Text,Encoding.GetEncoding("gb2312"));
                    break;
                case "UTF-8":
                    this.textBox2.Text = System.Web.HttpUtility.UrlEncode(this.textBox1.Text, Encoding.UTF8);
                    break;
                case "gbk":
                    this.textBox2.Text = System.Web.HttpUtility.UrlEncode(this.textBox1.Text, Encoding.GetEncoding("gbk"));
                    break;
                case "big5":
                    this.textBox2.Text = System.Web.HttpUtility.UrlEncode(this.textBox1.Text, Encoding.GetEncoding("big5"));
                    break;
            }
                    
        
        }

        private void button2_Click(object sender, EventArgs e)
        {
            switch (this.comWebCode.SelectedItem.ToString())
            {
                case "gb2312":
                    this.textBox2.Text = System.Web.HttpUtility.UrlDecode(this.textBox1.Text, Encoding.GetEncoding("gb2312"));
                    break;
                case "UTF-8":
                    this.textBox2.Text = System.Web.HttpUtility.UrlDecode(this.textBox1.Text, Encoding.UTF8);
                    break;
                case "gbk":
                    this.textBox2.Text = System.Web.HttpUtility.UrlDecode(this.textBox1.Text, Encoding.GetEncoding("gbk"));
                    break;
                case "big5":
                    this.textBox2.Text = System.Web.HttpUtility.UrlDecode(this.textBox1.Text, Encoding.GetEncoding("big5"));
                    break;
            }
                    
            
        }
    }
}