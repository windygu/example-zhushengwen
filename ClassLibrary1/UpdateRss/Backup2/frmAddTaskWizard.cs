using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SoukeyNetget.Task;
using System.Data.OleDb;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Resources;

namespace SoukeyNetget
{
    public partial class frmAddTaskWizard : Form
    {
        private int CurStep = 1;

        private Single m_SupportTaskVersion = Single.Parse("1.3");

        private ResourceManager rm;

        //此类别可处理的任务版本号，注意从1.3开始，任务处理不再向前兼容，需升级后方可操作
        public Single SupportTaskVersion
        {
            get { return m_SupportTaskVersion; }
        }

        public delegate void ReturnTaskClass(string tClass);
        public ReturnTaskClass rTClass;

        //是否已保存了任务，如果保存，即便在取消的时候，
        //也需要将任务所述分类进行返回，主要是用在“应用”
        //和“取消”按钮的判断上
        private bool IsSaveTask = false;

        //定义一个ToolTip
        ToolTip HelpTip = new ToolTip();
        Task.cUrlAnalyze gUrl = new Task.cUrlAnalyze();

        //定义一个集合类用于存储Url地址对应的导航规则,因为当前界面无法显示所有的
        //导航规则，所以，需要通过一个结合类进行存储
        private List<cNavigRules> m_listNaviRules = new List<cNavigRules>();

        public frmAddTaskWizard()
        {
            InitializeComponent();

            IniData();
        }

        //设置ToolTip的信息
        private void SetTooltip()
        {
            HelpTip.SetToolTip(this.tTask, @"输入任务名称，任务名称可以" + "\r\n" + @"是中文或英文，但不允许出现.*\/,等");
        }

        #region 启动初始化数据 根据启动的状态进行加载：新建、修改、浏览等

        private void IniData()
        {
            //初始化页面加载数据
            rm = new ResourceManager("SoukeyNetget.Resources.globalUI", Assembly.GetExecutingAssembly());

            //根据当前的区域进行显示信息的加载
            ResourceManager rmPara = new ResourceManager("SoukeyNetget.Resources.globalPara", Assembly.GetExecutingAssembly());

            this.TaskType.Items.Add(rmPara.GetString("TaskType1"));
            this.TaskType.Items.Add(rmPara.GetString("TaskType4"));
            this.TaskType.SelectedIndex = 0;

            this.comRunType.Items.Add(rmPara.GetString("TaskRunType2"));
            this.comRunType.Items.Add(rmPara.GetString("TaskRunType1"));
            this.comRunType.SelectedIndex = 1;

            this.comWebCode.Items.Add(rmPara.GetString("WebCode2"));
            this.comWebCode.Items.Add(rmPara.GetString("WebCode3"));
            this.comWebCode.Items.Add(rmPara.GetString("WebCode4"));
            this.comWebCode.Items.Add(rmPara.GetString("WebCode5"));
            this.comWebCode.Items.Add(rmPara.GetString("WebCode6"));


            this.comExportUrlCode.Items.Add(rmPara.GetString("WebCode1"));
            this.comExportUrlCode.Items.Add(rmPara.GetString("WebCode3"));
            this.comExportUrlCode.Items.Add(rmPara.GetString("WebCode4"));
            this.comExportUrlCode.Items.Add(rmPara.GetString("WebCode5"));
            this.comExportUrlCode.Items.Add(rmPara.GetString("WebCode6"));
            this.comExportUrlCode.SelectedIndex = 0;

            this.comWebCode.SelectedIndex = 0;

            this.comUrlEncode.Items.Add(rmPara.GetString("WebCode3"));
            this.comUrlEncode.Items.Add(rmPara.GetString("WebCode4"));
            this.comUrlEncode.Items.Add(rmPara.GetString("WebCode5"));
            this.comUrlEncode.Items.Add(rmPara.GetString("WebCode6"));

            rmPara = null;

            this.txtSavePath.Text = Program.getPrjPath() + "data";

         

            //初始化页面加载时各个控件的状态


            //初始化任务分类
            //开始初始化树形结构,取xml中的数据,读取任务分类
            this.comTaskClass.Items.Add(rm.GetString ("Label31"));
            Task.cTaskClass xmlTClass = new Task.cTaskClass();

            int TClassCount = xmlTClass.GetTaskClassCount();

            for (int i = 0; i < TClassCount; i++)
            {
                this.comTaskClass.Items.Add(xmlTClass.GetTaskClassName(i));
            }
            xmlTClass = null;

            this.comTaskClass.SelectedIndex = 0;
        }

        #endregion
      
        #region 窗体进入的状态
        private cGlobalParas.FormState m_FormState;
        public cGlobalParas.FormState FormState
        {
            get { return m_FormState; }
            set { m_FormState = value; }
        }
        #endregion

        private void frmAddTaskWizard_Load(object sender, EventArgs e)
        {
            //对Tooltip进行初始化设置
            HelpTip.ToolTipIcon = ToolTipIcon.None;
            HelpTip.ForeColor = Color.YellowGreen;
            HelpTip.BackColor = Color.LightGray;
            HelpTip.AutoPopDelay = 5000;
            HelpTip.ShowAlways = true;
            HelpTip.ToolTipTitle = "";

            SetTooltip();

            switch (this.FormState)
            {
                case cGlobalParas.FormState.New:
                    break;
                case cGlobalParas.FormState.Edit:
                    //编辑状态进来不能修改分类

                    this.tTask.ReadOnly = true;
                    this.comTaskClass.Enabled = false;

                    break;
                case cGlobalParas.FormState.Browser:
                    
                    break;
                default:
                    break;
            }

            this.labLogSavePath.Text = Program.getPrjPath() + "log";

            this.IsSave.Text = "false";
        }

