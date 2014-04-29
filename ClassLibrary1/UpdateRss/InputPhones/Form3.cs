using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using CCWin;
using StringTool;
using InputPhones.Properties;
namespace InputPhones
{

    public partial class Form3 : CCSkinMain
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            data_xp_newstype[] ns = Query<data_xp_newstype>.GetAll();
            //skinComboBox1.Items.Clear();
            foreach (data_xp_newstype item in ns)
            {
                skinComboBox1.Items.Add(item.typename);
            }
            if (ns.Length != 0)
            {
                skinComboBox1.SelectedIndex = 0;
                string typeid = gettypeid(skinComboBox1.Text);
                data_xp_news[] nes = Query<data_xp_news>.Select("type_id='" + typeid + "'");
                //skinComboBox1.Items.Clear();
                foreach (data_xp_news item in nes)
                {
                    skinComboBox2.Items.Add(item.title);
                }

                if (nes.Length != 0)
                {
                    t_n = nes[0];
                    skinComboBox2.SelectedIndex = 0;
                    string content = nes.Length == 0 ? "" : nes[0].content;
                    webBrowser1.Navigate("about:blank");
                    webBrowser1.Document.Write(GetHtml(content));
                }
                else
                {
                    webBrowser1.Navigate("about:blank");
                    skinButtom1.Enabled = false;
                    skinButtom2.Enabled = false;
                }

                
            }
            else
            {
                skinButtom1.Enabled = false;
                skinButtom2.Enabled = false;
            }
     
            this.skinComboBox1.SelectedIndexChanged += new System.EventHandler(this.skinComboBox1_SelectedIndexChanged);
            this.skinComboBox2.SelectedIndexChanged += new System.EventHandler(this.skinComboBox2_SelectedIndexChanged);

        }
        data_xp_news t_n = null;
        private string gettypeid(string name)
        {
           return (string)MyClass.ExecuteScalar(string.Format("SELECT id from data_xp_newstype where typename='{0}'", name)).ToString();
        }
        private void skinComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string typeid = gettypeid(skinComboBox1.Text);
            data_xp_news[] nes = Query<data_xp_news>.Select("type_id='" + typeid + "'");
            skinComboBox2.Items.Clear();
            foreach (data_xp_news item in nes)
            {
                skinComboBox2.Items.Add(item.title);
            }
            if (nes.Length == 0)
            {
                webBrowser1.Navigate("about:blank");
                skinButtom1.Enabled = false;
                skinButtom2.Enabled = false;
                return;
            }
            skinComboBox2.SelectedIndex = 0;
            skinButtom1.Enabled = false;
        }

        private void skinComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            t_n = Query<data_xp_news>.SelectOne("title='"+skinComboBox2.Text+"'");
            WebBrowser wb = new WebBrowser();
            wb.Navigate("about:blank");
            wb.Document.Write(GetHtml(t_n.content));
            wb.Size = webBrowser1.Size;
            wb.Location = webBrowser1.Location;
            webBrowser1.Visible = false;
            skinPanel1.Controls.Remove(webBrowser1);
            webBrowser1 = wb;
            skinPanel1.Controls.Add(webBrowser1);

          // webBrowser1.Document.Body.InnerHtml = style+t_n.content;
            string typeid = gettypeid(skinComboBox1.Text);
            data_xp_news tt_n = Query<data_xp_news>.GetNext(t_n, false, "type_id='" + typeid + "'");
            skinButtom2.Enabled = tt_n != null;
            tt_n = Query<data_xp_news>.GetLast(t_n, false, "type_id='" + typeid + "'");
            skinButtom1.Enabled = tt_n != null;
        }
        string GetHtml(string content)
        {
            return Resources.html.Replace("CONTENT", content);
        }
        private void skinButtom2_Click(object sender, EventArgs e)
        {
            string typeid = gettypeid(skinComboBox1.Text);
            data_xp_news tt_n = Query<data_xp_news>.GetNext(t_n, false, "type_id='" + typeid + "'");
            if (tt_n != null)
            {
                t_n = tt_n;
                //webBrowser1.Document.Body.InnerHtml = t_n.content;
                skinComboBox2.Text = t_n.title;
            }
            else
            {
                MessageBox.Show("已经是最后了！");
            }
        }

        private void skinButtom1_Click(object sender, EventArgs e)
        {
            string typeid = gettypeid(skinComboBox1.Text);
            data_xp_news tt_n = Query<data_xp_news>.GetLast(t_n, false, "type_id='" + typeid + "'");
            if (tt_n != null)
            {
                t_n = tt_n;
                //webBrowser1.Document.Body.InnerHtml = t_n.content;
                skinComboBox2.Text = t_n.title;
            }
            else
            {
                MessageBox.Show("已经是最前了！");
            }
        }

    }
   
    public class data_xp_newstype
    {
        public string id = "";
        public string typename = "";
        public string url = "";
    }
    public class data_xp_news
    {
        public string id = "";
        public string title = "";
        public string type_id = "";
        public string content = "";
        public string source = "";
        public string misc = "";
        public string pubdate = "";
    }
}
