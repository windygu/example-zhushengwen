using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using SoukeyNetget.Task;
using System.Text.RegularExpressions;

namespace SoukeyNetget
{
    public partial class frmAddNavRules : Form
    {
        private ResourceManager rm;
        public delegate void ReturnNavRule(string NavRule);
        public ReturnNavRule rNavRule;

        public frmAddNavRules()
        {
            InitializeComponent();
        }

        public frmAddNavRules(string DemoUrl)
        {
            InitializeComponent();

            this.txtDemoUrl.Text = DemoUrl;
        }

        //构建两个属性，webCode和Cookie，主要用于测试网址导航，因为有些网址
        //导航时需要进行cookie认证的
        private cGlobalParas.WebCode m_webCode;
        public cGlobalParas.WebCode webCode
        {
            get { return m_webCode; }
            set { m_webCode = value; }
        }

        private string m_cookie;
        public string cookie
        {
            get { return m_cookie; }
            set { m_cookie = value; }
        }

        private void frmAddNavRules_Load(object sender, EventArgs e)
        {
            rm = new ResourceManager("SoukeyNetget.Resources.globalUI", Assembly.GetExecutingAssembly());

            this.comType.Items.Add(rm.GetString("Label32"));
            this.comType.Items.Add(rm.GetString("Label33"));
            this.comType.Items.Add(rm.GetString("Label34"));

            this.comType.SelectedIndex = 0;
        }

        private void frmAddNavRules_FontChanged(object sender, EventArgs e)
        {
            rm = null;
        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            switch (this.comType.SelectedIndex)
            {
                case 0:

                    if (this.txtNRule.Text != "")
                    {
                        MessageBox.Show(rm.GetString ("Info115"),rm.GetString ("MessageboxInfo"),MessageBoxButtons.OK ,MessageBoxIcon.Information);
                        return;
                    }

                    this.txtNRule.Text = "<Regex:" + this.txtInfo.Text.Trim() + ">";
                    this.comType.SelectedIndex = 1;

                    break;
                case 1:

                    if (this.txtNRule.Text == "")
                    {
                        MessageBox.Show(rm.GetString("Info116"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    this.txtNRule.Text += "<Common:" + this.txtInfo.Text.Trim() + ">";
                    break;
                case 2:
                    this.txtNRule.Text +=  "<End:" + this.txtInfo.Text.Trim() + ">";
                    break;
            }
        }

        private void cmdClear_Click(object sender, EventArgs e)
        {
            this.txtNRule.Text = "";
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            rNavRule(this.txtNRule.Text);
            this.Close();
        }

        private void comType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txtInfo.Text = "";
        }

        private void cmdTest_Click(object sender, EventArgs e)
        {
            if (this.txtNRule .Text == "")
            {
                MessageBox.Show(rm.GetString("Error5"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string Url="";

            try
            {
                this.txtTestUrl.Text = "";

                List<cNavigRule> cns = new List<cNavigRule>();
                cNavigRule cn;

                cn = new cNavigRule();
                cn.Url = this.txtNRule.Text;
                cn.Level = 1;
                cn.NavigRule = this.txtNRule.Text;

                cns.Add(cn);

                GetTestUrl(this.txtDemoUrl.Text, cns);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Error26"), rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
            
        }

        private void GetTestUrl(string webLink, List<cNavigRule> NavRule)
        {
            List<string> Urls;
            Task.cUrlAnalyze gUrl = new Task.cUrlAnalyze();

            Urls = gUrl.ParseUrlRule(webLink, NavRule,m_webCode ,m_cookie );


            if (Urls == null || Urls.Count == 0)
                return;

            string isReg = "[\"\\s]";
            string Url = "";

            for (int m = 0; m < Urls.Count; m++)
            {
                this.txtTestUrl.Text += Urls[m].ToString() + "\r\n";
            }

        }
     
    }
}