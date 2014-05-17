using System;
using System.Collections.Generic;
using System.Text;

namespace SoukeyNetget.Task
{
    public class cTriggerTask
    {
        public cTriggerTask()
        {
        }

        ~cTriggerTask()
        {
        }

        private int m_RunTaskType;
        public int RunTaskType
        {
            get { return m_RunTaskType; }
            set { m_RunTaskType = value; }
        }

        private string m_RunTaskName;
        public string RunTaskName
        {
            get { return m_RunTaskName; }
            set { m_RunTaskName = value; }
        }

        private string m_RunTaskPara;
        public string RunTaskPara
        {
            get { return m_RunTaskPara; }
            set { m_RunTaskPara = value; }
        }
    }
}
