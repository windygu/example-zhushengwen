using System;
using System.Collections.Generic;
using System.Text;
using SoukeyNetget.Plan;
using System.Data;
using System.IO;

///功能：计划检测类，由外部调用，检测需要执行的任务 
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace SoukeyNetget.Listener
{
    class cCheckPlan
    {
        List<cPlan> m_runTasks;
        private bool m_IsReloading = false;
        private cFileMonitor m_FileMonitor;


        #region 构造和析构
        public cCheckPlan()
        {
            if (!File.Exists(Program.getPrjPath() + "tasks\\plan\\plan.xml"))
            {
                cPlans cs = new cPlans();
                cs.NewIndexFile();
                cs = null;
            }

            try
            {
                m_runTasks = new List<cPlan>();

                IniCheckPlan();

                //初始化任务文件监听类,，根据文件变化不断更新任务监控信息
                m_FileMonitor = new cFileMonitor(Program.getPrjPath() + "tasks\\plan\\plan.xml");
                m_FileMonitor.ReloadPlanFile += this.On_Reload;

                //系统默认为启动文件监听
                StartListenPlanFile();
            }
            catch (System.Exception ex)
            {
                if (e_ListenErrorEvent != null)
                {
                    e_ListenErrorEvent(this, new cListenErrorEventArgs(ex.Message));
                }
            }
        }

        ~cCheckPlan()
        {
            m_FileMonitor.Stop();
            m_FileMonitor = null;
        }
        #endregion 

        //控制文件监听
        public void StartListenPlanFile()
        {
           
            m_FileMonitor.Start();
        }

        public void StopListenPlanFile()
        {
            m_FileMonitor.Stop();
        }

        //定期由外部调用
        //检测需要执行的任务，如果需要执行，则把任务
        //压到执行的任务队列中
        public void CheckPlan()
        {
            //重新加载计划期间禁止运行计划检测操作
            if (m_IsReloading == false)
            {
                cTaskPlan tPlan;

                
 
                for (int i = 0; i < m_runTasks.Count; i++)
                {
                    //无效的计划不进行判断，计划的状态由执行时进行维护

                    if (m_runTasks[i].PlanState ==(int) cGlobalParas.PlanState.Enabled)
                    {
                        if (DateTime.Compare(DateTime.Now, DateTime.Parse(m_runTasks[i].EnabledDateTime)) < 0)
                        {
                            //表示还未到生效时间，不启用此任务的检测
                            continue;
                        }

                        if (m_runTasks[i].PlanRunTime == "" || m_runTasks[i].PlanRunTime == null)
                        {
                            m_runTasks[i].PlanRunTime = m_runTasks[i].NextRunTime;
                        }
                        else
                        {
                            double douTime = TimeSpan.Parse(DateTime.Now.Subtract(DateTime.Parse(m_runTasks[i].PlanRunTime)).ToString()).TotalSeconds;

                            //表示下次运行的时间已经到了，但还未超过5分钟，如果超过5分钟，则默认系统没有执行此任务
                            //是否再次执行由配置参数决定：IsOverRun
                            if (douTime > 0 && douTime < 300)
                            {
                                //将任务压入任务队列

                                for (int j = 0; j < m_runTasks[i].RunTasks.Count; j++)
                                {
                                    //在此需要重新初始化执行任务的数据，主要是要增加计划的ID和计划的名称
                                    tPlan = new cTaskPlan();

                                    tPlan.PlanID = m_runTasks[i].PlanID.ToString();
                                    tPlan.PlanName = m_runTasks[i].PlanName;
                                    tPlan.RunTaskType = m_runTasks[i].RunTasks[j].RunTaskType;
                                    tPlan.RunTaskName = m_runTasks[i].RunTasks[j].RunTaskName;
                                    tPlan.RunTaskPara = m_runTasks[i].RunTasks[j].RunTaskPara;

                                    e_AddRunTaskEvent(this, new cAddRunTaskEventArgs(tPlan));
                                }

                                m_runTasks[i].PlanRunTime = m_runTasks[i].NextRunTime;

                            }
                        }
                    }
                }
            }
        }

        //加载计划,加载计划的时候需要对计划的状态进行维护
        private void IniCheckPlan()
        {
            try
            {
                cXmlIO xmlConfig = new cXmlIO(Program.getPrjPath() + "tasks\\plan\\plan.xml");

                DataView d = xmlConfig.GetData("descendant::Plans");

                if (d == null)
                {
                    return;
                }

                cPlan p;

                for (int i = 0; i < d.Count; i++)
                {
                    p = new cPlan();

                    if (int.Parse(d[i].Row["PlanState"].ToString()) == (int)cGlobalParas.PlanState.Enabled)
                    {
                        p.PlanID = Int64.Parse(d[i].Row["ID"].ToString());
                        p.PlanName = d[i].Row["PlanName"].ToString();
                        p.PlanState = int.Parse(d[i].Row["PlanState"].ToString());
                        p.IsOverRun = d[i].Row["IsOverRun"].ToString() == "True" ? true : false;
                        p.IsDisabled = d[i].Row["IsDisabled"].ToString() == "True" ? true : false;
                        p.DisabledType = int.Parse(d[i].Row["DisabledType"].ToString());
                        p.DisabledTime = int.Parse(d[i].Row["DisabledTime"].ToString());
                        p.DisabledDateTime = DateTime.Parse(d[i].Row["DisabledDateTime"].ToString());
                        p.RunTaskPlanType = int.Parse(d[i].Row["RunTaskPlanType"].ToString());
                        p.EnabledDateTime = d[i].Row["EnabledDateTime"].ToString();
                        p.RunOnesTime = d[i].Row["RunOnesTime"].ToString();
                        p.RunDayTime = d[i].Row["RunDayTime"].ToString();
                        p.RunAMTime = d[i].Row["RunAMTime"].ToString();
                        p.RunPMTime = d[i].Row["RunPMTime"].ToString();
                        p.RunWeeklyTime = d[i].Row["RunWeeklyTime"].ToString();
                        p.RunWeekly = d[i].Row["RunWeekly"].ToString();
                        p.RunTimeCount = d[i].Row["RunTimeCount"].ToString();
                        p.FirstRunTime = d[i].Row["FirstRunTime"].ToString();
                        p.RunInterval = d[i].Row["RunInterval"].ToString();

                        cTaskPlan tp;
                        DataView t = d[i].CreateChildView("Plan_Tasks")[0].CreateChildView("Tasks_Task");

                        for (int j = 0; j < t.Count; j++)
                        {
                            tp = new cTaskPlan();
                            tp.RunTaskType = int.Parse(t[j].Row["RunTaskType"].ToString());
                            tp.RunTaskName = t[j].Row["RunTaskName"].ToString();
                            tp.RunTaskPara = t[j].Row["RunTaskPara"].ToString();
                            p.RunTasks.Add(tp);
                        }

                        m_runTasks.Add(p);
                    }
                }

                p = null;

                xmlConfig = null;

                //自动维护计划状态
                AutoState();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        //重新加载任务
        private void ReIniCheckPlan()
        {
            m_IsReloading = true;
            m_runTasks = null;
            m_runTasks = new List<cPlan>();
            IniCheckPlan();
            m_IsReloading = false;
        }

        private void AutoState()
        {
            cPlans cp=new cPlans ();

            for (int i = 0; i < m_runTasks.Count; i++)
            {
                if (m_runTasks[i].IsDisabled == true)
                    cp.IfEnabled(m_runTasks[i].PlanID,true);
            }

            cp = null;
        }

        private void On_Reload(object sender, cCommandEventArgs e)
        {
            switch (e.MessType)
            {
                case cGlobalParas.MessageType.ReloadPlan :
                    ReIniCheckPlan();
                    break;
                case cGlobalParas.MessageType.MonitorFileFaild :
                    m_FileMonitor.Stop();
                    m_FileMonitor.Start();
                    break;
                default :
                    break;
            }
        }

        private readonly Object m_eventLock = new Object();

        #region 事件

        /// 采集任务完成事件
        private event EventHandler<cAddRunTaskEventArgs> e_AddRunTaskEvent;
        internal event EventHandler<cAddRunTaskEventArgs> AddRunTaskEvent
        {
            add { lock (m_eventLock) { e_AddRunTaskEvent += value; } }
            remove { lock (m_eventLock) { e_AddRunTaskEvent -= value; } }
        }

        private event EventHandler<cListenErrorEventArgs> e_ListenErrorEvent;
        internal event EventHandler<cListenErrorEventArgs> ListenErrorEvent
        {
            add { lock (m_eventLock) { e_ListenErrorEvent += value; } }
            remove { lock (m_eventLock) { e_ListenErrorEvent -= value; } }
        }

        #endregion
    
    }
}