        private void butNext_Click(object sender, EventArgs e)
        {
            try
            {
                CheckData(CurStep);
            }
            catch (cSoukeyException ex)
            {
                MessageBox.Show(ex.Message, "Soukey采摘 系统信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (CurStep == 10)
            {
                //表示完成 进行保存

                SaveTask("");

                if (this.comTaskClass.SelectedIndex == 0)
                {
                    rTClass("");
                }
                else
                {
                    rTClass(this.comTaskClass.SelectedItem.ToString());
                }

                this.Close();

            }
            else
            {
                for (int i = 1; i <= 10; i++)
                {
                     this.Controls["step" + i].Visible = false;
                }
                

                if (CurStep == 9)
                {
                    this.butNext.Text = rm.GetString ("Label30");
                    this.butNext.TextAlign = ContentAlignment.MiddleRight;
                    this.butNext.Image = global ::SoukeyNetget.Properties.Resources.A10;
                    this.butNext.ImageAlign = ContentAlignment.MiddleLeft;
                    
                }

                string CurLab="lab" + CurStep;
                string CurPic="pic" + CurStep ;

                string NextLab="lab" + (CurStep +1);
                string NextPic="pic" + (CurStep +1);

                ((Label)this.Controls["panel2"].Controls[CurLab]).Font = new System.Drawing.Font(DefaultFont, System.Drawing.FontStyle.Regular) ;
                ((PictureBox)this.Controls["panel2"].Controls[CurPic]).Image = global :: SoukeyNetget.Properties.Resources.agree;

                ((Label)this.Controls["panel2"].Controls[NextLab]).Font = new System.Drawing.Font(DefaultFont, System.Drawing.FontStyle.Bold); 
                ((PictureBox)this.Controls["panel2"].Controls[NextPic]).Image = global ::SoukeyNetget.Properties.Resources.A33;

                CurStep++;
                this.Controls["step" + CurStep].Visible = true;
                string info=this.Controls["panel2"].Controls[NextLab].Text ;
                this.labInfo.Text ="— " + info.Substring (info.IndexOf ("、")+1,info.Length -info.IndexOf ("、")-1);
                
            }

            if (this.butPre.Enabled == false)
            {
                this.butPre.Enabled = true;
            }

        }

        private void butPre_Click(object sender, EventArgs e)
        {
            for (int i =1; i <= 10; i++)
            {
                this.Controls["step" + i].Visible = false;
            }

            if (CurStep == 10)
            {
                //表示已经配置结束，可以进行任务信息总结，并保存
                this.butNext.Text = rm.GetString ("Label27");
                this.butNext.TextAlign = ContentAlignment.MiddleLeft;
                this.butNext.Image = global ::SoukeyNetget.Properties.Resources.right;
                this.butNext.ImageAlign = ContentAlignment.MiddleRight;

            }

            if (CurStep == 2)
            {
                //表示到了第一步
                this.butPre.Enabled = false;

            }

            string CurLab = "lab" + CurStep;
            string CurPic = "pic" + CurStep;

            string PreLab = "lab" + (CurStep - 1);
            string PrePic = "pic" + (CurStep - 1);

            ((Label)this.Controls["panel2"].Controls[CurLab]).Font = new System.Drawing.Font(DefaultFont, System.Drawing.FontStyle.Regular);
            ((PictureBox)this.Controls["panel2"].Controls[CurPic]).Image = null;

            ((Label)this.Controls["panel2"].Controls[PreLab]).Font = new System.Drawing.Font(DefaultFont, System.Drawing.FontStyle.Bold );
            ((PictureBox)this.Controls["panel2"].Controls[PrePic]).Image = global :: SoukeyNetget.Properties.Resources.A33;

            CurStep--;
            this.Controls["step" + CurStep].Visible = true;

            this.Controls["step" + CurStep].Visible = true;
            string info = this.Controls["panel2"].Controls[PreLab].Text;
            this.labInfo.Text = "— " + info.Substring(info.IndexOf("、") + 1, info.Length - info.IndexOf("、") - 1);

        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            frmBrowser wftm = new frmBrowser();
            wftm.getFlag = 0;
            wftm.rCookie = new frmBrowser.ReturnCookie(GetCookie);
            wftm.ShowDialog();
            wftm.Dispose();

        }

        private void GetCookie(string strCookie)
        {
            this.txtCookie.Text = strCookie;
        }

        private void cmdAddWeblink_Click(object sender, EventArgs e)
        {
            frmAddGatherUrl f = new frmAddGatherUrl();
            f.rData = this.GetWebLink;
            f.ShowDialog();
            f.Dispose();

        }

        private void GetWebLink(ListViewItem Litem, cNavigRules nRuls)
        {
            this.listWeblink.Items.Add(Litem);
            
            m_listNaviRules.Add(nRuls);

            //this.listWeblink

            if (Litem.SubItems[1].Text == "Y")
            {
                //添加导航规则

                this.dataNRule.Rows.Clear();

                for (int i = 0; i < m_listNaviRules.Count; i++)
                {
                    if (Litem.Text == m_listNaviRules[i].Url)
                    {
                        for (int j = 0; j < m_listNaviRules[i].NavigRule.Count; j++)
                        {
                            this.dataNRule.Rows.Add(m_listNaviRules[i].NavigRule[j].Level, m_listNaviRules[i].NavigRule[j].NavigRule);
                        }
                    }
                }

            }
            else
            {
                this.dataNRule.Rows.Clear();
            }

             this.listWeblink.Items[this.listWeblink.Items.Count - 1].Selected = true;
        }

        private void cmdDelWeblink_Click(object sender, EventArgs e)
        {
            if (this.listWeblink.Items.Count == 0)
                return;

            //如果有导航需要删除导航的规则
            //删除存储的导航规则
            if (this.listWeblink.SelectedItems[0].SubItems[1].Text.ToString() == "Y")
            {
                for (int i = 0; i < m_listNaviRules.Count; i++)
                {
                    if (this.listWeblink.SelectedItems[0].Text == m_listNaviRules[i].Url)
                        m_listNaviRules.Remove(m_listNaviRules[i]);
                }
            }

            if (this.listWeblink.SelectedItems.Count != 0)
            {
                this.listWeblink.Items.Remove(this.listWeblink.SelectedItems[0]);
            }

            if (this.listWeblink.Items.Count == 0)
            {
                this.txtWeblinkDemo.Text = "";
            }

            this.IsSave.Text = "true";

        }

        private void listWeblink_Click(object sender, EventArgs e)
        {
            if (this.listWeblink.SelectedItems[0].SubItems[1].Text == "Y")
            {
                //添加导航规则

                this.dataNRule.Rows.Clear();

                for (int i = 0; i < m_listNaviRules.Count; i++)
                {
                    if (this.listWeblink.SelectedItems[0].Text == m_listNaviRules[i].Url)
                    {
                        for (int j = 0; j < m_listNaviRules[i].NavigRule.Count; j++)
                        {
                            this.dataNRule.Rows.Add(m_listNaviRules[i].NavigRule[j].Level, m_listNaviRules[i].NavigRule[j].NavigRule);
                        }
                    }
                }

            }
            else
            {
                this.dataNRule.Rows.Clear();
            }

        }

        private void cmdAddCutFlag_Click(object sender, EventArgs e)
        {
            frmAddGatherRule f = new frmAddGatherRule();
            f.rData = this.GetGatherRule;
            f.ShowDialog();
            f.Dispose();
        }

        private void GetGatherRule(ListViewItem Litem)
        {
            this.listWebGetFlag.Items.Add(Litem);
        }

        private void cmdDelCutFlag_Click(object sender, EventArgs e)
        {
            if (this.listWebGetFlag.SelectedItems.Count != 0)
            {
                this.listWebGetFlag.Items.Remove(this.listWebGetFlag.SelectedItems[0]);

                this.IsSave.Text = "true";
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            frmSetData fSD = new frmSetData();

            if (this.raExportAccess.Checked == true)
                fSD.FormState = 0;
            else if (this.raExportMSSQL.Checked == true)
                fSD.FormState = 1;
            else if (this.raExportMySql.Checked == true)
                fSD.FormState = 2;

            fSD.rDataSource = new frmSetData.ReturnDataSource(GetDataSource);
            fSD.ShowDialog();
            fSD.Dispose();
        }

        private void GetDataSource(string strDataConn)
        {
            this.txtDataSource.Text = strDataConn;
        }

        private void comTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillInsertSql(this.comTableName.SelectedItem.ToString());

            this.IsSave.Text = "true";
        }

        private void comTableName_TextChanged(object sender, EventArgs e)
        {
            string iSql = "insert into " + this.comTableName.Text + "(";
            string strColumns = "";
            string strColumnsValue = "";

            for (int j = 0; j < this.listWebGetFlag.Items.Count; j++)
            {
                if (this.listWebGetFlag.Items[j].SubItems[1].Text == "文本" || this.listWebGetFlag.Items[j].SubItems[1].Text == "Text")
                    strColumns += this.listWebGetFlag.Items[j].Text + ",";
                strColumnsValue += "\"{" + this.listWebGetFlag.Items[j].Text + "}\",";

            }

            if (strColumns != "")
            {
                strColumns = strColumns.Substring(0, strColumns.Length - 1);
                strColumnsValue = strColumnsValue.Substring(0, strColumnsValue.Length - 1);
            }


            iSql = iSql + strColumns + ") values (" + strColumnsValue + ")";
            this.txtInsertSql.Text = iSql;
        }

        private void FillInsertSql(string TableName)
        {
            string iSql = "";
            string strColumns = "";

            iSql = "insert into " + TableName + " (";

            DataTable tc = GetTableColumns(TableName);

            for (int i = 0; i < tc.Rows.Count; i++)
            {
                strColumns += tc.Rows[i][3].ToString() + ",";
            }

            strColumns = strColumns.Substring(0, strColumns.Length - 1);

            iSql = iSql + strColumns + ") values ( ";

            string strColumnsValue = "";

            for (int j = 0; j < this.listWebGetFlag.Items.Count; j++)
            {
                if (this.listWebGetFlag.Items[j].SubItems[1].Text == "文本" || this.listWebGetFlag.Items[j].SubItems[1].Text == "Text")
                    strColumnsValue += "\"{" + this.listWebGetFlag.Items[j].Text + "}\",";

            }

            if (strColumnsValue != "")
                strColumnsValue = strColumnsValue.Substring(0, strColumnsValue.Length - 1);

            iSql = iSql + strColumnsValue + ")";

            this.txtInsertSql.Text = iSql;

        }

        private void FillAccessTable()
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
                MessageBox.Show(rm.GetString("Error12") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {
                if (r[3].ToString() == "TABLE")
                {
                    this.comTableName.Items.Add(r[2].ToString());
                }

            }

        }

        private void FillMSSqlTable()
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
                MessageBox.Show(rm.GetString("Error12") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {

                this.comTableName.Items.Add(r[2].ToString());

            }
        }

        private void FillMySql()
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
                MessageBox.Show(rm.GetString("Error12") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {

                this.comTableName.Items.Add(r[2].ToString());

            }
        }

        private void comTableName_DragDrop(object sender, DragEventArgs e)
        {
            if (this.comTableName.Items.Count == 0)
            {
                if (this.raExportAccess.Checked == true)
                {
                    FillAccessTable();
                }
                else if (this.raExportMSSQL.Checked == true)
                {
                    FillMSSqlTable();
                }
                else if (this.raExportMySql.Checked == true)
                {
                    FillMySql();
                }

            }
        }

        private DataTable GetTableColumns(string tName)
        {
            DataTable tc = new DataTable();

            try
            {

                if (this.raExportAccess.Checked == true)
                {
                    OleDbConnection conn = new OleDbConnection();
                    conn.ConnectionString = this.txtDataSource.Text;

                    conn.Open();

                    string[] Restrictions = new string[4];
                    Restrictions[2] = tName;

                    tc = conn.GetSchema("Columns", Restrictions);

                    return tc;

                }
                else if (this.raExportMSSQL.Checked == true)
                {
                    SqlConnection conn = new SqlConnection();
                    conn.ConnectionString = this.txtDataSource.Text;

                    conn.Open();

                    string[] Restrictions = new string[4];
                    Restrictions[2] = tName;

                    tc = conn.GetSchema("Columns", Restrictions);

                    return tc;
                }
                else if (this.raExportMySql.Checked == true)
                {
                    MySqlConnection conn = new MySqlConnection();
                    conn.ConnectionString = this.txtDataSource.Text;

                    conn.Open();

                    string[] Restrictions = new string[4];
                    Restrictions[2] = tName;

                    tc = conn.GetSchema("Columns", Restrictions);

                    return tc;
                }

                return tc;

            }
            catch (System.Exception)
            {
                return null;
            }


        }

        private void cmdBrowser_Click(object sender, EventArgs e)
        {
            if (this.raExportTxt.Checked == true)
            {
                this.saveFileDialog1.Title = rm.GetString("Info12");

                saveFileDialog1.InitialDirectory = Program.getPrjPath();
                saveFileDialog1.Filter = "txt Files(*.txt)|*.txt|All Files(*.*)|*.*";
            }
            else if (this.raExportExcel.Checked == true)
            {
                this.saveFileDialog1.Title = rm.GetString("Info13");

                saveFileDialog1.InitialDirectory = Program.getPrjPath();
                saveFileDialog1.Filter = "Excel Files(*.xls)|*.xls|All Files(*.*)|*.*";
            }

            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txtFileName.Text = this.saveFileDialog1.FileName;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            frmBrowser wftm = new frmBrowser();
            wftm.getFlag = 2;
            wftm.rExportCookie = new frmBrowser.ReturnExportCookie(GetExportCookie);
            wftm.ShowDialog();
            wftm.Dispose();
        }

        private void GetExportCookie(string strCookie)
        {
            this.txtExportCookie.Text = strCookie;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.rmenuGetFormat.Show(this.button9, 0, 21);
        }

        private void IsStartTrigger_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsStartTrigger.Checked == true)
            {
                this.groupBox13.Enabled = true;

            }
            else
            {
                this.groupBox13.Enabled = false;
            }
            this.IsSave.Text = "true";
        }

