using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Reflection ;
using System.Resources ;

namespace SoukeyNetget
{
    public partial class frmAddPlanTask : Form
    {

        public delegate void ReturnRunTask(cGlobalParas.RunTaskType rType,string TaskName,string Para);
        public ReturnRunTask RTask;

        private ResourceManager rm;

        public frmAddPlanTask()
        {
            InitializeComponent();
        }

        private void frmAddPlanTask_Load(object sender, EventArgs e)
        {
            int i;

            Task.cTaskClass xmlTClass = new Task.cTaskClass();
            string TaskClass;

            int TClassCount = xmlTClass.GetTaskClassCount();

            //每次点击任务分类要根据任务分类的编号索引任务，但combobox无法保存编号
            //所以采用这种方法
            for (i = 0; i < TClassCount; i++)
            {

                TaskClass = xmlTClass.GetTaskClassName(i);
                TaskClass += "                                                                                                                         ";
                TaskClass += "-" + xmlTClass.GetTaskClassID(i);
                this.comTaskClass.Items.Add(TaskClass);
            }
            xmlTClass = null;

            rm = new ResourceManager("SoukeyNetget.Resources.globalUI", Assembly.GetExecutingAssembly());
        }

        private void comTaskClass_SelectedIndexChanged(object sender, EventArgs e)
        {
             this.listTask.Items.Clear();

            try
            {

                ListViewItem litem;
                int TaskClassID = 0;

                int Starti = this.comTaskClass.SelectedItem.ToString().IndexOf("-");

                TaskClassID = int.Parse(this.comTaskClass.SelectedItem.ToString().Substring((Starti + 1), (this.comTaskClass.SelectedItem.ToString().Length - Starti - 1)));

                Task.cTaskIndex xmlTasks = new Task.cTaskIndex();
                xmlTasks.GetTaskDataByClass(TaskClassID);

                //开始初始化此分类下的任务
                int count = xmlTasks.GetTaskClassCount();
                this.listTask.Items.Clear();

                for (int i = 0; i < count; i++)
                {
                    litem = new ListViewItem();
                    litem.Name = "S" + xmlTasks.GetTaskID(i);
                    litem.Text = xmlTasks.GetTaskName(i);
                    litem.SubItems.Add(this.comTaskClass.SelectedItem.ToString().Substring(0, this.comTaskClass.SelectedItem.ToString().IndexOf("-")).Trim());
                    litem.SubItems.Add(cGlobalParas.ConvertName(int.Parse (xmlTasks.GetTaskType(i))));
                    litem.SubItems.Add(cGlobalParas.ConvertName(int.Parse(xmlTasks.GetTaskRunType(i))));
                    litem.ImageIndex = 0;
                    this.listTask.Items.Add(litem);
                    litem = null;
                }
                xmlTasks = null;
            }

            catch (System.IO.IOException)
            {
                MessageBox.Show(rm.GetString("Info72"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            catch (System.Exception)
            {
                MessageBox.Show(rm.GetString("Info73"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return ;
            }
        
        }

        private void raSoukeyTask_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raSoukeyTask.Checked == true)
            {
                this.groupBox1.Text = rm.GetString("Label16");
                this.panel1.Visible = true;
                this.panel2.Visible = false;
                this.panel3.Visible = false;
            }
        }

        private void raOtherTask_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raOtherTask.Checked == true)
            {
                this.groupBox1.Text = rm.GetString("Label17");
                this.panel1.Visible = false;
                this.panel2.Visible = true;
                this.panel3.Visible = false;
            }
        }

        private void raDataTask_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raDataTask.Checked == true)
            {
                this.groupBox1.Text = rm.GetString("Label18");
                this.panel1.Visible = false;
                this.panel2.Visible = false;
                this.panel3.Visible = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            this.openFileDialog1.Title = rm.GetString("Info74");

            openFileDialog1.InitialDirectory = Program.getPrjPath();
            openFileDialog1.Filter = "All Files(*.*)|*.*";

            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txtFileName.Text = this.openFileDialog1.FileName;
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            SelectTask();
        }

