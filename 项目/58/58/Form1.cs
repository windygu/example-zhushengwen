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
using System.Net;
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
            LoadImg();
            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, 25);//分别是宽和高
            cxListView1.SmallImageList = imgList;
            TimeSpan ts = (TimeSpan)(DateTime.Parse("2013-12-13 21:24:13") - DateTime.Now);
            if (ts.TotalSeconds < 0)
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
                Timers_Timer.Interval = 10 * 1000;
            }
            clickisreg = 1;


        }
        int clickisreg = 0;

        void RR()
        {


            if (ic["seq"] == "")
            {
                ic["seq"] = 0.ToString();
            }

            int seq = Int32.Parse(ic["seq"]);


            if (true)
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
            string file = Path.Combine(path, "account.txt");
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
                //ListViewItem lvi = new ListViewItem();
                //lvi.Text = accs[0]; //lvSessions.Items.Count.ToString();
                ////lvi.SubItems.Add(accs[0]);
                //lvi.SubItems.Add(accs[1]);
                //lvi.SubItems.Add(accs[2]);
                //this.lvSessions.Items.Insert(0, lvi);
                ListViewItem lvi = cxListView1.Items.Add((cxListView1.Items.Count + 1).ToString());
                
                


                foreach (string  item in accs)
                {

                    lvi.SubItems.Add(item);
                }
                CXListView.CXListView.EXControlListViewSubItem cs = new CXListView.CXListView.EXControlListViewSubItem();
                lvi.SubItems.Add(cs);
                this.cxListView1.AddControlToSubItem(4, cxListView1.Items.Count-1, cs, DockStyle.Fill);
                cs.Btn.Text = "删除";
                cs.Btn.Font = new System.Drawing.Font("宋体", 8.25F, System.Drawing.FontStyle.Regular);
               // cs.Btn.Click += new EventHandler(button_Click);

            }
        }
        private void button_Click(object sender, EventArgs e)
        {
            
        }
        private void label9_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            //webBrowser2.Url=new Uri(label9.Text); //链接的具体内容可动态设置,此链接即为举
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            textBox5.Text = textBox4.Text;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox3.Text = textBox2.Text = textBox1.Text;
        }
        void SetCookie(CookieCollection c)
        {
            if (c != null)
            {
                foreach (Cookie item in c)
                {
                    Cc.Add(item);
                }
            }
        }
        HttpClient hc = new HttpClient(ref Cc);
        void LoadImg()
        {
            // string outstr;
            //hc.GetSrc("https://passport.58.com/login", "utf-8", out outstr);
            //
            string imgurl = "https://passport.58.com/validatecode";
            Image im = hc.GetImg(imgurl);
            if (im != null)
                pbSource.Image = im;

        }
        HttpItem itemx = new HttpItem();
        HttpResult resx = new HttpResult();
        HttpHelper hhx = new HttpHelper();

        private void button2_Click_1(object sender, EventArgs e)
        {


            string msg = "成功提交！";
            //https://passport.58.com/validatecode
            //string s = hc.GetSrc("https://passport.58.com/login", "utf-8", out msg);

            string purl2 = "https://passport.58.com/douilogin";
            ListView.SelectedListViewItemCollection breakfast = lvSessions.SelectedItems;

            if (breakfast.Count != 0)
            {
                ListViewItem lvi = breakfast[0];
                string post = "domain=58.com&callback=handleLoginResult&sysIndex=0&pptusername={0}&pptpassword={1}&pptvalidatecode={2}";
                post = string.Format(post, lvi.SubItems[0].Text, lvi.SubItems[1].Text, textBox7.Text);
                string rs = hc.PostStr(purl2, post);
                // string rs = hc.PostData(purl2, post, "utf-8", "utf-8", out msg);
                if (rs.IndexOf("type=success") != -1)
                {
                    WI(lvi.SubItems[0].Text + " 登陆成功！");
                    Cc = hc.Cookies;
                    //SetCookie(resx1.CookieCollection);
                }
                else
                {
                    WI(lvi.SubItems[0].Text + " 登陆失败！");
                }
                hc = new HttpClient(ref Cc);
            }

            //    HtmlElement he = webBrowser2.Document.GetElementById("btnSubmit");
            //    if (he != null) he.InvokeMember("click");
        }
        void WI(string str)
        {
            textBox6.Text += MyClass.GetNowTime() + ": " + str + "\r\n";
        }
        void Login()
        {

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



        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string mm = (string)comboBox2.SelectedItem.GetType().GetProperty("Value").GetValue(comboBox2.SelectedItem, null);
            label7.Text = comboBox1.Text + "  " + comboBox2.Text;
            label9.Text = mm + "13941/j5";
        }


        private void lvSessions_SelectedIndexChanged(object sender, EventArgs e)
        {



        }





        private void button6_Click(object sender, EventArgs e)
        {
            string path = Path.Combine(Application.StartupPath, "data");

            path = Path.Combine(path, "account.txt");
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

                //Height += inc;
                Location = new Point(Location.X, Location.Y - inc / 2);
            }
            else
            {
                bt.Text = "发布兼职";
                panel1.Height -= (inc);
                panel1.Location = new Point(572, 290);
                panel1.Size = new Size(258, 218);

                //Height -= inc;
                Location = new Point(Location.X, Location.Y + inc / 2);
            }


        }

        private void lvSessions_DoubleClick(object sender, EventArgs e)
        {

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
        void LoadCompany(bool ismodify = false)
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

        private void button9_Click(object sender, EventArgs e)
        {
            //IEBrowser ie = new IEBrowser ( this.webBrowser2 );


            //string showMessageScript = "";

            //ie.InstallScript ( showMessageScript );
            LoadCompany();


            // ie.ExecuteScript("alert($.fn.jquery)");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            LoadCompany(true);
        }
        static CookieContainer Cc = new CookieContainer();
        private void button13_Click(object sender, EventArgs e)
        {

            string purl2 = "http://qy.58.com/addparttimerent";
            //https://passport.58.com/douilogin

            string qdata = "fc=&enterpriseName=%E6%B2%B3%E5%8D%97%E7%91%9E%E5%85%8B%E4%BF%A1%E6%81%AF&enterpriseAlias=%E8%8C%B9%E5%8F%AF&intro=%E6%B2%B3%E5%8D%97%E7%91%9E%E5%85%8B%E4%BF%A1%E6%81%AF%E6%B2%B3%E5%8D%97%E7%91%9E%E5%85%8B%E4%BF%A1%E6%81%AF%E6%B2%B3%E5%8D%97%E7%91%9E%E5%85%8B%E4%BF%A1%E6%81%AF%E6%B2%B3%E5%8D%97%E7%91%9E%E5%85%8B%E4%BF%A1%E6%81%AF%E6%B2%B3%E5%8D%97%E7%91%9E%E5%85%8B%E4%BF%A1%E6%81%AF%E6%B2%B3%E5%8D%97%E7%91%9E%E5%85%8B%E4%BF%A1%E6%81%AF%E6%B2%B3%E5%8D%97%E7%91%9E%E5%85%8B%E4%BF%A1%E6%81%AF%E6%B2%B3%E5%8D%97%E7%91%9E%E5%85%8B%E4%BF%A1%E6%81%AF%E6%B2%B3%E5%8D%97%E7%91%9E%E5%85%8B%E4%BF%A1%E6%81%AF%E6%B2%B3%E5%8D%97%E7%91%9E%E5%85%8B%E4%BF%A1%E6%81%AF%E6%B2%B3%E5%8D%97%E7%91%9E%E5%85%8B%E4%BF%A1%E6%81%AF%E6%B2%B3%E5%8D%97%E7%91%9E%E5%85%8B%E4%BF%A1%E6%81%AF%E6%B2%B3%E5%8D%97%E7%91%9E%E5%85%8B%E4%BF%A1%E6%81%AF%E6%B2%B3%E5%8D%97%E7%91%9E%E5%85%8B%E4%BF%A1%E6%81%AF%E6%B2%B3%E5%8D%97%E7%91%9E%E5%85%8B%E4%BF%A1%E6%81%AF%E6%B2%B3%E5%8D%97%E7%91%9E%E5%85%8B%E4%BF%A1%E6%81%AF%E6%B2%B3%E5%8D%97%E7%91%9E%E5%85%8B%E4%BF%A1%E6%81%AF&linkMan=%E7%8E%8B%E8%B4%BA&phone=13168574694&telPhone=&areaid=1&address=%E5%8C%97%E4%BA%AC%E5%92%8C%E5%B9%B3%E5%A4%A7%E9%81%93100%E5%8F%B7&pics=&jscode=53535250536349535353&jsmoverecord=276%2C37%2C1386765316952%2CtxtCompIntro%2Cblur%2Ctxt%3B190%2C140%2C1386765319244%2CtxtCompIntro%2Cfocus%2Ctxt%3B177%2C431%2C1386765322818%2CtxtCompIntro%2Cblur%2Ctxt%3B233%2C705%2C1386765353084%2Csave%2Cclick%2Cbutton&validatecode=&checkTelphone=&checkBackCode=";

            string rs = hc.PostStr(purl2, qdata);
            WI(rs);


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





        private void pbSource_Click(object sender, EventArgs e)
        {
            LoadImg();
        }

        private void button7_Click(object sender, EventArgs e)
        {


        }

        private void button15_Click(object sender, EventArgs e)
        {
           // Query.InsertObj(cmp.GetType(), cmp);
            Company[] cps = Query<Company>.GetAll();
        }


        //-641, -130
        //970, 686
    }
}
