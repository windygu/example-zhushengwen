using System;
using System.Collections.Generic;
using System.Text;
using SoukeyNetget.Plan;
using System.Data;
using System.IO;

///���ܣ��ƻ�����࣬���ⲿ���ã������Ҫִ�е����� 
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget.Listener
{
    class cCheckPlan
    {
        List<cPlan> m_runTasks;
        private bool m_IsReloading = false;
        private cFileMonitor m_FileMonitor;


        #region ���������
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

                //��ʼ�������ļ�������,�������ļ��仯���ϸ�����������Ϣ
                m_FileMonitor = new cFileMonitor(Program.getPrjPath() + "tasks\\plan\\plan.xml");
                m_FileMonitor.ReloadPlanFile += this.On_Reload;

                //ϵͳĬ��Ϊ�����ļ�����
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

        //�����ļ�����
        public void StartListenPlanFile()
        {
           
            m_FileMonitor.Start();
        }

        public void StopListenPlanFile()
        {
            m_FileMonitor.Stop();
        }

        //�������ⲿ����
        //�����Ҫִ�е����������Ҫִ�У��������
        //ѹ��ִ�е����������
        public void CheckPlan()
        {
            //���¼��ؼƻ��ڼ��ֹ���мƻ�������
            if (m_IsReloading == false)
            {
                cTaskPlan tPlan;

                
 
                for (int i = 0; i < m_runTasks.Count; i++)
                {
                    //��Ч�ļƻ��������жϣ��ƻ���״̬��ִ��ʱ����ά��

                    if (m_runTasks[i].PlanState ==(int) cGlobalParas.PlanState.Enabled)
                    {
                        if (DateTime.Compare(DateTime.Now, DateTime.Parse(m_runTasks[i].EnabledDateTime)) < 0)
                        {
                            //��ʾ��δ����Чʱ�䣬�����ô�����ļ��
                            continue;
                        }

                        if (m_runTasks[i].PlanRunTime == "" || m_runTasks[i].PlanRunTime == null)
                        {
                            m_runTasks[i].PlanRunTime = m_runTasks[i].NextRunTime;
                        }
                        else
                        {
                            double douTime = TimeSpan.Parse(DateTime.Now.Subtract(DateTime.Parse(m_runTasks[i].PlanRunTime)).ToString()).TotalSeconds;

                            //��ʾ�´����е�ʱ���Ѿ����ˣ�����δ����5���ӣ��������5���ӣ���Ĭ��ϵͳû��ִ�д�����
                            //�Ƿ��ٴ�ִ�������ò���������IsOverRun
                            if (douTime > 0 && douTime < 300)
                            {
                                //������ѹ���������

                                for (int j = 0; j < m_runTasks[i].RunTasks.Count; j++)
                                {
                                    //�ڴ���Ҫ���³�ʼ��ִ����������ݣ���Ҫ��Ҫ���Ӽƻ���ID�ͼƻ�������
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

        //���ؼƻ�,���ؼƻ���ʱ����Ҫ�Լƻ���״̬����ά��
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

                //�Զ�ά���ƻ�״̬
                AutoState();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        //���¼�������
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

        #region �¼�

        /// �ɼ���������¼�
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