        private void cmdAddTask_Click(object sender, EventArgs e)
        {
            frmAddPlanTask f = new frmAddPlanTask();
            f.RTask = GetTaskInfo;
            f.ShowDialog();
            f.Dispose();

            this.IsSave.Text = "true";
        }

        private void GetTaskInfo(cGlobalParas.RunTaskType rType, string TaskName, string TaskPara)
        {
            ListViewItem Litem = new ListViewItem();

            Litem.Text = cGlobalParas.ConvertName((int)rType);
            Litem.SubItems.Add(TaskName);
            Litem.SubItems.Add(TaskPara);

            this.listTask.Items.Add(Litem);

        }

        private void cmdDelTask_Click(object sender, EventArgs e)
        {
            this.listTask.Focus();
            SendKeys.Send("{Del}");

            this.IsSave.Text = "true";
        }

        private bool SaveTask(string TaskPath)
        {

            Task.cTask t = new Task.cTask();

            //如果是编辑状态，则需要删除原有文件
            if (this.FormState == cGlobalParas.FormState.Edit)
            {
                t.TaskName = this.tTask.Text;

                if (this.comTaskClass.SelectedIndex == 0)
                {
                    try
                    {
                        //删除原有任务的主要目的是为了备份，但如果发生错误，则忽略
                        t.DeleTask("", this.tTask.Text);
                    }
                    catch (System.Exception)
                    {
                    }
                }
                else
                {
                    //获取此任务分类的路径
                    Task.cTaskClass tClass = new Task.cTaskClass();
                    string tPath = tClass.GetTaskClassPathByName(this.comTaskClass.SelectedItem.ToString());
                    try
                    {
                        //删除原有任务的主要目的是为了备份，但如果发生错误，则忽略
                        t.DeleTask(tPath, this.tTask.Text);
                    }
                    catch (System.Exception)
                    {

                    }
                }
            }

            int i = 0;
            int UrlCount = 0;



            //保存任务


            //新建一个任务
            t.New();

            t.TaskName = this.tTask.Text;
            t.TaskDemo = this.txtTaskDemo.Text;

            if (this.comTaskClass.SelectedIndex == 0)
            {
                t.TaskClass = "";
            }
            else
            {
                t.TaskClass = this.comTaskClass.SelectedItem.ToString();
            }

            t.TaskType = cGlobalParas.ConvertID(this.TaskType.SelectedItem.ToString()).ToString();
            t.RunType = cGlobalParas.ConvertID(this.comRunType.SelectedItem.ToString()).ToString();
            if (this.txtSavePath.Text.Trim().ToString() == "")
                t.SavePath = Program.getPrjPath() + "data";
            else
                t.SavePath = this.txtSavePath.Text;
            t.ThreadCount = int.Parse(this.udThread.Value.ToString());
            t.StartPos = this.txtStartPos.Text;
            t.EndPos = this.txtEndPos.Text;
            t.DemoUrl = this.txtWeblinkDemo.Text;
            t.Cookie = this.txtCookie.Text;
            t.WebCode = cGlobalParas.ConvertID(this.comWebCode.SelectedItem.ToString()).ToString();
            t.IsLogin = this.IsLogin.Checked;
            t.LoginUrl = this.txtLoginUrl.Text;
            t.IsUrlEncode = this.IsUrlEncode.Checked;
            if (this.IsUrlEncode.Checked == false)
            {
                t.UrlEncode = "";
            }
            else
            {
                t.UrlEncode = cGlobalParas.ConvertID(this.comUrlEncode.SelectedItem.ToString()).ToString();
            }

            //判断是否导出文件，存储导出的配置信息
            if (this.comRunType.SelectedIndex == 0)
            {

                if (this.raExportTxt.Checked == true)
                {
                    t.ExportType = ((int)cGlobalParas.PublishType.PublishTxt).ToString();
                }
                else if (this.raExportExcel.Checked == true)
                {
                    t.ExportType = ((int)cGlobalParas.PublishType.PublishExcel).ToString();
                }
                else if (this.raExportAccess.Checked == true)
                {
                    t.ExportType = ((int)cGlobalParas.PublishType.PublishAccess).ToString();
                }
                else if (this.raExportMSSQL.Checked == true)
                {
                    t.ExportType = ((int)cGlobalParas.PublishType.PublishMSSql).ToString();
                }
                else if (this.raExportMySql.Checked == true)
                {
                    t.ExportType = ((int)cGlobalParas.PublishType.PublishMySql).ToString();
                }
                else if (this.raExportWeb.Checked == true)
                {
                    t.ExportType = ((int)cGlobalParas.PublishType.PublishWeb).ToString();
                }

                t.ExportFile = this.txtFileName.Text.ToString();
                t.DataSource = this.txtDataSource.Text.ToString();

                t.DataTableName = this.comTableName.Text.ToString();
                t.InsertSql = this.txtInsertSql.Text;
                t.ExportUrl = this.txtExportUrl.Text;
                t.ExportUrlCode = cGlobalParas.ConvertID(this.comExportUrlCode.SelectedItem.ToString()).ToString();
                t.ExportCookie = this.txtExportCookie.Text;
            }
            else
            {

                t.ExportFile = "";
                t.DataSource = "";

                t.ExportType = ((int)cGlobalParas.PublishType.NoPublish).ToString();
                t.DataSource = "";
                t.DataTableName = "";
            }

            //开始存储高级配置信息
            t.GatherAgainNumber = int.Parse(this.udAgainNumber.Value.ToString());
            t.IsIgnore404 = this.IsIgnore404.Checked;
            t.IsExportHeader = this.IsIncludeHeader.Checked;
            t.IsDelRepRow = this.IsDelRepRow.Checked;
            t.IsErrorLog = this.IsSaveErrorLog.Checked;

            t.IsTrigger = this.IsStartTrigger.Checked;

            if (this.raGatheredRun.Checked == true)
                t.TriggerType = ((int)cGlobalParas.TriggerType.GatheredRun).ToString();
            else if (this.raPublishedRun.Checked == true)
                t.TriggerType = ((int)cGlobalParas.TriggerType.PublishedRun).ToString();

            cTriggerTask tt;

            //开始添加触发器执行的任务
            for (i = 0; i < this.listTask.Items.Count; i++)
            {
                tt = new cTriggerTask();
                tt.RunTaskType = cGlobalParas.ConvertID(this.listTask.Items[i].Text);

                if (cGlobalParas.ConvertID(this.listTask.Items[i].Text) == (int)cGlobalParas.RunTaskType.DataTask)
                    tt.RunTaskName = cGlobalParas.ConvertID(this.listTask.Items[i].SubItems[1].Text.ToString()).ToString();
                else
                    tt.RunTaskName = this.listTask.Items[i].SubItems[1].Text.ToString();

                tt.RunTaskPara = this.listTask.Items[i].SubItems[2].Text.ToString();

                t.TriggerTask.Add(tt);
            }


            for (i = 0; i < this.listWeblink.Items.Count; i++)
            {
                UrlCount += int.Parse(this.listWeblink.Items[i].SubItems[4].Text);
            }
            t.UrlCount = UrlCount;

            Task.cWebLink w;
            for (i = 0; i < this.listWeblink.Items.Count; i++)
            {
                w = new Task.cWebLink();
                w.id = i;
                w.Weblink = this.listWeblink.Items[i].Text;
                if (this.listWeblink.Items[i].SubItems[1].Text == "N")
                {
                    w.IsNavigation = false;
                }
                else
                {
                    w.IsNavigation = true;

                    //添加导航规则
                    for (int m = 0; m < m_listNaviRules.Count; m++)
                    {
                        if (m_listNaviRules[m].Url == this.listWeblink.Items[i].Text)
                        {
                            w.NavigRules = m_listNaviRules[m].NavigRule;
                            break;
                        }
                    }

                }

                if (this.listWeblink.Items[i].SubItems[3].Text == "" || this.listWeblink.Items[i].SubItems[3].Text == null)
                {
                    w.IsNextpage = false;
                }
                else
                {
                    w.IsNextpage = true;
                    w.NextPageRule = this.listWeblink.Items[i].SubItems[3].Text;
                }

                t.WebpageLink.Add(w);
                w = null;
            }

            Task.cWebpageCutFlag c;
            for (i = 0; i < this.listWebGetFlag.Items.Count; i++)
            {
                c = new Task.cWebpageCutFlag();
                c.id = i;
                c.Title = this.listWebGetFlag.Items[i].Text;
                c.DataType = cGlobalParas.ConvertID(this.listWebGetFlag.Items[i].SubItems[1].Text);
                c.StartPos = this.listWebGetFlag.Items[i].SubItems[2].Text;
                c.EndPos = this.listWebGetFlag.Items[i].SubItems[3].Text;
                c.LimitSign = cGlobalParas.ConvertID(this.listWebGetFlag.Items[i].SubItems[4].Text);

                try
                {
                    c.RegionExpression = this.listWebGetFlag.Items[i].SubItems[5].Text;
                    c.ExportLimit = cGlobalParas.ConvertID(this.listWebGetFlag.Items[i].SubItems[6].Text);
                    c.ExportExpression = this.listWebGetFlag.Items[i].SubItems[7].Text;
                }
                catch (System.Exception)
                {
                    //捕获错误不处理，兼容1.0版本
                }

                t.WebpageCutFlag.Add(c);
                c = null;

            }

            t.Save(TaskPath);
            t = null;

            return true;

        }