        private void SelectTask()
        {
            if (this.raSoukeyTask.Checked == true)
            {
                RTask(cGlobalParas.RunTaskType.SoukeyTask, this.listTask.SelectedItems[0].SubItems[1].Text + "\\" + this.listTask.SelectedItems[0].Text, "");
            }
            else if (this.raOtherTask.Checked == true)
            {
                RTask(cGlobalParas.RunTaskType.OtherTask, this.txtFileName.Text, this.txtPara.Text);
            }
            else if (this.raDataTask.Checked == true)
            {
                if (this.raAccessTask.Checked == true)
                {
                    RTask(cGlobalParas.RunTaskType.DataTask, cGlobalParas.ConvertName ((int) cGlobalParas.DatabaseType.Access), this.txtDataSource.Text + "Para=" + this.comTableName.SelectedItem.ToString());
                }
                else if (this.raMSSQLTask.Checked == true)
                {
                    RTask(cGlobalParas.RunTaskType.DataTask, cGlobalParas.ConvertName((int)cGlobalParas.DatabaseType.MSSqlServer), this.txtDataSource.Text + "Para=" + this.comTableName.SelectedItem.ToString());
                }
                else if (this.raMySqlTask.Checked == true)
                {
                    RTask(cGlobalParas.RunTaskType.DataTask, cGlobalParas.ConvertName((int)cGlobalParas.DatabaseType.MySql), this.txtDataSource.Text + "Para=" + this.comTableName.SelectedItem.ToString());
                }
            }

            this.Close();
        }

        private void listTask_DoubleClick(object sender, EventArgs e)
        {
            SelectTask();
        }
       
        private void GetDataSource(string strDataConn)
        {
            this.txtDataSource.Text = strDataConn;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            frmSetData fSD = new frmSetData();

            if (this.raAccessTask.Checked == true)
                fSD.FormState = 0;
            else if (this.raMSSQLTask.Checked == true)
                fSD.FormState = 1;
            else if (this.raMySqlTask.Checked == true)
                fSD.FormState = 2;

            fSD.rDataSource = new frmSetData.ReturnDataSource(GetDataSource);
            fSD.ShowDialog();
            fSD.Dispose();
        }

        private void comTableName_DropDown(object sender, EventArgs e)
        {
            if (this.raAccessTask.Checked == true)
            {
                FillAccessQuery();
            }
            else if (this.raMSSQLTask.Checked == true)
            {
                FillMSSqlQuery();
            }
            else if (this.raMySqlTask.Checked == true)
            {
                FillMySqlQuery();
            }
        }

        private void FillAccessQuery()
        {
            if (this.comTableName.Items.Count != 0)
                return;

            OleDbConnection conn = new OleDbConnection();
            conn.ConnectionString = this.txtDataSource.Text;

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Info75") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string[] Restrictions = new string[4];
            Restrictions[3] = "VIEW";

            DataTable tb = conn.GetSchema("Tables",Restrictions );

            foreach (DataRow r in tb.Rows)
            {
                 this.comTableName.Items.Add(r[2].ToString());
            }

        }

        private void FillMSSqlQuery()
        {
            if (this.comTableName.Items.Count != 0)
                return;

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = this.txtDataSource.Text;

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Info75") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = conn.GetSchema("Procedures");

            foreach (DataRow r in tb.Rows)
            {

                this.comTableName.Items.Add(r[5].ToString());

            }
        }

        private void FillMySqlQuery()
        {
            if (this.comTableName.Items.Count != 0)
                return;

            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = this.txtDataSource.Text;

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Info75") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = conn.GetSchema("Procedures");

            foreach (DataRow r in tb.Rows)
            {

                this.comTableName.Items.Add(r[3].ToString());

            }
        }

        private void frmAddPlanTask_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

    }
}