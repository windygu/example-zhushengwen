using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SoukeyNetget.Task;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Resources;

namespace SoukeyNetget
{
    public partial class frmUpgradeTask : Form
    {
        private ResourceManager rm;

        //����һ���������ڸ��½������Ϣ��������������
        delegate void ShowProgressDelegate(int total, int CurI, ListViewItem Litem, bool statusDone);

        //ShowProgressDelegate showProgress = new ShowProgressDelegate(ShowProgress);

        //Delegate m_senderDelegate = null;

        public frmUpgradeTask()
        {
            InitializeComponent();
        }

        //����ָ����������ϵͳ����ʱ�ͽ���������ؽ���
        public frmUpgradeTask(string FileName)
        {
            InitializeComponent();

            AddUpgradeTask(FileName);
        }

        private void tmenuFindTask_Click(object sender, EventArgs e)
        {
            //����һ����̨�߳����ڵ������ݲ���

            this.tmenuAddTask.Enabled = false;
            this.tmenuExit.Enabled = false;
            this.tmenuFindTask.Enabled = false;
            this.tmenuResetTask.Enabled = false;
            this.tmenuStart.Enabled = false;

            ShowProgressDelegate showProgress = new ShowProgressDelegate(ShowProgress);

            cAutoFindUpgradeTask aAddTask = new cAutoFindUpgradeTask(this, showProgress);
            Thread t = new Thread(new ThreadStart(aAddTask.AutoAddTask));
            t.IsBackground = true;
            t.Start();
           

        }

        public void ShowProgress(int total, int CurI, ListViewItem Litem, bool done)
        {
            if (this.ProBar.Maximum != total)
            {
                this.ProBar.Maximum = total;
            }

            ProBar.Value = CurI;


            if (Litem != null)
            {
                this.listTask.Items.Add(Litem);
                this.stalabel.Text = rm.GetString ("Info103") + Litem.SubItems[1].Text;
            }
            else
            {
                this.stalabel.Text = rm.GetString("Info104");
            }

            if (done)
            {
                this.stalabel.Text = rm.GetString("State8");
                this.ProBar.Visible = false;

                this.tmenuAddTask.Enabled = true ;
                this.tmenuExit.Enabled = true;
                this.tmenuFindTask.Enabled = false ;
                this.tmenuResetTask.Enabled = true;
                this.tmenuStart.Enabled = true;
            }
        }

        private void tmenuExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tmenuResetTask_Click(object sender, EventArgs e)
        {
            this.listTask.Items.Clear();
            this.tmenuAddTask.Enabled = true;
            this.tmenuExit.Enabled = true;
            this.tmenuFindTask.Enabled = true;
            this.tmenuResetTask.Enabled = true;
            this.tmenuStart.Enabled = true;
        }

        private void tmenuAddTask_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = rm.GetString ("Info105");

            openFileDialog1.InitialDirectory = Program.getPrjPath() + "tasks";
            openFileDialog1.Filter = "Soukey Task Files(*.xml)|*.xml";


            if (this.openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            string FileName = this.openFileDialog1.FileName;

            AddUpgradeTask(FileName);

        }

