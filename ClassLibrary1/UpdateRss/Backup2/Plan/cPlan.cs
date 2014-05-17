using System;
using System.Collections.Generic;
using System.Text;

///功能：计划类
///完成时间：2009-8-21
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：实体类 
///版本：01.10.00
///修订：无
namespace SoukeyNetget.Plan
{
    class cPlan
    {
        #region 类的构造和销毁
        public cPlan()
        {
            m_RunTasks=new List<cTaskPlan> ();
        }

        ~cPlan()
        {
        }
        #endregion

        #region 计划属性
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

        //是否过期
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

        //记录每周有那几天运行，记录格式为字符串，数字代表星期几
        //1,3,4 表示周一 周三和周四运行，为空则表示不运行；注意：0-表示星期日
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

        //此任务下次运行的时间，此内容动态计算，不进行保存。
        private string m_NextRunTime;
        public string NextRunTime
        {
            get 
            {
                return GetNextRunTime();
            }
            
        }

        //已经运行的次数
        private string m_RunTimeCount;
        public string RunTimeCount
        {
            get { return m_RunTimeCount; }
            set { m_RunTimeCount = value; }
        }

        //最后一次运行时间
        private string m_LastRunTime;
        public string LastRunTime
        {
            get { return m_LastRunTime; }
            set { m_LastRunTime = value; }
        }

        //动态计算下次运行的时间
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
                        //超过了当天的下午运行时间
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
                            //表示根据运行时间和间隔递减，但递减符合条件后，还需增加一个间隔，因为时间是向后运行
                            //递减符合条件的时间是已经过去的时间
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
        /// 此属性主要是用来控制任务自动执行，系统检测时会根据此时间进行判断，第一次肯定为空，如果为空
        /// 则去除下一次运行的时间，然后再次检测得到可执行的任务，当任务压入执行列表，则调自动修改此属性
        /// 为下次运行的时间
        /// 不用下次运行时间作为判断是因为：运行时间精确到秒，所以一旦过了一秒，下次运行时间就会发生变化（动态维护）
        /// 为检测间隔为半分钟，所以肯定会出问题。
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
