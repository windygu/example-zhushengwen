using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

///���ܣ� ����ɼ���������¼�
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget.Gather
{

        #region ����ɼ�������ص��¼�

        //�����ʼ���¼�
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

        //�����̴߳�����Ӧ�¼�
        public class TaskThreadErrorEventArgs : EventArgs
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="error">������쳣</param>
            public TaskThreadErrorEventArgs(Exception error)
            {
                m_Error = error;
            }

            private Exception m_Error;
            /// <summary>
            /// ������Ϣ
            /// </summary>
            public Exception Error
            {
                get { return m_Error; }
                set { m_Error = value; }
            }
        }

        //���������Ӧ�¼�
        public class TaskErrorEventArgs : EventArgs
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="dtc">��������ķֿ�</param>
            /// <param name="error">������쳣</param>
            public TaskErrorEventArgs(cGatherTaskSplit dtc, Exception error)
            {
                m_Error = error;
                m_ErrorThread = dtc;
            }

            private Exception m_Error;
            private cGatherTaskSplit m_ErrorThread;
            /// <summary>
            /// ������Ϣ
            /// </summary>
            public Exception Error
            {
                get { return m_Error; }
                set { m_Error = value; }
            }
            /// <summary>
            /// ��������ķֿ�
            /// </summary>
            public cGatherTaskSplit ErrorThread
            {
                get { return m_ErrorThread; }
                set { m_ErrorThread = value; }
            }
        }

        //�ɼ�����״̬�ı��¼�
        public class TaskStateChangedEventArgs : cTaskEventArgs
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="old_state">�ɵ�״̬</param>
            /// <param name="new_statue">�µ�״̬</param>
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
            /// �ɵ�״̬
            /// </summary>
            public cGlobalParas.TaskState OldState
            {
                get { return m_OldState; }
                set { m_OldState = value; }
            }
            /// <summary>
            /// �µ�״̬
            /// </summary>
            public cGlobalParas.TaskState NewState
            {
                get { return m_NewState; }
                set { m_NewState = value; }
            }
        }

        //�����¼�
        public class cTaskEventArgs : EventArgs
        {

            public cTaskEventArgs()
            {

            }

            /// <param name="cancel">�Ƿ�ȡ���¼�</param>
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
            /// �Ƿ�ȡ���¼�
            /// </summary>
            public bool Cancel
            {
                get { return m_Cancel; }
                set { m_Cancel = value; }
            }
        }

        //�ɼ�������� �¼� 
        //�����ÿ��ַ�ɼ�������ɴ���
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

        //�ɼ���־�¼�
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

        //�ɼ������������¼�
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
