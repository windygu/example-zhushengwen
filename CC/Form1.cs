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
using System.Drawing.Drawing2D;
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
            return (new Random(DateTime.Now.Millisecond).Next(4) * 2 + 92).ToString();
        }
        public void SetText(string no)
        {
            label6.Text = no;
        }
        private bool KanShiPin()
        {
            FrmInformation fi = new FrmInformation(this);
            OpenWindow++;
            fi.auto = true;
            fi.ShowDialog();
            return fi.GetResult();
        }
        private bool ZuoTI()
        {
            int kid = GetLiuShui(txtId.Text);
            if (kid != 0)
            {
                label7.Text = kid.ToString();
                string fen = RandomFen();
                if (skinRadioButton1.Checked)
                {
                    if (jkb.ZQ_XueXiRiZhiToExamOne(kid, fen)==0)
                    {
                        SetText("科目一考试已通过！");
                        label9.Text = fen;
                        return true;
                    }
                    else
                    {
                        SetText("今天课程已结束，请明天再来吧！");
                    }
                }
                else
                {
                    if (jkb.ZQ_XueXiRiZhiToExamThree(kid, fen) == 0)
                    {
                        SetText("科目三考试已通过！");
                        label9.Text = fen;
                        return true;
                    }
                    else
                    {
                        SetText("今天课程已结束，请明天再来吧！");
                    }
                }
            }
            else
            {
                SetText("课程获取失败，请检查身份证号！");
              
            }
            return false;
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
            if (skinRadioButton3.Checked)
            {
                FrmInformation fi = new FrmInformation(this);
                fi.ShowDialog();
                return;
            }
            ZuoTI();
            
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
            Import();
            Format();
        }
        private void Format()
        {
            if (txtId.Items.Count != 0)
            {
                if (!txtId.Items.Contains(txtId.Text))
                {
                    txtId.Text = txtId.Items[0].ToString();
                }
            }
            else
            {
                txtId.Text = "";
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            label4.Text = GetNowTime();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "选择 文本 文件";
            dialog.Filter = "TXT文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            dialog.ShowDialog();
            if (File.Exists(dialog.FileName))
            {
                Import(dialog.FileName, true);
                Format();
            }
        }

        private void Import(string file = "", bool append = false)
        {
            string path = Path.Combine(Application.StartupPath, "data");
            if (file == "" || !File.Exists(file) || !append)
            {
                file = Path.Combine(path, "account.txt");
                if (File.Exists(file))
                {
                    using (StreamReader reader = new StreamReader(file))
                    {
                        string str2 = reader.ReadLine();
                        while (str2 != null)
                        {
                            if (str2.Length == 18 && !txtId.Items.Contains(str2))
                            {
                                txtId.Items.Add(str2);
                            }
                            str2 = reader.ReadLine();
                            
                        }
                    }
                }
                
                return;
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (StreamReader reader = new StreamReader(file))
            using (StreamWriter writer = new StreamWriter(Path.Combine(path, "account.txt"), true))
            {
                string str2 = reader.ReadLine();
                while (str2 != null)
                {
                    if (str2.Length == 18 && !txtId.Items.Contains(str2))
                    {
                        txtId.Items.Add(str2);
                        if (append) writer.WriteLine(str2);
                    }
                    str2 = reader.ReadLine();

                }
                writer.Close();
            }
        }

        private void skinButtom1_Click(object sender, EventArgs e)
        {
            string path = Path.Combine(Application.StartupPath, "data");

            path = Path.Combine(path, "account.txt");
            if (File.Exists(path))
                System.Diagnostics.Process.Start("notepad", path);
            else
                MessageBox.Show("账户文件不存在哦！");
        }

        private void skinButtom2_Click(object sender, EventArgs e)
        {
            txtId.Items.Clear();
            Import();
            Format();

        }
        private bool Senable
        {
            set {
                skinButtom2.Enabled = txtId.Enabled = button1.Enabled = skinRadioButton1.Enabled = skinRadioButton2.Enabled = skinRadioButton3.Enabled = value;
                timer2.Enabled = label11.Visible = skinGifBox1.Visible = !value;
                if (!value)
                {
                    (skinButtom3 as Button).Text = "取消答卷";
                }
                else
                {
                    (skinButtom3 as Button).Text = "循环答卷";
                }
            }   
        }
        private void skinButtom3_Click(object sender, EventArgs e)
        {
            
            if ((sender as Button).Text == "循环答卷")
            {
                Senable = false;
                isk1 = true;
             }
            else
            {
                Senable = true;
                
            }
        }
        public class CheckDo
        {
            public bool k1 = false;
            public bool k2 = false;
            public bool sp = false;
        }
        Dictionary<string, CheckDo> slt = new Dictionary<string, CheckDo>();
        bool isk1 = true;
        public string totals = "60";
        public static int OpenWindow = 0;
        private void ProcessForeach()
        {
            bool has = false;
            totals = "60";
            if (skinRadioButton3.Visible)
            {
                //做视频
                totals = "60";
                skinRadioButton3.Checked = true;
                bool next1 = false;
                if (txtId.Items.Count != 0 && txtId.Tag == txtId.Items[txtId.Items.Count - 1])
                {
                    txtId.Tag = null;
                }
                foreach (string item in txtId.Items)
                {
                    if (!next1)
                    {
                        if (txtId.Tag == null)
                        {
                            next1 = true;
                        }
                        else if (item == txtId.Tag.ToString())
                        {
                            next1 = true;
                            continue;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    txtId.Text = item;
                    txtId.Tag = item;
                    if (slt.ContainsKey(item) && slt[item].sp) continue;
                    has = true;
                    if (KanShiPin())
                    {
                        return;
                    }
                    else
                    {
                        if (slt.ContainsKey(item))
                        {
                            slt[item].sp = true;
                        }
                        else
                            slt.Add(item, new CheckDo() { sp = true });
                    }
                }
                if (!has)
                {
                    Senable = true;
                    SetText("循环答题已经完毕!");
                } //timer1.Enabled = has;
                return;
            }
            else
            {
                //做试卷
                // 科目一
                if (skinRadioButton1.Checked)
                {

                    bool next = false;
                    if (txtId.Items.Count != 0 && txtId.Tag == txtId.Items[txtId.Items.Count - 1])
                    {
                        txtId.Tag = null;
                    }

                    foreach (string item in txtId.Items)
                    {
                        if (!next)
                        {
                            if (txtId.Tag == null)
                            {
                                next = true;
                            }
                            else if (item == txtId.Tag.ToString())
                            {
                                next = true;
                                continue;
                            }
                            else
                            {
                                continue;
                            }
                        }
                       
                        txtId.Text = item;
                        txtId.Tag = item;
                        if (slt.ContainsKey(item) && slt[item].k1) continue;
                        has = true;
                        if (ZuoTI())
                        {
                            return;
                        }
                        else
                        {
                            if (slt.ContainsKey(item))
                            {
                                slt[item].k1 = true;
                            }
                            else
                                slt.Add(item, new CheckDo() { k1 = true });
                        }

                    }
                    if (!has)
                    {
                        isk1 = has;
                        txtId.Tag = null;
                    }
                }
                // 科目二
                isk1 = false;
                skinRadioButton2.Checked = true;
                bool next1 = false;
                foreach (string item in txtId.Items)
                {
                    if (!next1)
                    {
                        if (txtId.Tag == null)
                        {
                            next1 = true;
                        }
                        else if (item == txtId.Tag.ToString())
                        {
                            next1 = true;
                            continue;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    txtId.Text = item;
                    txtId.Tag = item;
                    if (slt.ContainsKey(item) && slt[item].k2) continue;
                    has = true;
                    if (ZuoTI())
                    {
                        return;
                    }
                    else
                    {
                        if (slt.ContainsKey(item))
                        {
                            slt[item].k2 = true;
                        }
                        else
                        slt.Add(item, new CheckDo() { k2 = true });
                    }

                }
                if (!has)
                {
                    Senable = true;
                    SetText("循环答题已经完毕!");
                } //timer1.Enabled = has;
                
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (label11.Text == "0")
            {
                label11.Text = totals;
                if (OpenWindow == 0) ProcessForeach();
            }
            else
            {
                if (OpenWindow == 0) label11.Text = "0";
                else
                label11.Text = (Int32.Parse(label11.Text) - 1).ToString();
            }
            label11.BringToFront();
        }


    }
    

}
