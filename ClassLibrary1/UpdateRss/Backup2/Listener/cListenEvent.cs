using System;
using System.Collections.Generic;
using System.Text;
using SoukeyNetget.Plan ;

namespace SoukeyNetget.Listener
{
    //�����ʼ���¼�
    public class cListenInitializedEventArgs : EventArgs
    {
        public cListenInitializedEventArgs()
        {
        }

        public cListenInitializedEventArgs(cGlobalParas.MessageType MessType)
        {
            m_MessType = MessType;
        }

        private cGlobalParas.MessageType m_MessType;
        public cGlobalParas.MessageType MessType
        {
            get { return m_MessType; }
            set { m_MessType = value; }
        }

    }

    public class cCommandEventArgs : cListenInitializedEventArgs
    {
        public cCommandEventArgs(cGlobalParas.MessageType MessType)
        {
            base.MessType = MessType;
        }
    }

    //�������񴥷��¼�
    public class cRunTaskEventArgs : cListenInitializedEventArgs
    {
    

        public cRunTaskEventArgs(cGlobalParas.MessageType MessType ,string RunName ,string RunPara)
        {
            base.MessType = MessType;
            m_RunName = RunName;
            m_RunPara = RunPara;
        }

        private string m_RunName;
        public string RunName
        {
            get { return m_RunName; }
            set { m_RunName = value; }
        }

        private string m_RunPara;
        public string RunPara
        {
            get { return m_RunPara; }
            set { m_RunPara = value; }
        }
    }

    //�����������������¼�
    public class cAddRunTaskEventArgs : EventArgs
    {
        public cAddRunTaskEventArgs()
        {
        }

        public cAddRunTaskEventArgs(cTaskPlan RTask)
        {
            m_RunTask = RTask;
        }

        private cTaskPlan m_RunTask;
        public cTaskPlan RunTask
        {
            get { return m_RunTask; }
            set { m_RunTask = value; }
        }
    }

    //����ʧ���¼�
    public class cListenErrorEventArgs : EventArgs
    {
        public cListenErrorEventArgs()
        {
        }

        public cListenErrorEventArgs(string Mess)
        {
            m_Message = Mess;
        }

        private string m_Message;
        public string Message
        {
            get { return m_Message; }
            set { m_Message = value; }
        }

    }

}
