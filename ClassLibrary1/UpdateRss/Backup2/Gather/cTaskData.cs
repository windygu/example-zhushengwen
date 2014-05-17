using System;
using System.Collections.Generic;
using System.Text;
using SoukeyNetget.Task;

///���ܣ��ɼ���������
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget.Gather
{
    public class cTaskData
    {
        public cTaskData()
        {
            m_TaskSplitData = new List<cTaskSplitData>();
            m_Weblink=new List<Task.cWebLink> ();
            m_CutFlag = new List<Task.cWebpageCutFlag>();
        }

        ~cTaskData()
        {
            m_TaskSplitData = null;
        }

        /// <summary>
        /// ������
        /// </summary>
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

        private string m_TaskDemo;
        public string TaskDemo
        {
            get { return m_TaskDemo; }
            set { m_TaskDemo = value; }
        }

        private cGlobalParas.TaskType m_TaskType;
        public cGlobalParas.TaskType TaskType
        {
            get { return m_TaskType; }
            set { m_TaskType = value; }
        }

        private cGlobalParas.TaskRunType m_RunType;
        public cGlobalParas.TaskRunType RunType
        {
            get { return m_RunType; }
            set { m_RunType = value; }
        }

        private cGlobalParas.TaskState m_TaskState;
        public cGlobalParas.TaskState TaskState
        {
            get { return m_TaskState; }
            set { m_TaskState = value; }
        }

        //��Ҫ�ɼ�����ҳ����
        private int m_UrlCount;
        public int UrlCount
        {
            get { return m_UrlCount; }
            set { m_UrlCount = value; }
        }

        //ʵ����Ҫ�ɼ�����ַ����
        private int m_TrueUrlCount;
        public int TrueUrlCount
        {
            get { return m_TrueUrlCount; }
            set { m_TrueUrlCount = value; }
        }

        //�Ѿ��ɼ�������
        private int m_GatheredUrlCount;
        public int GatheredUrlCount
        {
            get { return m_GatheredUrlCount; }
            set { m_GatheredUrlCount = value; }
        }

        //�Ѳɼ���ʵ����ַ����
        private int m_GatheredTrueUrlCount;
        public int GatheredTrueUrlCount
        {
            get { return m_GatheredTrueUrlCount; }
            set { m_GatheredTrueUrlCount = value; }
        }

        /// �ɼ������url����
        private int m_GatherErrUrlCount;
        public int GatherErrUrlCount
        {
            get { return m_GatherErrUrlCount; }
            set { m_GatherErrUrlCount = value; }
        }

        //�Ѿ��ɼ���ʵ�������������
        private int m_GatheredTrueErrUrlCount;
        public int GatheredTrueErrUrlCount
        {
            get { return m_GatheredTrueErrUrlCount; }
            set { m_GatheredTrueErrUrlCount = value; }
        }

        private string m_tempFileName;
        public string tempFileName
        {
            get { return m_tempFileName; }
            set { m_tempFileName = value; }
        }

        private string m_ExportFileName;
        public string ExportFileName
        {
            get { return m_ExportFileName; }
            set { m_ExportFileName = value; }
        }

        private string m_Cookie;
        public string Cookie
        {
            get { return m_Cookie; }
            set { m_Cookie = value; }
        }

        private cGlobalParas.WebCode m_WebCode;
        public cGlobalParas.WebCode WebCode
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

        //Ϊ��Ҫ��string����Ϊ�п���Ϊ��
        private string m_UrlEncode;
        public string UrlEncode
        {
            get { return m_UrlEncode; }
            set { m_UrlEncode = value; }
        }

        private cGlobalParas.PublishType m_PublishType;
        public cGlobalParas.PublishType PublishType
        {
            get { return m_PublishType; }
            set { m_PublishType = value; }
        }

        //��ʱ���ݴ洢��λ��
        //�����Կ�������չ,���ɼ����洢,������ʱ�ļ�,
        //����������֧�������ж�,����֮�󼴿�ʵ�ּ����ɼ�
        //�����������ܻ�ռ�ô�������Դ,��Ҫʵ�ʲ���

        private string m_SavePath;
        public string SavePath
        {
            get { return m_SavePath; }
            set { m_SavePath = value; }
        }

        private int m_ThreadCount;
        public int ThreadCount
        {
            get { return m_ThreadCount; }
            set { m_ThreadCount = value; }
        }

        //���Դ���
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

        private bool m_IsErrorLog;
        public bool IsErrorLog
        {
            get { return m_IsErrorLog; }
            set { m_IsErrorLog = value; }
        }

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

        #region ��ҳ�ɼ��ĵ�ַ�͹���

        private List<Task.cWebLink> m_Weblink;
        public List<Task.cWebLink> Weblink
        {
            get { return m_Weblink; }
            set { m_Weblink = value; }
        }

        private List<Task.cWebpageCutFlag> m_CutFlag;
        public List<Task.cWebpageCutFlag> CutFlag
        {
            get { return m_CutFlag; }
            set { m_CutFlag = value; }
        }

        //����ֽ�����
        private List<cTaskSplitData> m_TaskSplitData;
        public List<cTaskSplitData> TaskSplitData
        {
            get { return m_TaskSplitData; }
            set { m_TaskSplitData = value; }
        }

        #endregion

        #region ��ҳ�ɼ�����ʼ����ֹ��ַ

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

        #endregion

    }
}
