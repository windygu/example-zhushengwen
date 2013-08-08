using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace StringTool
{
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }
        public string OsPath
        {
            get
            {
                return Path.Combine(System.Environment.OSVersion.Version.Major > 5 ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) : Environment.GetFolderPath(Environment.SpecialFolder.System), profile);
            }
        }
        private string filepath = "";
        public string profile
        {
            get
            {
                if (filepath == "")
                    return Path.GetFileNameWithoutExtension(Application.ExecutablePath);
                else return filepath;
            }
            set
            {
                filepath = value;
            }

        }
        public string MyText
        {
            get
            {
                return textBox1.Text;
            }
            set
            {
                textBox1.Text = value;
            }
        }
        private void SaveUrl(string url)
        {
            FileInfo f = new FileInfo(OsPath);
            string content = "";
            List<string> lists=new List<string>();
            if (f.Exists)
            {

                StreamReader sr = f.OpenText();
                while (!sr.EndOfStream)
                {
                    string tsrs=sr.ReadLine().Trim();
                    if(tsrs==url)continue;
                    if (lists.Contains(tsrs))
                    {
                        lists.Remove(tsrs);
                    }
                    lists.Add(tsrs);
                }
                sr.Close();
            }

            lists.Add(url);

            FileStream wn1 = f.Create();
            bool isfirst = true;
            foreach (string item in lists)
            {
                if (isfirst)
                {
                    isfirst = false;
                }
                else
                {
                    content += "\r\n";
                }
                content += item;
            }
            byte[] b = System.Text.Encoding.Default.GetBytes(content);
            wn1.Write(b, 0, b.Length);
            wn1.Close();
        }

        public void AddItem()
        {

            SaveUrl(textBox1.Text);
            if(!textBox1.AutoCompleteCustomSource.Contains(textBox1.Text))
            textBox1.AutoCompleteCustomSource.Add(textBox1.Text);
        }
        public event KeyEventHandler MyKeyDown;

        private string InitUrl()
        {
            if (!File.Exists(OsPath))
            {
                return "";
            }
            FileInfo fi = new FileInfo(OsPath);
            StreamReader sr = fi.OpenText();
            string temp = "";
            while (!sr.EndOfStream)
            {
                temp = sr.ReadLine();
                if (!textBox1.AutoCompleteCustomSource.Contains(temp))
                    textBox1.AutoCompleteCustomSource.Add(temp);
            }
            sr.Close();
            return temp;
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            string text = InitUrl();
            if (text != "")
                textBox1.Text = text;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (MyKeyDown != null)
            {
                MyKeyDown(sender, e);
            }
        }
    }
}
