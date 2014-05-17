using System;
using System.Collections.Generic;
using System.Text;

///���ܣ��ƻ���
///���ʱ�䣺2009-8-21
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵����ʵ���� 
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget.Plan
{
    class cPlan
    {
        #region ��Ĺ��������
        public cPlan()
        {
            m_RunTasks=new List<cTaskPlan> ();
        }

        ~cPlan()
        {
        }
        #endregion

        #region �ƻ�����
        private Int64 m_PlanID;
        public Int64 PlanID
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

        private int m_PlanState;
        public int PlanState
        {
            get { return m_PlanState; }
            set { m_PlanState = value; }
        }

        private string m_PlanRemark;
        public string PlanRemark
        {
            get { return m_PlanRemark; }
            set { m_PlanRemark = value; }
        }

        private bool m_IsOverRun;
        public bool IsOverRun
        {
            get { return m_IsOverRun; }
            set { m_IsOverRun = value; }
        }

        //�Ƿ����
        private bool m_IsDisabled;
        public bool IsDisabled
        {
            get { return m_IsDisabled; }
            set { m_IsDisabled = value; }
        }

        private int m_DisabledType;
        public int DisabledType
        {
            get { return m_DisabledType; }
            set { m_DisabledType = value; }
        }

        private int m_DisabledTime;
        public int DisabledTime
        {
            get { return m_DisabledTime; }
            set { m_DisabledTime = value; }
        }

        private DateTime m_DisabledDateTime;
        public DateTime DisabledDateTime
        {
            get { return m_DisabledDateTime; }
            set { m_DisabledDateTime = value; }
        }

        private List<cTaskPlan> m_RunTasks;
        public List<cTaskPlan> RunTasks
        {
            get { return m_RunTasks; }
            set { m_RunTasks = value; }
        }

        private int m_RunTaskPlanType;
        public int RunTaskPlanType
        {
            get { return m_RunTaskPlanType; }
            set { m_RunTaskPlanType = value; }
        }

        private string m_EnabledDateTime;
        public string EnabledDateTime
        {
            get { return m_EnabledDateTime; }
            set { m_EnabledDateTime = value; }
        }

        private string m_RunOnesTime;
        public string RunOnesTime
        {
            get { return m_RunOnesTime; }
            set { m_RunOnesTime = value; }
        }

        private string m_RunDayTime;
        public string RunDayTime
        {
            get { return m_RunDayTime; }
            set { m_RunDayTime = value; }
        }

        private string m_RunAMTime;
        public string RunAMTime
        {
            get { return m_RunAMTime; }
            set { m_RunAMTime = value; }
        }

        private string m_RunPMTime;
        public string RunPMTime
        {
            get { return m_RunPMTime; }
            set { m_RunPMTime = value; }
        }

        private string m_RunWeeklyTime;
        public string RunWeeklyTime
        {
            get { return m_RunWeeklyTime; }
            set { m_RunWeeklyTime = value; }
        }

        //��¼ÿ�����Ǽ������У���¼��ʽΪ�ַ��������ִ������ڼ�
        //1,3,4 ��ʾ��һ �������������У�Ϊ�����ʾ�����У�ע�⣺0-��ʾ������
        private string m_RunWeekly;
        public string RunWeekly
        {
            get { return m_RunWeekly; }
            set { m_RunWeekly = value; }
        }

        private string m_FirstRunTime;
        public string FirstRunTime
        {
            get { return m_FirstRunTime; }
            set { m_FirstRunTime = value; }
        }

        private string m_RunInterval;
        public string RunInterval
        {
            get { return m_RunInterval; }
            set { m_RunInterval = value; }
        }

        //�������´����е�ʱ�䣬�����ݶ�̬���㣬�����б��档
        private string m_NextRunTime;
        public string NextRunTime
        {
            get 
            {
                return GetNextRunTime();
            }
            
        }

        //�Ѿ����еĴ���
        private string m_RunTimeCount;
        public string RunTimeCount
        {
            get { return m_RunTimeCount; }
            set { m_RunTimeCount = value; }
        }

        //���һ������ʱ��
        private string m_LastRunTime;
        public string LastRunTime
        {
            get { return m_LastRunTime; }
            set { m_LastRunTime = value; }
        }

        //��̬�����´����е�ʱ��
        private string GetNextRunTime()
        {
            
            switch (this.RunTaskPlanType)
            {
                case (int)cGlobalParas.RunTaskPlanType.Ones :
                    return this.RunOnesTime;
                    
                case (int)cGlobalParas.RunTaskPlanType.DayOnes :
                    if (DateTime.Compare(DateTime.Parse(DateTime.Now.ToShortTimeString()), DateTime.Parse(this.RunOnesTime)) > 0)
                        return DateTime.Now.ToLongDateString() + this.RunDayTime;
                    else
                        return DateTime.Now.AddDays(1).ToLongDateString() + this.RunDayTime;
                   
                case (int)cGlobalParas.RunTaskPlanType.DayTwice :
                    if (DateTime.Compare(DateTime.Parse(DateTime.Now.ToShortTimeString()), DateTime.Parse(this.RunPMTime)) < 0)
                        //�����˵������������ʱ��
                        return DateTime.Now.AddDays(1).ToLongDateString() + this.RunAMTime;
                    else if ((DateTime.Compare(DateTime.Parse(DateTime.Now.ToShortTimeString()), DateTime.Parse(this.RunAMTime)) > 0))
                        return DateTime.Now.ToLongDateString() + this.RunAMTime;
                    else
                        return DateTime.Now.ToLongDateString() + this.RunPMTime;
                    
                case (int)cGlobalParas.RunTaskPlanType.Weekly :
                    int CurWeek = (int)(DateTime.Now.DayOfWeek);

                    if (this.RunWeekly.IndexOf(CurWeek.ToString ()) >=0)
                    {
                        if (DateTime.Compare(DateTime.Parse(DateTime.Now.ToShortTimeString()), DateTime.Parse(this.RunWeeklyTime)) > 0)
                            return DateTime.Now.ToLongDateString() + this.RunWeeklyTime;
                        else
                            return GetWeekNextTime(CurWeek,this.RunWeekly , this.RunWeeklyTime);
                    }
                    else
                    {
                        return GetWeekNextTime(CurWeek, this.RunWeekly, this.RunWeeklyTime);
                    }


                case (int)cGlobalParas.RunTaskPlanType.Custom :
                    string FirstDateTime = this.EnabledDateTime + " " + this.FirstRunTime;

                    if (DateTime.Compare(DateTime.Parse(DateTime.Now.ToShortTimeString()), DateTime.Parse(FirstDateTime)) < 0)
                        //
                        return FirstDateTime;
                    else
                    {
                        string NextTime=DateTime.Now.ToLongDateString () + " " + this.FirstRunTime ;

                        if (DateTime.Compare(DateTime.Now, DateTime.Parse(NextTime)) > 0)
                        {
                             while (DateTime.Compare(DateTime.Now, DateTime.Parse(NextTime)) > 0)
                            {

                                NextTime = ((DateTime.Parse(NextTime)).AddHours(double.Parse(this.RunInterval))).ToString();
                            }
                        }
                        else
                        {
                            //��ʾ��������ʱ��ͼ���ݼ������ݼ����������󣬻�������һ���������Ϊʱ�����������
                            //�ݼ�����������ʱ�����Ѿ���ȥ��ʱ��
                           while (DateTime.Compare(DateTime.Now, DateTime.Parse(NextTime)) < 0)
                            {

                                NextTime = ((DateTime.Parse(NextTime)).AddHours(-double.Parse(this.RunInterval))).ToString();
                            }

                            NextTime = ((DateTime.Parse(NextTime)).AddHours(double.Parse(this.RunInterval))).ToString();
                        }

                        return  NextTime ;
                    }


                default :
                    break;
            }

            return "";
        }

        private string GetWeekNextTime(int CurWeek,string Weekly, string WeeklyTime)
        {
            for (int i=0;i<7;i++)
            {
                if (CurWeek == 7)
                    CurWeek = 0;

                if (Weekly.IndexOf(CurWeek.ToString ())>=0)
                    return DateTime.Now.AddDays(i).ToLongDateString() + WeeklyTime;

                CurWeek++;
            }

            return "";
        }

        /// <summary>
        /// ��������Ҫ���������������Զ�ִ�У�ϵͳ���ʱ����ݴ�ʱ������жϣ���һ�ο϶�Ϊ�գ����Ϊ��
        /// ��ȥ����һ�����е�ʱ�䣬Ȼ���ٴμ��õ���ִ�е����񣬵�����ѹ��ִ���б�����Զ��޸Ĵ�����
        /// Ϊ�´����е�ʱ��
        /// �����´�����ʱ����Ϊ�ж�����Ϊ������ʱ�侫ȷ���룬����һ������һ�룬�´�����ʱ��ͻᷢ���仯����̬ά����
        /// Ϊ�����Ϊ����ӣ����Կ϶�������⡣
        /// </summary>
        private string m_PlanRunTime;
        public string PlanRunTime
        {
            get { return m_PlanRunTime; }
            set { m_PlanRunTime = value; }
        }

        #endregion



    }
}
