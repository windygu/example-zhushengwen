using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using MySql.Data.MySqlClient;
using System.Web;
using SoukeyNetget.Task ;
using System.Resources;
using System.Reflection;

///���ܣ��ɼ�������Ϣ����  
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶��������ʽ�汾�Ѿ��޸���1.2
namespace SoukeyNetget
{
    public partial class frmTask : Form
    {

        private Single m_SupportTaskVersion = Single.Parse("1.3");

        public delegate void RIsShowWizard(bool IsShowWizard);
        public RIsShowWizard RShowWizard;

        //����һ��������Դ�ļ��ı���
        private ResourceManager rm;

        //�����ɴ��������汾�ţ�ע���1.3��ʼ������������ǰ���ݣ��������󷽿ɲ���
        public Single SupportTaskVersion
        {
            get { return m_SupportTaskVersion; }
        }

        public delegate void ReturnTaskClass(string tClass);
        public ReturnTaskClass rTClass;

        //�Ƿ��ѱ���������������棬������ȡ����ʱ��
        //Ҳ��Ҫ����������������з��أ���Ҫ�����ڡ�Ӧ�á�
        //�͡�ȡ������ť���ж���
        private bool IsSaveTask = false;

        //����һ��ToolTip
        ToolTip HelpTip = new ToolTip();
        Task.cUrlAnalyze gUrl = new Task.cUrlAnalyze();

        //����һ�����������ڴ洢Url��ַ��Ӧ�ĵ�������,��Ϊ��ǰ�����޷���ʾ���е�
        //�����������ԣ���Ҫͨ��һ���������д洢
        private List<cNavigRules> m_listNaviRules = new List<cNavigRules>();

        public frmTask()
        {
            InitializeComponent();
            IniData();
        }

        #region ��������״̬
        private cGlobalParas.FormState m_FormState;
        public cGlobalParas.FormState FormState
        {
            get { return m_FormState;}
            set { m_FormState = value; }
        }
        #endregion

