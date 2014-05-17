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
    public partial class frmSelectTask : Form
    {
        public delegate void ReturnTaskItem(ListView.SelectedListViewItemCollection lItems);

        public ReturnTaskItem RTaskItem;

        private ResourceManager rm;

        public frmSelectTask()
        {
            InitializeComponent();
        }

        private void frmSelectTask_Load(object sender, EventArgs e)
        {
            rm = new ResourceManager("SoukeyNetget.Resources.globalUI", Assembly.GetExecutingAssembly());

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
                    litem.SubItems.Add(cGlobalParas.ConvertName(int.Parse(xmlTasks.GetTaskType(i))));
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
                return;
            }
        }

        private void ReturnValue()
        {
            if (this.listTask.Items.Count == 0)
            {
                this.errorProvider1.SetError(this.listTask, rm.GetString("Info83"));
                return;
            }
            //int tClassID = int.Parse(this.listTask.SelectedItems[0].Name.Substring(1, this.listTask.SelectedItems[0].Name.Length - 1).ToString());
            //string tClassName = this.listTask.SelectedItems[0].Text.ToString();
            RTaskItem(this.listTask.SelectedItems);
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            ReturnValue();
            this.Close();
        }

        private void listTask_DoubleClick(object sender, EventArgs e)
        {
            ReturnValue();
            this.Close();
        }

        private void frmSelectTask_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }
    }
}