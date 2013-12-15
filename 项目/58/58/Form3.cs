using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using StringTool;
using System.Text.RegularExpressions;
using System.Net;
namespace _58
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }
        void WI(string str)
        {
            textBox16.Text += MyClass.GetNowTime() + ": " + str + "\r\n";
            if (!textBox16.Focused)
            {
                this.textBox16.Select(this.textBox16.Text.Length, 0);
                this.textBox16.ScrollToCaret();
            }
        }
        class Config
        {
            public string id = "1";
            public string FaTieId = "1";
            public string FaTieYanShi = "10";
            public string ShuiJiSheZhi = "1,1,1";
            public string ShuiJiNeiRong = "1";
            public string ShuiJiChangDu = "8";
            public string ChuShiAccountId = "1";
            public string ChuShiZhangHao = "ailice_tcft";
            public string ChuShiSuoYin = "0";
            public string ChuShiMiMa = "ailice_tcft";
            public string CmpName = "感动电子有限公司";
            public string CmpDiZhi = "名品汇三楼1308";
            public string CmpJianJie = "公司简介---公司简介";
            public string SiXunHuan = "1";
            public string ChuShiChengShi = "1";
            public string ChuShiDaiLi = "1";
            public string ChuShiDaiLiGL = "1";
            public string AutoSwitch = "1";
            private Form3 f;
            public Config()
            {
            }
            public Config(ref Form3 form)
            {
                GetFromDb();
                this.f = form;
            }
            void SetColor(string id)
            {
                foreach (ListViewItem item in f.cxListView1.Items)
                {
                    if (item.Tag.ToString() == id)
                    {
                        item.BackColor = Color.LimeGreen;
                    }
                    else
                    {
                        item.BackColor = Color.White;
                    }
                }
            }
            public void SetFaTieInfo()
            {
                if (IsInt(FaTieId))
                {
                    Job job = Query<Job>.GetOne(FaTieId);
                    if (job != null)
                    {
                        f.textBox1.Text = job.name;
                        f.textBox6.Text = job.address;
                        f.textBox2.Text = job.salary;
                        f.textBox4.Text = job.people;
                        f.textBox5.Text = job.num;
                        f.textBox7.Text = job.profile;
                    }
                }
            }
            public Job GetFaTieInfo()
            {



                if (!IsInt(f.textBox2.Text))
                {
                    MessageBox.Show("工资应为整数，保存失败！");
                    return null;
                }
                if (!IsInt(f.textBox5.Text))
                {
                    MessageBox.Show("招聘人数应为整数，保存失败！");
                    return null;
                }

                Job job = new Job();
                job.name = f.textBox1.Text;
                job.address = f.textBox6.Text;
                job.salary = f.textBox2.Text;
                job.people = f.textBox4.Text;
                job.num = f.textBox5.Text;
                job.profile = f.textBox7.Text;

                return job;
            }
            public static string IncStr(string val)
            {
                if (IsInt(val))
                {
                    int i = Int32.Parse(val)+1;
                    val = i.ToString();
                }
                return val;
            }
            public static bool IsInt(object val)
            {
                if (val is int) return true;
                if (val is string)
                {
                    try
                    {
                        int i = Int32.Parse(val.ToString());
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
                return false;
            }
            public bool SaveFaTieInfo()
            {
                if (IsInt(f.groupBox3.Tag))
                {

                    FaTieId = f.groupBox3.Tag.ToString();
                    Job job = Query<Job>.GetOne(FaTieId);
                    if (job == null) return false;

                    job.name = f.textBox1.Text;
                    job.address = f.textBox6.Text;
                    job.salary = f.textBox2.Text;
                    job.people = f.textBox4.Text;
                    job.num = f.textBox5.Text;
                    job.profile = f.textBox7.Text;
                    return Query<Job>.UpdateObj(job);
                }
                return false;
            }
            public void SetToUi()
            {
                f.groupBox3.Tag = FaTieId;
                f.textBox3.Text = FaTieYanShi;
                string[] fs = ShuiJiSheZhi.Split(',');
                if (fs != null)
                {
                    if (fs.Length == 3)
                    {
                        f.checkBox1.Checked = fs[0] == "1";
                        f.checkBox2.Checked = fs[1] == "1";
                        f.checkBox3.Checked = fs[2] == "1";
                    }
                }
                f.comboBox1.SelectedIndex = int.Parse(ShuiJiNeiRong);
                f.textBox18.Text = ShuiJiChangDu;
                f.groupBox3.Tag = ChuShiAccountId;
                SetColor(ChuShiAccountId);

                f.textBox10.Text = f.textBox13.Text = ChuShiZhangHao;
                f.textBox11.Text = f.textBox12.Text = ChuShiSuoYin;

                f.textBox18.Text = CmpName;
                f.textBox14.Text = CmpDiZhi;
                f.textBox15.Text = CmpJianJie;
                f.checkBox4.Checked = SiXunHuan == "1";
                f.groupBox6.Tag = ChuShiChengShi;
                f.groupBox7.Tag = ChuShiDaiLi;
                f.groupBox7.Tag = ChuShiDaiLiGL;
                f.checkBox5.Checked = AutoSwitch == "1";
            }

            public void SetChuShiDaiLiGL()
            {
                if (IsInt(ChuShiDaiLiGL))
                {
                    ProxyIP city = Query<ProxyIP>.GetOne(ChuShiDaiLiGL);
                    if (city != null)
                    {
                        f.label27.Text = city.id;
                        f.textBox22.Text = city.ip;
                        f.textBox23.Text = city.port;
                        f.textBox24.Text = city.user;
                        f.textBox25.Text = city.pass;
                    }
                }
            }
            public ProxyIP GetChuShiDaiLiGLModel()
            {
                if (IsInt(f.label27.Text))
                {
                    ProxyIP city = new ProxyIP();
                    city.id = f.label27.Text;
                    city.ip = f.textBox22.Text;
                    city.port = f.textBox23.Text;
                    city.user = f.textBox24.Text;
                    city.pass = f.textBox25.Text;
                    return city;
                }
                return null;
            }
            public City GetChuShiChengShiModel()
            {
                if (IsInt(f.label18.Text))
                {
                    City city = new City();
                    city.id = f.label18.Text;
                    city.name = f.label21.Text;
                    city.cityid = f.CID.Text;
                    city.tel = f.textBox21.Text;

                    return city;
                }
                return null;
            }
            public void SetChuShiDaiLi()
            {
                if (IsInt(ChuShiDaiLi))
                {
                    ProxyIP city = Query<ProxyIP>.GetOne(ChuShiDaiLi);
                    if (city != null)
                    {
                        f.textBox17.Text = city.ip + ":" + city.port;
                    }
                }
            }

            public void SetChuShiChengShi()
            {
                if (IsInt(ChuShiChengShi))
                {
                    City city = Query<City>.GetOne(ChuShiChengShi);
                    if (city != null)
                    {
                        f.label18.Text = city.id;
                        f.label21.Text = city.name;
                        f.textBox21.Text = city.tel;
                        f.CID.Text = city.cityid;
                    }
                }
            }
            public bool SaveChuShiChengShi()
            {
                if (IsInt(f.groupBox6.Tag))
                {

                    ChuShiChengShi = f.groupBox6.Tag.ToString();
                    City city = Query<City>.GetOne(ChuShiChengShi);
                    if (city == null) return false;

                    city.id = f.label18.Text = city.id;
                    city.name = f.label21.Text = city.name;
                    city.tel = f.textBox21.Text = city.tel;

                    return Query<City>.UpdateObj(city);
                }
                return false;
            }

            public void GetFromUi()
            {
                FaTieId = f.groupBox3.Tag.ToString();
                if (!IsInt(f.textBox3.Text))
                {
                    MessageBox.Show("发帖延时应为整数，保存失败！");
                    return;
                }
                FaTieYanShi = f.textBox3.Text;
                ShuiJiSheZhi = ((f.checkBox1.Checked ? "1" : "0") + "," + (f.checkBox2.Checked ? "1" : "0") + "," + (f.checkBox3.Checked ? "1" : "0"));
                ShuiJiNeiRong = f.comboBox1.SelectedIndex.ToString();
                if (!IsInt(f.textBox18.Text))
                {
                    MessageBox.Show("随机长度应为整数，保存失败！");
                    return;
                }
                ShuiJiChangDu = f.textBox18.Text;
                ChuShiAccountId = f.groupBox3.Tag.ToString();
                ChuShiZhangHao = f.textBox10.Text;
                f.textBox12.Text = f.textBox11.Text;
                CmpName = f.textBox18.Text;
                CmpDiZhi = f.textBox14.Text;
                CmpJianJie = f.textBox15.Text;
                SiXunHuan = f.checkBox4.Checked ? "1" : "0";
                ChuShiChengShi = f.groupBox6.Tag.ToString();
                ChuShiDaiLi = f.groupBox7.Tag.ToString();
                ChuShiDaiLiGL = f.groupBox7.Tag.ToString();
                AutoSwitch = f.checkBox5.Checked ? "1" : "0";
            }
            public void SaveToDb()
            {
                Query<Config>.UpdateObj(this);
            }
            public void GetFromDb()
            {
                Config tc = Query<Config>.GetAnyOne();
                if (tc != null) CopyToSelf(tc);
                else
                    Query<Config>.InsertObj(this, "f");
            }
            public void CopyToSelf(Config s)
            {
                Type t = this.GetType();
                FieldInfo[] fis = t.GetFields();
                foreach (FieldInfo item in fis)
                {
                    item.SetValue(this, t.GetField(item.Name).GetValue(s));
                }
            }
        }
        Config cfg;
        static CookieContainer Cc = new CookieContainer();
        HttpClient hc = new HttpClient(ref Cc);
        void Login()
        {
            ListView.SelectedListViewItemCollection breakfast = cxListView1.SelectedItems;

            if (breakfast.Count != 0)
            {
                ListViewItem lvi = breakfast[0];
                string post = "domain=58.com&callback=handleLoginResult&sysIndex=0&pptusername={0}&pptpassword={1}&pptvalidatecode={2}";
                post = string.Format(post, lvi.SubItems[0].Text, lvi.SubItems[1].Text, textBox7.Text);
            }
        }
        private void Form3_Load(object sender, EventArgs e)
        {
            Form.CheckForIllegalCrossThreadCalls = false;
            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, 20);//分别是宽和高
            cxListView1.SmallImageList = imgList;
            cfg = new Config(ref Program.f);
            InitList();
            cfg.SetToUi();
            
            SetDelegate();
            Timers_Timer.Interval = 1000;
            Timers_Timer.Elapsed += new System.Timers.ElapsedEventHandler(StartS);
            Reg_Timer.Interval = 1000;
            Reg_Timer.Elapsed += new System.Timers.ElapsedEventHandler(StartR);
        }
        void StartS(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Timers_Timer.Interval == 1000)
            {
                Timers_Timer.Interval = 10 * 1000;
            }
            Account acc = Query<Account>.GetNext(cfg.ChuShiAccountId);
            if (acc != null)
            {
                if (hc.Login(acc.user, acc.pass))
                    WI(acc.user + " 登陆成功！");
                else WI(acc.user + " 登陆失败！");
                cfg.ChuShiAccountId = acc.id;
            }
            else
            {
                WI("账号已循环结束！");
            }
        }
        void StartR(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Reg_Timer.Interval == 1000)
            {
                Reg_Timer.Interval = 10 * 1000;
            }
        }
        void InitList()
        {
            InitAccount();
            InitProxyIP();
            cfg.SetChuShiDaiLi();
            cfg.SetChuShiDaiLiGL();
            LoadCity();
            cfg.SetChuShiChengShi();
            InitJob();
            cfg.SetFaTieInfo();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            InitJob();
        }
        private void button_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;
            if (Config.IsInt(bt.Tag))
            {
                Query<Account>.DelObj(bt.Tag);
                button9_Click(sender, e);
            }
        }
        private void button7_Click(object sender, EventArgs e)
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
        private void InsertAccount(Account acc)
        {

            ListViewItem lvi = cxListView1.Items.Add((cxListView1.Items.Count + 1).ToString());
            lvi.Tag = acc.id;
            lvi.SubItems.Add(acc.user);
            lvi.SubItems.Add(acc.pass);
            lvi.SubItems.Add(acc.email);
            CXListView.CXListView.EXControlListViewSubItem cs = new CXListView.CXListView.EXControlListViewSubItem();
            lvi.SubItems.Add(cs);
            this.cxListView1.AddControlToSubItem(4, cxListView1.Items.Count - 1, cs, DockStyle.Fill);
            cs.Btn.Tag = acc.id;
            cs.Btn.Text = "删除";
            cs.Btn.Font = new System.Drawing.Font("宋体", 8.25F, System.Drawing.FontStyle.Regular);
            cs.Btn.Click += new EventHandler(button_Click);


        }

        private void InsertProxy(ProxyIP acc)
        {
            ListViewItem lvi = new ListViewItem();

            lvi.Text = (listView3.Items.Count + 1).ToString();
            lvi.Tag = acc.id;
            // lvi.SubItems.Add(accs[0]);
            lvi.SubItems.Add(acc.ip);
            lvi.SubItems.Add(acc.port);
            lvi.SubItems.Add(acc.user);
            lvi.SubItems.Add(acc.pass);
            this.listView3.Items.Add(lvi);
        }
        private void InsertJob(Job acc)
        {
            ListViewItem lvi = new ListViewItem();

            lvi.Text = (listView1.Items.Count + 1).ToString();
            lvi.Tag = acc.id;
            // lvi.SubItems.Add(accs[0]);
            lvi.SubItems.Add(acc.name);
            lvi.SubItems.Add(acc.salary);
            lvi.SubItems.Add(acc.address);
            lvi.SubItems.Add(acc.profile);
            lvi.SubItems.Add(acc.people);
            lvi.SubItems.Add(acc.num);
            this.listView1.Items.Add(lvi);
        }
        private void InitAccount()
        {
            Account[] accs = Query<Account>.GetAll();
            foreach (Account acc in accs)
            {
                InsertAccount(acc);
            }
        }
        private void InitProxyIP()
        {
            ProxyIP[] accs = Query<ProxyIP>.GetAll();
            foreach (ProxyIP acc in accs)
            {
                InsertProxy(acc);
            }
        }
        private void InitJob()
        {
            Job[] accs = Query<Job>.GetAll(true);
            foreach (Job acc in accs)
            {
                InsertJob(acc);
            }
        }
        private void Import(string file)
        {

            if (!File.Exists(file)) return;

            using (StreamReader reader = new StreamReader(file))
            {
                //  StreamWriter writer = new StreamWriter(Path.Combine(path, "account.txt"), true);
                string str2 = reader.ReadLine();
                string[] accs = str2.Split(' ');
                while ((str2 != null) && (str2 != ""))
                {
                    if (accs.Length == 3)
                    {

                        Account ac = new Account();
                        ac.user = accs[0];
                        ac.pass = accs[1];
                        ac.email = accs[2];
                        if (Query<Account>.Count("user='" + ac.user + "'") == 0)
                        {
                            InsertAccount(ac);
                            Query<Account>.InsertObj(ac);
                            //  writer.WriteLine(str2);
                        }
                    }
                    str2 = reader.ReadLine();
                    if (str2 != null) accs = str2.Split(' ');
                }
                //  writer.Close();
            }
        }

        private void ImportProxy(string file)
        {

            if (!File.Exists(file)) return;
            using (StreamReader reader = new StreamReader(file))
            {
                //  StreamWriter writer = new StreamWriter(Path.Combine(path, "account.txt"), true);
                string str2 = reader.ReadLine();

                while ((str2 != null) && (str2 != ""))
                {
                    string[] accs = str2.Split(':');
                    if (accs.Length == 2)
                    {
                        ProxyIP ac = new ProxyIP();
                        ac.ip = accs[0];
                        ac.port = accs[1];

                        if (Query<ProxyIP>.Count("ip='" + ac.ip + "'") == 0)
                        {
                            Query<ProxyIP>.InsertObj(ac);
                            InsertProxy(ac);
                        }
                        //  writer.WriteLine(str2);
                    }
                    str2 = reader.ReadLine();
                }
                //  writer.Close();
            }
        }
        private void button9_Click(object sender, EventArgs e)
        {
            cxListView1.Items.Clear();
            InitAccount();
        }
        void SetDelegate()
        {
            this.textBox18.TextChanged += new System.EventHandler(this.UI_TextChanged);
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.UI_TextChanged);
            this.textBox13.TextChanged += new System.EventHandler(this.UI_TextChanged);
            this.textBox8.TextChanged += new System.EventHandler(this.UI_TextChanged);
            this.textBox12.TextChanged += new System.EventHandler(this.UI_TextChanged);
            this.textBox15.TextChanged += new System.EventHandler(this.UI_TextChanged);
            this.textBox14.TextChanged += new System.EventHandler(this.UI_TextChanged);
            this.textBox3.TextChanged += new System.EventHandler(this.UI_TextChanged);
            this.checkBox1.CheckStateChanged += new System.EventHandler(this.UI_TextChanged);
            this.checkBox2.CheckStateChanged += new System.EventHandler(this.UI_TextChanged);
            this.checkBox3.CheckStateChanged += new System.EventHandler(this.UI_TextChanged);
            this.checkBox5.CheckStateChanged += new System.EventHandler(this.UI_TextChanged);
        }
        private void UI_TextChanged(object sender, EventArgs e)
        {
            cfg.GetFromUi();
            cfg.SaveToDb();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "选择 文本 文件";
            dialog.Filter = "TXT文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            dialog.ShowDialog();
            if (File.Exists(dialog.FileName))
            {
                ImportProxy(dialog.FileName);
            }
        }



        private void listView3_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (listView3.SelectedItems.Count > 0)
            {
                if (cfg.AutoSwitch != "1")
                    cfg.ChuShiDaiLi = listView3.SelectedItems[0].Tag.ToString();
                cfg.ChuShiDaiLiGL = listView3.SelectedItems[0].Tag.ToString();
                cfg.SaveToDb();

                if (cfg.AutoSwitch != "1")
                    cfg.SetChuShiDaiLi();
                cfg.SetChuShiDaiLiGL();
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            listView3.Items.Clear();
            InitProxyIP();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            ProxyIP p = cfg.GetChuShiDaiLiGLModel();
            if (p != null)
            {
                Query<ProxyIP>.UpdateObj(p);
                ListViewItem[] lvi = MyClass.Select<ListViewItem>(listView3.Items, "Tag", p.id);
                if (lvi.Length == 1)
                {
                    lvi[0].SubItems[1].Text = p.ip;
                    lvi[0].SubItems[2].Text = p.port;
                    lvi[0].SubItems[3].Text = p.user;
                    lvi[0].SubItems[4].Text = p.pass;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string condit = MyClass.SelectStr<ListViewItem>(listView3.CheckedItems, "Tag");
            if (condit != "")
            {
                Query<ProxyIP>.DelObj(condit);
                button13_Click(sender, e);

            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            City p = cfg.GetChuShiChengShiModel();
            if (p != null)
            {
                Query<City>.UpdateObj(p);
                ListViewItem[] lvi = MyClass.Select<ListViewItem>(listView2.Items, "Tag", p.id);
                if (lvi.Length == 1)
                {
                    lvi[0].SubItems[1].Text = p.name;
                    lvi[0].SubItems[2].Text = p.cityid;
                    lvi[0].SubItems[3].Text = p.tel;
                }
            }
        }
        class MyMap
        {
            public string key = "";
            public string value = "";
        }
        void InsertCity(City c)
        {
            int index = listView2.Items.Count + 1;
            ListViewItem lvi = new ListViewItem();
            lvi.Text = index.ToString();
            listView2.Items.Add(lvi);
            lvi.SubItems.Add(c.name);
            lvi.SubItems.Add(c.cityid);
            lvi.SubItems.Add(c.tel);
            lvi.Tag = c.id;
        }
        void LoadCity()
        {
            if (Query<City>.Count() == 0)
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


                            string cityid = Regex.Match(str5, @"/(?<id>\d+)/").Result("${id}");
                            City c = new City();
                            c.name = str6;
                            c.cityid = cityid;
                            Query<City>.InsertObj(c);

                            //if (comboBox1.Items.Count == 0)
                            //{
                            //  comboBox2.Items.Add(new { Name = str6, Value = str5 });
                            //}
                            // lms.Add(new MyMap { key = str6, value = str5 });

                        }
                        // comboBox1.Items.Add(new { Name = str21, Value = lms });
                        // comboBox1.DisplayMember = "Name";
                        //  comboBox1.ValueMember = "Value";
                        //  comboBox1.SelectedIndex = 0;
                        //  comboBox2.DisplayMember = "Name";
                        //  comboBox2.ValueMember = "Value";
                        //  comboBox2.SelectedIndex = 0;
                    }
                }
                button14_Click_1(null, null);
            }
            else
            {
                City[] cs = Query<City>.GetAll();
                foreach (City c in cs)
                {
                    InsertCity(c);
                }

            }

        }

        private void button19_Click(object sender, EventArgs e)
        {
            string condit = MyClass.SelectStr<ListViewItem>(listView2.CheckedItems, "Tag");
            if (condit != "")
            {
                Query<City>.DelObj(condit);
                button14_Click_1(sender, e);

            }
        }

        private void button14_Click_1(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            LoadCity();
        }

        private void listView2_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {

                cfg.ChuShiChengShi = listView2.SelectedItems[0].Tag.ToString();
                cfg.SaveToDb();
                cfg.SetChuShiChengShi();
            }
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            textBox10.Text = textBox9.Text = textBox13.Text;
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            textBox11.Text = textBox12.Text;
        }

        private void button6_Click(object sender, EventArgs e)
        {

            Job job = cfg.GetFaTieInfo();
            if (job == null) return;
            if (Query<Job>.Count(job.name, "name") == 0)
            {
                Query<Job>.InsertObj(job);
                button1_Click(sender, e);
            }
            else
            {
                MessageBox.Show("标题与库存中的标题有重复！", "重复", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void button15_Click(object sender, EventArgs e)
        {
            string condit = MyClass.SelectStr<ListViewItem>(listView1.CheckedItems, "Tag");
            if (condit != "")
            {
                Query<Job>.DelObj(condit);
                button1_Click(sender, e);

            }
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {

                cfg.FaTieId = listView1.SelectedItems[0].Tag.ToString();
                cfg.SaveToDb();
                cfg.SetFaTieInfo();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Job p = cfg.GetFaTieInfo();
            if (p != null)
            {
                Query<Job>.UpdateObj(p);
                ListViewItem[] lvi = MyClass.Select<ListViewItem>(listView1.Items, "Tag", p.id);
                if (lvi.Length == 1)
                {
                    lvi[0].SubItems[1].Text = p.name;
                    lvi[0].SubItems[2].Text = p.salary;
                    lvi[0].SubItems[3].Text = p.address;
                    lvi[0].SubItems[3].Text = p.profile;
                    lvi[0].SubItems[3].Text = p.people;
                    lvi[0].SubItems[3].Text = p.num;

                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                item.Checked = true;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                item.Checked = !item.Checked;
            }
        }
        void SetBtnEnable(bool sg)
        {
            button10.Enabled = !sg;
            button11.Enabled = sg;
        }
        private void button8_Click(object sender, EventArgs e)
        {
            Reg_Timer.Interval = 1000;

            if (button8.Text == "注册")
            {
                button8.Text = "停止";
                Reg_Timer.Start();
            }
            else
            {
                button8.Text = "注册";
                Reg_Timer.Stop();
            }
        }
        System.Timers.Timer Timers_Timer = new System.Timers.Timer();
        System.Timers.Timer Reg_Timer = new System.Timers.Timer();
        private void button10_Click(object sender, EventArgs e)
        {
            button11_Click(sender, e);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Button btn = button10;
            Timers_Timer.Interval = 1000;

            if (btn.Enabled)
            {
                SetBtnEnable(true);
                Timers_Timer.Start();
            }
            else
            {
                SetBtnEnable(false);
                Timers_Timer.Stop();
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            StartS(null, null);
        }






    }
}
