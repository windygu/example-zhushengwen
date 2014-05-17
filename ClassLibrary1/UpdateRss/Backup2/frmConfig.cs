using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;

namespace SoukeyNetget
{
    public partial class frmConfig : Form
    {
        private ResourceManager rm;

        public frmConfig()
        {
            InitializeComponent();
        }

        private void treeMenu_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (e.Node.Name)
            {
                case "nodNormal":
                    this.panel1.Visible =true ;
                    this.panel2.Visible =false ;
                    break;

                case "nodExit":
                    this.panel1.Visible =false ;
                    this.panel2.Visible =true ;
                    break;
                default :
                    break;
            }
        }

        //±£¥Ê≈‰÷√–≈œ¢
        private void SaveConfigData()
        {
            try
            {
                cXmlSConfig Config = new cXmlSConfig();
                Config.IsInstantSave = false;

                if (this.raMin.Checked == true)
                    Config.ExitSelected = 0;
                else
                    Config.ExitSelected = 1;

                if (this.checkBox1.Checked == true)
                    Config.ExitIsShow = true;
                else
                    Config.ExitIsShow = false;

                if (this.IsAutoSystemLog.Checked == true)
                    Config.AutoSaveLog = true;
                else
                    Config.AutoSaveLog = false;

                if (this.comUILanguage.SelectedIndex == 0)
                    Config.CurrentLanguage = cGlobalParas.CurLanguage.Auto;
                else if (this.comUILanguage.SelectedIndex == 1)
                    Config.CurrentLanguage = cGlobalParas.CurLanguage.enUS;
                else if (this.comUILanguage.SelectedIndex == 2)
                    Config.CurrentLanguage = cGlobalParas.CurLanguage.zhCN;

                Config.Save();

                Config = null;

                this.IsSave.Text = "false";
            }
            catch (System.Exception)
            {
                MessageBox.Show(rm.GetString("Info76"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmConfig_Load(object sender, EventArgs e)
        {
            rm = new ResourceManager("SoukeyNetget.Resources.globalUI", Assembly.GetExecutingAssembly());

            this.comUILanguage.Items.Add(rm.GetString ("Label24"));
            this.comUILanguage.Items.Add(rm.GetString("Label25"));
            this.comUILanguage.Items.Add(rm.GetString("Label26"));

            this.txtLogPath.Text = Program.getPrjPath() + "log";

            try
            {
                cXmlSConfig Config = new cXmlSConfig();

                if (Config.ExitSelected == 0)
                    this.raMin.Checked = true;
                else
                    this.raExit.Checked = true;

                if (Config.ExitIsShow == true)
                    this.checkBox1.Checked = true;
                else
                    this.checkBox1.Checked = false;

                this.IsAutoSystemLog.Checked = Config.AutoSaveLog;

                switch (Config.CurrentLanguage)
                {
                    case cGlobalParas.CurLanguage .Auto :
                        this.comUILanguage.SelectedIndex = 0;
                        break;
                    case cGlobalParas.CurLanguage .enUS :
                        this.comUILanguage.SelectedIndex=1;
                        break ;
                    case cGlobalParas.CurLanguage .zhCN :
                        this.comUILanguage.SelectedIndex =2;
                        break;
                    default :
                        break ;
                }
                
                Config = null;

                this.cmdApply.Enabled = false;
                this.IsSave.Text = "false";

            }
            catch (System.Exception)
            {
                MessageBox.Show(rm.GetString("Info76"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void IsSave_TextChanged(object sender, EventArgs e)
        {
            if (this.IsSave.Text == "true" )
            {
                this.cmdApply.Enabled = true;
            }
            else if (this.IsSave.Text == "false")
            {
                this.cmdApply.Enabled = false;
            }
        }

        private void IsAutoSystemLog_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void raMin_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void raExit_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            SaveConfigData();
            this.cmdApply.Enabled = false;
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (this.cmdApply.Enabled == true)
                SaveConfigData();

            this.Close();
        }

        private void frmConfig_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        private void comUILanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

      
    }
}