        private void CheckData(int CheckStep)
        {
            switch (CheckStep)
            {
                case 1:
                    if (this.tTask.Text == "" || this.tTask.Text == null)
                    {
                        this.tTask.Focus();
                        throw new cSoukeyException(rm.GetString ("Error22"));
                    }
                    break;
                case 2:
                    break;
                case 3:
                    if (this.listWeblink.Items.Count == 0)
                    {
                        this.cmdAddWeblink.Focus();
                        throw new cSoukeyException(rm.GetString ("Error24"));
                    }
                    break;
                case 4:
                    break;
                case 5:
                    if (this.listWebGetFlag.Items.Count == 0)
                    {
                        this.cmdAddCutFlag.Focus();
                        throw new cSoukeyException(rm.GetString ("Error25"));
                    }
                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    break;
                default :
                    break;
            }
        }

        private void IsLogin_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsLogin.Checked == true)
            {
                this.txtLoginUrl.Enabled = true;
            }
            else
            {
                this.txtLoginUrl.Enabled = false;
            }

            this.IsSave.Text = "true";
        }

        private void IsUrlEncode_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsUrlEncode.Checked == true)
            {
                this.comUrlEncode.Enabled = true;
                this.comUrlEncode.SelectedIndex = 0;
            }
            else
            {
                this.comUrlEncode.Enabled = false;
                this.comUrlEncode.SelectedIndex = -1;
            }

            this.IsSave.Text = "true";
        }

        private void raExportTxt_CheckedChanged(object sender, EventArgs e)
        {
            if (raExportTxt.Checked == true)
            {
                this.raExportExcel.Checked = false;
                this.raExportAccess.Checked = false;
                this.raExportMSSQL.Checked = false;
                this.raExportMySql.Checked = false;
                this.raExportWeb.Checked = false;

                SetExportFile();

                if (this.txtFileName.Text.Trim() != "")
                {
                    if (this.txtFileName.Text.EndsWith("xls"))
                        this.txtFileName.Text = this.txtFileName.Text.Substring(0, this.txtFileName.Text.Length - 3) + "txt";
                }

                this.IsSave.Text = "true";
            }
        }

        private void raExportExcel_CheckedChanged(object sender, EventArgs e)
        {
            if (raExportExcel.Checked == true)
            {
                this.raExportTxt.Checked = false;
                this.raExportAccess.Checked = false;
                this.raExportMSSQL.Checked = false;
                this.raExportMySql.Checked = false;
                this.raExportWeb.Checked = false;

                SetExportFile();

                if (this.txtFileName.Text.Trim() != "")
                {
                    if (this.txtFileName.Text.EndsWith("txt"))
                        this.txtFileName.Text = this.txtFileName.Text.Substring(0, this.txtFileName.Text.Length - 3) + "xls";
                }

                this.IsSave.Text = "true";
            }
        }

        private void raExportAccess_CheckedChanged(object sender, EventArgs e)
        {
            if (raExportAccess.Checked == true)
            {
                this.raExportTxt.Checked = false;
                this.raExportExcel.Checked = false;
                this.raExportMSSQL.Checked = false;
                this.raExportMySql.Checked = false;
                this.raExportWeb.Checked = false;

                SetExportDB();

                this.txtDataSource.Text = "";
                this.comTableName.Items.Clear();

                this.IsSave.Text = "true";
            }
        }

        private void raExportMSSQL_CheckedChanged(object sender, EventArgs e)
        {
            if (raExportMSSQL.Checked == true)
            {
                this.raExportTxt.Checked = false;
                this.raExportExcel.Checked = false;
                this.raExportAccess.Checked = false;
                this.raExportMySql.Checked = false;
                this.raExportWeb.Checked = false;

                SetExportDB();

                this.txtDataSource.Text = "";
                this.comTableName.Items.Clear();

                this.IsSave.Text = "true";
            }
        }

        private void raExportMySql_CheckedChanged(object sender, EventArgs e)
        {
            if (raExportMySql.Checked == true)
            {
                this.raExportTxt.Checked = false;
                this.raExportExcel.Checked = false;
                this.raExportAccess.Checked = false;
                this.raExportMSSQL.Checked = false;
                this.raExportWeb.Checked = false;

                SetExportDB();

                this.txtDataSource.Text = "";
                this.comTableName.Items.Clear();

                this.IsSave.Text = "true";
            }
        }

        private void raExportWeb_CheckedChanged(object sender, EventArgs e)
        {
            if (raExportWeb.Checked == true)
            {
                this.raExportTxt.Checked = false;
                this.raExportExcel.Checked = false;
                this.raExportAccess.Checked = false;
                this.raExportMSSQL.Checked = false;
                this.raExportMySql.Checked = false;

                SetExportWeb();

                this.IsSave.Text = "true";
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            GetDemoUrl();
        }

        private void cmdWebSource_Click(object sender, EventArgs e)
        {
            if (this.txtWeblinkDemo.Text.Trim().ToString() == "")
            {
                MessageBox.Show(rm.GetString ("Info6"), rm.GetString ("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Question);
                this.button7.Focus();
                return;
            }

            string tmpPath = Path.GetTempPath();

            try
            {
                //获取源码需要采用cGatherWeb中的方法进行，因为有可能Url中包含
                //POST参数
                Gather.cGatherWeb cg = new SoukeyNetget.Gather.cGatherWeb();

                bool IsAjax = false;

                if (cGlobalParas.ConvertID(this.TaskType.SelectedItem.ToString()) == (int)cGlobalParas.TaskType.AjaxHtmlByUrl)
                    IsAjax = true;

                string WebSource = cg.GetHtml(this.txtWeblinkDemo.Text, (cGlobalParas.WebCode)cGlobalParas.ConvertID(this.comWebCode.Text), this.txtCookie.Text, "", "", false, IsAjax);
                cg = null;

                //创建临时文件
                string m_FileName = "~" + DateTime.Now.ToFileTime().ToString() + ".txt";
                m_FileName = tmpPath + "\\" + m_FileName;
                FileStream myStream = File.Open(m_FileName, FileMode.Create, FileAccess.Write, FileShare.Write);
                StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gb2312"));
                sw.Write(WebSource);
                sw.Close();
                myStream.Close();

                System.Diagnostics.Process.Start(m_FileName);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString ("Error7") + ex.Message, rm.GetString ("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GetDemoUrl()
        {
            if (this.listWeblink.Items.Count >= 1)
            {
                bool IsNav;
                List<cNavigRule> cns = new List<cNavigRule>();
                string DemoUrl = "";

                if (this.listWeblink.Items[0].SubItems[1].Text == "N")
                {
                    IsNav = false;
                }
                else
                {
                    IsNav = true;

                    for (int i = 0; i < m_listNaviRules.Count; i++)
                    {
                        if (this.listWeblink.Items[0].Text == m_listNaviRules[i].Url)
                        {
                            cns = m_listNaviRules[i].NavigRule;
                            break;
                        }
                    }
                }

                DemoUrl = AddDemoUrl(this.listWeblink.Items[0].Text.ToString(), IsNav, cns);

                //根据判断当前是否需要Url进行编码
                if (this.IsUrlEncode.Checked == true)
                {
                    this.txtWeblinkDemo.Text = cTool.UrlEncode(DemoUrl, (cGlobalParas.WebCode)(cGlobalParas.ConvertID(this.comUrlEncode.SelectedItem.ToString())));
                }
                else
                {
                    this.txtWeblinkDemo.Text = DemoUrl;
                }
            }
        }

        private void SetExportFile()
        {
            this.txtFileName.Enabled = true;
            this.cmdBrowser.Enabled = true;
            this.IsIncludeHeader.Enabled = true;

            this.txtDataSource.Enabled = false;
            this.comTableName.Enabled = false;
            this.button12.Enabled = false;
            this.txtInsertSql.Enabled = false;

            this.txtExportUrl.Enabled = false;
            this.txtExportCookie.Enabled = false;
            this.button11.Enabled = false;
            this.button9.Enabled = false;
            this.comExportUrlCode.Enabled = false;
        }

        private void SetExportDB()
        {
            this.txtFileName.Enabled = false;
            this.cmdBrowser.Enabled = false;
            this.IsIncludeHeader.Enabled = false;

            this.txtDataSource.Enabled = true;
            this.comTableName.Enabled = true;
            this.button12.Enabled = true;
            this.txtInsertSql.Enabled = true;

            this.txtExportUrl.Enabled = false;
            this.txtExportCookie.Enabled = false;
            this.button11.Enabled = false;
            this.button9.Enabled = false;
            this.comExportUrlCode.Enabled = false;
        }

        private void SetExportWeb()
        {
            this.txtFileName.Enabled = false;
            this.cmdBrowser.Enabled = false;
            this.IsIncludeHeader.Enabled = false;

            this.txtDataSource.Enabled = false;
            this.comTableName.Enabled = false;
            this.button12.Enabled = false;
            this.txtInsertSql.Enabled = false;

            this.txtExportUrl.Enabled = true;
            this.txtExportCookie.Enabled = true;
            this.button11.Enabled = true;
            this.button9.Enabled = true;
            this.comExportUrlCode.Enabled = true;
        }

        private string AddDemoUrl(string SourceUrl, bool IsNavPage, List<cNavigRule> NavRule)
        {
            string Url;
            List<string> Urls;


            Urls = gUrl.SplitWebUrl(SourceUrl);

            if (IsNavPage == true)
            {

                Url = GetTestUrl(Urls[0].ToString(), NavRule);
            }
            else
            {
                Url = Urls[0].ToString();
            }

            Urls = null;
            return Url;

        }

        private string GetTestUrl(string webLink, List<cNavigRule> NavRule)
        {
            List<string> Urls;


            Urls = gUrl.ParseUrlRule(webLink, NavRule,(cGlobalParas.WebCode) cGlobalParas.ConvertID(this.comWebCode.SelectedItem.ToString()) ,this.txtCookie.Text);


            if (Urls == null || Urls.Count == 0)
                return "";

            string isReg = "[\"\\s]";
            string Url = "";

            for (int m = 0; m < Urls.Count; m++)
            {
                if (!Regex.IsMatch(Urls[m].ToString(), isReg))
                {
                    Url = Urls[m].ToString();
                    break;
                }
            }


            string PreUrl = "";

            //需要判断网页地址前后存在单引号或双引号
            if (Url.Substring(0, 1) == "'" || Url.Substring(0, 1) == "\"")
            {
                Url = Url.Substring(1, Url.Length - 1);
            }

            if (Url.Substring(Url.Length - 1, 1) == "'" || Url.Substring(Url.Length - 1, 1) == "\"")
            {
                Url = Url.Substring(0, Url.Length - 1);
            }

            //去除了相对网址表示，通过程序进行判断
            if (string.Compare(Url.Substring(0, 4), "http", true) != 0)
            {
                if (Url.Substring(0, 1) == "/")
                {
                    PreUrl = webLink.Substring(7, webLink.Length - 7);
                    PreUrl = PreUrl.Substring(0, PreUrl.IndexOf("/"));
                    PreUrl = "http://" + PreUrl;
                }
                else
                {
                    Match aa = Regex.Match(webLink, ".*/");
                    PreUrl = aa.Groups[0].Value.ToString();
                }
            }

            return PreUrl + Url;

        }

        public void NewTask(string TaskClassName)
        {
            if (TaskClassName != "")
            {
                for (int i = 0; i < this.comTaskClass.Items.Count; i++)
                {
                    if (TaskClassName == this.comTaskClass.Items[i].ToString())
                    {
                        this.comTaskClass.SelectedIndex = i;
                        return;
                    }
                }
            }

        }

        private void comRunType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.comRunType.SelectedIndex)
            {
                case 0:
                    this.groupBox4.Enabled = true;
                    this.groupBox9.Enabled = true;
                    this.groupBox10.Enabled = true;

                    this.raPublishedRun.Enabled = true;
                    this.raPublishedRun.Checked = true;

                    break;

                case 1:
                    this.groupBox4.Enabled = false;
                    this.groupBox9.Enabled = false;
                    this.groupBox10.Enabled = false;

                    this.raGatheredRun.Checked = true;
                    this.raPublishedRun.Enabled = false;

                    break;

                default:
                    break;
            }
        }

        private void cmdUp_Click(object sender, EventArgs e)
        {
            int i = this.listWebGetFlag.SelectedItems[0].Index;

            ListViewItem Litem = this.listWebGetFlag.SelectedItems[0];

            this.listWebGetFlag.Items.Remove(this.listWebGetFlag.SelectedItems[0]);
            this.listWebGetFlag.Items.Insert(i - 1, Litem);
            this.listWebGetFlag.SelectedItems[0].EnsureVisible();

            this.IsSave.Text = "true";
        }

        private void cmdDown_Click(object sender, EventArgs e)
        {
            int i = this.listWebGetFlag.SelectedItems[0].Index;

            ListViewItem Litem = this.listWebGetFlag.SelectedItems[0];

            this.listWebGetFlag.Items.Remove(this.listWebGetFlag.SelectedItems[0]);
            this.listWebGetFlag.Items.Insert(i + 1, Litem);
            this.listWebGetFlag.SelectedItems[0].EnsureVisible();

            this.IsSave.Text = "true";
        }

        private void listWebGetFlag_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (this.listWebGetFlag.SelectedItems.Count == 0 || this.listWebGetFlag.Items.Count == 1)
            {
                this.cmdUp.Enabled = false;
                this.cmdDown.Enabled = false;
            }
            else
            {
                if (this.listWebGetFlag.SelectedItems[0].Index == 0)
                {
                    this.cmdUp.Enabled = false;
                    this.cmdDown.Enabled = true;
                }
                else if (this.listWebGetFlag.SelectedItems[0].Index == this.listWebGetFlag.Items.Count - 1)
                {
                    this.cmdUp.Enabled = true;
                    this.cmdDown.Enabled = false;
                }
                else
                {
                    this.cmdUp.Enabled = true;
                    this.cmdDown.Enabled = true;
                }
            }
        }

        private void listWebGetFlag_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.listWebGetFlag.Items.Count == 0)
            {
                return;
            }

            if (e.KeyCode == Keys.Delete)
            {
                this.listWebGetFlag.Items.Remove(this.listWebGetFlag.SelectedItems[0]);

                this.IsSave.Text = "true";
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void frmAddTaskWizard_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }
    }
}