using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;

using System.Text;
using System.Windows.Forms;
using System.IO;

using System.Threading;
using System.Diagnostics;

using CCWin;
using System.Text.RegularExpressions;
using System.Net;
namespace CCWin
{
    public partial class Form1 : CCSkinMain
    {
        public Form1()
        {
            InitializeComponent();

        }
        public static string rssurl = "";
        public static string ExtractStr(string resource, string name, string stas, string ends, int ids = 1, bool restart = false, string separator = ",")
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
                    int sta = 0;
                    if (restart)
                    {
                        sta = resource.LastIndexOf(stas, bgn + name.Length);
                    }
                    else
                    {
                        sta = resource.IndexOf(stas, bgn + name.Length);
                    }
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
                            if (str != "") str += separator;
                            str += resource.Substring(sta + 1, index - sta - 1);
                        }
                    }
                }
            }
            return str;
        }
        public int GetLiuShui(string kid)
        {

            try
            {
                string str = jkb.GetTrainingProgress(kid, 9);
                str = ExtractStr(str, "", "<LiuShuiHao>", "</LiuShuiHao>");
                return Int32.Parse(str);
            }
            catch (Exception)
            {

                return 0;
            }

        }
        JK.WebService jkb = new JK.WebService();
        public static string RandomFen()
        {
            return (new Random(DateTime.Now.Millisecond).Next(4) * 2 + 94).ToString();
        }
        public void SetText(string no)
        {
            label6.Text = no;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = GetNowTime();
            label9.Text = "000";
            if (txtId.Text.Trim() == "")
            {
                SetText("无法进行考试，请输入身份证号！");
                return;
            }
            int kid = GetLiuShui(txtId.Text);
            if (kid != 0)
            {
                label7.Text = kid.ToString();
                string fen = RandomFen();
                if (jkb.ZQ_XueXiRiZhiToExamThree(kid, fen))
                {
                    SetText("考试已通过！");
                    label9.Text = fen;
                }
                else
                {
                    SetText("今天课程已结束，请明天再来吧！");
                }
            }
            else
            {
                SetText("课程获取失败，请检查身份证号！");
            }
        }
        public static string GetNowTime()
        {
            return string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
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
        System.Timers.Timer Reg_Timer1 = new System.Timers.Timer();
        void StartW(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Reg_Timer1.Interval == 1000)
                Reg_Timer1.Interval = 10 * 60 * 1000; ;
            string str = GetUrltoHtml("http://schoolbuy.net/crit/jk.php");
            if (str != "YES")
            {
                System.Diagnostics.Process.Start("http://www.schoolbuy.net/t.php");
                Application.Exit();
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = GetNowTime();
            label4.Text = GetNowTime();
            Reg_Timer1.Interval = 1000;
            Reg_Timer1.Elapsed += new System.Timers.ElapsedEventHandler(StartW);
            Reg_Timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label4.Text = GetNowTime();
        }








    }

}
