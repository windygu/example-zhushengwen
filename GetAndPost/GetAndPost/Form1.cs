using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace GetAndPost
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public  string GetUrltoHtml(string Url)
        {
            StringBuilder content = new StringBuilder();
            try
            {
                // 与指定URL创建HTTP请求
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";
                request.Method = "GET";
                request.Accept = "*/*";

                //不保持连接
                request.KeepAlive = true;
                // 获取对应HTTP请求的响应
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                // 获取响应流
                Stream responseStream = response.GetResponseStream();
                // 对接响应流(以"GBK"字符集)
                //StreamReader sReader = new StreamReader(responseStream, Encoding.GetEncoding("gb2312"));
                StreamReader sReader = radioButton4.Checked?new StreamReader(responseStream, Encoding.GetEncoding("gb2312")):new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                // 开始读取数据
                Char[] sReaderBuffer = new Char[256];
                int count = sReader.Read(sReaderBuffer, 0, 256);
                while (count > 0)
                {
                    String tempStr = new String(sReaderBuffer, 0, count);
                    content.Append(tempStr);
                    count = sReader.Read(sReaderBuffer, 0, 256);
                }
                // 读取结束
                sReader.Close();
                textBox1.AddItem();
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return content.ToString();
        }
        public  string PostUrltoHtml(string url, string postString,string method="POST")
        {
            string srcString = "";
            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                byte[] postData = Encoding.Default.GetBytes(postString);
                byte[] responseData = webClient.UploadData(url, method, postData);
                srcString = Encoding.Default.GetString(responseData);
                textBox1.AddItem();
            }
            catch (Exception e)
            {

                return e.Message;
            }

            return srcString;

        }
        private void EnterPressed(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                GetUrlString();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {

            GetUrlString();
        }

        private void GetUrlString()
        {
            if (radioButton2.Checked)
            {
                richTextBox1.Text = PostUrltoHtml(textBox1.Controls[0].Text, textBox2.Text);
            }
            else
                richTextBox1.Text = GetUrltoHtml(textBox1.Controls[0].Text + "?" + textBox2.Text);
        }
    }
}
