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
    public partial class frmAddGatherRule : Form
    {
        public delegate void ReturnData(ListViewItem Litem);
        public ReturnData rData;

        private ResourceManager rm;

        public frmAddGatherRule()
        {
            InitializeComponent();
        }

        private void frmAddGatherRule_Load(object sender, EventArgs e)
        {
            rm = new ResourceManager("SoukeyNetget.Resources.globalUI", Assembly.GetExecutingAssembly());

            //根据当前的区域进行显示信息的加载
            ResourceManager rmPara = new ResourceManager("SoukeyNetget.Resources.globalPara", Assembly.GetExecutingAssembly());

            this.comLimit.Items.Add(rmPara.GetString("LimitSign1"));
            this.comLimit.Items.Add(rmPara.GetString("LimitSign2"));
            this.comLimit.Items.Add(rmPara.GetString("LimitSign3"));
            this.comLimit.Items.Add(rmPara.GetString("LimitSign4"));
            this.comLimit.Items.Add(rmPara.GetString("LimitSign5"));
            this.comLimit.Items.Add(rmPara.GetString("LimitSign6"));
            this.comLimit.Items.Add(rmPara.GetString("LimitSign7"));
            this.comLimit.SelectedIndex = 0;

            this.comExportLimit.Items.Add(rmPara.GetString("ExportLimit1"));
            this.comExportLimit.Items.Add(rmPara.GetString("ExportLimit2"));
            this.comExportLimit.Items.Add(rmPara.GetString("ExportLimit3"));
            this.comExportLimit.Items.Add(rmPara.GetString("ExportLimit4"));
            this.comExportLimit.Items.Add(rmPara.GetString("ExportLimit5"));
            this.comExportLimit.Items.Add(rmPara.GetString("ExportLimit6"));
            this.comExportLimit.Items.Add(rmPara.GetString("ExportLimit7"));
            this.comExportLimit.Items.Add(rmPara.GetString("ExportLimit8"));
            this.comExportLimit.Items.Add(rmPara.GetString("ExportLimit9"));
            this.comExportLimit.SelectedIndex = 0;

            this.comGetType.Items.Add(rmPara.GetString("GDataType4"));
            this.comGetType.Items.Add(rmPara.GetString("GDataType3"));
            this.comGetType.Items.Add(rmPara.GetString("GDataType2"));
            this.comGetType.Items.Add(rmPara.GetString("GDataType1"));
            this.comGetType.SelectedIndex = 0;

            this.txtGetTitleName.Items.Add("链接地址");
            this.txtGetTitleName.Items.Add("标题");
            this.txtGetTitleName.Items.Add("内容");
            this.txtGetTitleName.Items.Add("图片");

            rmPara = null;
        }

        private void comLimit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comLimit.SelectedIndex == 6)
                this.txtRegion.Enabled = true;
            else
                this.txtRegion.Enabled = false;
        }

        private void comExportLimit_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.comExportLimit.SelectedIndex)
            {
                case 0:
                    this.label37.Text = rm.GetString("Label1");
                    this.txtExpression.Text = "";
                    this.txtExpression.Enabled = false;
                    break;
                case 1:
                    this.label37.Text = rm.GetString("Label1");
                    this.txtExpression.Text = "";
                    this.txtExpression.Enabled = false;
                    break;
                case 2:
                    this.label37.Text = rm.GetString("Label2");
                    this.txtExpression.Text = "";
                    this.txtExpression.Enabled = true;
                    break;
                case 3:
                    this.label37.Text = rm.GetString("Label3");
                    this.txtExpression.Text = "";
                    this.txtExpression.Enabled = true;
                    break;
                case 4:
                    this.label37.Text = rm.GetString("Label4");
                    this.txtExpression.Text = "0";
                    this.txtExpression.Enabled = true;
                    break;
                case 5:
                    this.label37.Text = rm.GetString("Label4");
                    this.txtExpression.Text = "0";
                    this.txtExpression.Enabled = true;
                    break;
                case 6:
                    this.label37.Text = rm.GetString("Label5");
                    this.txtExpression.Text = "\"\",\"\"";
                    this.txtExpression.Enabled = true;
                    break;
                case 7:
                    this.txtExpression.Enabled = false;
                    break;
                case 8:
                    this.label37.Text = rm.GetString("Label6");
                    this.txtExpression.Text = "\"\",\"\"";
                    this.txtExpression.Enabled = true;
                    break;
                default:
                    this.txtExpression.Enabled = false;
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.txtGetTitleName.Text.Trim().ToString() == "")
            {
                MessageBox.Show(rm.GetString ("Error8"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtGetTitleName.Focus();
                return;
            }

            if (this.txtGetStart.Text.Trim().ToString() == "")
            {
                MessageBox.Show(rm.GetString("Error9"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtGetStart.Focus();
                return;

            }

            if (this.txtGetEnd.Text.Trim().ToString() == "")
            {
                MessageBox.Show(rm.GetString("Error10"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtGetEnd.Focus();
                return;
            }

            if (this.comLimit.SelectedIndex == -1)
            {
                this.comLimit.SelectedIndex = 0;
            }

            ListViewItem item = new ListViewItem();
            item.Text = this.txtGetTitleName.Text.ToString();
            item.SubItems.Add(this.comGetType.SelectedItem.ToString());
            item.SubItems.Add(cTool.ClearFlag(this.txtGetStart.Text.ToString()));
            item.SubItems.Add(cTool.ClearFlag(this.txtGetEnd.Text.ToString()));
            item.SubItems.Add(this.comLimit.SelectedItem.ToString());
            item.SubItems.Add(this.txtRegion.Text.ToString());
            item.SubItems.Add(this.comExportLimit.SelectedItem.ToString());
            item.SubItems.Add(this.txtExpression.Text.ToString());

            rData(item);

            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmAddGatherRule_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }
    }
}