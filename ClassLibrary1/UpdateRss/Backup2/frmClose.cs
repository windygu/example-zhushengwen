using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;

namespace SoukeyNetget
{
    public partial class frmClose : Form
    {
        public delegate void ReturnExitPara(cGlobalParas.ExitPara ePara);
        public ReturnExitPara RExitPara;
        private ResourceManager rm;

        public frmClose()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            cGlobalParas.ExitPara ePara = cGlobalParas.ExitPara.MinForm;

            if (this.raMin.Checked ==true )
            {
                ePara = cGlobalParas.ExitPara.MinForm;
            }
            else if (this.raExit.Checked == true)
            {
                ePara = cGlobalParas.ExitPara.Exit;
            }

            RExitPara(ePara);
            this.Close();
        }

        //±£¥Ê≈‰÷√–≈œ¢
        private void SaveConfigData()
        {
            try
            {
                cXmlSConfig Config = new cXmlSConfig();
                if (this.raMin.Checked == true)
                    Config.ExitSelected = 0;
                else
                    Config.ExitSelected = 1;
                if (this.checkBox1.Checked == true)
                    Config.ExitIsShow = false;
                else
                    Config.ExitIsShow = true;
                Config = null;
            }
            catch (System.Exception)
            {
                MessageBox.Show(rm.GetString("Info76"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmClose_Load(object sender, EventArgs e)
        {
            rm = new ResourceManager("SoukeyNetget.Resources.globalUI", Assembly.GetExecutingAssembly());

            try
            {
                cXmlSConfig Config = new cXmlSConfig();
                if (Config.ExitSelected == 0)
                    this.raMin.Checked = true;
                else
                    this.raExit.Checked = true;

                if (Config.ExitIsShow == true)
                    this.checkBox1.Checked = false;
                else
                    this.checkBox1.Checked = true;

                Config = null;
            }
            catch (System.Exception)
            {
                MessageBox.Show(rm.GetString("Info76"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            SaveConfigData();
        }

        private void raExit_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raExit.Checked == true)
            {
                SaveConfigData();
            }
        }

        private void raMin_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raMin.Checked == true)
            {
                SaveConfigData();
            }
        }
    }
}