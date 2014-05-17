using System;
using System.Collections.Generic;
using System.Text;

namespace SoukeyNetget.publish
{
        #region 定义发布事件
    //任务事件
    public class cPublishEventArgs : EventArgs
    {

        public cPublishEventArgs()
        {

        }
    
        /// <param name="cancel">是否取消事件</param>
        public cPublishEventArgs(bool cancel)
        {
            m_Cancel = cancel;
        }

        private bool m_Cancel;
        /// <summary>
        /// 是否取消事件
        /// </summary>
        public bool Cancel
        {
            get { return m_Cancel; }
            set { m_Cancel = value; }
        }
    }

    public class PublishErrorEventArgs : cPublishEventArgs
    {
        public PublishErrorEventArgs(Int64 TaskID, string TaskName, Exception error)
        {
            m_TaskID = TaskID;
            m_TaskName = TaskName;
            m_Error = error;
        }

        private Exception m_Error;
 
        public Exception Error
        {
            get { return m_Error; }
            set { m_Error = value; }
        }

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
    }

    //任务状态改变事件
    public class PublishStartedEventArgs : cPublishEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="old_state">旧的状态</param>
        /// <param name="new_statue">新的状态</param>
        public PublishStartedEventArgs(Int64 TaskID, string TaskName)
        {
            m_TaskID = TaskID;

            m_TaskName = TaskName;
        }

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
        
    }

    public class PublishCompletedEventArgs : cPublishEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="old_state">旧的状态</param>
        /// <param name="new_statue">新的状态</param>
        public PublishCompletedEventArgs(Int64 TaskID, string TaskName)
        {
            m_TaskID = TaskID;
            m_TaskName = TaskName;
        }

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

    }

    public class PublishFailedEventArgs : cPublishEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="old_state">旧的状态</param>
        /// <param name="new_statue">新的状态</param>
        public PublishFailedEventArgs(Int64 TaskID, string TaskName)
        {
            m_TaskID = TaskID;
            m_TaskName = TaskName;
        }

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

    }

    public class PublishTempDataCompletedEventArgs : cPublishEventArgs
    {
        public PublishTempDataCompletedEventArgs(Int64 TaskID, string TaskName)
        {
            m_TaskID = TaskID;
            m_TaskName = TaskName;
        }

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

    }

    //发布日志事件
    public class PublishLogEventArgs : cPublishEventArgs
    {
        public PublishLogEventArgs(Int64 TaskID, string strLog)
        {
            m_TaskID = TaskID;
            m_strLog = strLog;
        }

        private string m_strLog;
        public string strLog
        {
            get { return m_strLog; }
            set { m_strLog = value; }
        }

        private Int64 m_TaskID;
        public Int64 TaskID
        {
            get { return m_TaskID; }
            set { m_TaskID = value; }
        }
    }

    #endregion
}
