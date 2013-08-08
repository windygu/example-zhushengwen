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
using HtmlAgilityPack;
using System.Xml;
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
        public  string PostUrltoHtml(string url, string postString="",string method="POST")
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
                richTextBox1.Text = PostUrltoHtml(textBox1.Controls[0].Text);
            }
            else
                richTextBox1.Text = GetUrltoHtml(textBox1.Controls[0].Text );
        }

        private void button2_Click(object sender, EventArgs e)
        {

            HtmlWeb webClient = new HtmlWeb();
            string url1 = textBox1.Controls[0].Text;
            
            //HtmlAgilityPack.HtmlDocument doc = webClient.Load(url1);

            //HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("/#document");
            ////*[@id="w3c_most-recently"]
            //                                                 ///html/body/div/div[2]/div[3]/div[2]/div/div/div/div/div
            //foreach (HtmlNode node in nodes)
            //{
            //    Console.WriteLine(node.InnerText.Trim());
            //}

            //doc = null;
            //nodes = null;
            //webClient = null;

            //Console.WriteLine("Completed.");
            //Console.ReadLine();
            XmlDocument doc = new XmlDocument();
            doc.Load(url1);
            //使用xPath选择需要的节点
            XmlNodeList nodes = doc.SelectNodes("/rss/channel//item");
            if (nodes != null)
            {
                comboBox1.DataSource = null;
            }
            else
                return;
            comboBox1.Items.Clear();
            foreach (XmlNode item in nodes)
            {
                string title = item.SelectSingleNode("title").InnerText;
                string url = item.SelectSingleNode("link").InnerText;
                Console.WriteLine("{0} = {1}", title, url);
                comboBox1.Items.Add(new { Name = title, Value = url });
            }
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "Value";
            comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            string url =(string) comboBox1.SelectedItem.GetType().GetProperty("Value").GetValue(comboBox1.SelectedItem, null);
            if (radioButton2.Checked)
            {
                richTextBox1.Text = PostUrltoHtml(url);
            }
            else
                richTextBox1.Text = GetUrltoHtml(url);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "") return;
            string url1 = (string)comboBox1.SelectedItem.GetType().GetProperty("Value").GetValue(comboBox1.SelectedItem, null);

            //HtmlWeb webClient = new HtmlWeb();
            //webClient.AutoDetectEncoding = true;
            //HtmlAgilityPack.HtmlDocument doc = webClient.Load(url1);
            //HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("/html/body/div[4]/div/h3");
            string str = ExtractStr(richTextBox1.Text, "<title", ">", "<");
            string str1 = ExtractStr(richTextBox1.Text, "id=\"contentText\"", "<div", "div>");

            str1 = "<div" + str1 + "div>";

            richTextBox1.Text =  str1;
        }
        public static string Convert(string str)
        {
            byte[] bt = System.Text.Encoding.Default.GetBytes(str);
            return Encoding.GetEncoding("gb2312").GetString(bt);
        }
        public static string ExtractStr(string resource, string name, string stas, string ends, int ids = 1)
        {
            string str = "";
            int index = 0;
            //首先定位到名称
            while (ids != 0)
            {
                ids--;
                int bgn = resource.IndexOf(name, index);
                //如果未找到直接返回
                if (bgn != -1)
                {
                    //再次定位到开始字符
                    int sta = resource.IndexOf(stas, bgn + name.Length);
                    if (sta != -1)
                    {
                        //建立栈结构,开始字符和结束字符分别进行压栈出栈
                        int i = 1;
                        sta += stas.Length - 1;
                        index = sta + 1;
                        string tmps = "";
                        while (0 != i && index < resource.Length)
                        {

                            if (index + ends.Length > resource.Length) break;
                            tmps = resource.Substring(index, ends.Length);
                            if (tmps == ends)
                            {
                                i--;
                                if (0 == i) break;
                                index++;
                                continue;
                            }
                            if (index + stas.Length > resource.Length) break;
                            tmps = resource.Substring(index, stas.Length);
                            if (tmps == stas)
                            {
                                i++;
                            }
                            index++;
                        }
                        if (0 == i && index <= resource.Length)
                        {
                            str += resource.Substring(sta + 1, index - sta - 1);
                            if (ids != 0) str += ",";
                        }
                    }
                }
            }
            return str;
        }
    }
}
