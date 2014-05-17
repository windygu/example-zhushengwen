using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

///���ܣ��ɼ������࣬��ǰ�汾Ϊ1.3 ע�⣺�ӵ�ǰ�İ汾��ʼ���ٶ���ǰ�ɰ汾������м���
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget.Task
{
    [Serializable]
    
    ///�������һ���๦�ܵ��࣬Ӧ��������ƣ����ü��ɵķ�ʽ������
    ///����������һ���汾���޸�
    ///��������Ӧ����һ������Ļ��ࣨ���ܻ����ɳ����ࣩ����������������Ӧ������������������ִ���࣬�������������
    /// ��ǰ�ɼ�������ṩ��һ�֣����ڻ��ṩ���ֲɼ��������ԣ��Դ��໹�ݲ��޸�
    ///������˵��ǰ�����⣬������Ҫ�������࣬ͬʱ������ִ�е���Ϣ�ϲ����ˣ������Ҫע�⣬ע���л�����˵��

    public class cTask
    {
        cXmlIO xmlConfig;
        private Single m_SupportTaskVersion = Single.Parse ("1.3");

        //�����ɴ��������汾�ţ�ע���1.3��ʼ���������಻����ǰ����
        public Single SupportTaskVersion
        {
            get { return m_SupportTaskVersion; }
        }


        #region ��Ĺ��������
        public cTask()
        {
            this.WebpageLink = new List<cWebLink>();
            this.WebpageCutFlag = new List<cWebpageCutFlag>();
            this.TriggerTask = new List<cTriggerTask>();
        }

        ~cTask()
        {
            this.WebpageLink = null;
            this.WebpageCutFlag = null;
            this.TriggerType = null;
        }
        #endregion

        #region TaskProperty

        //����Ϊ����ǰ״̬�����ԣ�������½�������Ӧ��Ϊδ����
        //�������д����ǣ���ǰδ��
        public int TaskState
        {
            get { return this.TaskState; }
            set { this.TaskState = value; }
        }

      

        //*****************************************************************************************************************
        //���¶���ΪTask����

        private Int64 m_TaskID;
        public Int64 TaskID
        {
            get { return m_TaskID; }
            set { m_TaskID = value; }
        }

        private string m_TaskName;
        public string TaskName
        {
            get { return m_TaskName; }
            set { m_TaskName = value; }
        }

        /// <summary>
        /// ����汾��Ϣ�����������ʱ������İ汾Ҳ��������������Ҫ��ÿһ������
        /// �汾����ʶ�𣬱����������ݵ�Ǩ�ƣ��������Ǽ�����һ�汾������������Ϣ��
        /// ������汾���̫���򲻻���м��ݣ���ר�ù���ʵ����������Ǩ��
        /// �˰汾����汾��Ϊ��1.2���Ƚ�1.0�汾����Ҫ�������������ݼӹ�������������
        /// </summary>
        private Single m_TaskVersion;
        public Single TaskVersion
        {
            get { return m_TaskVersion; }
            set { m_TaskVersion = value; }
        }

        private string m_TaskDemo;
        public string TaskDemo
        {
            get { return m_TaskDemo; }
            set { m_TaskDemo = value; }
        }

        private string m_TaskClass;
        public string TaskClass
        {
            get { return m_TaskClass; }
            set { m_TaskClass = value; }
        }

        private string m_TaskType;
        public string TaskType
        {
            get { return m_TaskType; }
            set { m_TaskType = value; }
        }

        private string m_SavePath;
        public string SavePath
        {
            get { return m_SavePath; }
            set { m_SavePath = value; }
        }

        private string m_TaskTemplate;
        public string TaskTemplate
        {
            get { return m_TaskTemplate; }
            set { m_TaskTemplate = value; }
        }

        private string m_RunType;
        public string RunType
        {
            get { return m_RunType; }
            set { m_RunType = value; }
        }

        private int m_UrlCount;
        public int UrlCount
        {
            get { return m_UrlCount; }
            set { m_UrlCount = value; }
        }

        private string m_DemoUrl;
        public string DemoUrl
        {
            get { return m_DemoUrl; }
            set { m_DemoUrl = value; }
        }

        private string m_Cookie;
        public string Cookie
        {
            get { return m_Cookie; }
            set { m_Cookie = value; }
        }

        private string m_WebCode;
        public string WebCode
        {
            get { return m_WebCode; }
            set { m_WebCode = value; }
        }

        private bool m_IsLogin;
        public bool IsLogin
        {
            get { return m_IsLogin; }
            set { m_IsLogin = value; }
        }

        private string m_LoginUrl;
        public string LoginUrl
        {
            get { return m_LoginUrl; }
            set { m_LoginUrl = value; }
        }

        private bool m_IsUrlEncode;
        public bool IsUrlEncode
        {
            get { return m_IsUrlEncode; }
            set { m_IsUrlEncode = value; }
        }

        private string m_UrlEncode;
        public string UrlEncode
        {
            get { return m_UrlEncode; }
            set { m_UrlEncode = value; }
        }

        //�������ݽ�֧��Access��MSsqlserver
        private string m_ExportType;
        public string ExportType
        {
            get { return m_ExportType; }
            set { m_ExportType = value; }
        }

        private string m_ExportFile;
        public string ExportFile
        {
            get { return m_ExportFile; }
            set { m_ExportFile = value; }
        }

        private string m_DataSource;
        public string DataSource
        {
            get { return m_DataSource; }
            set { m_DataSource = value; }
        }

        private string m_ExportUrl;
        public string ExportUrl
        {
            get { return m_ExportUrl; }
            set { m_ExportUrl = value; }
        }

        private string m_ExportUrlCode;
        public string ExportUrlCode
        {
            get { return m_ExportUrlCode; }
            set { m_ExportUrlCode = value; }
        }

        private string m_ExportCookie;
        public string ExportCookie
        {
            get { return m_ExportCookie; }
            set { m_ExportCookie = value; }
        }

        private string m_DataTableName;
        public string DataTableName
        {
            get { return m_DataTableName; }
            set { m_DataTableName = value; }
        }

        private string m_InsertSql;
        public string InsertSql
        {
            get { return m_InsertSql; }
            set { m_InsertSql =value ;}
        }

        private int m_ThreadCount;
        public int ThreadCount
        {
            get { return m_ThreadCount; }
            set { m_ThreadCount = value; }
        }

        //�ɼ�ҳ�����ݵ���ʼλ��
        private string m_StartPos;
        public string StartPos
        {
            get { return m_StartPos; }
            set { m_StartPos = value; }
        }

        private string m_EndPos;
        public string EndPos
        {
            get { return m_EndPos; }
            set { m_EndPos = value; }
        }

        private List<cWebLink> m_WebpageLink;
        public List<cWebLink> WebpageLink
        {
            get { return m_WebpageLink; }
            set { m_WebpageLink = value; }
        }

        private List<cWebpageCutFlag> m_WebpageCutFlag;
        public List<cWebpageCutFlag> WebpageCutFlag
        {
            get { return m_WebpageCutFlag; }
            set { m_WebpageCutFlag = value; }
        }

        //����Ϊ�������ݣ���Ҫ������汾������Ϊ1.3
        private int m_GatherAgainNumber;
        public int GatherAgainNumber
        {
            get { return m_GatherAgainNumber; }
            set { m_GatherAgainNumber = value; }
        }

        private bool m_IsIgnore404;
        public bool IsIgnore404
        {
            get { return m_IsIgnore404; }
            set { m_IsIgnore404 = value; }
        }

        private bool m_IsExportHeader;
        public bool IsExportHeader
        {
            get { return m_IsExportHeader; }
            set { m_IsExportHeader = value; }
        }

        //�Ƿ����������Ϣ
        private bool m_IsErrorLog;
        public bool IsErrorLog
        {
            get { return m_IsErrorLog; }
            set { m_IsErrorLog = value; }
        }

        //�Ƿ�ȥ���ظ�����
        private bool m_IsDelRepRow;
        public bool IsDelRepRow
        {
            get { return m_IsDelRepRow; }
            set { m_IsDelRepRow = value; }
        }

        private bool m_IsTrigger;
        public bool IsTrigger
        {
            get { return m_IsTrigger; }
            set { m_IsTrigger = value; }
        }

        private string m_TriggerType;
        public string TriggerType
        {
            get { return m_TriggerType; }
            set { m_TriggerType = value; }
        }

        private List<cTriggerTask> m_TriggerTask;
        public List<cTriggerTask> TriggerTask
        {
            get { return m_TriggerTask; }
            set { m_TriggerTask = value; }
        }

        #endregion

        
        #region ����Ϊ�෽��
        
        //�ж�����������ļ��Ƿ����
        private bool IsExistTaskIndex(string Path)
        {
            string FileName;
            FileName = Path + "\\index.xml";
            bool IsExists = System.IO.File.Exists(FileName);
            return IsExists;
        }

        private string GetTaskClassPath()
        {
            string TClassName = this.TaskClass;
            string Path;

            if (TClassName == null || TClassName == "" || TClassName =="�������")
            {
                Path = Program.getPrjPath() + "Tasks";
            }
            else
            {
                cTaskClass tClass = new cTaskClass();
                Path = tClass.GetTaskClassPathByName(TClassName);
                tClass = null;
            }
            return Path;
        }

        //�ж������ļ��Ƿ����
        public bool IsExistTaskFile(string FileName)
        {
            string Path = GetTaskClassPath();
            string File = Path + "\\" + FileName;
            bool IsExists = System.IO.File.Exists(File);
            return IsExists;
        }

        //����������Ϣ���ڱ���������Ϣ��ͬʱ���Զ�ά�������������
        public void Save(string TaskPath)
        {
            //��ȡ��Ҫ���������·��
            string tPath = "";

            if (TaskPath == "" || TaskPath == null)
            {
                tPath = GetTaskClassPath() + "\\";
            }
            else
            {
                tPath = TaskPath;
            }
            int i=0;

            //�жϴ�·�����Ƿ��Ѿ������˴�������������򷵻ش�����Ϣ
            if (IsExistTaskFile(tPath + this.TaskName))
            {
                throw new cSoukeyException ("�����Ѿ����ڣ����ܽ���");
            }

            //ά�������Index.xml�ļ�
            int TaskID=InsertTaskIndex(tPath);

            //��ʼ����Task����
            //����Task�����XML�ĵ���ʽ
            //��ǰ����xml�ļ�ȫ�����õ�ƴд�ַ�������ʽ,��û�в���xml���캯��
            string tXml;
            tXml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                "<Task>" +
                "<State></State>" +       ///��״ֵ̬��ǰ��Ч,���ڽ�������ʹ��
                "<BaseInfo>" +
                "<Version>1.3</Version>" +   //Ĭ�ϵ�ǰ������汾��Ϊ��1.2
                "<ID>" + TaskID + "</ID>" +
                "<Name>" + this.TaskName + "</Name>" +
                "<TaskDemo>" + this.TaskDemo + "</TaskDemo>" +
                "<Class>" + this.TaskClass + "</Class>" +
                "<Type>" + this.TaskType + "</Type>" +
                "<RunType>" + this.RunType + "</RunType>" +

                //ѡӴת�������·��
                "<SavePath>" + cTool.GetRelativePath(this.SavePath) + "</SavePath>" +
                "<ThreadCount>" + this.ThreadCount + "</ThreadCount>" +
                "<UrlCount>" + this.UrlCount + "</UrlCount>" +
                "<StartPos>" + cTool.ReplaceTrans(this.StartPos) + "</StartPos>" +
                "<EndPos>" + cTool.ReplaceTrans(this.EndPos) + "</EndPos>" +
                "<DemoUrl>" + cTool.ReplaceTrans(this.DemoUrl) + "</DemoUrl>" +
                "<Cookie>" + cTool.ReplaceTrans(this.Cookie) + "</Cookie>" +
                "<WebCode>" + this.WebCode + "</WebCode>" +
                "<IsLogin>" + this.IsLogin + "</IsLogin>" +
                "<LoginUrl>" + this.LoginUrl + "</LoginUrl>" +
                "<IsUrlEncode>" + this.IsUrlEncode + "</IsUrlEncode>" +
                "<UrlEncode>" + this.UrlEncode + "</UrlEncode>" +
                "</BaseInfo>" +
                "<Result>" +
                "<ExportType>" + this.ExportType + "</ExportType>" +
                "<ExportFileName>" + this.ExportFile + "</ExportFileName>" +
                "<DataSource>" + this.DataSource + "</DataSource>" +
                "<DataTableName>" + this.DataTableName + "</DataTableName>" +


                "<InsertSql>" + this.InsertSql + "</InsertSql>" +
                "<ExportUrl>" + cTool.ReplaceTrans(this.ExportUrl) + "</ExportUrl>" +
                "<ExportUrlCode>" + this.ExportUrlCode + "</ExportUrlCode>" +
                "<ExportCookie>" + cTool.ReplaceTrans(this.ExportCookie) + "</ExportCookie>" +
                "</Result>";

            tXml += "<Advance>" +
                "<GatherAgainNumber>" + this.GatherAgainNumber + "</GatherAgainNumber>" +
                "<IsIgnore404>" + this.IsIgnore404 + "</IsIgnore404>" +
                "<IsErrorLog>" + this.IsErrorLog + "</IsErrorLog>" +
                "<IsExportHeader>" + this.IsExportHeader + "</IsExportHeader>" +
                "<IsDelRepeatRow>" + this.IsDelRepRow + "</IsDelRepeatRow>" +
                "<IsTrigger>" + this.IsTrigger + "</IsTrigger>" +
                "<TriggerType>" + this.TriggerType + "</TriggerType>" +
                "</Advance>";

            tXml += "<Trigger>";
            for (i = 0; i < this.m_TriggerTask.Count; i++)
            {
                tXml += "<Task>";
                tXml += "<RunTaskType>" + this.m_TriggerTask[i].RunTaskType + "</RunTaskType>";
                tXml += "<RunTaskName>" + this.m_TriggerTask[i].RunTaskName + "</RunTaskName>";
                tXml += "<RunTaskPara>" + this.m_TriggerTask[i].RunTaskPara + "</RunTaskPara>";
                tXml += "</Task>";
            }
            tXml += "</Trigger>";

            tXml +="<WebLinks>";

            if (this.WebpageLink != null)
            {
                for (i = 0; i < this.WebpageLink.Count; i++)
                {
                    tXml += "<WebLink>";
                    tXml += "<Url>" + cTool.ReplaceTrans ( this.WebpageLink[i].Weblink.ToString ()) + "</Url>";
                    tXml += "<IsNag>" + this.WebpageLink[i].IsNavigation + "</IsNag>";
                    tXml += "<IsNextPage>" + this.WebpageLink[i].IsNextpage + "</IsNextPage>";
                    tXml += "<NextPageRule>" + cTool.ReplaceTrans(this.WebpageLink[i].NextPageRule) + "</NextPageRule>";

                    //Ĭ�ϲ���һ���ڵ㣬��ʾ�����ӵ�ַ��δ���вɼ�����Ϊ��ϵͳ�����������Ĭ��ΪUnGather
                    tXml += "<IsGathered>" + (int)cGlobalParas.UrlGatherResult.UnGather + "</IsGathered>";

                    //�������ַ�ĵ�������
                    if (this.WebpageLink[i].IsNavigation ==true)
                    {
                        tXml += "<NavigationRules>";
                        for (int j = 0; j < this.WebpageLink[i].NavigRules.Count; j++)
                        {
                            tXml +="<Rule>";
                            tXml += "<Url>" + cTool.ReplaceTrans(this.WebpageLink[i].NavigRules[j].Url) + "</Url>";
                            tXml += "<Level>" + this.WebpageLink[i].NavigRules[j].Level + "</Level>";
                            tXml += "<NagRule>" + cTool.ReplaceTrans(this.WebpageLink[i].NavigRules[j].NavigRule) + "</NagRule>";
                            tXml +="</Rule>";
                        }
                        tXml +="</NavigationRules>";
                    }
                    tXml += "</WebLink>";
                }
            }
                 
		    tXml +="</WebLinks>" +
                "<GatherRule>" ;
            if (this.WebpageCutFlag != null)
            {
                for (i = 0; i < this.WebpageCutFlag.Count; i++)
                {
                    tXml += "<Rule>";
                    tXml += "<Title>" + cTool.ReplaceTrans( this.WebpageCutFlag[i].Title) + "</Title>";
                    tXml += "<DataType>" + this.WebpageCutFlag[i].DataType + "</DataType>";
                    tXml += "<StartFlag>" + cTool.ReplaceTrans (this.WebpageCutFlag[i].StartPos) + "</StartFlag>";
                    tXml += "<EndFlag>" + cTool.ReplaceTrans (this.WebpageCutFlag[i].EndPos) + "</EndFlag>";
                    tXml += "<LimitSign>" + this.WebpageCutFlag[i].LimitSign + "</LimitSign>";
                    tXml += "<RegionExpression>" + cTool.ReplaceTrans(this.WebpageCutFlag[i].RegionExpression) + "</RegionExpression>";
                    tXml += "<ExportLimit>" + this.WebpageCutFlag [i].ExportLimit + "</ExportLimit>";
                    tXml += "<ExportExpression>" + cTool.ReplaceTrans(this.WebpageCutFlag[i].ExportExpression) + "</ExportExpression>";
                    tXml += "</Rule>";
                }
            }
             tXml +="</GatherRule>" +
                "</Task>";
            
            xmlConfig =new cXmlIO ();
            xmlConfig.NewXmlFile (tPath + this.TaskName + ".xml",tXml );
            xmlConfig = null;

        }

        //������������Ϣ���������ļ��������������ݵ����⣬�˷���
        //��Ҫ����֧����������ʹ��
        public void SaveTaskFile(string TaskPath)
        {
            //��ȡ��Ҫ���������·��
            string tPath = "";

            if (TaskPath == "" || TaskPath == null)
            {
                tPath = GetTaskClassPath() + "\\";
            }
            else
            {
                tPath = TaskPath;
            }
            int i = 0;

            //�жϴ�·�����Ƿ��Ѿ������˴�������������򷵻ش�����Ϣ
            if (IsExistTaskFile(tPath + this.TaskName))
            {
                throw new cSoukeyException("�����Ѿ����ڣ����ܽ���");
            }

            //��ʼ����Task����
            //����Task�����XML�ĵ���ʽ
            //��ǰ����xml�ļ�ȫ�����õ�ƴд�ַ�������ʽ,��û�в���xml���캯��
            string tXml;
            tXml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                "<Task>" +
                "<State></State>" +       ///��״ֵ̬��ǰ��Ч,���ڽ�������ʹ��
                "<BaseInfo>" +
                "<Version>1.3</Version>" +   //Ĭ�ϵ�ǰ������汾��Ϊ��1.2
                "<ID>" + TaskID + "</ID>" +
                "<Name>" + this.TaskName + "</Name>" +
                "<TaskDemo>" + this.TaskDemo + "</TaskDemo>" +
                "<Class>" + this.TaskClass + "</Class>" +
                "<Type>" + this.TaskType + "</Type>" +
                "<RunType>" + this.RunType + "</RunType>" +

                //ѡӴת�������·��
                "<SavePath>" + cTool.GetRelativePath(this.SavePath) + "</SavePath>" +
                "<ThreadCount>" + this.ThreadCount + "</ThreadCount>" +
                "<UrlCount>" + this.UrlCount + "</UrlCount>" +
                "<StartPos>" + cTool.ReplaceTrans(this.StartPos) + "</StartPos>" +
                "<EndPos>" + cTool.ReplaceTrans(this.EndPos) + "</EndPos>" +
                "<DemoUrl>" + cTool.ReplaceTrans(this.DemoUrl) + "</DemoUrl>" +
                "<Cookie>" + cTool.ReplaceTrans(this.Cookie) + "</Cookie>" +
                "<WebCode>" + this.WebCode + "</WebCode>" +
                "<IsLogin>" + this.IsLogin + "</IsLogin>" +
                "<LoginUrl>" + this.LoginUrl + "</LoginUrl>" +
                "<IsUrlEncode>" + this.IsUrlEncode + "</IsUrlEncode>" +
                "<UrlEncode>" + this.UrlEncode + "</UrlEncode>" +
                "</BaseInfo>" +
                "<Result>" +
                "<ExportType>" + this.ExportType + "</ExportType>" +
                "<ExportFileName>" + this.ExportFile + "</ExportFileName>" +
                "<DataSource>" + this.DataSource + "</DataSource>" +
                "<DataTableName>" + this.DataTableName + "</DataTableName>" +


                "<InsertSql>" + this.InsertSql + "</InsertSql>" +
                "<ExportUrl>" + cTool.ReplaceTrans(this.ExportUrl) + "</ExportUrl>" +
                "<ExportUrlCode>" + this.ExportUrlCode + "</ExportUrlCode>" +
                "<ExportCookie>" + cTool.ReplaceTrans(this.ExportCookie) + "</ExportCookie>" +
                "</Result>";

            tXml += "<Advance>" +
                "<GatherAgainNumber>" + this.GatherAgainNumber + "</GatherAgainNumber>" +
                "<IsIgnore404>" + this.IsIgnore404 + "</IsIgnore404>" +
                "<IsErrorLog>" + this.IsErrorLog + "</IsErrorLog>" +
                "<IsExportHeader>" + this.IsExportHeader + "</IsExportHeader>" +
                "<IsDelRepeatRow>" + this.IsDelRepRow + "</IsDelRepeatRow>" +
                "<IsTrigger>" + this.IsTrigger + "</IsTrigger>" +
                "<TriggerType>" + this.TriggerType + "</TriggerType>" +
                "</Advance>";

            tXml += "<Trigger>";
            for (i = 0; i < this.m_TriggerTask.Count; i++)
            {
                tXml += "<Task>";
                tXml += "<RunTaskType>" + this.m_TriggerTask[i].RunTaskType + "</RunTaskType>";
                tXml += "<RunTaskName>" + this.m_TriggerTask[i].RunTaskName + "</RunTaskName>";
                tXml += "<RunTaskPara>" + this.m_TriggerTask[i].RunTaskPara + "</RunTaskPara>";
                tXml += "</Task>";
            }
            tXml += "</Trigger>";

            tXml += "<WebLinks>";

            if (this.WebpageLink != null)
            {
                for (i = 0; i < this.WebpageLink.Count; i++)
                {
                    tXml += "<WebLink>";
                    tXml += "<Url>" + cTool.ReplaceTrans(this.WebpageLink[i].Weblink.ToString()) + "</Url>";
                    tXml += "<IsNag>" + this.WebpageLink[i].IsNavigation + "</IsNag>";
                    tXml += "<IsNextPage>" + this.WebpageLink[i].IsNextpage + "</IsNextPage>";
                    tXml += "<NextPageRule>" + cTool.ReplaceTrans(this.WebpageLink[i].NextPageRule) + "</NextPageRule>";

                    //Ĭ�ϲ���һ���ڵ㣬��ʾ�����ӵ�ַ��δ���вɼ�����Ϊ��ϵͳ�����������Ĭ��ΪUnGather
                    tXml += "<IsGathered>" + (int)cGlobalParas.UrlGatherResult.UnGather + "</IsGathered>";

                    //�������ַ�ĵ�������
                    if (this.WebpageLink[i].IsNavigation == true)
                    {
                        tXml += "<NavigationRules>";
                        for (int j = 0; j < this.WebpageLink[i].NavigRules.Count; j++)
                        {
                            tXml += "<Rule>";
                            tXml += "<Url>" + cTool.ReplaceTrans(this.WebpageLink[i].NavigRules[j].Url) + "</Url>";
                            tXml += "<Level>" + this.WebpageLink[i].NavigRules[j].Level + "</Level>";
                            tXml += "<NagRule>" + cTool.ReplaceTrans(this.WebpageLink[i].NavigRules[j].NavigRule) + "</NagRule>";
                            tXml += "</Rule>";
                        }
                        tXml += "</NavigationRules>";
                    }
                    tXml += "</WebLink>";
                }
            }

            tXml += "</WebLinks>" +
                "<GatherRule>";
            if (this.WebpageCutFlag != null)
            {
                for (i = 0; i < this.WebpageCutFlag.Count; i++)
                {
                    tXml += "<Rule>";
                    tXml += "<Title>" + cTool.ReplaceTrans(this.WebpageCutFlag[i].Title) + "</Title>";
                    tXml += "<DataType>" + this.WebpageCutFlag[i].DataType + "</DataType>";
                    tXml += "<StartFlag>" + cTool.ReplaceTrans(this.WebpageCutFlag[i].StartPos) + "</StartFlag>";
                    tXml += "<EndFlag>" + cTool.ReplaceTrans(this.WebpageCutFlag[i].EndPos) + "</EndFlag>";
                    tXml += "<LimitSign>" + this.WebpageCutFlag[i].LimitSign + "</LimitSign>";
                    tXml += "<RegionExpression>" + cTool.ReplaceTrans(this.WebpageCutFlag[i].RegionExpression) + "</RegionExpression>";
                    tXml += "<ExportLimit>" + this.WebpageCutFlag[i].ExportLimit + "</ExportLimit>";
                    tXml += "<ExportExpression>" + cTool.ReplaceTrans(this.WebpageCutFlag[i].ExportExpression) + "</ExportExpression>";
                    tXml += "</Rule>";
                }
            }
            tXml += "</GatherRule>" +
               "</Task>";

            xmlConfig = new cXmlIO();
            xmlConfig.NewXmlFile(tPath + this.TaskName + ".xml", tXml);
            xmlConfig = null;
        }

        //�����������������
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TaskName">��������</param>
        /// <param name="OldTaskClass">ԭ���������</param>
        /// <param name="NewTaskClass">���������</param>
        /// 
        public void ChangeTaskClass(string TaskName, string OldTaskClass, string NewTaskClass)
        {
            cTaskClass tc = new cTaskClass();
            string oldPath="";
            string NewPath="";

            if (OldTaskClass == "")
                oldPath = Program.getPrjPath() + "tasks";
            else
                oldPath = tc.GetTaskClassPathByName(OldTaskClass);

            if (NewTaskClass =="")
                NewPath = Program.getPrjPath() + "tasks";
            else
                NewPath = tc.GetTaskClassPathByName(NewTaskClass);

            string FileName = TaskName + ".xml";

            System.IO.File.Copy(oldPath + "\\" + FileName, NewPath + "\\" + FileName);

            LoadTask(NewPath + "\\" + FileName);

            if (NewTaskClass =="")
                this.TaskClass ="";
            else
                this.TaskClass = NewTaskClass;

            Save("");

            DeleTask(oldPath, TaskName);

            tc = null;
        }

        //���������������һ�������ԭ�з��࿽������һ��������
        public void CopyTask(string TaskName, string OldTaskClass, string NewTaskClass)
        {
            cTaskClass tc = new cTaskClass();
            string oldPath = "";
            string NewPath = "";

            if (OldTaskClass == "")
                oldPath = Program.getPrjPath() + "tasks";
            else
                oldPath = tc.GetTaskClassPathByName(OldTaskClass);

            if (NewTaskClass == "")
                NewPath = Program.getPrjPath() + "tasks";
            else
                NewPath = tc.GetTaskClassPathByName(NewTaskClass);

            tc = null;

            string FileName = "";

            if (OldTaskClass == NewTaskClass || (File.Exists(NewPath + "\\" + TaskName + ".xml")))
            {
                FileName = TaskName + "-����.xml";
                
                System.IO.File.Copy(oldPath + "\\" + TaskName + ".xml", NewPath + "\\" + FileName);
                TaskName = TaskName + "-����";
                
            }
            else
            {
                FileName = TaskName + ".xml";
                System.IO.File.Copy(oldPath + "\\" + FileName, NewPath + "\\" + FileName);
            }

            LoadTask(NewPath + "\\" + FileName);

            //�޸����������
            this.TaskName = TaskName;

            if (NewTaskClass == "")
                this.TaskClass = "";
            else
                this.TaskClass = NewTaskClass;

            Save("");

            
        }

        //����������Ϣ�����������ļ��������½���������������id
        public int InsertTaskIndex(string tPath)
        {

            cTaskIndex tIndex;

            //�жϴ�·�����Ƿ��������������ļ�
            if (!IsExistTaskIndex(tPath))
            {
                //��������������ļ�������Ҫ����һ���ļ�
                tIndex = new cTaskIndex();
                tIndex.NewIndexFile(tPath);
            }
            else
            {
                tIndex = new cTaskIndex(tPath + "\\index.xml");
            }

            tIndex.GetTaskDataByClass(this.TaskClass);

            int MaxTaskID = tIndex.GetTaskClassCount();

            //����TaskIndex�ļ�����,�˲�������Ӧ�ð�����TaskIndex����
            string indexXml = "<id>" + MaxTaskID + "</id>" +
                    "<Name>" + this.TaskName + "</Name>" +
                    "<Type>" + this.TaskType + "</Type>" +
                    "<RunType>" + this.RunType + "</RunType>" +
                    "<ExportFile>" + this.ExportFile + "</ExportFile>" +
                    "<WebLinkCount>" + this.UrlCount + "</WebLinkCount>" +
                    "<IsLogin>" + this.IsLogin + "</IsLogin>" +
                    "<PublishType>" + this.ExportType + "</PublishType>";
            tIndex.InsertTaskIndex(indexXml);
            tIndex = null;

            return MaxTaskID;

        }

        //���½�һ������ʱ�����ô˷���
        public void New()
        {
            //this.TaskState =(int) cGlobalParas.TaskState.TaskUnStart;

            if (xmlConfig != null)
            {
                xmlConfig = null;
            }
        }

        //����һ�����񵽴�����
        public void LoadTask(String FileName)
        {
            LoadTaskInfo(FileName);
        }

        //����һ�������������񵽴����У����ش�����Ϣ
        //�˷�����taskrunר��
        public void LoadTask(Int64  TaskID)
        {
            string FileName = Program.getPrjPath() + "Tasks\\run\\task" + TaskID + ".xml";
            LoadTaskInfo(FileName);
        }

        //����ָ���������Ϣ
        private void LoadTaskInfo(string FileName)
        {
            //����һ����������װ��һ������
            try
            {
                xmlConfig = new cXmlIO(FileName);

                //��ȡTaskClass�ڵ�
                //TaskClass = xmlConfig.GetData("descendant::TaskClasses");
            }
            catch (System.Exception ex)
            {
                if (!File.Exists(FileName))
                {
                    throw new System.IO.IOException("��ָ���������ļ������ڣ�");
                }
                else
                {
                    throw ex;
                }
            }

            //����������Ϣ
            this.TaskID =Int64.Parse ( xmlConfig.GetNodeValue("Task/BaseInfo/ID"));
            this.TaskName = xmlConfig.GetNodeValue("Task/BaseInfo/Name");

            ///��������汾��Ϣ��ע�⣺1.0���ǲ����ڰ汾��Ϣ�����ģ������������1.0������
            ///�����
            try 
            {
                this.TaskVersion = Single.Parse(xmlConfig.GetNodeValue("Task/BaseInfo/Version"));
            }
            catch (System.Exception )
            {
                this.TaskVersion =Single.Parse ("1.0");
            }

            if (TaskVersion != SupportTaskVersion)
            {
                throw new cSoukeyException("����������İ汾����ϵͳҪ��İ汾���������������������ԣ�");
            }


            this.TaskDemo = xmlConfig.GetNodeValue("Task/BaseInfo/TaskDemo");
            this.TaskClass = xmlConfig.GetNodeValue("Task/BaseInfo/Class");
            this.TaskType=xmlConfig.GetNodeValue("Task/BaseInfo/Type");
            this.RunType = xmlConfig.GetNodeValue("Task/BaseInfo/RunType");

            //���������·��������Ҫ����ϵͳ·��
            this.SavePath = Program.getPrjPath () + xmlConfig.GetNodeValue("Task/BaseInfo/SavePath");
            this.UrlCount =int.Parse (xmlConfig.GetNodeValue("Task/BaseInfo/UrlCount").ToString ());
            this.ThreadCount = int.Parse (xmlConfig.GetNodeValue("Task/BaseInfo/ThreadCount"));
            this.Cookie = xmlConfig.GetNodeValue("Task/BaseInfo/Cookie");
            this.DemoUrl = xmlConfig.GetNodeValue("Task/BaseInfo/DemoUrl");
            this.StartPos = xmlConfig.GetNodeValue("Task/BaseInfo/StartPos");
            this.EndPos = xmlConfig.GetNodeValue("Task/BaseInfo/EndPos");
            this.WebCode = xmlConfig.GetNodeValue("Task/BaseInfo/WebCode");
            this.IsLogin =( xmlConfig.GetNodeValue("Task/BaseInfo/IsLogin")=="True"? true :false );
            this.LoginUrl = xmlConfig.GetNodeValue("Task/BaseInfo/LoginUrl");
            this.IsUrlEncode = (xmlConfig.GetNodeValue("Task/BaseInfo/IsUrlEncode") == "True" ? true : false);
            this.UrlEncode = xmlConfig.GetNodeValue("Task/BaseInfo/UrlEncode");

            this.ExportType =xmlConfig.GetNodeValue("Task/Result/ExportType") ;
            this.ExportFile = xmlConfig.GetNodeValue("Task/Result/ExportFileName");
            this.DataSource = xmlConfig.GetNodeValue("Task/Result/DataSource");
            this.DataTableName = xmlConfig.GetNodeValue("Task/Result/DataTableName");

            this.InsertSql = xmlConfig.GetNodeValue("Task/Result/InsertSql");
            this.ExportUrl = xmlConfig.GetNodeValue("Task/Result/ExportUrl");
            this.ExportUrlCode = xmlConfig.GetNodeValue("Task/Result/ExportUrlCode");
            this.ExportCookie = xmlConfig.GetNodeValue("Task/Result/ExportCookie");
       
            //���ظ߼�������Ϣ
            this.GatherAgainNumber= int.Parse (xmlConfig.GetNodeValue("Task/Advance/GatherAgainNumber"));
            this.IsIgnore404 = (xmlConfig.GetNodeValue("Task/Advance/IsIgnore404") == "True" ? true : false);
            this.IsErrorLog = (xmlConfig.GetNodeValue("Task/Advance/IsErrorLog") == "True" ? true : false);
            this.IsDelRepRow = (xmlConfig.GetNodeValue("Task/Advance/IsDelRepeatRow") == "True" ? true : false);
            this.IsExportHeader =( xmlConfig.GetNodeValue("Task/Advance/IsExportHeader") == "True" ? true : false);
            this.IsTrigger =( xmlConfig.GetNodeValue("Task/Advance/IsTrigger") == "True" ? true : false);
            this.TriggerType = xmlConfig.GetNodeValue("Task/Advance/TriggerType");
    
            DataView dw = new DataView();
            int i;
            
            //����Trigger��Ϣ
            dw = xmlConfig.GetData("descendant::Trigger");
            cTriggerTask tt;

            if (dw != null)
            {
                for (i = 0; i < dw.Count; i++)
                {
                    tt = new cTriggerTask();
                    tt.RunTaskType = int.Parse ( dw[i].Row["RunTaskType"].ToString());
                    tt.RunTaskName = dw[i].Row["RunTaskName"].ToString();
                    tt.RunTaskPara = dw[i].Row["RunTaskPara"].ToString();

                    this.TriggerTask.Add(tt);
                }
            }

            dw = null;
            dw = new DataView();
            
            dw = xmlConfig.GetData("descendant::WebLinks");
            cWebLink w;

            DataView dn;

            if (dw!=null)
            {
                for (i = 0; i < dw.Count; i++)
                {
                    w = new cWebLink();
                    w.id = i;
                    w.Weblink  = dw[i].Row["Url"].ToString();
                    if (dw[i].Row["IsNag"].ToString() == "True")
                        w.IsNavigation = true;
                    else
                        w.IsNavigation = false;

                    if (dw[i].Row["IsNextPage"].ToString() == "True")
                        w.IsNextpage = true;
                    else
                        w.IsNextpage = false;

                    w.NextPageRule = dw[i].Row["NextPageRule"].ToString();
                    w.IsGathered = int.Parse((dw[i].Row["IsGathered"].ToString() == null || dw[i].Row["IsGathered"].ToString() == "") ? "2031" : dw[i].Row["IsGathered"].ToString());
                    
                    //���ص�������
                    if (w.IsNavigation == true)
                    {
                        dn = dw[i].CreateChildView("WebLink_NavigationRules")[0].CreateChildView("NavigationRules_Rule");
                        cNavigRule nRule;

                        for (int m = 0; m < dn.Count; m++)
                        {
                            nRule = new cNavigRule();
                            nRule.Url = dn[m].Row["Url"].ToString();
                            nRule.Level = int.Parse(dn[m].Row["Level"].ToString());
                            nRule.NavigRule = dn[m].Row["NagRule"].ToString();

                            w.NavigRules.Add(nRule);
                        }
                    }
                    this.WebpageLink.Add(w);
                    w = null;
                }
            }

            dw = null;
            dw = new DataView();
            dw = xmlConfig.GetData("descendant::GatherRule");
            Task.cWebpageCutFlag c;
            if (dw != null)
            {
                for (i = 0; i < dw.Count; i++)
                {
                    c = new Task.cWebpageCutFlag();
                    c.Title = dw[i].Row["Title"].ToString();
                    c.DataType = int.Parse((dw[i].Row["DataType"].ToString() == null || dw[i].Row["DataType"].ToString() == "") ? "0" : dw[i].Row["DataType"].ToString());
                    c.StartPos = dw[i].Row["StartFlag"].ToString();
                    c.EndPos = dw[i].Row["EndFlag"].ToString();
                    c.LimitSign = int.Parse((dw[i].Row["LimitSign"].ToString() == null || dw[i].Row["LimitSign"].ToString() == "") ? "0" : dw[i].Row["LimitSign"].ToString());

                    //����汾��ͬʱ��ɵĴ��󣬲��񲻴���
                    try
                    {
                        c.RegionExpression = dw[i].Row["RegionExpression"].ToString();
                        c.ExportLimit = int.Parse((dw[i].Row["ExportLimit"].ToString() == null || dw[i].Row["ExportLimit"].ToString() == "") ? "0" : dw[i].Row["ExportLimit"].ToString());
                        c.ExportExpression = dw[i].Row["ExportExpression"].ToString();
                    }
                    catch (System.Exception)
                    {
                    }

                    this.WebpageCutFlag.Add(c);
                    c = null;
                }
            }
            dw=null;

        }

        //ɾ��һ������
        //ɾ�������ʱ��ϵͳ����һ�����������Զ�����һ��
        //�����ļ�����~
        public bool DeleTask(string TaskPath,string TaskName)
        {
            //����ɾ�����������ڷ����µ�index.xml�е���������Ȼ����ɾ������������ļ�
            string tPath = "";

            if (TaskPath == "")
            {
                tPath = Program.getPrjPath() + "Tasks";
            }
            else
            {
                tPath = TaskPath;
            }

            //��ɾ�������ļ��е�������������
            cTaskIndex tIndex = new cTaskIndex(tPath + "\\index.xml");
            tIndex.DeleTaskIndex(TaskName);
            tIndex =null;

            //����Ǳ༭״̬��Ϊ�˷�ֹɾ�����ļ������񱣴�ʧ�ܣ���
            //�����ļ�����ʧ�����⣬�����Ȳ�ɾ�����ļ���ֻ�ǽ������

            //ɾ������������ļ�
            string FileName =TaskPath   + "\\" + TaskName + ".xml" ;
            string tmpFileName=TaskPath   + "\\~" + TaskName + ".xml" ;

            try
            {
                //ɾ��������ʱ�ļ�
                if (File.Exists(tmpFileName))
                {
                    File.SetAttributes(tmpFileName, System.IO.FileAttributes.Normal);
                    System.IO.File.Delete(tmpFileName);
                }
           
                System.IO.File.Move(FileName, tmpFileName);

            }
            catch (System.Exception )
            {
                //���������ʱ�ļ����ݲ���ʧ�ܣ���������У�����Ӱ�쵽���յ��ļ�����
                //������ļ�����Ҳʧ�ܣ���ֻ�ܱ�����
            }

            //ɾ�����������ļ�
            if (File.Exists(FileName))
            {
                File.SetAttributes(FileName, System.IO.FileAttributes.Normal);
                System.IO.File.Delete(FileName);
            }

            //���ļ�����Ϊ����
            //System.IO.File.SetAttributes(tmpFileName, System.IO.FileAttributes.Hidden);
            return true;
        }

        //����taskid�޸����������
        public bool RenameTask(string TClass,string OldTaskName, string NewTaskName)
        {
            try
            {
                //������������ȡ���������·��
                cTaskClass tc = new cTaskClass();
                string tClassPath = "";

                //�ж��µ�����·���Ƿ���ڣ���������򱨴�
                if (TClass == "")
                {
                    tClassPath = Program.getPrjPath() + "tasks";
                }
                else
                {
                    tClassPath = tc.GetTaskClassPathByName(TClass);
                }

                tc = null;

                if (File.Exists(tClassPath + "\\" + NewTaskName + ".xml"))
                    throw new cSoukeyException("���޸ĵ����������Ѿ����ڣ��������޸ģ�");

                cTaskIndex xmlTasks = new cTaskIndex();

                if (TClass == "")
                {
                    xmlTasks.GetTaskDataByClass();
                }
                else
                {
                    xmlTasks.GetTaskDataByClass(TClass);
                }

                //��ʼ��ʼ���˷����µ�����
                int count = xmlTasks.GetTaskClassCount();

                for (int i = 0; i < count; i++)
                {
                    if (xmlTasks.GetTaskName(i) == NewTaskName)
                    {
                        xmlTasks = null;
                        throw new cSoukeyException("���޸ĵ����������Ѿ����ڣ��������޸ģ�");
                    }
                }
                xmlTasks = null;

                //��ʼ�޸����������
                //�ȿ�ʼ�޸�index.xml������
                cXmlIO xmlIndex = new cXmlIO(tClassPath + "\\index.xml");
                xmlIndex.EditNodeValue("TaskIndex", "Name", OldTaskName, "Name", NewTaskName);
                xmlIndex.Save();
                xmlIndex = null;

                //��ʼ�޸����������
                cXmlIO xmlTask = new cXmlIO(tClassPath + "\\" + OldTaskName + ".xml");
                xmlTask.EditNodeValue("Task/BaseInfo/Name", NewTaskName);
                xmlTask.Save();
                xmlTask = null;

                File.SetAttributes(tClassPath + "\\" + OldTaskName + ".xml", System.IO.FileAttributes.Normal);
                File.Move(tClassPath + "\\" + OldTaskName + ".xml", tClassPath + "\\" + NewTaskName + ".xml");
            }
            catch (System.Exception ex)
            {
                throw ex;
                return false;
            }

            return true;
        }

         #endregion

    }
}
