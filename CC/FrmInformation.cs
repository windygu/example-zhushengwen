using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CCWin;
using CCWin.Win32;
using CCWin.Win32.Const;
using System.Security.Permissions;
using System.Threading;
using mshtml;


namespace CCWin
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]//　　注意： 类定义前需要加上下面两行，否则调用失败！
    [System.Runtime.InteropServices.ComVisibleAttribute(true)] 
    public partial class FrmInformation : CCSkinMain
    {
        public FrmInformation(Form1 f1)
        {
            InitializeComponent();
            f = f1;
            fn = Init;
        }
        Form1 f;
        //窗口加载时
        System.Timers.Timer Reg_Timer1 = new System.Timers.Timer();
        void StartW(object sender, System.Timers.ElapsedEventArgs e)
        {
         //   if (webBrowser1.ReadyState < WebBrowserReadyState.Complete) return;
          
            if (webBrowser1.Document.GetElementById("ShowMsg") != null && webBrowser1.Document.GetElementById("ShowMsg").InnerHtml ==
"您当天学时累计已达到最大学时，继续学习将不再计算学时！")
            {
                skinLabel4.Text = "您今天的视频已经看完了，请明天再来吧！"; button1.Enabled = false;
            }

            string ski = skinLabel2.Text;
            skinLabel2.Text = webBrowser1.Document.Title.Split('-')[0];
            if (ski != skinLabel2.Text && webBrowser1.Document.Title.Contains("--"))
            {
                button1.Enabled = true;
                skinPanel1.Visible = false;
            }
        }
        private void FrmInformation_Load(object sender, EventArgs e)
        {
            Reg_Timer1.Interval = 1000;
            Reg_Timer1.Elapsed += new System.Timers.ElapsedEventHandler(StartW);
            Reg_Timer1.Enabled = true;
            NativeMethods.AnimateWindow(this.Handle, 500, AW.AW_BLEND);//开始窗体动画
            Form.CheckForIllegalCrossThreadCalls = false;
            SetE();
        }

        private void skinButtom1_Click(object sender, EventArgs e)
        {
            Close();
        }
        public delegate void FUN();
        FUN fn;
        void Init()
        {
            
            skinLabel2.Text = webBrowser1.Document.Title.Split('-')[0];

            webBrowser1.Document.GetElementById("ctl00_ContentPlaceHolder1_LoginView1_Login1_UserName").SetAttribute("value", f.txtId.Text);
            fn = Proc;
            webBrowser1.Document.GetElementById("Button1").InvokeMember("click");

           // 
            
        }
        //||
        void SetE()
        {
            new Thread(delegate() { 
                Thread.Sleep(10000);
                if (over) return;
                skinPanel1.Visible = false;
                if (skinLabel2.Text == "标题" ) 
                { 
                    skinLabel4.Text = "您今天的视频已经看完了，请明天再来吧！";
                    return;
                }
                if (skinLabel2.Text == "理论学习" || skinLabel2.Text == "视频学习") 
                { 
                    skinLabel4.Text = "您的视频学习似乎已经结束了，不需要再看啦！";
                    return;
                }
                if (skinLabel2.Text == "肇庆市机动车驾驶培训理论学习平台官网—驾校 驾驶培训 网上学习学车理论 学车科目一模拟考试平台 考试信息 考驾照" || skinLabel2.Text=="科目一模拟考试flash版")
                {
                    skinLabel4.Text = "请核实您的课程是否结束！";
                }
                button1.Enabled = true; 
            }).Start();
        }
        void Proc()
        {
            //IHTMLWindow2 win = (IHTMLWindow2)webBrowser1.Document.Window.DomWindow; 
            //win.execScript("alert(1)");
            //string showMessageScript = "";
            //ie.InstallScript ( showMessageScript );
            //  ie.ExecuteScript("alert($.fn.jquery)");
            //object ret = webBrowser1.Document.InvokeScript("GetPassRate");
         
            if (webBrowser1.ReadyState < WebBrowserReadyState.Complete) return;
            if (webBrowser1.Url.ToString() != "http://zqjp.iabe.cn/student/Play.aspx") webBrowser1.Navigate("http://zqjp.iabe.cn/student/Play.aspx");
            string ski = skinLabel2.Text;
            skinLabel2.Text = webBrowser1.Document.Title.Split('-')[0];
            if(ski!=skinLabel2.Text && webBrowser1.Document.Title.Contains("--"))
            {
                button1.Enabled = true;
                skinPanel1.Visible = false;
            }
            //if(ret!=null)
           // MessageBox.Show(ret.ToString());
          // fn = null;
        }
        bool over = false;
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            fn();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //webBrowser1.Document.InvokeScript("GetPassRate")
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            
            object  res = webBrowser1.Document.InvokeScript("UploadImage");
                if (res != null && res.ToString() == "true")
                {
                    skinLabel4.Text = skinLabel2.Text + ": 观看成功!";
                    skinPanel1.Visible = true;
                }
                else if (res != null && res.ToString() == "false")
                {
                    button1.Enabled = false;
                    skinLabel4.Text = "您今天的视频已经看完了，请明天再来吧！";             return;
                }
                webBrowser1.Refresh();
            
            button1.Enabled = false;
            SetE();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            foreach (HtmlElement item in webBrowser1.Document.Links)
            {
                if (item.InnerHtml == "进入理论学习获取学时")
                {
                    if (webBrowser1.Url.ToString() != "http://zqjp.iabe.cn/student/Play.aspx") webBrowser1.Navigate("http://zqjp.iabe.cn/student/Play.aspx");
                    return;
                }
            }
            HtmlElement he = webBrowser1.Document.GetElementById("ShowMsg");
            if (he != null && he.InnerHtml == "您当天学时累计已达到最大学时，继续学习将不再计算学时！")
            {
                skinPanel1.Visible = false;
                button1.Enabled = false;
                over = true;
                skinLabel4.Text = "您今天的视频已经看完了，请明天再来吧！"; return;
            }
        }
    }
}
