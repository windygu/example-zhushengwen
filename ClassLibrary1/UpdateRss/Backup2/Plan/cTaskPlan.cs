using System;
using System.Collections.Generic;
using System.Text;

///���ܣ��������� ����������Ϣ��
///���ʱ�䣺2009-8-21
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget.Plan
{
    public class cTaskPlan
    {
    
        public cTaskPlan()
        {
            
        }

        ~cTaskPlan()
        {
        }

        #region ���������Ը���¼��־ʹ�ã������������ʹ��
        private string m_PlanID;
        public string PlanID
        {
            get { return m_PlanID; }
            set { m_PlanID = value; }
        }

        private string m_PlanName;
        public string PlanName
        {
            get { return m_PlanName; }
            set { m_PlanName = value; }
        }

        #endregion

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
