using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

///功能： 定义采集任务相关事件
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace SoukeyNetget.Gather
{

        #region 定义采集任务相关的事件

        //任务初始化事件
        public class TaskInitializedEventArgs : EventArgs
        {

            public TaskInitializedEventArgs(Int64 TaskID)
            {
                m_TaskID = TaskID;
            }

            private Int64 m_TaskID;
            public Int64 TaskID
            {
                get { return m_TaskID; }
                set { m_TaskID = value; }
            }
        }

        //任务线程错误响应事件
        public class TaskThreadErrorEventArgs : EventArgs
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="error">捕获的异常</param>
            public TaskThreadErrorEventArgs(Exception error)
            {
                m_Error = error;
            }

            private Exception m_Error;
            /// <summary>
            /// 错误信息
            /// </summary>
            public Exception Error
            {
                get { return m_Error; }
                set { m_Error = value; }
            }
        }

        //任务错误响应事件
        public class TaskErrorEventArgs : EventArgs
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="dtc">发生错误的分块</param>
            /// <param name="error">捕获的异常</param>
            public TaskErrorEventArgs(cGatherTaskSplit dtc, Exception error)
            {
                m_Error = error;
                m_ErrorThread = dtc;
            }

            private Exception m_Error;
            private cGatherTaskSplit m_ErrorThread;
            /// <summary>
            /// 错误信息
            /// </summary>
            public Exception Error
            {
                get { return m_Error; }
                set { m_Error = value; }
            }
            /// <summary>
            /// 发生错误的分块
            /// </summary>
            public cGatherTaskSplit ErrorThread
            {
                get { return m_ErrorThread; }
                set { m_ErrorThread = value; }
            }
        }

        //采集任务状态改变事件
        public class TaskStateChangedEventArgs : cTaskEventArgs
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="old_state">旧的状态</param>
            /// <param name="new_statue">新的状态</param>
            public TaskStateChangedEventArgs(Int64 TaskID, cGlobalParas.TaskState oldState, cGlobalParas.TaskState newState)
            {
                //m_TaskID = TaskID;
                base.TaskID = TaskID;
                m_OldState = oldState;
                m_NewState = newState;
            }

            //private Int64 m_TaskID;
            //public Int64 TaskID
            //{
            //    get { return m_TaskID; }
            //    set { m_TaskID = value; }
            //}

            private cGlobalParas.TaskState m_OldState;
            private cGlobalParas.TaskState m_NewState;
            /// <summary>
            /// 旧的状态
            /// </summary>
            public cGlobalParas.TaskState OldState
            {
                get { return m_OldState; }
                set { m_OldState = value; }
            }
            /// <summary>
            /// 新的状态
            /// </summary>
            public cGlobalParas.TaskState NewState
            {
                get { return m_NewState; }
                set { m_NewState = value; }
            }
        }

        //任务事件
        public class cTaskEventArgs : EventArgs
        {

            public cTaskEventArgs()
            {

            }

            /// <param name="cancel">是否取消事件</param>
            public cTaskEventArgs(Int64 TaskID,string TaskName, bool cancel)
            {
                m_TaskID = TaskID;
                m_TaskName = TaskName;
                m_Cancel = cancel;
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

        //采集结果数据 事件 
        //是针对每网址采集数据完成触发
        public class cGatherDataEventArgs : cTaskEventArgs
        {
            public cGatherDataEventArgs(Int64 TaskID, DataTable cData)
            {
                //m_TaskID = TaskID;
                base.TaskID = TaskID;
                m_gData = cData;
            }

            private DataTable m_gData;
            public DataTable gData
            {
                get { return m_gData; }
                set { m_gData = value; }
            }

            //private Int64 m_TaskID;
            //public Int64 TaskID
            //{
            //    get { return m_TaskID; }
            //    set { m_TaskID = value; }
            //}
        }

        //采集日志事件
        public class cGatherTaskLogArgs : cTaskEventArgs
        {
            public cGatherTaskLogArgs(Int64 TaskID,  string strLog ,bool IsSaveError)
            {
                //m_TaskID = TaskID;
                base.TaskID = TaskID;
                base.TaskName = TaskName;
                m_strLog = strLog;
                m_IsSaveErrorLog = IsSaveError;
            }

            private string m_strLog;
            public string strLog
            {
                get { return m_strLog; }
                set { m_strLog = value; }
            }

            private bool m_IsSaveErrorLog;
            public bool IsSaveErrorLog
            {
                get { return m_IsSaveErrorLog; }
                set { m_IsSaveErrorLog = value; }
            }

            //private Int64 m_TaskID;
            //public Int64 TaskID
            //{
            //    get { return m_TaskID; }
            //    set { m_TaskID = value; }
            //}
        }

        //采集完成数量变更事件
        public class cGatherUrlCountArgs : cTaskEventArgs
        {
            public cGatherUrlCountArgs(Int64 TaskID, cGlobalParas.UpdateUrlCountType uType, int TrueUrlCount)
            {
                //m_TaskID = TaskID;
                base.TaskID = TaskID;
                m_TrueUrlCount = TrueUrlCount;
                m_UType = uType;
            }

            private cGlobalParas.UpdateUrlCountType m_UType;
            public cGlobalParas.UpdateUrlCountType UType
            {
                get { return m_UType; }
                set { m_UType = value; }
            }

            //private Int64 m_TaskID;
            //public Int64 TaskID
            //{
            //    get { return m_TaskID; }
            //    set { m_TaskID = value; }
            //}

            private int m_TrueUrlCount;
            public int TrueUrlCount
            {
                get { return m_TrueUrlCount; }
                set { m_TrueUrlCount = value; }
            }
        }

        #endregion
    
}