        //����ToolTip����Ϣ
        private void SetTooltip()
        {
            HelpTip.SetToolTip(this.tTask, @"�����������ƣ��������ƿ�����" + "\r\n" + @"���Ļ�Ӣ�ģ������������.*\/,��" + "\r\n" + "����Ϊ������");
            //HelpTip.SetToolTip(this.txtTaskDemo, "��������ע��Ϣ��");
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

        public void EditTask(string TClassPath, string TaskName)
        {
            LoadTask(TClassPath, TaskName);
        }

        public void Browser(string TClassPath, string TaskName)
        {
            LoadTask(TClassPath, TaskName);
        }

        private void LoadTask(string TClassPath, string TaskName)
        {
            //ÿ�μ�������ǰ�������뽫�������ÿ�
            int i = 0;
            m_listNaviRules = null;
            m_listNaviRules = new List<cNavigRules>();

            Task.cTask t = new Task.cTask();

            try
            {
                t.LoadTask(TClassPath + "\\" + TaskName);
            }
            catch (cSoukeyException ex)
            {
                throw ex; 
            }

            //��ʼ�ж�����汾�ţ��������汾�Ų�ƥ�䣬�򲻽���������Ϣ�ļ���
            if (this.SupportTaskVersion !=t.TaskVersion)
            {
                t = null;
                MessageBox.Show(rm.GetString("Info1"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            this.tTask.Text =t.TaskName ;
            this.txtTaskDemo.Text = t.TaskDemo;

            if (t.TaskClass == "")
            {
                this.comTaskClass.SelectedIndex = 0;
            }
            else
            {
                this.comTaskClass.SelectedItem=t.TaskClass ;
            }


            this.TaskType.SelectedItem = cGlobalParas.ConvertName(int.Parse(t.TaskType));
            this.comRunType.SelectedItem = cGlobalParas.ConvertName(int.Parse(t.RunType));

            if (this.comRunType.SelectedIndex == 0)
            {
                //���ڵ�������
                switch ((cGlobalParas.PublishType)int.Parse(t.ExportType))
                {
                    case cGlobalParas.PublishType.PublishExcel :
                        this.raExportExcel.Checked = true;
                        break;
                    case cGlobalParas.PublishType.PublishTxt :

                        this.raExportTxt.Checked = true;
                        break;
                    case cGlobalParas.PublishType.PublishAccess:
                        this.raExportAccess.Checked = true;
                        break;
                    case cGlobalParas.PublishType.PublishMSSql :
                        this.raExportMSSQL.Checked = true;
                        break;
                    case cGlobalParas.PublishType .PublishMySql :
                        this.raExportMySql.Checked = true;
                        break;
                    case cGlobalParas.PublishType .PublishWeb :
                        this.raExportWeb.Checked = true;
                        break;
                    default :
                        break;
                }
                this.txtFileName.Text = t.ExportFile;
                this.txtDataSource.Text = t.DataSource;
                this.comTableName.Text = t.DataTableName;
                this.txtInsertSql.Text  = t.InsertSql;
                this.txtExportUrl.Text  = t.ExportUrl;
                if (t.ExportUrlCode == null || t.ExportUrlCode =="")
                    this.comExportUrlCode.SelectedIndex = 0;
                else
                    this.comExportUrlCode.SelectedItem = cGlobalParas.ConvertName(int.Parse(t.ExportUrlCode));
                this.txtExportCookie.Text  = t.ExportCookie;
            }
            else
            {
                //���ɼ�����
                this.groupBox4.Enabled = false;
                this.txtFileName.Text = "";
                this.comTableName.Text = "";

            }

            this.txtSavePath.Text = t.SavePath;
            this.udThread.Value = t.ThreadCount;
            this.txtStartPos.Text = t.StartPos;
            this.txtEndPos.Text = t.EndPos;
            this.txtWeblinkDemo.Text = t.DemoUrl;
            this.txtCookie.Text = t.Cookie;
            this.comWebCode.SelectedItem = cGlobalParas.ConvertName(int.Parse (t.WebCode));
            this.IsLogin.Checked = t.IsLogin;
            this.txtLoginUrl.Text = t.LoginUrl;
            this.IsUrlEncode.Checked = t.IsUrlEncode;
            if (t.UrlEncode == "")
            {
                this.comUrlEncode.SelectedIndex = -1;
            }
            else
            {
                this.comUrlEncode.SelectedItem = cGlobalParas.ConvertName(int.Parse(t.UrlEncode));
            }

            //���ظ߼�����
            this.udAgainNumber.Value = t.GatherAgainNumber;
            this.IsIgnore404.Checked = t.IsIgnore404;
            this.IsSaveErrorLog.Checked = t.IsErrorLog;
            this.IsDelRepRow.Checked = t.IsDelRepRow;
            this.IsIncludeHeader.Checked = t.IsExportHeader;

            this.IsStartTrigger.Checked = t.IsTrigger;

            if (t.TriggerType == ((int)cGlobalParas.TriggerType.GatheredRun).ToString ())
                this.raGatheredRun.Checked = true;
            else if (t.TriggerType == ((int)cGlobalParas.TriggerType.PublishedRun).ToString ())
                this.raPublishedRun.Checked = true;

            //������������
            if (this.IsStartTrigger.Checked == true)
            {
                ListViewItem litem;

                for (i = 0; i <t.TriggerTask.Count ; i++)
                {
                    litem = new ListViewItem();
                    litem.Text = cGlobalParas.ConvertName(t.TriggerTask[i].RunTaskType);
                    if (t.TriggerTask[i].RunTaskType == (int)cGlobalParas.RunTaskType.DataTask)
                        litem.SubItems.Add(cGlobalParas.ConvertName(int.Parse(t.TriggerTask[i].RunTaskName)));
                    else
                        litem.SubItems.Add(t.TriggerTask[i].RunTaskName);
                    litem.SubItems.Add(t.TriggerTask[i].RunTaskPara);

                    this.listTask.Items.Add(litem);
                }
            }

            ListViewItem item;
           
            for (i = 0; i < t.WebpageLink.Count;  i++)
            {
                item=new ListViewItem ();

                if (t.WebpageLink[i].IsNavigation == true)
                {
                    cNavigRules cns = new cNavigRules();
                    cns.Url = t.WebpageLink[i].Weblink.ToString();
                    cns.NavigRule = t.WebpageLink[i].NavigRules;

                    //for (int m=0;m<t.WebpageLink [i].NavigRules.Count;m++)
                    //{
                    //    cn=new cNavigRule ();
                    //    cn.Url =t.WebpageLink[i].NavigRules[m].Url ;
                    //    cn.Level = t.WebpageLink[i].NavigRules[m].Level;
                    //    cn.IsOppUrl = t.WebpageLink[i].NavigRules[m].IsOppUrl;
                    //    cn.NavigRule = t.WebpageLink[i].NavigRules[m].NavigRule;

                    //    cns.NavigRule.Add(cn);
                    //}

                    m_listNaviRules.Add(cns);
                }

                item.Name =t.WebpageLink[i].id.ToString ();
                item.Text =t.WebpageLink[i].Weblink.ToString ();
                if (t.WebpageLink[i].IsNavigation == true)
                {
                    item.SubItems.Add("Y");
                    item.SubItems.Add(t.WebpageLink[i].NavigRules.Count.ToString ());
                }
                else
                {
                    item.SubItems.Add("N");
                    item.SubItems.Add("0");

                }
                item.SubItems.Add(t.WebpageLink[i].NextPageRule);
                item.SubItems.Add(gUrl.GetUrlCount(t.WebpageLink[i].Weblink.ToString ()).ToString ());

                

                this.listWeblink.Items.Add (item);
                item=null;
            }

            
            for (i = 0; i < t.WebpageCutFlag.Count ; i++)
            {
                item=new ListViewItem() ;
                item.Name =t.WebpageCutFlag[i].id.ToString ();
                item.Text =t.WebpageCutFlag[i].Title.ToString ();
                item.SubItems.Add(cGlobalParas.ConvertName (t.WebpageCutFlag[i].DataType) );
                item.SubItems.Add (t.WebpageCutFlag[i].StartPos.ToString ());
                item.SubItems.Add (t.WebpageCutFlag[i].EndPos .ToString ());
                item.SubItems.Add(cGlobalParas.ConvertName (t.WebpageCutFlag[i].LimitSign));

                item.SubItems.Add(t.WebpageCutFlag [i].RegionExpression );
                if ((int)t.WebpageCutFlag[i].ExportLimit == 0)
                    item.SubItems.Add("");
                else
                    item.SubItems.Add(cGlobalParas.ConvertName(t.WebpageCutFlag[i].ExportLimit));

                item.SubItems.Add(t.WebpageCutFlag [i].ExportExpression );
                
                this.listWebGetFlag.Items.Add(item);
                item=null;
            }

            t=null ;
            
        }

        #region ������ʼ������ ����������״̬���м��أ��½����޸ġ������
        
        private void IniData()
        {
            //��ʼ��ҳ���������

            //��ʼ����Դ�ļ��Ĳ���
            rm = new ResourceManager("SoukeyNetget.Resources.globalUI", Assembly.GetExecutingAssembly());

            //���ݵ�ǰ�����������ʾ��Ϣ�ļ���
            ResourceManager rmPara = new ResourceManager("SoukeyNetget.Resources.globalPara", Assembly.GetExecutingAssembly());

            this.TaskType.Items.Add(rmPara.GetString("TaskType1"));
            this.TaskType.Items.Add(rmPara.GetString("TaskType4"));
            this.TaskType.SelectedIndex = 0;

            this.comRunType.Items.Add(rmPara.GetString("TaskRunType2"));
            this.comRunType.Items.Add(rmPara.GetString("TaskRunType1"));
            this.comRunType.SelectedIndex = 1;

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

            this.comGetType.Items.Add(rmPara.GetString("GDataType4"));
            this.comGetType.Items.Add(rmPara.GetString("GDataType3"));
            this.comGetType.Items.Add(rmPara.GetString("GDataType2"));
            this.comGetType.Items.Add(rmPara.GetString("GDataType1"));
            this.comGetType.SelectedIndex = 0;

            rmPara = null;

            this.txtSavePath.Text = Program.getPrjPath() + "data";

            this.txtGetTitleName.Items.Add("���ӵ�ַ");
            this.txtGetTitleName.Items.Add("����");
            this.txtGetTitleName.Items.Add("����");
            this.txtGetTitleName.Items.Add("ͼƬ");

            this.labLogSavePath.Text = rm.GetString("Info2") + "��" + Program.getPrjPath() + "Log";

            //��ʼ��ҳ�����ʱ�����ؼ���״̬

            //��ʼ���������
            //��ʼ��ʼ�����νṹ,ȡxml�е�����,��ȡ�������
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

        #region ��ť��������Ʋ���

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            if (IsSaveTask == true)
            {
                if (this.comTaskClass.SelectedIndex == 0)
                {
                    rTClass("");
                }
                else
                {
                    rTClass(this.comTaskClass.SelectedItem.ToString());
                }
            }

            //if (this.FormState == cGlobalParas.FormState.New)
            //{
                RShowWizard(false);
            //}
            this.Close();
        }

        private void cmdAddWeblink_Click(object sender, EventArgs e)
        {
            this.errorProvider1.Clear();
            int UrlCount = 0;

            if (this.txtWebLink.Text.ToString() == null || this.txtWebLink.Text.Trim().ToString() == "" || this.txtWebLink.Text .Trim().ToString ()=="http://")
            {
                this.errorProvider1.SetError(this.txtWebLink, rm.GetString("Error1"));
                this.txtWebLink.Focus();
                return;
            }
            else
            {
                if (!Regex.IsMatch (this.txtWebLink.Text.Trim().ToString (),"http://",RegexOptions.IgnoreCase))
                {
                    this.errorProvider1.SetError(this.txtWebLink, rm.GetString("Error2"));
                    this.txtWebLink.Focus();
                    return;
                }
            }

            ListViewItem litem;
            litem = new ListViewItem();
            litem.Text = this.txtWebLink.Text.ToString();

            if (this.IsNavigPage.Checked == true)
            {
                litem.SubItems.Add("Y");

                cNavigRule cn;
                cNavigRules m_listNaviRule = new cNavigRules();

                for (int m = 0; m < this.dataNRule.Rows.Count; m++)
                {
                    cn = new cNavigRule();
                    cn.Url = this.txtWebLink.Text;
                    cn.Level = m+1;
                    cn.NavigRule = this.dataNRule.Rows[m].Cells[1].Value.ToString ();

                    m_listNaviRule.Url = this.txtWebLink.Text;
                    m_listNaviRule.NavigRule.Add(cn);
                }

                m_listNaviRules.Add(m_listNaviRule);

                litem.SubItems.Add(this.dataNRule.Rows.Count.ToString ());
               
            }
            else
            {
                litem.SubItems.Add("N");
                litem.SubItems.Add("0");
            }

           

            if (this.IsAutoNextPage.Checked == true)
            {
                litem.SubItems.Add(this.txtNextPage.Text.ToString());
            }
            else
            {
                litem.SubItems.Add("");
            }

            UrlCount = gUrl.GetUrlCount(this.txtWebLink.Text.ToString());

            litem.SubItems.Add(UrlCount.ToString());

            this.listWeblink.Items.Add(litem);
            litem = null;

            this.txtWebLink.Text = "http://";
            this.IsNavigPage.Checked = false;
            this.IsAutoNextPage.Checked = false;
            this.txtNag.Text = "";
            this.txtNextPage.Text = "";
            this.dataNRule.Rows.Clear();

            this.IsSave.Text = "true";
        }

        private string AddDemoUrl(string SourceUrl,bool IsNavPage,List<cNavigRule> NavRule)
        {
                string Url;
                List<string> Urls;

                
                Urls = gUrl.SplitWebUrl(SourceUrl);
               
                if (IsNavPage ==true )
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

        private void cmdGetCode_Click(object sender, EventArgs e)
        {

            //string Code = cTool.GetWebpageCode(this.txtWeblink.Text);
            //this.comCode.Text = Code;

            //wait.Dispose();

            //if (Code == "")
            //{
            //    MessageBox.Show("ϵͳ�޷��Զ���ȡ��ҳ���룬��ͨ���鿴ҳ�����ԣ�Firefox����鿴ҳ����루IE�����ж�ҳ������ʽ", "��Ϣ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            //}
        }

        private void cmdDelWeblink_Click(object sender, EventArgs e)
        {
            if (this.listWeblink.Items.Count == 0)
                return;

            //����е�����Ҫɾ�������Ĺ���
            //ɾ���洢�ĵ�������
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

        private void comRunType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.comRunType.SelectedIndex )
            {
                case 0:
                    this.groupBox4.Enabled = true;
                    this.groupBox9.Enabled =true ;
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

                default :
                    break;
            }
        }

        //���񱣴棬�����񱣴��ʱ�򣬲����������ģ����Ϣ�����������û��Զ���
        //�˹���ֻ���û��ڽ�������ʱ��һ�ֿ��ٲ���
        private bool SaveTask(string TaskPath)
        {

            Task.cTask t = new Task.cTask();

            //����Ǳ༭״̬������Ҫɾ��ԭ���ļ�
            if (this.FormState == cGlobalParas.FormState.Edit)
            {
                t.TaskName = this.tTask.Text;

                if (this.comTaskClass.SelectedIndex == 0)
                {
                    try
                    {
                        //ɾ��ԭ���������ҪĿ����Ϊ�˱��ݣ�������������������
                        t.DeleTask("", this.tTask.Text);
                    }
                    catch (System.Exception )
                    {
                    }
                }
                else
                {
                    //��ȡ����������·��
                    Task.cTaskClass tClass = new Task.cTaskClass();
                    string tPath=tClass.GetTaskClassPathByName(this.comTaskClass.SelectedItem.ToString());
                    try
                    {
                        //ɾ��ԭ���������ҪĿ����Ϊ�˱��ݣ�������������������
                        t.DeleTask(tPath, this.tTask.Text);
                    }
                    catch (System.Exception )
                    {

                    }
                }
            }

            int i = 0;
            int UrlCount = 0;

            

            //��������
            

            //�½�һ������
            t.New();

            t.TaskName = this.tTask.Text;
            t.TaskDemo = this.txtTaskDemo.Text;

            if (this.comTaskClass.SelectedIndex == 0)
            {
                t.TaskClass = "";
            }
            else
            {
                t.TaskClass = this.comTaskClass.SelectedItem.ToString ();
            }

            t.TaskType = cGlobalParas.ConvertID ( this.TaskType.SelectedItem.ToString ()).ToString ();
            t.RunType = cGlobalParas.ConvertID(this.comRunType.SelectedItem.ToString()).ToString();
            if (this.txtSavePath.Text.Trim().ToString() == "")
                t.SavePath = Program.getPrjPath() + "data";
            else
                t.SavePath = this.txtSavePath.Text;
            t.ThreadCount = int.Parse (this.udThread.Value.ToString ());
            t.StartPos = this.txtStartPos.Text;
            t.EndPos = this.txtEndPos.Text;
            t.DemoUrl = this.txtWeblinkDemo.Text;
            t.Cookie = this.txtCookie.Text;
            t.WebCode = cGlobalParas.ConvertID(this.comWebCode.SelectedItem.ToString()).ToString();
            t.IsLogin = this.IsLogin.Checked;
            t.LoginUrl = this.txtLoginUrl.Text;
            t.IsUrlEncode = this.IsUrlEncode.Checked;
            if (this.IsUrlEncode.Checked==false )
            {
                t.UrlEncode = "";
            }
            else
            {
                t.UrlEncode = cGlobalParas.ConvertID(this.comUrlEncode.SelectedItem.ToString()).ToString();
            }

            //�ж��Ƿ񵼳��ļ����洢������������Ϣ
            if (this.comRunType.SelectedIndex == 0)
            {
               
                if (this.raExportTxt.Checked ==true )
                {
                    t.ExportType = ((int)cGlobalParas.PublishType.PublishTxt).ToString ();
                }
                else if (this.raExportExcel.Checked == true)
                {
                    t.ExportType = ((int)cGlobalParas.PublishType.PublishExcel ).ToString();
                }
                else if (this.raExportAccess.Checked == true)
                {
                    t.ExportType =((int) cGlobalParas.PublishType.PublishAccess).ToString () ;
                }
                else if (this.raExportMSSQL.Checked == true)
                {
                    t.ExportType = ((int)cGlobalParas.PublishType.PublishMSSql).ToString();
                }
                else if (this.raExportMySql.Checked == true)
                {
                    t.ExportType = ((int)cGlobalParas.PublishType.PublishMySql ).ToString();
                }
                else if (this.raExportWeb.Checked == true)
                {
                    t.ExportType = ((int)cGlobalParas.PublishType.PublishWeb ).ToString();
                }

                t.ExportFile = this.txtFileName.Text.ToString();
                t.DataSource = this.txtDataSource.Text.ToString();

                t.DataTableName = this.comTableName.Text.ToString();
                t.InsertSql = this.txtInsertSql.Text;
                t.ExportUrl = this.txtExportUrl.Text;
                t.ExportUrlCode =cGlobalParas.ConvertID( this.comExportUrlCode.SelectedItem.ToString()).ToString ();
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

            //��ʼ�洢�߼�������Ϣ
            t.GatherAgainNumber = int.Parse (this.udAgainNumber.Value.ToString ());
            t.IsIgnore404 = this.IsIgnore404.Checked;
            t.IsExportHeader = this.IsIncludeHeader.Checked;
            t.IsDelRepRow = this.IsDelRepRow.Checked;
            t.IsErrorLog = this.IsSaveErrorLog.Checked;

            t.IsTrigger = this.IsStartTrigger.Checked;

            if (this.raGatheredRun.Checked == true)
                t.TriggerType = ((int)cGlobalParas.TriggerType.GatheredRun).ToString ();
            else if (this.raPublishedRun.Checked == true)
                t.TriggerType =((int) cGlobalParas.TriggerType.PublishedRun).ToString ();

            cTriggerTask tt;

            //��ʼ��Ӵ�����ִ�е�����
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
                UrlCount += int.Parse ( this.listWeblink.Items[i].SubItems[4].Text);
            }
            t.UrlCount = UrlCount;

            Task.cWebLink w;
            for (i = 0; i < this.listWeblink.Items.Count; i++)
            {
                w=new Task.cWebLink ();
                w.id = i;
                w.Weblink=this.listWeblink.Items[i].Text ;
                if (this.listWeblink.Items[i].SubItems[1].Text == "N")
                {
                    w.IsNavigation = false;
                }
                else
                {
                    w.IsNavigation = true;

                    //��ӵ�������
                    for (int m = 0; m < m_listNaviRules.Count; m++)
                    {
                        if (m_listNaviRules[m].Url == this.listWeblink.Items[i].Text)
                        {
                            w.NavigRules = m_listNaviRules[m].NavigRule;
                            break;
                        }
                    }
         
                }

                if (this.listWeblink.Items[i].SubItems[3].Text == "" || this.listWeblink.Items[i].SubItems[3].Text==null)
                {
                    w.IsNextpage = false;
                }
                else 
                {
                    w.IsNextpage = true;
                    w.NextPageRule = this.listWeblink.Items[i].SubItems[3].Text;
                }

                t.WebpageLink.Add (w);
                w=null;
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
                c.LimitSign =cGlobalParas.ConvertID (this.listWebGetFlag.Items[i].SubItems[4].Text);
                
                try
                {
                    c.RegionExpression = this.listWebGetFlag.Items[i].SubItems[5].Text;
                    c.ExportLimit = cGlobalParas.ConvertID(this.listWebGetFlag.Items[i].SubItems[6].Text);
                    c.ExportExpression = this.listWebGetFlag.Items[i].SubItems[7].Text;
                }
                catch (System.Exception)
                {
                    //������󲻴�������1.0�汾
                }
                
                t.WebpageCutFlag.Add(c);
                c = null;

            }

            t.Save(TaskPath);
            t=null;

            return true;

        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (!CheckInputvalidity())
            {
                return ;
            }

            if (this.listWeblink.Items.Count == 0 || this.listWebGetFlag.Items.Count == 0)
            {
                if (MessageBox.Show(rm.GetString("Quaere1"), rm.GetString ("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }

            try
            {
                if (this.IsSave.Text == "true")
                {
                    if (!SaveTask(""))
                    {
                        return;
                    }
                }

                if (this.comTaskClass.SelectedIndex == 0)
                {
                    rTClass("");
                }
                else
                {
                    rTClass(this.comTaskClass.SelectedItem.ToString());
                }

                this.IsSave.Text = "false";

                RShowWizard(false);
                this.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString ("Error3") + ex.Message, rm.GetString ("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        #endregion

        #region �������� ������
        //����û��������ݵ���Ч�ԣ�ֻҪ���������ƾͿ��Ա��棬����ʹ���Ѷ�
        //�����������һ������ִ�У���Ҫ��ִ��ǰ����һ���޸�
        private bool CheckInputvalidity()
        {
            this.errorProvider1.Clear();

            if (this.tTask.Text.ToString () == null || this.tTask.Text.Trim().ToString () == "")
            {
                this.errorProvider1.SetError(this.tTask, rm.GetString ("Error22"));
                return false ;
            }

            //if (this.TaskType.SelectedIndex ==1)
            //{
            //    //��ʾѡ���˸���������н���
            //    if (this.txtTaskTemp.Text.ToString() == null || this.txtTaskTemp.Text.ToString() == "")
            //    {
            //        this.errorProvider1.SetError(this.txtTaskTemp, "��ѡ����ͨ������ģ�彨���������뵼��������Ϣ");
            //        return;
            //    }
            //}

            //if (this.comRunType.SelectedIndex  == 0)
            //{
            //    //��ʾ�߲ɼ����ݱ�ֱ�ӵ�������
            //    if (this.txtFileName.Text.ToString() == null || this.txtFileName.Text.ToString() == "")
            //    {
            //        this.errorProvider1.SetError(this.txtFileName, "��ѡ������Ҫʵʱ�������ݣ����������뵼�����ݵ��ļ�����");
            //        return;
            //    }

            //}

            //if (this.listWeblink.Items.Count == 0)
            //{
            //    this.tabControl1.SelectedTab = this.tabControl1.TabPages[1];

            //    this.errorProvider1.SetError(this.listWeblink, "����Ҫ�ɼ�����ַ�����飡");
            //    return;
            //}

            //if (this.comWebCode.SelectedItem == null)
            //{
            //    this.tabControl1.SelectedTab = this.tabControl1.TabPages[1];

            //    this.errorProvider1.SetError(this.comWebCode, "��ҳ���벻��Ϊ�գ����Ϊ������ܵ����������");
            //    return;
            //}

            //if (this.listWebGetFlag.Items.Count == 0)
            //{
            //    this.tabControl1.SelectedTab = this.tabControl1.TabPages[2];
            //    this.errorProvider1.SetError(this.listWebGetFlag, "����Ҫ�ɼ������ݱ�ǩ������");
            //    return;
            //}
            return true;
        }

        #endregion

        #region ���ݸ�ί�еķ���
        //private void GetTaskID(int TaskID,string TaskName)
        //{
        //    this.txtTaskTemp.Tag  = TaskID.ToString();
        //    this.txtTaskTemp.Text = TaskName;

        //}

        private void GetDataSource(string strDataConn)
        {
            this.txtDataSource.Text = strDataConn;
        }

        private void GetUrl(string Url, int UrlCount)
        {
            this.txtWebLink.Text = Url;
            this.txtWebLink.Tag = UrlCount;
        }

        private void GetCookie(string strCookie)
        {
            this.txtCookie.Text = strCookie;
        }

        private void GetExportCookie(string strCookie)
        {
            this.txtExportCookie.Text = strCookie;
        }

        private void GetPData(string strCookie, string pData)
        {
            this.txtCookie.Text = strCookie;
            this.txtWebLink.Text += "<POST>" + pData + "</POST>";
        }

        private void GetExportpData(string strCookie, string pData)
        {
            this.txtExportUrl.Text += "<POST>" + pData + "</POST>";
        }

        #endregion

        private void listWeblink_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                this.listWeblink.Items.Remove(this.listWeblink.SelectedItems[0]);
                if (this.listWeblink.Items.Count == 0)
                {
                    this.txtWeblinkDemo.Text = "";
                }

                this.IsSave.Text = "true";
            }
        }

       
        private void listWebGetFlag_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.listWebGetFlag.Items.Count  == 0)
            {
                return;
            }

            if (e.KeyCode == Keys.Delete)
            {
                this.listWebGetFlag.Items.Remove(this.listWebGetFlag.SelectedItems[0]);

                this.IsSave.Text = "true";
            }
        }

        private void frmTask_Load(object sender, EventArgs e)
        {
            //��Tooltip���г�ʼ������
            HelpTip.ToolTipIcon = ToolTipIcon.None;
            HelpTip.ForeColor =Color.YellowGreen;
            HelpTip.BackColor = Color.LightGray;
            HelpTip.AutoPopDelay = 8000;
            HelpTip.ShowAlways = true;
            HelpTip.ToolTipTitle = "";

            SetTooltip();

            switch (this.FormState)
            {
                case cGlobalParas.FormState.New :
                    break;
                case cGlobalParas.FormState.Edit :
                    //�༭״̬���������޸ķ���
                    this.cmdWizard.Enabled = false;
                    //this.cmdAddByTask.Enabled = false;

                    this.tTask.ReadOnly = true;
                    this.comTaskClass.Enabled = false;

                    break;
                case cGlobalParas.FormState.Browser :
                    SetFormBrowser();
                    break;
                default :
                    break ;
            }

            //ResourceManager rm = new ResourceManager("SoukeyNetget.Resources.globalUI", Assembly.GetExecutingAssembly());
            ////��ʼ�����������datagrid�ı�ͷ
            //DataGridViewTextBoxColumn nRuleLevel = new DataGridViewTextBoxColumn();
            //nRuleLevel.HeaderText = rm.GetString("NaviLevel");
            //nRuleLevel.Width = 40;
            //this.dataNRule.Columns.Insert(0, nRuleLevel);

            //DataGridViewTextBoxColumn nRule = new DataGridViewTextBoxColumn();
            //nRule.HeaderText = rm.GetString("NaviRule");
            //nRule.Width = 240;
            //this.dataNRule.Columns.Insert(1, nRule);

            //rm = null;

            this.IsSave.Text = "false";
        }

        private void SetFormBrowser()
        {
            this.cmdWizard.Enabled = false;
            //this.cmdAddByTask.Enabled = false;

            this.cmdOpenFolder.Enabled = false;
            this.button10.Enabled = false;
            this.cmdBrowser.Enabled = false;
            this.button2.Enabled = false;
            this.button3.Enabled = false;
            this.button4.Enabled = false;
            this.button6.Enabled = false;
            this.cmdAddWeblink.Enabled = false;
            this.cmdEditWeblink.Enabled = false;
            this.cmdDelWeblink.Enabled = false;

            this.cmdAddCutFlag.Enabled = false;
            this.button8.Enabled = false;
            this.cmdDelCutFlag.Enabled = false;

            this.button7.Enabled = false;

            this.button1.Enabled = false;

            this.cmdCancel.Text = "�� ��";

            this.cmdOK.Enabled =false ;

            this.cmdApply.Enabled = false;
        }

        //����һ�����������޸����������
        private delegate DataTable delegateGData(string Url,List<cWebpageCutFlag> gCutFlags, cGlobalParas.WebCode webCode, string cookie, string startPos, string endPos, string sPath, bool IsAjax);
        private void GatherData()
        {

            if (this.listWeblink.Items.Count == 0)
            {
                MessageBox.Show(rm.GetString ("Info3"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.tabControl1.SelectedTab = this.tabControl1.TabPages[1];
                return ;
            }

            if (this.listWebGetFlag.Items.Count == 0)
            {
                MessageBox.Show(rm.GetString ("Info4"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.tabControl1.SelectedTab = this.tabControl1.TabPages[2];
                return ;
            }


            //���Բɼ��������û���������ݲ��Բɼ�
            //��֤�����Ƿ���ȷ
            //�Ȳ��������п�������һ���е���һ��ҳ��������

            //�ж��Ƿ��Ѿ���ȡ��ʾ����ַ�����û�У��������ȡ
            if (this.txtWeblinkDemo.Text.ToString() == null || this.txtWeblinkDemo.Text.ToString() == "")
            {
                GetDemoUrl();
            }

            this.tabControl1.SelectedTab = this.tabControl1.TabPages[4];

            Application.DoEvents();

            

            //���Ӳɼ��ı�־
            cWebpageCutFlag c;
            List<cWebpageCutFlag> gFlag = new List<cWebpageCutFlag>();

            for (int i = 0; i < this.listWebGetFlag.Items.Count; i++)
            {
                c = new Task.cWebpageCutFlag();
                c.id = i;
                c.Title = this.listWebGetFlag.Items[i].Text;
                c.DataType = cGlobalParas.ConvertID(this.listWebGetFlag.Items[i].SubItems[1].Text);
                c.StartPos = this.listWebGetFlag.Items[i].SubItems[2].Text;
                c.EndPos = this.listWebGetFlag.Items[i].SubItems[3].Text;
                c.LimitSign = cGlobalParas.ConvertID(this.listWebGetFlag.Items[i].SubItems[4].Text);
                c.RegionExpression = this.listWebGetFlag.Items[i].SubItems[5].Text;
                c.ExportLimit = cGlobalParas.ConvertID(this.listWebGetFlag.Items[i].SubItems[6].Text);
                c.ExportExpression = this.listWebGetFlag.Items[i].SubItems[7].Text;
                gFlag.Add(c);
                c = null;
            }

            string tmpSavePath = this.txtSavePath.Text.ToString() + "\\" + this.tTask.Text.ToString() + "_file";

            bool IsAjax = false;

            if (cGlobalParas.ConvertID(this.TaskType.SelectedItem.ToString()) == (int)cGlobalParas.TaskType.AjaxHtmlByUrl)
                IsAjax = true;

            //����һ���޸ķ������Ƶ�ί��
            delegateGData sd = new delegateGData(this.GatherTestData);

            //��ʼ���ú���,���Դ����� 
            IAsyncResult ir = sd.BeginInvoke(this.txtWeblinkDemo.Text.ToString(),gFlag, (cGlobalParas.WebCode)cGlobalParas.ConvertID(this.comWebCode.SelectedItem.ToString()), this.txtCookie.Text.ToString(), this.txtStartPos.Text.ToString(), this.txtEndPos.Text.ToString(), tmpSavePath, IsAjax,null, null);

            //��ʾ�ȴ��Ĵ��� 
            frmWaiting fWait = new frmWaiting(rm.GetString ("Info5"));
            fWait.Text = rm.GetString("Info5");
            fWait.Show(this);

            //ˢ������ȴ��Ĵ��� 
            Application.DoEvents();

            //ѭ������Ƿ�������첽�Ĳ��� 
            while (true)
            {
                if (ir.IsCompleted)
                {
                    //����˲�����رմ���
                    fWait.Close();
                    break;
                }

            }

            //ȡ�����ķ���ֵ 
            DataTable retValue = sd.EndInvoke(ir);

            //�󶨵���ʾ��DataGrid��
            this.dataTestGather.DataSource = retValue;
        }
      
        private DataTable GatherTestData(string Url,List<cWebpageCutFlag> gCutFlags, cGlobalParas.WebCode webCode, string cookie, string startPos, string endPos, string sPath, bool IsAjax)
        {
            Gather.cGatherWeb gData = new Gather.cGatherWeb();
            gData.CutFlag = gCutFlags;
            DataTable dGather = new DataTable();
            try
            {
                dGather = gData.GetGatherData(Url, webCode ,cookie ,startPos ,endPos , sPath, IsAjax);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString ("Error4") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return dGather;

        }

      
        private void button2_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.Show(this.button2,0,21);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            frmDict dfrm = new frmDict();
            dfrm.ShowDialog();
            dfrm.Dispose();

        }

        private void menuOpenDict_Click(object sender, EventArgs e)
        {
            frmDict d = new frmDict();
            d.ShowDialog();
            d.Dispose();
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "rmenuGetPostData")
            {
                frmBrowser wftm = new frmBrowser();
                wftm.getFlag = 1;
                wftm.rPData  = new frmBrowser.ReturnPOST (GetPData);
                wftm.ShowDialog();
                wftm.Dispose();

                return;
            }

            Match s;

            if (Regex.IsMatch(e.ClickedItem.ToString(), "[{].*[}]"))
            {
                s = Regex.Match(e.ClickedItem.ToString(), "[{].*[}]");
            }
            else
            {
                s = Regex.Match(e.ClickedItem.ToString(), "[<].*[>]");
            }

            int startPos = this.txtWebLink.SelectionStart;
            int l = this.txtWebLink.SelectionLength;

            this.txtWebLink.Text = this.txtWebLink.Text.Substring(0, startPos) + s.Groups[0].Value + this.txtWebLink.Text.Substring(startPos + l , this.txtWebLink.Text.Length - startPos - l);

            this.txtWebLink.SelectionStart = startPos + s.Groups[0].Value.Length;
            this.txtWebLink.ScrollToCaret();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            GatherData();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (this.dataNRule.Rows.Count ==0)
            {
                MessageBox.Show(rm.GetString ("Error5"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            List<cNavigRule> cns;
            cNavigRule cn;

            string nUrl = this.txtWebLink.Text;

            for (int m = 0; m < this.dataNRule.Rows.Count; m++)
            {
                cns= new List<cNavigRule>();

                cn = new cNavigRule();
                cn.Url = nUrl;
                cn.Level = 1;
                cn.NavigRule = this.dataNRule.Rows[m].Cells[1].Value.ToString();

                cns.Add(cn);

                nUrl = GetTestUrl(nUrl, cns);
            }

            string Url = nUrl;

            if (!Regex.IsMatch(Url, @"(http|https|ftp)+://[^\s]*"))
            {
                MessageBox.Show(rm.GetString ("Error6"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ;
            }

            System.Diagnostics.Process.Start( Url);

        }

        private delegate string delegateGNavUrl(string webLink, List<cNavigRule> NavRule, cGlobalParas.WebCode webCode, string cookie);
        private string GetTestUrl(string webLink, List<cNavigRule> NavRule)
        {
            //����һ����ȡ������ַ��ί��
            delegateGNavUrl sd = new delegateGNavUrl(this.GetNavUrl);

            //��ʼ���ú���,���Դ����� 
            IAsyncResult ir = sd.BeginInvoke(webLink, NavRule,(cGlobalParas.WebCode) cGlobalParas.ConvertID(this.comWebCode.SelectedItem.ToString()) ,this.txtCookie.Text, null, null);

            //��ʾ�ȴ��Ĵ��� 
            frmWaiting fWait = new frmWaiting(rm.GetString("Info117"));
            fWait.Text = rm.GetString("Info117");
            fWait.Show(this);

            //ˢ������ȴ��Ĵ��� 
            Application.DoEvents();

            //ѭ������Ƿ�������첽�Ĳ��� 
            while (true)
            {
                if (ir.IsCompleted)
                {
                    //����˲�����رմ���
                    fWait.Close();
                    break;
                }

            }

            //ȡ�����ķ���ֵ 
            string rUrl = sd.EndInvoke(ir);

            return rUrl;
        }

        private string GetNavUrl(string webLink, List<cNavigRule> NavRule, cGlobalParas.WebCode webCode, string cookie)
        {
            List<string> Urls;


            Urls = gUrl.ParseUrlRule(webLink, NavRule,webCode ,cookie );
           

            if (Urls == null || Urls.Count ==0)
                return "";

            string isReg="[\"\\s]";
            string Url="";

            for (int m=0 ;m<Urls.Count ;m++)
            {
                if (!Regex.IsMatch (Urls[m].ToString (),isReg ))
                {
                    Url = Urls[m].ToString();
                    break ;
                }
            }
             

            string PreUrl = "";

            //��Ҫ�ж���ҳ��ַǰ����ڵ����Ż�˫����
            if (Url.Substring(0, 1) == "'" || Url.Substring(0, 1) == "\"")
            {
                Url = Url.Substring(1, Url.Length - 1);
            }

            if (Url.Substring(Url.Length - 1, 1) == "'" || Url.Substring(Url.Length - 1, 1) == "\"")
            {
                Url = Url.Substring(0, Url.Length - 1);
            }

            //ȥ���������ַ��ʾ��ͨ����������ж�
            if (string.Compare (Url.Substring (0,4),"http",true)!=0)
            {
                if (Url.Substring(0, 1) == "/")
                {
                    PreUrl = webLink.Substring(7, webLink.Length  - 7);
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

        private void cmdWebSource_Click(object sender, EventArgs e)
        {
            if (this.txtWeblinkDemo.Text.Trim().ToString() == "")
            {
                MessageBox.Show(rm.GetString ("Info6"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Question);
                this.errorProvider1.Clear();
                this.errorProvider1.SetError(this.txtWeblinkDemo, "��������ַ��Ϣ");

                return;
            }

            string tmpPath = Path.GetTempPath();

            try
            {
                //��ȡԴ����Ҫ����cGatherWeb�еķ������У���Ϊ�п���Url�а���
                //POST����
                Gather.cGatherWeb cg = new SoukeyNetget.Gather.cGatherWeb();

                bool IsAjax = false;

                if (cGlobalParas.ConvertID (this.TaskType.SelectedItem.ToString ())==(int)cGlobalParas.TaskType.AjaxHtmlByUrl )
                    IsAjax =true ;

                string WebSource = cg.GetHtml(this.txtWeblinkDemo.Text, (cGlobalParas.WebCode)cGlobalParas.ConvertID(this.comWebCode.Text), this.txtCookie.Text, "", "", false,IsAjax);
                cg = null;

                //������ʱ�ļ�
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
                MessageBox.Show(rm.GetString ("Error7") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmdOKRun_Click(object sender, EventArgs e)
        {

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

                    DemoUrl = AddDemoUrl(this.listWeblink.Items[0].Text.ToString(), IsNav, cns);
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

                    string nUrl = this.listWeblink.Items[0].Text.ToString();
                    List<cNavigRule> tempcns;
                    cNavigRule tempcn;

                    for (int j = 0; j < cns.Count; j++)
                    {
                        tempcns = new List<cNavigRule>();
                        tempcn =new cNavigRule ();
                        tempcn.Url = nUrl;
                        tempcn.Level = 1;
                        tempcn.NavigRule = cns[j].NavigRule;
                        tempcns.Add(tempcn);
                        nUrl = AddDemoUrl(nUrl, IsNav, tempcns);
                    }
                    DemoUrl=nUrl;
                }


                //�����жϵ�ǰ�Ƿ���ҪUrl���б���
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

        private void ConnectAccess()
        {
            string connectionstring = "provider=microsoft.jet.oledb.4.0;data source=";
            connectionstring += this.txtFileName.Text;
            //if (this.txtDataUser.Text.Trim() != "")
            //{
            //    connectionstring += "UID=" + this.txtDataUser.Text;

            //}
            //OleDbConnection con = new OleDbConnection(connectionstring);
            //con.Open();
            //con.Close ();
            //MessageBox.Show("���ݿ����ӳɹ���", "soukey��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ConnectSqlServer()
        {
            //string strDataBase = "Server=.;DataBase=Library;Uid=" + this.txtDataUser.Text.Trim() + ";pwd=" + this.txtDataPwd.Text + ";";
            //SqlConnection conn = new SqlConnection(strDataBase);
            //conn.Open();
            //conn.Close();
            //MessageBox.Show("���ݿ����ӳɹ���", "soukey��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            frmBrowser wftm = new frmBrowser();
            wftm.getFlag = 0;
            wftm.rCookie = new frmBrowser.ReturnCookie(GetCookie);
            wftm.ShowDialog();
            wftm.Dispose();

        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            GatherData();
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

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            this.contextMenuStrip1.Items.Clear();

            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString ("rmenu1") + "{Num:1,100,1}", null, null, "rmenuAddNum"));
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString ("rmenu2") + "{Num:100,1,-1}",null,null,"rmenuDegreNum"));
            this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString ("rmenu3") + "{Letter:a,z}",null,null ,"rmenuAddLetter"));
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString ("rmenu4") + "{Letter:z,a}",null ,null ,"rmenuDegreLetter"));
            this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString ("rmenu5") + "<POST>",null,null,"rmenuPostPrefix"));
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString ("rmenu6") + "</POST>", null, null, "rmenuPostSuffix"));
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString ("rmenu7"), null, null, "rmenuGetPostData"));
            this.contextMenuStrip1.Items.Add(new ToolStripSeparator());

            //��ʼ���ֵ�˵�����Ŀ
            cDict d = new cDict();
            int count = d.GetDictClassCount();
            

            for (int i = 0; i < count; i++)
            {
                this.contextMenuStrip1.Items.Add(rm.GetString ("rmenu8") + ":{Dict:" + d.GetDictClassName(i).ToString() + "}");
            }

        }

        private void IsUrlEncode_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsUrlEncode.Checked == true)
            {
                this.comUrlEncode.Enabled = true;
                this.comUrlEncode.SelectedIndex = 0;
                this.cmdUrlEncoding.Enabled = true;
            }
            else
            {
                this.comUrlEncode.Enabled = false;
                this.comUrlEncode.SelectedIndex= - 1;
                this.cmdUrlEncoding.Enabled = false;
            }

            this.IsSave.Text = "true";
        }

        private void listWebGetFlag_Click(object sender, EventArgs e)
        {
            if (this.listWebGetFlag.SelectedItems.Count != 0)
            {
                this.txtGetTitleName.Text = this.listWebGetFlag.SelectedItems[0].Text;
                this.comGetType.Text = this.listWebGetFlag.SelectedItems[0].SubItems[1].Text;
                this.txtGetStart.Text = this.listWebGetFlag.SelectedItems[0].SubItems[2].Text;
                this.txtGetEnd.Text = this.listWebGetFlag.SelectedItems[0].SubItems[3].Text;
                this.comLimit.SelectedItem = this.listWebGetFlag.SelectedItems[0].SubItems[4].Text;
                try
                {
                    this.txtRegion.Text = this.listWebGetFlag.SelectedItems[0].SubItems[5].Text;
                    this.comExportLimit.SelectedItem = this.listWebGetFlag.SelectedItems[0].SubItems[6].Text;
                    this.txtExpression.Text = this.listWebGetFlag.SelectedItems[0].SubItems[7].Text;
                }
                catch (System.Exception)
                {
                    //�������������Ϊ���Ǽ���1.0�汾��������Ϣ
                }

                //this.IsSave.Text = "false";
            }
        }

        private void listWeblink_Click(object sender, EventArgs e)
        {

            this.txtWebLink.Text = this.listWeblink.SelectedItems[0].Text;

            if (this.listWeblink.SelectedItems[0].SubItems[1].Text == "N" )
            {
                this.IsNavigPage.Checked = false;
                this.dataNRule.Rows.Clear();
            }
            else
            {
                this.IsNavigPage.Checked = true;

                this.txtNag.Text = "";

                //��ӵ�������

                this.dataNRule.Rows.Clear();
                
                for (int i = 0; i < m_listNaviRules.Count; i++)
                {
                    if (this.listWeblink.SelectedItems[0].Text == m_listNaviRules[i].Url)
                    {
                        for (int j = 0; j < m_listNaviRules[i].NavigRule.Count; j++)
                        {
                            this.dataNRule.Rows.Add (m_listNaviRules[i].NavigRule[j].Level ,m_listNaviRules[i].NavigRule[j].NavigRule);
                        }
                    }
                }
               
            }

            //��һҳ�����
            if (this.listWeblink.SelectedItems[0].SubItems[3].Text == "")
            {
                this.IsAutoNextPage.Checked = false;
                this.txtNextPage.Text = "";
            }
            else
            {
                this.IsAutoNextPage.Checked = true;
                this.txtNextPage.Text = this.listWeblink.SelectedItems[0].SubItems[3].Text;
            }

            //this.IsSave.Text = "false";

        }

        private void cmdEditWeblink_Click(object sender, EventArgs e)
        {
            if (this.listWeblink.SelectedItems.Count == 0)
            {
                return;
            }

            this.errorProvider1.Clear();
            int UrlCount = 0;

            if (this.txtWebLink.Text.ToString() == null || this.txtWebLink.Text.Trim().ToString() == "")
            {
                this.errorProvider1.SetError(this.txtWebLink, rm.GetString("Error1"));
                return;
            }
            else
            {
                if (!Regex.IsMatch(this.txtWebLink.Text.Trim().ToString(), "http://", RegexOptions.IgnoreCase))
                {
                    this.errorProvider1.SetError(this.txtWebLink, rm.GetString("Error2"));
                    return;
                }
            }

            this.listWeblink.SelectedItems[0].Text = this.txtWebLink.Text.ToString();
            if (this.IsNavigPage.Checked == true)
            {
                this.listWeblink.SelectedItems[0].SubItems [1].Text ="Y";

                //ɾ���洢�ĵ�������
                for (int i = 0; i < m_listNaviRules.Count; i++)
                {
                    if (this.listWeblink.SelectedItems[0].Text == m_listNaviRules[i].Url)
                        m_listNaviRules.Remove(m_listNaviRules[i]);
                }

                cNavigRule cn;
                cNavigRules m_listNaviRule = new cNavigRules();

                for (int m = 0; m < this.dataNRule.Rows.Count; m++)
                {
                    cn = new cNavigRule();
                    cn.Url = this.txtWebLink.Text;
                    cn.Level = m+1;
                    cn.NavigRule = this.dataNRule.Rows[m].Cells[1].Value.ToString();

                    m_listNaviRule.Url = this.txtWebLink.Text;
                    m_listNaviRule.NavigRule.Add(cn);
                }

                m_listNaviRules.Add(m_listNaviRule);

                this.listWeblink.SelectedItems[0].SubItems[2].Text = this.dataNRule.Rows.Count.ToString();
                
            }
            else
            {
                this.listWeblink.SelectedItems[0].SubItems[1].Text = "N";
                this.listWeblink.SelectedItems[0].SubItems[2].Text = "0";
            }



            if (this.IsAutoNextPage.Checked == true)
            {
                this.listWeblink.SelectedItems[0].SubItems[3].Text=this.txtNextPage.Text.ToString();
            }
            else
            {
                this.listWeblink.SelectedItems[0].SubItems[3].Text="";
            }

            UrlCount = gUrl.GetUrlCount(this.txtWebLink.Text.ToString());
            this.listWeblink.SelectedItems[0].SubItems[4].Text = UrlCount.ToString ();

            this.txtWebLink.Text = "http://";
            this.IsNavigPage.Checked = false;
            this.IsAutoNextPage.Checked = false;
            this.txtNag.Text = "";
            this.txtNextPage.Text = "";
            this.dataNRule.Rows.Clear();

            this.IsSave.Text = "true";
        }

        //�Զ�ʶ����һҳ�ĵ������򣬵�ǰ��ʱδ�ã����Ҵ���ٳ�������ȫ��ע�ͣ�
        private void button4_Click(object sender, EventArgs e)
        {
            //if (!Regex.IsMatch(this.txtWebLink.Text, @"(http|https|ftp)+://[^\s]*"))
            //{
            //    MessageBox.Show("��ַ�޷��򿪣����ܳ���������ַ����������", "soukey��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            
            //string Url=AddDemoUrl (this.txtWebLink.Text,false,false,"" );
            //GetNextPageFlag(Url);

        }
        
        //�Զ���ȡ��һҳ�ı�ʶ
        private string GetNextPageFlag(string Url)
        {
            string webCode = cTool.GetHtmlSource(Url, true);
            return "";
        }

        private void comGetType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comGetType.SelectedIndex == 0)
            {
                this.comLimit.Enabled = true;
                this.comLimit.SelectedIndex = 0;

                this.comExportLimit.Enabled = true;
                this.comExportLimit.SelectedIndex = 0;
            }
            else
            {
                this.comLimit.SelectedIndex = -1;
                this.comLimit.Enabled = false;

                this.comExportLimit.SelectedIndex = -1;
                this.comExportLimit.Enabled = false;

                this.txtExpression.Text = "";
                this.txtExpression.Enabled = false;
            }
        }

        private void cmdAddCutFlag_Click(object sender, EventArgs e)
        {
            this.errorProvider1.Clear();

            if (this.txtGetTitleName.Text.Trim().ToString() == "")
            {
                this.errorProvider1.SetError(this.txtGetTitleName, rm.GetString("Error8"));
                return;
            }

            if (this.txtGetStart.Text.Trim().ToString() == "")
            {
                this.errorProvider1.SetError(this.txtGetStart, rm.GetString ("Error9"));
                return;

            }

            if (this.txtGetEnd.Text.Trim().ToString() == "")
            {
                this.errorProvider1.SetError(this.txtGetEnd, rm.GetString("Error10"));
                return;
            }

            if (this.comLimit.SelectedIndex == -1)
                this.comLimit.SelectedIndex = 0;

            if (this.comExportLimit.SelectedIndex == -1)
                this.comExportLimit.SelectedIndex = 0;

            //�ж������Ƿ��Ѿ��ظ�
            for (int i = 0; i < this.listWebGetFlag.Items.Count; i++)
            {
                if (this.listWebGetFlag.Items[i].Text == this.txtGetTitleName.Text)
                {
                    this.errorProvider1.Clear();
                    this.errorProvider1.SetError(this.txtGetEnd, rm.GetString("Error11"));
                    return;
                }
            }

            ListViewItem item = new ListViewItem();
            item.Text = this.txtGetTitleName.Text.ToString();
            item.SubItems.Add(this.comGetType.SelectedItem.ToString());
            item.SubItems.Add(cTool.ClearFlag(this.txtGetStart.Text.ToString()));
            item.SubItems.Add(cTool.ClearFlag(this.txtGetEnd.Text.ToString()));
            item.SubItems.Add(this.comLimit.SelectedItem.ToString());
            item.SubItems.Add(this.txtRegion.Text.ToString());
            item.SubItems.Add(this.comExportLimit.SelectedItem.ToString () );
            item.SubItems.Add(this.txtExpression.Text .ToString ());
            this.listWebGetFlag.Items.Add(item);
            item = null;

            this.txtGetTitleName.Text = "";
            this.txtGetStart.Text = "";
            this.txtGetEnd.Text = "";
            this.comLimit.SelectedIndex =0;
            this.txtRegion.Text = "";
            this.comExportLimit.SelectedIndex =0;
            this.txtExpression.Text = "";

            this.IsSave.Text = "true";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (this.listWebGetFlag.SelectedItems.Count == 0)
            {
                return;
            }

            this.errorProvider1.Clear();

            if (this.txtGetTitleName.Text.Trim().ToString() == "")
            {
                this.errorProvider1.SetError(this.txtGetTitleName, rm.GetString("Error8"));
                return;
            }

            if (this.txtGetStart.Text.Trim().ToString() == "")
            {
                this.errorProvider1.SetError(this.txtGetStart, rm.GetString("Error9"));
                return;

            }

            if (this.txtGetEnd.Text.Trim().ToString() == "")
            {
                this.errorProvider1.SetError(this.txtGetEnd, rm.GetString("Error10"));
                return;
            }

            this.listWebGetFlag.SelectedItems[0].Text = this.txtGetTitleName.Text.ToString();
            this.listWebGetFlag.SelectedItems[0].SubItems[1].Text = this.comGetType.SelectedItem.ToString();
            this.listWebGetFlag.SelectedItems[0].SubItems[2].Text = cTool.ClearFlag(this.txtGetStart.Text.ToString());
            this.listWebGetFlag.SelectedItems[0].SubItems[3].Text = cTool.ClearFlag(this.txtGetEnd.Text.ToString());
            if (this.comLimit.SelectedIndex == -1)
            {
                this.listWebGetFlag.SelectedItems[0].SubItems[4].Text = this.comLimit.Items[0].ToString();
            }
            else
            {
                this.listWebGetFlag.SelectedItems[0].SubItems[4].Text = this.comLimit.SelectedItem.ToString();
            }

            this.listWebGetFlag.SelectedItems[0].SubItems[5].Text=this.txtRegion.Text.ToString();
            this.listWebGetFlag.SelectedItems[0].SubItems[6].Text = this.comExportLimit.SelectedItem.ToString();
            this.listWebGetFlag.SelectedItems[0].SubItems[7].Text = this.txtExpression.Text.ToString();


            this.txtGetTitleName.Text = "";
            this.txtGetStart.Text = "";
            this.txtGetEnd.Text = "";
            this.comLimit.SelectedIndex =0;
            this.txtRegion.Text = "";
            this.comExportLimit.SelectedIndex = 0;
            this.txtExpression.Text = "";

            this.IsSave.Text = "true";
        }

        private void cmdDelCutFlag_Click(object sender, EventArgs e)
        {
            if (this.listWebGetFlag.SelectedItems.Count != 0)
            {
                this.listWebGetFlag.Items.Remove(this.listWebGetFlag.SelectedItems[0]);

                this.IsSave.Text = "true";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GatherData();
        }

        private void cmdOpenFolder_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.Description = rm.GetString ("Info7");
            this.folderBrowserDialog1.SelectedPath = Program.getPrjPath();
            if (this.folderBrowserDialog1.ShowDialog()==DialogResult.OK)
            {
                this.txtSavePath.Text = this.folderBrowserDialog1.SelectedPath;
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            GetDemoUrl();
        }

        private void listWebGetFlag_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listWebGetFlag.SelectedItems.Count != 0)
            {
                this.txtGetTitleName.Text = this.listWebGetFlag.SelectedItems[0].Text;
                this.comGetType.Text = this.listWebGetFlag.SelectedItems[0].SubItems[1].Text;
                this.txtGetStart.Text = this.listWebGetFlag.SelectedItems[0].SubItems[2].Text;
                this.txtGetEnd.Text = this.listWebGetFlag.SelectedItems[0].SubItems[3].Text;
                this.comLimit.SelectedItem = this.listWebGetFlag.SelectedItems[0].SubItems[4].Text;
                try
                {
                    this.txtRegion.Text = this.listWebGetFlag.SelectedItems[0].SubItems[5].Text;
                    this.comExportLimit.SelectedItem = this.listWebGetFlag.SelectedItems[0].SubItems[6].Text;
                    this.txtExpression.Text = this.listWebGetFlag.SelectedItems[0].SubItems[7].Text;
                }
                catch (System.Exception)
                {
                    //�������������Ϊ���Ǽ���1.0�汾��������Ϣ
                }

                //this.IsSave.Text = "true";
            }
        }

        private void frmTask_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_FormState == cGlobalParas.FormState.Browser)
                return;

            if (this.IsSave.Text == "true")
            {
                if (MessageBox.Show(rm.GetString ("Quaere2"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    e.Cancel = true;
                    return;
            }
            
        }

        #region �����޸ı�����
        private void tTask_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtTaskDemo_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void comTaskClass_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void TaskType_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void comRunType_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void udThread_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtSavePath_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void comWebCode_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtCookie_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtLoginUrl_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void DataSource_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtDataPwd_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtTableName_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void comUrlEncode_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtStartPos_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtEndPos_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        #endregion

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
                    this.label37.Text = rm.GetString ("Label1");
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
                default :
                    this.txtExpression.Enabled = false;
                    break;
            }
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
            this.comExportUrlCode.Enabled = true ;
        }

        private void cmdBrowser_Click(object sender, EventArgs e)
        {
            if (this.raExportTxt.Checked == true)
            {
                this.saveFileDialog1.Title = rm.GetString ("Info12");

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

        private void button12_Click(object sender, EventArgs e)
        {
            frmSetData fSD = new frmSetData();

            if (this.raExportAccess.Checked == true)
                fSD.FormState = 0;
            else if (this.raExportMSSQL .Checked ==true )
                fSD.FormState=1;
            else if (this.raExportMySql.Checked ==true )
                fSD.FormState =2;

            fSD.rDataSource = new frmSetData.ReturnDataSource(GetDataSource);
            fSD.ShowDialog();
            fSD.Dispose();
           
        }

        private void button11_Click(object sender, EventArgs e)
        {
            frmBrowser wftm = new frmBrowser();
            wftm.getFlag = 2;
            wftm.rExportCookie = new frmBrowser.ReturnExportCookie(GetExportCookie);
            wftm.ShowDialog();
            wftm.Dispose();

        }

        private void comTableName_DropDown(object sender, EventArgs e)
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
                MessageBox.Show(rm.GetString ("Error12") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void comTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillInsertSql(this.comTableName.SelectedItem.ToString ());

            this.IsSave.Text = "true";
        }

        private void txtDataSource_TextChanged(object sender, EventArgs e)
        {
            if (this.comTableName.Items.Count != 0)
                this.comTableName.Items.Clear();

            this.IsSave.Text = "true";
        }

        private DataTable GetTableColumns(string tName)
        {
            DataTable tc=new DataTable ();

            try
            {

                if (this.raExportAccess.Checked == true)
                {
                    OleDbConnection conn = new OleDbConnection();
                    conn.ConnectionString = this.txtDataSource.Text;

                    conn.Open();

                    string[] Restrictions = new string[4];
                    Restrictions[2] = tName;

                    tc = conn.GetSchema("Columns",Restrictions);

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
            catch (System.Exception )
            {
                return null;
            }


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
                if (this.listWebGetFlag.Items[j].SubItems[1].Text == "�ı�" || this.listWebGetFlag.Items[j].SubItems[1].Text == "Text")
                    strColumnsValue += "\"{" + this.listWebGetFlag.Items[j].Text + "}\",";

            }

            if (strColumnsValue!="")
                strColumnsValue = strColumnsValue.Substring(0, strColumnsValue.Length - 1);

            iSql = iSql + strColumnsValue + ")";

            this.txtInsertSql .Text = iSql;

        }

        private void comTableName_TextChanged(object sender, EventArgs e)
        {
            string iSql = "insert into " + this.comTableName.Text + "(";
            string strColumns = "";
            string strColumnsValue = "";

            for (int j = 0; j < this.listWebGetFlag.Items.Count; j++)
            {
                if (this.listWebGetFlag.Items[j].SubItems[1].Text == "�ı�" || this.listWebGetFlag.Items[j].SubItems[1].Text == "Text")
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

        private void rmenuGetFormat_Opening(object sender, CancelEventArgs e)
        {
           
            this.rmenuGetFormat.Items.Clear();
            this.rmenuGetFormat.Items.Add(new ToolStripMenuItem ( rm.GetString("rmenu5") + "<POST>",null,null,"rmenuPost1"));
            this.rmenuGetFormat.Items.Add(new ToolStripMenuItem (rm.GetString("rmenu6") + "</POST>",null,null,"rmenuPost2"));
            this.rmenuGetFormat.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu7"), null, null, "rmenuPublishPostData"));
            this.rmenuGetFormat.Items.Add(new ToolStripSeparator());

            for (int i = 0; i < this.listWebGetFlag.Items.Count; i++)
            {
                this.rmenuGetFormat.Items.Add("{" + this.listWebGetFlag.Items[i].Text + "}") ;
            }
        }

        private void rmenuGetFormat_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "rmenuPublishPostData")
            {
                frmBrowser wftm = new frmBrowser();
                wftm.getFlag = 3;
                wftm.rExportPData = new frmBrowser.ReturnExportPOST(GetExportpData);
                wftm.ShowDialog();
                wftm.Dispose();

                return;
            }

            Match s;

            if (Regex.IsMatch(e.ClickedItem.ToString(), "[{].*[}]"))
            {
                s = Regex.Match(e.ClickedItem.ToString(), "[{].*[}]");
            }
            else
            {
                s = Regex.Match(e.ClickedItem.ToString(), "[<].*[>]");
            }

            int startPos = this.txtExportUrl.SelectionStart;
            int l = this.txtExportUrl.SelectionLength;

            this.txtExportUrl.Text = this.txtExportUrl.Text.Substring(0, startPos) + s.Groups[0].Value + this.txtExportUrl.Text.Substring(startPos + l, this.txtExportUrl.Text.Length - startPos - l);

            this.txtExportUrl.SelectionStart = startPos + s.Groups[0].Value.Length;
            this.txtExportUrl.ScrollToCaret();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.rmenuGetFormat.Show(this.button9, 0, 21);
        }

        private void IsSave_TextChanged(object sender, EventArgs e)
        {
            if (this.IsSave.Text == "true" && this.FormState !=cGlobalParas.FormState .Browser )
            {
                this.cmdApply.Enabled = true;
            }
            else if (this.IsSave.Text == "false")
            {
                this.cmdApply.Enabled = false;
            }
        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            if (!CheckInputvalidity())
            {
                return;
            }

            try
            {

                if (!SaveTask(""))
                {
                    return;
                }
                
                this.IsSave.Text = "false";

                IsSaveTask = true;

                if (this.FormState == cGlobalParas.FormState.New)
                {
                    this.FormState = cGlobalParas.FormState.Edit;
                }
                
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString ("Error13") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void txtWeblinkDemo_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtFileName_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtInsertSql_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtExportUrl_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtExportCookie_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void comExportUrlCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void TaskType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cGlobalParas.ConvertID(this.TaskType.SelectedItem.ToString()) == (int)cGlobalParas.TaskType.AjaxHtmlByUrl)
                this.label42.Visible = true;
            else
                this.label42.Visible = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtNag_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void cmdAddNRule_Click(object sender, EventArgs e)
        {
            if (this.txtNag.Text == "" || this.txtNag.Text == null)
            {
                this.txtNag.Focus();
                MessageBox.Show(rm.GetString("Info8"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            for (int i = 0; i<this.dataNRule.Rows.Count; i++)
            {
                this.dataNRule.Rows[i].Cells[0].Value = i + 1;
            }

            int MaxLevel = 0;
            if (this.dataNRule.Rows.Count == 0)
                MaxLevel = 1;
            else
                MaxLevel = this.dataNRule.Rows.Count + 1;

            this.dataNRule.Rows.Add(MaxLevel.ToString (),this.txtNag.Text);

            this.txtNag.Text = "";
            
            this.IsSave.Text = "true";
        }

        private void cmdDelNRule_Click(object sender, EventArgs e)
        {
            this.dataNRule.Focus();
            SendKeys.Send("{Del}");
            this.IsSave.Text = "true";
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

        private void listTask_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                while (this.listTask.SelectedItems.Count > 0)
                {
                    this.listTask.Items.Remove(this.listTask.SelectedItems[0]);
                }

                this.IsSave.Text = "true";
            }
        }

        private void IsNavigPage_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsNavigPage.Checked == true)
            {

                this.groupBox14.Enabled = true;
            }
            else
            {

                this.groupBox14.Enabled = false;
            }

            this.IsSave.Text = "true";
        }

        private void IsAutoNextPage_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsAutoNextPage.Checked == true)
            {
                this.label13.Enabled = true;
                this.txtNextPage.Enabled = true;
                this.txtNextPage.Text = rm.GetString("NextPage");
            }
            else
            {
                if (this.txtNextPage.Text == rm.GetString("NextPage"))
                {
                    this.txtNextPage.Text = "";
                }
                this.label13.Enabled = false;
                this.txtNextPage.Enabled = false;
            }
            this.IsSave.Text = "true";
        }

        private void txtWebLink_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void udAgainNumber_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsIgnore404_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsIncludeHeader_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsDelRepRow_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsSaveErrorLog_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void cmdWizard_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
            RShowWizard(true);
            
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

        private void cmdUp_Click(object sender, EventArgs e)
        {
            int i = this.listWebGetFlag.SelectedItems[0].Index;

            ListViewItem Litem = this.listWebGetFlag.SelectedItems[0];

            this.listWebGetFlag.Items.Remove(this.listWebGetFlag.SelectedItems[0]);
            this.listWebGetFlag.Items.Insert(i-1,Litem );
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

        private void cmdUrlEncoding_Click(object sender, EventArgs e)
        {
            frmUrlEncoding f = new frmUrlEncoding();
            f.Show();
        }

        private void frmTask_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }


        private void cmdMoreNRule_Click(object sender, EventArgs e)
        {
            if (this.txtWebLink.Text=="http://" || this.txtWebLink.Text=="")
            {
                MessageBox.Show(rm.GetString ("Info114"),rm.GetString ("MessageboxInfo"),MessageBoxButtons.OK ,MessageBoxIcon.Information );
                this.txtWebLink.Focus ();
                return ;
            }

            frmAddNavRules fn = new frmAddNavRules(this.txtWebLink.Text);
            fn.rNavRule = new frmAddNavRules.ReturnNavRule(GetNavRule);
            fn.ShowDialog();
            fn.Dispose();

        }

        private void GetNavRule(string strNavRule)
        {
            this.txtNag.Text = strNavRule;
        }
          
   }

        
}