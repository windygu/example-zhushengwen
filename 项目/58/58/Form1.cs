using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using StringTool;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using zoyobar.shared.panzer.web.ib;
namespace _58
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ConfigFile.Instanse.fileName = Path.Combine(OsPath, "config_58.ini");
            ConfigFileDES.Instanse.SetIConfig(ConfigFile.Instanse);
            Timers_Timer.Interval = 1000;
            Timers_Timer.Elapsed += new System.Timers.ElapsedEventHandler(StartR);

        }

        void ProcessUrl()
        {
            string str = MyClass.GetUrltoHtml("http://post.58.com/postindex.htm");
            string str1 = MyClass.ExtractStr(str, "<dl id=\"clist\">", "<dl", "</dl>", 1, true);
            string str2 = MyClass.ExtractStr(str1, "<dt", "<dt", "</dd>", 100, true);
            string[] str3s = str2.Split(',');
            if (str3s != null)
            {
                foreach (string item in str3s)
                {
                    string str21 = MyClass.ExtractStr(item, ">", ">", "<", 1, true);
                    string str3 = MyClass.ExtractStr(item + "</dd>", "<dd", "<dd", "</dd>", 1, true);
                    string str4 = MyClass.ExtractStr(str3, "<a", "<a", "</a>", 100, true);
                    List<MyMap> lms = new List<MyMap>();
                    foreach (string item1 in str4.Split(','))
                    {
                        string str5 = MyClass.ExtractStr(item1, "href", "\"", "\"", 100);
                        string str6 = MyClass.ExtractStr(item1 + "<", "href", ">", "<", 100);
                        if (comboBox2.Items.Count == 0)
                        {
                            label7.Text = str21 + "  " + str6;
                            label9.Text = str5;
                        }
                        if (comboBox1.Items.Count == 0)
                        {
                            comboBox2.Items.Add(new { Name = str6, Value = str5 });
                        }
                        lms.Add(new MyMap { key = str6, value = str5 });

                    }
                    comboBox1.Items.Add(new { Name = str21, Value = lms });
                    comboBox1.DisplayMember = "Name";
                    comboBox1.ValueMember = "Value";
                    comboBox1.SelectedIndex = 0;
                    comboBox2.DisplayMember = "Name";
                    comboBox2.ValueMember = "Value";
                    comboBox2.SelectedIndex = 0;
                }
            }

        }
        class MyMap
        {
            public string key = "";
            public string value = "";
        }
        private void inc()
        {
            int seq = int.Parse(ic["seq"]);
            seq++;
            textBox4.Text = (Int32.Parse(textBox4.Text) + 1).ToString();
            ic["seq"] = seq.ToString();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            TimeSpan ts =(TimeSpan)(DateTime.Parse("2013-12-13 21:24:13")- DateTime.Now);
           if(ts.TotalSeconds<0)
           {
               MessageBox.Show("非法使用！");
               Application.Exit();
               return;
           }
            InitAccount();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            ProcessUrl();
            textBox4.Text = ic["seq"];
           
        }

        void PreGoInFn(string url, FUN f)
        {
            fn = f;
            webBrowser1.Url = new Uri(url);
        }
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

            HtmlElement[] hes = MyClass.Select<HtmlElement>(webBrowser1.Document.Links, "InnerHtml", "邮箱注册", "=");
            if (hes.Length != 0)
            {
                hes[0].InvokeMember("click");
            }
            if (clickisreg == 1)
            {
                fn();
            }
            else if (clickisreg == 2)
            {
                clickisreg = 0;
                HtmlElementCollection hes1 = webBrowser1.Document.GetElementsByTagName("span");
                List<HtmlElement> lh = new List<HtmlElement>();
                foreach (HtmlElement item in hes1)
                {
                    if (item.GetAttribute("classname") == "chenggong")
                        lh.Add(item);
                }
                if (lh.Count == 4 )
                {
                    string tip = "账号 " + textBox2.Text + ic["seq"] + " 注册成功！";
                    SetText(tip);
                    string account = textBox2.Text + ic["seq"] + " " + textBox3.Text + " " + textBox2.Text + ic["seq"] + "@126.com";
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = textBox2.Text + ic["seq"];
                    lvi.SubItems.Add(textBox3.Text);
                    lvi.SubItems.Add(textBox2.Text + ic["seq"] + "@126.com");
                    this.lvSessions.Items.Insert(0, lvi);
                    AddAccountToFile(account);
                    inc();
                }
                else
                {
                    Timers_Timer.Stop();
                    button1.Text = "注册";
                    SetText("账号" + textBox2.Text + textBox4.Text + "注册失败，请手动调整！");
                }

              

            }
        }
        const int BM_CLICK = 0x00F5;
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string
lpClassName, string lpWindowName);
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(
        IntPtr hWnd, // handle to destination window 
        int Msg, // message 
        int wParam, // first message parameter 
        ref COPYDATASTRUCT lParam // second message parameter 
        );
        [DllImport("user32.dll")]
        static extern IntPtr FindWindowEx(HandleRef hwndParent, HandleRef hwndChildAfter, string strClass, string strWindow);
        void CloseAlert()
        {
            IntPtr hwnd = FindWindow(null, "安全警报");
            if (hwnd != IntPtr.Zero)
            {

                IntPtr btnWnd = FindWindowEx(new HandleRef(this, hwnd), new HandleRef(this, IntPtr.Zero), "Button", "是(&Y)");
                if (btnWnd != null)
                {
                    const int BM_CLICK = 0x00F5;
                    COPYDATASTRUCT cds;
                    cds.dwData = (IntPtr)100;
                    cds.lpData = "";
                    cds.cbData = 1;
                    SendMessage(btnWnd, BM_CLICK, 0, ref cds);
                }
                else
                {

                }
            }
        }
        IConfig ic = ConfigFileDES.Instanse;
        public delegate void FUN();
        FUN fn;
        string rurl = "https://passport.58.com/reg/";

        void StartR(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Timers_Timer.Interval == 1000)
            {
                Timers_Timer.Interval = 10  * 1000;
            }
            clickisreg = 1;
            PreGoInFn(rurl, RR);

        }
        int clickisreg = 0;

        void RR()
        {


            if (ic["seq"] == "")
            {
                ic["seq"] = 0.ToString();
            }
           
            int seq = Int32.Parse(ic["seq"]);

            HtmlDocument doc = webBrowser1.Document;
            doc.GetElementById("nickName").SetAttribute("value", textBox2.Text + seq);
            doc.GetElementById("txtemail").SetAttribute("value", textBox2.Text + seq + "@126.com");
            doc.GetElementById("password").SetAttribute("value", textBox3.Text);
            doc.GetElementById("cpassword").SetAttribute("value", textBox3.Text);
            clickisreg = 2;
            doc.GetElementById("btnSubmit").InvokeMember("click");
            
            HtmlElementCollection hes = webBrowser1.Document.GetElementsByTagName("span");
            List<HtmlElement> lh = new List<HtmlElement>();
            foreach (HtmlElement item in hes)
            {
                if (item.GetAttribute("classname") == "chenggong")
                    lh.Add(item);
            }//&& webBrowser1.Document.GetElementById("resendbtn")==null
            if (lh.Count == 4 )
            {
                string tip = "账号 " + textBox2.Text + ic["seq"] + " 注册成功！";
                SetText(tip);
                string account = textBox2.Text + ic["seq"] + " " + textBox3.Text + " " + textBox2.Text + ic["seq"] + "@126.com";
                ListViewItem lvi = new ListViewItem();
                lvi.Text = textBox2.Text + ic["seq"];
                lvi.SubItems.Add(textBox3.Text);
                lvi.SubItems.Add(textBox2.Text + ic["seq"] + "@126.com");
                this.lvSessions.Items.Insert(0, lvi);
                AddAccountToFile(account);
                inc();
            }
            else
            {
                Timers_Timer.Stop();
                button1.Text = "注册";
                SetText("账号" + textBox2.Text + textBox4.Text + "注册失败，请手动调整！");
            }
        }
        public void SetText(string title)
        {
            Text = "58同城 --- " + title;
        }
        public static string OsPath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }
        }
        System.Timers.Timer Timers_Timer = new System.Timers.Timer();
        private void button1_Click(object sender, EventArgs e)
        {
            Timers_Timer.Interval = 1000;

            if (button1.Text == "注册")
            {
                button1.Text = "停止";
                Timers_Timer.Start();
            }
            else
            {
                button1.Text = "注册";
                Timers_Timer.Stop();
            }


        }
        void Go()
        {

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            CloseAlert();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            List<MyMap> mm = (List<MyMap>)comboBox1.SelectedItem.GetType().GetProperty("Value").GetValue(comboBox1.SelectedItem, null);
            comboBox2.Items.Clear();
            foreach (MyMap item in mm)
            {
                comboBox2.Items.Add(new { Name = item.key, Value = item.value });

            }
            comboBox2.DisplayMember = "Name";
            comboBox2.ValueMember = "Value";
            comboBox2.SelectedIndex = 0;
        }
        private void Import(string file)
        {

            if (!File.Exists(file)) return;
            string path = Path.Combine(Application.StartupPath, "data");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (StreamReader reader = new StreamReader(file))
            {
                StreamWriter writer = new StreamWriter(Path.Combine(path, "account.txt"), true);
                string str2 = reader.ReadLine();
                string[] accs = str2.Split(' ');
                while ((str2 != null) && (str2 != ""))
                {
                    if (accs.Length == 3)
                    {
                        InsertAccount(str2);
                        writer.WriteLine(str2);
                        str2 = reader.ReadLine();
                    }
                }
                writer.Close();
            }
        }



        private void AddAccountToFile(string account)
        {
            string path = Path.Combine(Application.StartupPath, "data");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            StreamWriter writer = new StreamWriter(Path.Combine(path, "account.txt"), true);
            writer.WriteLine(account);
            writer.Close();
            
        }
        private void InitAccount()
        {
            string path = Path.Combine(Application.StartupPath, "data");
            string file=Path.Combine(path, "account.txt");
            if (!File.Exists(file)) return;
            using (StreamReader reader = new StreamReader(file))
            {
                string str2 = reader.ReadLine();
                string[] accs = str2.Split(' ');
                while ((str2 != null) && (str2 != ""))
                {
                    if (accs.Length == 3)
                    {
                        InsertAccount(str2);
                       
                        str2 = reader.ReadLine();
                    }
                }
            
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "选择 文本 文件";
            dialog.Filter = "TXT文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            dialog.ShowDialog();
            if (File.Exists(dialog.FileName))
            {
                Import(dialog.FileName);
            }

        }

        private void InsertAccount(string content)
        {
            string[] accs = content.Split(' ');
            if (accs.Length == 3)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = accs[0]; //lvSessions.Items.Count.ToString();
                //lvi.SubItems.Add(accs[0]);
                lvi.SubItems.Add(accs[1]);
                lvi.SubItems.Add(accs[2]);
                this.lvSessions.Items.Insert(0, lvi);

            }
        }

        private void label9_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
            webBrowser2.Url=new Uri(label9.Text); //链接的具体内容可动态设置,此链接即为举
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            textBox5.Text = textBox4.Text;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox3.Text = textBox2.Text = textBox1.Text;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            HtmlElement he = webBrowser2.Document.GetElementById("btnSubmit");
            if (he != null) he.InvokeMember("click");
        }
        void Login()
        {
               ListView.SelectedListViewItemCollection breakfast = lvSessions.SelectedItems;
               foreach (ListViewItem item in lvSessions.Items)
               {
                   item.BackColor = Color.White;
               }
            if (breakfast.Count != 0)
            {
                ListViewItem lvi = breakfast[0];
                lvi.BackColor = Color.Aqua;
            }
            HtmlElement he = webBrowser2.Document.GetElementById("btnSubmit");
            if (he != null) he.InvokeMember("click");
            fn = null;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;
            if (bt.Text == "<<")
            {
                bt.Text = ">>";
                Width = 570;
            }
            else
            {
                bt.Text = "<<";
                Width = 835;
            }
        }

        private void textBox6_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (e.KeyCode == Keys.Up)
            {
                webBrowser2.Location = new Point(Int32.Parse(tb.Text) + 1, webBrowser2.Location.Y);
                tb.Text = (Int32.Parse(tb.Text) + 1).ToString();

            }
            else if (e.KeyCode == Keys.Down)
            {
                webBrowser2.Location = new Point(Int32.Parse(tb.Text) - 1, webBrowser2.Location.Y);
                tb.Text = (Int32.Parse(tb.Text) - 1).ToString();
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string mm = (string)comboBox2.SelectedItem.GetType().GetProperty("Value").GetValue(comboBox2.SelectedItem, null);
            label7.Text = comboBox1.Text + "  " + comboBox2.Text ;
            label9.Text = mm + "13941/j5";
        }
        FUN fn2;
        private void W2Log()
        {
            ListView.SelectedListViewItemCollection breakfast = lvSessions.SelectedItems;
           
            if (breakfast.Count != 0)
            {
                ListViewItem lvi = breakfast[0];
               
                webBrowser2.Document.GetElementById("btnSubmit");
                HtmlElement hu = webBrowser2.Document.GetElementById("username");
                if (hu != null)
                {
                    hu.InvokeMember("focus");
                    hu.SetAttribute("value", lvi.SubItems[0].Text);
                }
                hu = webBrowser2.Document.GetElementById("password");
                if (hu != null)
                {
                    hu.InvokeMember("focus");
                    hu.SetAttribute("value", lvi.SubItems[1].Text);
                }
            }
            fn2 -= W2Log;
        }
        private void lvSessions_SelectedIndexChanged(object sender, EventArgs e)
        {

            if ( webBrowser2.Url!=null && webBrowser2.Url.ToString() == "https://passport.58.com/login/")
                W2Log();
            else
            {
                fn2 = W2Log;
                webBrowser2.Url = new Uri("https://passport.58.com/login/");
            }
           
        }

        private void webBrowser2_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            fn2();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (webBrowser2.Url==null || webBrowser2.Url.ToString() != label9.Text)
                webBrowser2.Url = new Uri(label9.Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string path = Path.Combine(Application.StartupPath, "data");

            path=Path.Combine(path, "account.txt");
            if (File.Exists(path))
                System.Diagnostics.Process.Start("notepad", path);
            else
                MessageBox.Show("账户文件不存在哦！");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;
            bt.Enabled = false;
            lvSessions.Items.Clear();
            InitAccount();
            bt.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int inc = 180;
            Button bt = (Button)sender;
            if (bt.Text == "兼职面版")
            {
                bt.Text = "取消";
                panel1.Height += (inc);
                panel1.Location = new Point(0, 220);
                panel1.Size = new Size(835, 480);
                panel1.BringToFront();
                webBrowser2.Location = new Point(0, 0);
                webBrowser2.Height -= inc;
                Height += inc;
                Location = new Point(Location.X, Location.Y - inc/2);
            }
            else
            {
                bt.Text = "发布兼职";
                panel1.Height -= (inc);
                panel1.Location = new Point(572, 290);
                panel1.Size = new Size(258, 218);
                webBrowser2.Location = new Point(-641, -130);
                webBrowser2.Height += inc;
                Height -= inc;
                Location = new Point(Location.X, Location.Y + inc/2);
            }
           

        }

        private void lvSessions_DoubleClick(object sender, EventArgs e)
        {
            if (webBrowser2.Url!=null && webBrowser2.Url.ToString() == "https://passport.58.com/login/")
            {
                W2Log();
                Login();
            }
            else
            {
                fn2 += W2Log;
                fn2 += Login;
                webBrowser2.Url = new Uri("https://passport.58.com/login/");
            }
        }
        class Company
        {
            public string name = "百度";
            public string nick = "百度";
            public string profile = "百度（Nasdaq简称：BIDU）是全球最大的中文搜索引擎，2000年1月由李彦宏、徐勇两人创立于北京中关村，致力于向人们提供“简单，可依赖”的信息获取方式。“百度”二字源于中国宋朝词人辛弃疾的《青玉案·元夕》词句“众里寻他千百度”，象征着百度对中文信息检索技术的执著追求。";
            public string people = "李彦宏";
            public string phone = "13888888888";
            public string area = "北京 崇明";
            public string address = "北京祥和城天明路38号东100米";
        }
        Company cmp = new Company();
        void ProfileCompany(string msg)
        {
            if (msg.StartsWith("公司名称"))
            {
                cmp.name = msg.Substring(4).Trim();
            }
            else if (msg.StartsWith("公司别称"))
            {
                cmp.nick = msg.Substring(4).Trim();
            }
            else if (msg.StartsWith("公司简介"))
            {
                cmp.profile = msg.Substring(4).Trim();
            }
            else if (msg.StartsWith("联系人"))
            {
                cmp.people = msg.Substring(3).Trim();
            }
            else if (msg.StartsWith("联系电话"))
            {
                cmp.phone = msg.Substring(4).Trim();
            }
            else if (msg.StartsWith("所在地区"))
            {
                cmp.area = msg.Substring(4).Trim();
            }
            else if (msg.StartsWith("详细地址"))
            {
                cmp.address = msg.Substring(4).Trim();
            }
        }
        class Job
        {
            public string name = "网络兼职生";
            public string cat = "图片处理";
            public string num = "30";
            public string time = "ALL";
            public string tp = "ALL";
            public string salary = "100";
            public string jiesuan = "1";
            public string profile = "所以不能问PS能制作什么样子的图，而是问哪种类型的图不适合用PS来制作（比如图表、数据表格、数据走势图、各种平面结构或施工图纸……这类不适合用PS制作）";
            public string people = "李彦宏";
            public string phone = "13888888888";
            public string email = "shoujianren@126.com";
            public string area = "北京 崇明";
            public string address = "北京祥和城天明路38号东100米";

        }
        Job jb = new Job();
        void ProfileJob(string msg)
        {
            if (msg.StartsWith("兼职名称"))
            {
                jb.name = msg.Substring(4).Trim();
            }
            else if (msg.StartsWith("兼职类别"))
            {
                jb.cat = msg.Substring(4).Trim();
            }
            else if (msg.StartsWith("招聘人数"))
            {
                jb.num = msg.Substring(4).Trim();
            }
            else if (msg.StartsWith("薪资水平"))
            {
                jb.salary = msg.Substring(4).Trim();
            }
            else if (msg.StartsWith("兼职内容"))
            {
                jb.profile = msg.Substring(4).Trim();
            }
            else if (msg.StartsWith("联系人"))
            {
                jb.people = msg.Substring(3).Trim();
            }
            else if (msg.StartsWith("联系电话"))
            {
                jb.phone = msg.Substring(4).Trim();
            }
            else if (msg.StartsWith("接收邮箱"))
            {
                jb.email = msg.Substring(4).Trim();
            }
            else if (msg.StartsWith("工作区域"))
            {
                jb.area = msg.Substring(4).Trim();
            }
            else if (msg.StartsWith("详细地址"))
            {
                jb.address = msg.Substring(4).Trim();
            }
        }
        void LoadCompany(bool ismodify=false)
        {
            
            string path = Path.Combine(Application.StartupPath, "data");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string file = Path.Combine(path, "company.txt");
            if (!File.Exists(file))
            {
                StreamWriter writer = new StreamWriter(file, true);
                writer.WriteLine("公司名称 " + cmp.name);
                writer.WriteLine("公司别称 " + cmp.nick);
                writer.WriteLine("公司简介 " + cmp.profile);
                writer.WriteLine("联系人 " + cmp.people);
                writer.WriteLine("联系电话 " + cmp.phone);
                writer.WriteLine("所在地区 " + cmp.area);
                writer.WriteLine("详细地址 " + cmp.address);
                writer.Close();
                MessageBox.Show("首次加载，请填写公司模板后，重新加载！");
                System.Diagnostics.Process.Start(file);
                return;
            }
            if (ismodify)
            {
                System.Diagnostics.Process.Start(file);
                return;
            }
            using (StreamReader reader = new StreamReader(file))
            {
                string str2 = reader.ReadLine();
                while ((str2 != null) && (str2 != ""))
                {

                    ProfileCompany(str2);
                    str2 = reader.ReadLine();
                    
                }
              
            }
        }
        void FileCmp()
        {
            if (webBrowser2.Url != null && webBrowser2.Url.ToString() == label9.Text.Replace("j5","j2"))
            {
               HtmlDocument doc = webBrowser2.Document.Window.Frames["gsInfo"].Document;
               if (doc != null)
               {
                   HtmlElement he = doc.GetElementById("txtCompName");
                   if (he != null)
                   {
                       he.SetAttribute("value", cmp.name);
                   }
                   he = doc.GetElementById("txtAliasName");
                   if (he != null)
                   {
                       he.SetAttribute("value", cmp.nick);
                   }
                   he = doc.GetElementById("txtCompIntro");
                   if (he != null)
                   {
                       he.SetAttribute("value", cmp.profile);
                   }
                   he = doc.GetElementById("txtContacts");
                   if (he != null)
                   {
                       he.SetAttribute("value", cmp.people);
                   }
                   he = doc.GetElementById("txtPhone");
                   if (he != null)
                   {
                       he.SetAttribute("value", cmp.phone);
                   }
                   he = doc.GetElementById("seleCompCity");
                   if (he != null)
                   {
                       he.InnerText = cmp.area.Split(' ')[0]; ;
                   }
                   he = doc.GetElementById("txtCompAddr");
                   if (he != null)
                   {
                       he.SetAttribute("value", cmp.address);
                   }
               }

            }
            fn2 = null;
        }
        private void button9_Click(object sender, EventArgs e)
        {
              //IEBrowser ie = new IEBrowser ( this.webBrowser2 );
 

          //string showMessageScript = "";
 
          //ie.InstallScript ( showMessageScript );
            LoadCompany();

            if (webBrowser2.Url != null && webBrowser2.Url.ToString() == label9.Text.Replace("j5", "j2"))
            {
                FileCmp();
            }
            else
            {
                fn2 = FileCmp;
                webBrowser2.Url = new Uri(label9.Text.Replace("j5", "j2"));
            }
            
             // ie.ExecuteScript("alert($.fn.jquery)");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            LoadCompany(true);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            HtmlDocument doc = webBrowser2.Document.Window.Frames["gsInfo"].Document;
            if (doc != null)
            {
                HtmlElement he = doc.GetElementById("save");
                if (he != null)
                {
                    he.InvokeMember("click");
                }
            }
        }
        void LoadJob(bool ismodify = false)
        {

            string path = Path.Combine(Application.StartupPath, "data");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string file = Path.Combine(path, "job.txt");
            if (!File.Exists(file))
            {
                StreamWriter writer = new StreamWriter(file, true);
                writer.WriteLine("兼职名称 " + jb.name);
                writer.WriteLine("兼职类别 " + jb.cat);
                writer.WriteLine("招聘人数 " + jb.num);
                writer.WriteLine("薪资水平 " + jb.salary);
                writer.WriteLine("兼职内容 " + jb.profile);
                writer.WriteLine("联系人 " + jb.people);
                writer.WriteLine("联系电话 " + jb.phone);
                writer.WriteLine("接收邮箱 " + jb.email);
                writer.WriteLine("详细地址 " + jb.address);
                writer.Close();
                MessageBox.Show("首次加载，请填写兼职模板后，重新加载！");
                System.Diagnostics.Process.Start(file);
                return;
            }
            if (ismodify)
            {
                System.Diagnostics.Process.Start(file);
                return;
            }
            using (StreamReader reader = new StreamReader(file))
            {
                string str2 = reader.ReadLine();
                while ((str2 != null) && (str2 != ""))
                {

                    ProfileJob(str2);
                    str2 = reader.ReadLine();

                }

            }
        }
        private void button12_Click(object sender, EventArgs e)
        {
            LoadJob(true);
        }
        void FileJob()
        {
            if (webBrowser2.Url != null && webBrowser2.Url.ToString() == label9.Text)
            {
                HtmlElement he = webBrowser2.Document.GetElementById("txtJobName");
                if (he != null)
                {
                    he.SetAttribute("value", jb.name);
                }
                he = webBrowser2.Document.GetElementById("seleJobCate");
                if (he != null)
                {
                    he.InnerHtml = jb.cat;
                    he.SetAttribute("classname", "selestyle");
                    webBrowser2.Document.GetElementById("seleJobCate_Tip").SetAttribute("classname", "chenggong");
                }
                he = webBrowser2.Document.GetElementById("zhaopinrenshu");
                if (he != null)
                {
                    he.SetAttribute("value", jb.num);
                }
                he = webBrowser2.Document.GetElementById("selectTimeAll");
                if (he != null)
                {
                    if(he.GetAttribute("classname")!="selAlltime iselect")
                    he.InvokeMember("click");

                    //he.SetAttribute("classname", "selAlltime iselect");
                }
                he = webBrowser2.Document.GetElementById("longterm");
                if (he != null)
                {
                    he.SetAttribute("checked", "True");
                    //he.InvokeMember("click");
                }
                he = webBrowser2.Document.GetElementById("pt-txtProbSala");
                if (he != null)
                {
                    he.SetAttribute("value", jb.salary);
                }
                he = webBrowser2.Document.GetElementById("div_htmleditor");
                if (he.FirstChild != null)
                {
                    if (he.FirstChild.Children[1] != null)
                    {
                        he.FirstChild.Children[1].InnerHtml = jb.profile;
                    }

                }
                
                he = webBrowser2.Document.GetElementById("txtContP");
                if (he != null)
                {
                    he.SetAttribute("value", jb.people);
                }
                he = webBrowser2.Document.GetElementById("lianxifangshi");
                if (he != null)
                {
                    he.SetAttribute("value", jb.phone);
                }
                he = webBrowser2.Document.GetElementById("Email");
                if (he != null)
                {
                    he.SetAttribute("value", jb.email);
                }
                he = webBrowser2.Document.GetElementById("txtdeAddress");
                if (he != null)
                {
                    he.SetAttribute("value", jb.address);
                }


            }
            fn2 = null;
        }
        private void button11_Click(object sender, EventArgs e)
        {
            LoadJob();

            if (webBrowser2.Url != null && webBrowser2.Url.ToString() == label9.Text )
            {
                FileJob();
            }
            else
            {
                fn2 = FileJob;
                webBrowser2.Url = new Uri(label9.Text);
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            HtmlElement he = webBrowser2.Document.GetElementById("fabu");
            if (he != null)
            {
                he.InvokeMember("click");
            }

        }

        //-641, -130
        //970, 686
    }
}
