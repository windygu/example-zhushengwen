using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using SoukeyNetget.Task;

///功能：发布任务数据
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace SoukeyNetget.publish
{
    class cPublishTaskData
    {
        
        public cPublishTaskData()
        {
            m_PublishData = new DataTable();
        }

        ~cPublishTaskData()
        {
        }

        private Int64  m_TaskID;
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

        private string m_FileName;
        public string FileName
        {
            get { return m_FileName; }
            set { m_FileName = value; }
        }

        private DataTable m_PublishData;
        public DataTable PublishData
        {
            get { return m_PublishData; }
            set { m_PublishData = value; }
        }

        public int Count
        {
            get { return m_PublishData.Rows.Count; }
        }

        private cGlobalParas.PublishType m_PublishType;
        public cGlobalParas.PublishType PublishType
        {
            get { return m_PublishType; }
            set { m_PublishType = value; }
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

        #region 任务1.2版本已经取消了数据库用户名和密码，继续保留是为了兼容老版本
        private string m_DataUser;
        public string DataUser
        {
            get { return m_DataUser; }
            set { m_DataUser = value; }
        }

        private string m_DataPwd;
        public string DataPwd
        {
            get { return m_DataPwd; }
            set { m_DataPwd = value; }
        }
        #endregion

        private string m_DataTableName;
        public string DataTableName
        {
            get { return m_DataTableName; }
            set { m_DataTableName = value; }
        }

        //以下内容是为了可以支持任务1.2版本添加的任务发布属性
        private string m_InsertSql;
        public string InsertSql
        {
            get { return m_InsertSql; }
            set { m_InsertSql = value; }
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

        private bool m_IsErrorLog;
        public bool IsErrorLog
        {
            get { return m_IsErrorLog; }
            set { m_IsErrorLog = value; }
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

        /// <summary>
        /// 导出文本或Excel文件时，是否输出表头
        /// </summary>
        private bool m_IsExportHeader;
        public bool IsExportHeader
        {
            get { return m_IsExportHeader; }
            set { m_IsExportHeader = value; }
        }

    }
}