        private void AddUpgradeTask(string FileName)
        {
            cTask t = new cTask();

            ListViewItem Litem;
            
            try
            {
                t.LoadTask(FileName);
                t = null;
                MessageBox.Show(rm.GetString("Info106"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            catch (cSoukeyException)
            {
                Litem = new ListViewItem();
                Litem.ImageIndex = 0;
                Litem.Text = "�ȴ�����";
                Litem.SubItems.Add(t.TaskName);
                Litem.SubItems.Add(t.TaskVersion.ToString());
                Litem.SubItems.Add("");
                Litem.SubItems.Add(Path.GetDirectoryName (FileName ));
                Litem.SubItems.Add("");
                this.listTask.Items.Add(Litem);

                Application.DoEvents();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Info107") + ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        private void tmenuStart_Click(object sender, EventArgs e)
        {
            if (this.listTask.Items.Count == 0)
            {
                this.stalabel.Text = rm.GetString ("Info108");
                return;
            }

            this.tmenuAddTask.Enabled = false;
            this.tmenuExit.Enabled = false;
            this.tmenuFindTask.Enabled = false;
            this.tmenuResetTask.Enabled = false;
            this.tmenuStart.Enabled = false;

            cUpgradeTask cu = new cUpgradeTask();

            for (int i =0;i<this.listTask.Items.Count ;i++)
            {
                this.listTask.Items[i].Text = rm.GetString("Info109");
                this.listTask.Items[i].ImageIndex = 1;
                Application.DoEvents();
                try
                {
                    if (this.listTask.Items[i].SubItems[3].Text == null || this.listTask.Items[i].SubItems[3].Text == "")
                    {
                        cu.UpdradeTask(this.listTask.Items[i].SubItems[4].Text + "\\" + this.listTask.Items[i].SubItems[1].Text + ".xml", this.IsAutoBackup.Checked, false);
                    }
                    else
                    {
                        cu.UpdradeTask(this.listTask.Items[i].SubItems[4].Text + "\\" + this.listTask.Items[i].SubItems[1].Text + ".xml", this.IsAutoBackup.Checked,true );
                    }
                    this.listTask.Items[i].ImageIndex = 2;
                    this.listTask.Items[i].Text = rm.GetString("Info110");
                    this.listTask.Items[i].SubItems[2].Text = "1.3";
                    Application.DoEvents();

                }
                catch (System.Exception ex)
                {
                    this.listTask.Items[i].ImageIndex = 3;
                    this.listTask.Items[i].Text = rm.GetString("Info111");
                    this.listTask.Items[i].SubItems[5].Text = rm.GetString("Info112") +ex.Message;
                    Application.DoEvents();
                }
                
                
                //Thread.Sleep(100);
            }

            cu = null;

            this.tmenuExit.Enabled = true ;
            this.tmenuResetTask.Enabled = true ;
        }

        private void frmUpgradeTask_Load(object sender, EventArgs e)
        {
            rm = new ResourceManager("SoukeyNetget.Resources.globalUI", Assembly.GetExecutingAssembly());
        }

        private void frmUpgradeTask_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

    }

    class cAutoFindUpgradeTask
    {
        ContainerControl m_sender = null;
        Delegate m_senderDelegate = null;

        public cAutoFindUpgradeTask()
        {
        }

        public cAutoFindUpgradeTask(ContainerControl sender, Delegate senderDelegate)
        {
            m_sender = sender;
            m_senderDelegate = senderDelegate;
        }

        ~cAutoFindUpgradeTask()
        {
        }

        public void AutoAddTask()
        {
            //��ȡ���з��࣬Ȼ����ݷ����������
            cTaskClass xmlTClass = new cTaskClass();
            cTaskIndex xmlTasks;

            //��ȡ���ж��ٸ��������
            int TClassCount = xmlTClass.GetTaskClassCount();

            for (int i = 0; i < TClassCount; i++)
            {
                xmlTasks = new cTaskIndex();

                //��ȡ�˷����¹��й��ٸ�����
                xmlTasks.GetTaskDataByClass(xmlTClass.GetTaskClassName(i));

                int count = xmlTasks.GetTaskClassCount();

                cTask t;

                ListViewItem Litem;

                for (int j = 0; j < count; j++)
                {
                    t = new cTask();
                    try
                    {
                        t.LoadTask(xmlTClass.GetTaskClassPathByName(xmlTClass.GetTaskClassName(i)) + "\\" + xmlTasks.GetTaskName(j) + ".xml");
                        t = null;
                    }
                    catch (cSoukeyException)
                    {
                        Litem = new ListViewItem();

                        Litem.ImageIndex = 0;
                        Litem.Text = "�ȴ�����";
                        Litem.SubItems.Add(xmlTasks.GetTaskName(j));
                        Litem.SubItems.Add(t.TaskVersion.ToString());
                        Litem.SubItems.Add(xmlTClass.GetTaskClassName(i));
                        Litem.SubItems.Add(xmlTClass.GetTaskClassPathByName(xmlTClass.GetTaskClassName(i)));
                        Litem.SubItems.Add("");
                        //���½�������Ϣ
                        m_sender.BeginInvoke(m_senderDelegate, new object[] { TClassCount + count, j + i, Litem, false });

                        Application.DoEvents();
                    }
                    catch (System.Exception ex)
                    {

                    }


                }

                //���½�������Ϣ
                m_sender.BeginInvoke(m_senderDelegate, new object[] { TClassCount, i, null, false });
            }

            m_sender.BeginInvoke(m_senderDelegate, new object[] { TClassCount, TClassCount,null, true  });

            xmlTClass = null;
        }

      
    }

   
}