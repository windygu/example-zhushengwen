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
using StringTool;
using System.Xml;
namespace GetAndPost
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public string GetUrltoHtml(string Url)
        {
            StringBuilder content = new StringBuilder();
            try
            {
                // 与指定URL创建HTTP请求
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";
                request.Method = "GET";
                request.Accept = "*/*";
                // request.CookieContainer = new CookieContainer(3);
                //  request.CookieContainer.Add(new Cookie());
                //不保持连接
                request.KeepAlive = true;
                // 获取对应HTTP请求的响应
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                // 获取响应流
                Stream responseStream = response.GetResponseStream();

                // 对接响应流(以"GBK"字符集)
                //StreamReader sReader = new StreamReader(responseStream, Encoding.GetEncoding("gb2312"));
                StreamReader sReader = radioButton4.Checked ? new StreamReader(responseStream, Encoding.GetEncoding("gb2312")) : new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
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
                //textBox1.AddItem();
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return content.ToString();
        }
        public string PostUrltoHtml(string url, string postString, string method = "POST")
        {
            string srcString = "";
            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                byte[] postData = Encoding.Default.GetBytes(postString);
                byte[] responseData = webClient.UploadData(url, method, postData);
                srcString = Encoding.Default.GetString(responseData);
                // textBox1.AddItem();
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
                GetUrlString(textBox1.Controls[0].Text);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.MyText == "")
            {
                MessageBox.Show("请输入Webservice地址！");
                return;
            }
            string strs = (string)MyClass.DynamicInvokeWebService(textBox1.MyText, "onLineQuery", new object[] { });
            richTextBox1.Text = strs;
            string[] strns = strs.Split('#');
            comboBox1.DataSource = strns;
        }

        private void GetUrlString(string url)
        {
            if (radioButton2.Checked)
            {
                richTextBox1.Text = PostUrltoHtml(url, "");
            }
            else
                richTextBox1.Text = GetUrltoHtml(url);
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            //  599, 445
            //      570, 279
            //          9, 120
            Size szo1 = new Size(599, 445);
            Size szo2 = new Size(570, 239);
            Size sz = (szo1 - szo2);
            richTextBox1.Size = this.Size - (sz);
        }



        private void button3_Click(object sender, EventArgs e)
        {
            //if (comboBox1.Text == "")
            //{
            //    MessageBox.Show("请先获取设备信息！");
            //    return;
            //}
            string strs = MyClass.ReadTextFile("d.xml");//(string)MyClass.DynamicInvokeWebService(textBox1.MyText, "queryProcessInfo", new object[] {comboBox1.Text});
            richTextBox1.Text = strs;
            System.Xml.XmlDocument xdoc = new System.Xml.XmlDocument();
            StringReader sr = new StringReader(strs);
            xdoc.Load(sr);
            XmlNodeList xnl = xdoc.SelectNodes("/Root/processInfos/process");
            foreach (XmlNode xn in xnl)
            {
                try
                {
                    string pid = xn.SelectSingleNode("pid").InnerText;
                    string pname = xn.SelectSingleNode("pname").InnerText;
                    string user = xn.SelectSingleNode("user").InnerText;
                    string cpu = xn.SelectSingleNode("cpu").InnerText;
                    string memory = xn.SelectSingleNode("memory").InnerText;
                    string path = xn.SelectSingleNode("path").InnerText;
                }
                catch (Exception)
                {

                    throw;
                }

            }
        }
        HttpClient hc = new HttpClient();

        /**/
        /// <summary>
        /// 支持 Session 和 Cookie 的 WebClient。
        /// </summary>
        public class HttpClient : WebClient
        {
            // Cookie 容器
            private CookieContainer cookieContainer;

            /**/
            /// <summary>
            /// 创建一个新的 WebClient 实例。
            /// </summary>
            public HttpClient()
            {
                this.cookieContainer = new CookieContainer();
            }

            /**/
            /// <summary>
            /// 创建一个新的 WebClient 实例。
            /// </summary>
            /// <param name="cookie">Cookie 容器</param>
            public HttpClient(CookieContainer cookies)
            {
                this.cookieContainer = cookies;
            }

            /**/
            /// <summary>
            /// Cookie 容器
            /// </summary>
            public CookieContainer Cookies
            {
                get { return this.cookieContainer; }
                set { this.cookieContainer = value; }
            }

            /**/
            /// <summary>
            /// 返回带有 Cookie 的 HttpWebRequest。
            /// </summary>
            /// <param name="address"></param>
            /// <returns></returns>
            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest request = base.GetWebRequest(address);
                if (request is HttpWebRequest)
                {
                    HttpWebRequest httpRequest = request as HttpWebRequest;
                    httpRequest.CookieContainer = cookieContainer;
                }
                return request;
            }

            #region 封装了PostData, GetSrc 和 GetFile 方法#region 封装了PostData, GetSrc 和 GetFile 方法
            /**/
            /// <summary>
            /// 向指定的 URL POST 数据，并返回页面
            /// </summary>
            /// <param name="uriString">POST URL</param>
            /// <param name="postString">POST 的 数据</param>
            /// <param name="postStringEncoding">POST 数据的 CharSet</param>
            /// <param name="dataEncoding">页面的 CharSet</param>
            /// <returns>页面的源文件</returns>
            public string PostData(string uriString, string postString, string postStringEncoding, string dataEncoding, out string msg)
            {
                try
                {
                    // 将 Post 字符串转换成字节数组
                    byte[] postData = Encoding.GetEncoding(postStringEncoding).GetBytes(postString);
                    this.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    // 上传数据，返回页面的字节数组
                    byte[] responseData = this.UploadData(uriString, "POST", postData);
                    // 将返回的将字节数组转换成字符串(HTML);
                    string srcString = Encoding.GetEncoding(dataEncoding).GetString(responseData);
                    srcString = srcString.Replace("\t", "");
                    srcString = srcString.Replace("\r", "");
                    srcString = srcString.Replace("\n", "");
                    msg = string.Empty;
                    return srcString;
                }
                catch (WebException we)
                {
                    msg = we.Message;
                    return string.Empty;
                }
            }

            /**/
            /// <summary>
            /// 获得指定 URL 的源文件
            /// </summary>
            /// <param name="uriString">页面 URL</param>
            /// <param name="dataEncoding">页面的 CharSet</param>
            /// <returns>页面的源文件</returns>
            public string GetSrc(string uriString, string dataEncoding, out string msg)
            {
                try
                {
                    // 返回页面的字节数组
                    byte[] responseData = this.DownloadData(uriString);
                    // 将返回的将字节数组转换成字符串(HTML);
                    string srcString = Encoding.GetEncoding(dataEncoding).GetString(responseData);
                    srcString = srcString.Replace("\t", "");
                    srcString = srcString.Replace("\r", "");
                    srcString = srcString.Replace("\n", "");
                    msg = string.Empty;
                    return srcString;
                }
                catch (WebException we)
                {
                    msg = we.Message;
                    return string.Empty;
                }
            }

            /**/
            /// <summary>
            /// 从指定的 URL 下载文件到本地
            /// </summary>
            /// <param name="uriString">文件 URL</param>
            /// <param name="fileName">本地文件的完成路径</param>
            /// <returns></returns>
            public bool GetFile(string urlString, string fileName, out string msg)
            {
                try
                {
                    this.DownloadFile(urlString, fileName);
                    msg = string.Empty;
                    return true;
                }
                catch (WebException we)
                {
                    msg = we.Message;
                    return false;
                }
            }
            #endregion
        }


        private void button2_Click(object sender, EventArgs e)
        {

        }
        public static long NowToLong()
        {
            long timeTricks = (DateTime.Now.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000 - 8 * 3600000;
            return timeTricks;
        }
    }
}
