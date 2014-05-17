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
            Text = "iphone服务预约 -- " + no;
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


        private void Form1_SysBottomClick(object sender)
        {
            FrmInformation fi = new FrmInformation();
            fi.ShowDialog();
        }
       


        private void skinButtom1_Click_1(object sender, EventArgs e)
        {
            if (webBrowser1.Document.GetElementById("accountname") != null)
            {
                webBrowser1.Document.GetElementById("accountname").SetAttribute("value", FrmInformation.user); ;
                webBrowser1.Document.GetElementById("accountpassword").SetAttribute("value", FrmInformation.pass);
                webBrowser1.Document.GetElementById("signInHyperLink").InvokeMember("click");
            }
            ////
            //IEBrowser ie = new IEBrowser ( this.webBrowser1 );
            ////string showMessageScript = "";
            ////ie.InstallScript ( showMessageScript );
            //ie.ExecuteScript("$('#uid').val('"+user+"');");
            
            //skinButtom1.Enabled = false;
            //skinButtom2.Enabled = true;
        }

        private void skinButtom2_Click(object sender, EventArgs e)
        {
            if (webBrowser1.Url == null) return;
            if (webBrowser1.Url.ToString() != "http://safe.jd.com/findPwd/index.action")
            {
                webBrowser1.Navigate("http://safe.jd.com/findPwd/index.action");
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

        }
        int count = 0;
        public void msg(string abc)
        {
            //MessageBox.Show(abc);
            if (webBrowser1.Document.GetElementById("username") != null)
            {
                phone = abc;
                webBrowser1.Document.GetElementById("username").SetAttribute("value", abc);
                webBrowser1.Document.GetElementById("authCode").Focus();
            }
        }

        private void webBrowser2_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            skinButtom1.Enabled = true;

            if (webBrowser1.Url.ToString().Contains("http://concierge.apple.com/geniusbar/R484"))
            {

                IEBrowser ie = new IEBrowser(this.webBrowser1);

                ie.ExecuteScript("if($('#serviceType_iPhone').length){$('#serviceType_iPhone').trigger('click');$('#fwdButtonC').trigger('click');}");
                ie.ExecuteScript("if($('a[href=#]').text()=='进行 Genius Bar 天才吧预约')$('a[href=#]').trigger('click');");
            }
            if (webBrowser1.Url.ToString() == ("http://concierge.apple.com/geniusbar/R484/timeslots"))
            {
                             IEBrowser ie = new IEBrowser(this.webBrowser1);

                             ie.ExecuteScript("if($('.notimesavailable:visible').length)location.href='https://idmsa.apple.com/IDMSWebAuth/login.html?appIdKey=990d5c9e38720f4e832a8009a0fe4cad7dd151f99111dbea0df5e2934f267ec8&language=CN-zh&segment=R484&grpcode=g001&paramcode=h006&path=%2Fgeniusbar%2FR484%2Fsignin%2Fack&path2=%2Fgeniusbar%2FR484%2Fsignin%2Fack';else{$('#timeslotC').click();$('#fwdButtonC').click();}");
                

            }

            if (webBrowser1.Url.ToString().Contains("https://idmsa.apple.com/IDMSWebAuth/login.html?appIdKey=990d5c9e38720f4e832a8009a0fe4cad7dd151f99111dbea0df5e2934f267ec8&language=CN-zh&segment=R484&grpcode=g001&paramcode=h006&path=/geniusbar/R484/signin/ack&path2=/geniusbar/R484/signin/ack"))
            {
                Thread.Sleep(2000);
                skinButtom1_Click_1(null, null);
   
            }
            if (webBrowser1.Url.ToString().Contains("/retail/"))
            {
                webBrowser1.Navigate("https://idmsa.apple.com/IDMSWebAuth/login.html?appIdKey=990d5c9e38720f4e832a8009a0fe4cad7dd151f99111dbea0df5e2934f267ec8&language=CN-zh&segment=R484&grpcode=g001&paramcode=h006&path=%2Fgeniusbar%2FR484%2Fsignin%2Fack&path2=%2Fgeniusbar%2FR484%2Fsignin%2Fack");
            }
            if (webBrowser1.Url.ToString().Contains("reservationConfirmation"))
            {
                count++;
                SetText("恭喜：你已经成功抢到"+count+"张票了！");
                webBrowser1.Navigate("https://idmsa.apple.com/IDMSWebAuth/login.html?appIdKey=990d5c9e38720f4e832a8009a0fe4cad7dd151f99111dbea0df5e2934f267ec8&language=CN-zh&segment=R484&grpcode=g001&paramcode=h006&path=%2Fgeniusbar%2FR484%2Fsignin%2Fack&path2=%2Fgeniusbar%2FR484%2Fsignin%2Fack");
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


        private void webBrowser2_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            //IHTMLWindow2 win = (IHTMLWindow2)(sender as WebBrowser).Document.Window.DomWindow;
            //string s = @"function confirm() {";
            //s += @"return true;";
            //s += @"}";
            //s += @"function alert() {}";
            //win.execScript(s, "javascript");
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
            string str = GetUrltoHtml("http://schoolbuy.net/crit/ap.php");
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

        private void skinButtom3_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://idmsa.apple.com/IDMSWebAuth/login.html?appIdKey=990d5c9e38720f4e832a8009a0fe4cad7dd151f99111dbea0df5e2934f267ec8&language=CN-zh&segment=R484&grpcode=g001&paramcode=h006&path=%2Fgeniusbar%2FR484%2Fsignin%2Fack&path2=%2Fgeniusbar%2FR484%2Fsignin%2Fack");
        }

        private void skinButtom2_Click_1(object sender, EventArgs e)
        {
           
        }
    }

}
