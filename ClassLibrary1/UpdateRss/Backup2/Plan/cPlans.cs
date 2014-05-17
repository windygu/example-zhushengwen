using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

///���ܣ��ƻ���
///���ʱ�䣺2009-8-21
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵�������мƻ����񶼴洢��һ���ļ��У�ϵͳ��·��\tasks\plan ����������Ŀ����Ϊ�˿��Ը��õ�
///�����ƻ���ִ�������ϵͳ������ʱ�����Զ����ش��ļ������м�����
///
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget.Plan
{
    class cPlans
    {
        cXmlIO xmlPlan;
        List<cPlan> m_Plans;
        

        #region ���������
        public cPlans()
        {
            m_Plans = new List<cPlan>();
        }

        ~cPlans()
        {
        }
        #endregion

        public List<cPlan> Plans
        {
            get { return m_Plans; }
            set { m_Plans = value; }
        }

        #region 
        public void NewIndexFile()
        {
            cXmlIO xmlConfig = new cXmlIO();

            string strXml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                       "<Plans>" +
                       "</Plans>";
            xmlConfig.NewXmlFile(Program.getPrjPath() + "tasks\\plan\\plan.xml", strXml);

            xmlConfig = null;

        }

        private bool IsExist()
        {
            return File.Exists(Program.getPrjPath() + "tasks\\plan\\plan.xml");
        }

        #endregion

        #region ����

        //���ؼƻ�
        //ֻ���ؼƻ���ժҪ��Ϣ����ֻ����ļ��м�����Ҫ�б���ʾ��
        //�ƻ����ݣ����������ļ��ؼƻ���Ϣ
        public void LoadPlans()
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
                p.PlanID =Int64.Parse ( d[i].Row["ID"].ToString());
                p.PlanName = d[i].Row["PlanName"].ToString();
                p.PlanState =int.Parse (d[i].Row["PlanState"].ToString());
                p.IsOverRun = d[i].Row["IsOverRun"].ToString() == "True" ? true : false;
                p.IsDisabled = d[i].Row["IsDisabled"].ToString() == "True" ? true : false;
                p.DisabledType =int.Parse ( d[i].Row["DisabledType"].ToString());
                p.DisabledTime = int.Parse ( d[i].Row["DisabledTime"].ToString());
                p.DisabledDateTime = DateTime.Parse ( d[i].Row["DisabledDateTime"].ToString());
                p.RunTaskPlanType = int.Parse ( d[i].Row["RunTaskPlanType"].ToString());
                p.EnabledDateTime = d[i].Row["EnabledDateTime"].ToString();
                p.RunOnesTime = d[i].Row["RunOnesTime"].ToString();
                p.RunDayTime =d[i].Row["RunDayTime"].ToString();
                p.RunAMTime = d[i].Row["RunAMTime"].ToString();
                p.RunPMTime =d[i].Row["RunPMTime"].ToString();
                p.RunWeeklyTime = d[i].Row["RunWeeklyTime"].ToString();
                p.RunWeekly = d[i].Row["RunWeekly"].ToString();
                p.RunTimeCount = d[i].Row["RunTimeCount"].ToString();
                p.FirstRunTime = d[i].Row["FirstRunTime"].ToString();
                p.RunInterval = d[i].Row["RunInterval"].ToString();

                m_Plans.Add(p);

            }

            p = null;

        }

        //��ȡһ��ִ�еļƻ�
        public cPlan GetSinglePlan(Int64 PlanID)
        {
            //�жϼƻ��ļ��Ƿ���ڣ�������������½�
            if (!IsExist())
                NewIndexFile();

            //�жϼƻ��Ƿ������������������Ҫ���������޸�
            cXmlIO xmlConfig = new cXmlIO(Program.getPrjPath() + "tasks\\plan\\plan.xml");
            DataView d= xmlConfig.GetData("Plans", "ID", PlanID.ToString ());
            DataView t = d[0].CreateChildView("Plan_Tasks")[0].CreateChildView("Tasks_Task");
            cPlan p = new cPlan();

            p.PlanID = Int64.Parse(d[0].Row["ID"].ToString());
            p.PlanName = d[0].Row["PlanName"].ToString();
            p.PlanRemark = d[0].Row["PlanRemark"].ToString();
            p.PlanState = int.Parse(d[0].Row["PlanState"].ToString());
            p.IsOverRun = d[0].Row["IsOverRun"].ToString() == "True" ? true : false;
            p.IsDisabled = d[0].Row["IsDisabled"].ToString() == "True" ? true : false;
            p.DisabledType = int.Parse(d[0].Row["DisabledType"].ToString());
            p.DisabledTime = int.Parse(d[0].Row["DisabledTime"].ToString());
            p.DisabledDateTime = DateTime.Parse(d[0].Row["DisabledDateTime"].ToString());
            p.RunTaskPlanType = int.Parse(d[0].Row["RunTaskPlanType"].ToString());
            p.EnabledDateTime = d[0].Row["EnabledDateTime"].ToString();
            p.RunOnesTime =d[0].Row["RunOnesTime"].ToString();
            p.RunDayTime = d[0].Row["RunDayTime"].ToString();
            p.RunAMTime = d[0].Row["RunAMTime"].ToString();
            p.RunPMTime = d[0].Row["RunPMTime"].ToString();
            p.RunWeeklyTime = d[0].Row["RunWeeklyTime"].ToString();
            p.RunWeekly = d[0].Row["RunWeekly"].ToString();
            p.RunTimeCount = d[0].Row["RunTimeCount"].ToString();
            p.FirstRunTime = d[0].Row["FirstRunTime"].ToString();
            p.RunInterval = d[0].Row["RunInterval"].ToString();


            cTaskPlan tp;
            for (int i = 0; i < t.Count; i++)
            {
                tp = new cTaskPlan();
                tp.RunTaskType =int.Parse ( t[i].Row["RunTaskType"].ToString());
                tp.RunTaskName = t[i].Row["RunTaskName"].ToString();
                tp.RunTaskPara = t[i].Row["RunTaskPara"].ToString();
                p.RunTasks.Add(tp);
            }

            xmlConfig = null;
            return p;

            

        }

        //public int GetPlanCount()
        //{
        //    return 0;
        //}

        //����һ���ƻ�,�ƻ���������
        public void InsertPlan(cPlan NewPlan)
        {
            //�жϼƻ��ļ��Ƿ���ڣ�������������½�
            if (!IsExist())
                NewIndexFile();

            //�жϼƻ��Ƿ������������������Ҫ���������޸�
            cXmlIO xmlConfig = new cXmlIO(Program.getPrjPath() + "tasks\\plan\\plan.xml");

            DataView d = xmlConfig.GetData("descendant::Plans");

            if (d != null)
            {
                for (int i = 0; i < d.Count; i++)
                {
                    if (NewPlan.PlanName == d[i].Row["PlanName"].ToString())
                    {
                        xmlConfig = null;
                        throw new cSoukeyException("�Ѿ����ڴ˼ƻ������޸ļƻ����ƣ��ƻ����Ʋ����ظ���");
                    }
                }
            }

            string pXml = CreatePlanXml(NewPlan);
               

            xmlConfig.InsertElement("Plans", "Plan", pXml);
            xmlConfig.Save();
            xmlConfig = null;
           
        }

        public void EditPlan(cPlan ePlan)
        {
            //�жϼƻ��Ƿ������������������Ҫ���������޸�
            cXmlIO xmlConfig = new cXmlIO(Program.getPrjPath() + "tasks\\plan\\plan.xml");

            //ɾ���ƻ��ڵ�
            xmlConfig.DeleteChildNodes("Plans", "ID", ePlan.PlanID.ToString ());

            string pXml = CreatePlanXml(ePlan);

            xmlConfig.InsertElement("Plans", "Plan", pXml);
            xmlConfig.Save();
            xmlConfig = null;
        }

        private string CreatePlanXml(cPlan NewPlan)
        {
            //����xml�ļ�
            string pXml = "";

            pXml = "<ID>" + NewPlan.PlanID + "</ID>" +
                "<PlanName>" + NewPlan.PlanName + "</PlanName>" +
                "<PlanState>" + NewPlan.PlanState + "</PlanState>" +
                "<PlanRemark>" + NewPlan.PlanRemark + "</PlanRemark>" +
                "<IsOverRun>" + NewPlan.IsOverRun + "</IsOverRun>" +
                "<IsDisabled>" + NewPlan.IsDisabled + "</IsDisabled>" +
                "<DisabledType>" + NewPlan.DisabledType + "</DisabledType>" +
                "<DisabledTime>" + NewPlan.DisabledTime + "</DisabledTime>" +
                "<DisabledDateTime>" + NewPlan.DisabledDateTime + "</DisabledDateTime>" +
                "<RunTaskPlanType>" + NewPlan.RunTaskPlanType + "</RunTaskPlanType>" +
                "<EnabledDateTime>" + NewPlan.EnabledDateTime + "</EnabledDateTime>" +
                "<RunOnesTime>" + NewPlan.RunOnesTime + "</RunOnesTime>" +
                "<RunDayTime>" + NewPlan.RunDayTime + "</RunDayTime>" +
                "<RunAMTime>" + NewPlan.RunAMTime + "</RunAMTime>" +
                "<RunPMTime>" + NewPlan.RunPMTime + "</RunPMTime>" +
                "<RunWeeklyTime>" + NewPlan.RunWeeklyTime + "</RunWeeklyTime>" +
                "<RunWeekly>" + NewPlan.RunWeekly + "</RunWeekly>" +
                "<FirstRunTime>" + NewPlan.FirstRunTime + "</FirstRunTime>" +
                "<RunInterval>" + NewPlan .RunInterval + "</RunInterval>" +

                //�������д�����ֻҪ��������޸ģ���ʾ����һ�����������ֵ�޸�Ϊ��
                "<RunTimeCount>0</RunTimeCount>" +                            
                "<Tasks>";

            for (int i = 0; i < NewPlan.RunTasks.Count; i++)
            {
                pXml += "<Task>";
                pXml += "<RunTaskType>" + NewPlan.RunTasks[i].RunTaskType + "</RunTaskType>";
                pXml += "<RunTaskName>" + NewPlan.RunTasks[i].RunTaskName + "</RunTaskName>";
                pXml += "<RunTaskPara>" + NewPlan.RunTasks[i].RunTaskPara + "</RunTaskPara>";
                pXml += "</Task>";
            }

            pXml += "</Tasks>";

            return pXml;
        }

        public void DelPlan(string PlanID)
        {
            //�жϼƻ��Ƿ������������������Ҫ���������޸�
            cXmlIO xmlConfig = new cXmlIO(Program.getPrjPath() + "tasks\\plan\\plan.xml");

            //ɾ���ƻ��ڵ�
            xmlConfig.DeleteChildNodes("Plans", "ID", PlanID);
            xmlConfig.Save();
            xmlConfig = null;

        }

        //�ж�ָ���ļƻ��Ƿ���ڣ�����������޸ļƻ�״̬
        public void IfEnabled(Int64 PlanID,bool OnlyState)
        {
            cPlan p = GetSinglePlan(PlanID);

            if (p.IsDisabled == false )
            {
                p=null;
                return;
            }

            if (p.DisabledType == (int)cGlobalParas.PlanDisabledType.RunTime)
            {
                //�����д���ʧЧ
                if (p.DisabledTime < int.Parse(p.RunTimeCount) + 1)
                {
                    //��ʾʧЧ
                    ModifyPlanState(PlanID, cGlobalParas.PlanState.Disabled);
                }
                else if (OnlyState ==false )
                {
                    //��ʾû��ʧЧ���޸����д�������1
                    ModifyPlanRunTime(PlanID, int.Parse(p.RunTimeCount) + 1);

                }
            }
            else if (p.DisabledType == (int)cGlobalParas.PlanDisabledType.RunDateTime)
            {
                //������ʧЧ
                if (DateTime.Compare(DateTime.Now, p.DisabledDateTime) < 0)
                {
                    //�޸Ĵ�����ΪʧЧ
                    ModifyPlanState(PlanID, cGlobalParas.PlanState.Disabled);
                }
            }
        }

        private void ModifyPlanState(Int64 PlanID, cGlobalParas.PlanState pState)
        {
            cXmlIO xmlConfig = new cXmlIO(Program.getPrjPath() + "tasks\\plan\\plan.xml");

            xmlConfig.EditNodeValue("Plans", "ID", PlanID.ToString () ,"PlanState",((int)pState).ToString  ());
            xmlConfig.Save ();
            xmlConfig =null;
        }

        private void ModifyPlanRunTime(Int64 PlanID, int RunTime)
        {
            cXmlIO xmlConfig = new cXmlIO(Program.getPrjPath() + "tasks\\plan\\plan.xml");

            xmlConfig.EditNodeValue("Plans", "ID", PlanID.ToString(), "RunTimeCount", RunTime.ToString ());
            xmlConfig.Save();
            xmlConfig = null;

        }

        //�޸��ƶ��ƻ�������
        public bool RenamePlanName(string OldPlanName, string NewPlanName)
        {
            //�жϼƻ��ļ��Ƿ���ڣ�������������½�
            if (!IsExist())
                NewIndexFile();

            //�жϼƻ��Ƿ������������������Ҫ���������޸�
            cXmlIO xmlPlan = new cXmlIO(Program.getPrjPath() + "tasks\\plan\\plan.xml");

            DataView d = xmlPlan.GetData("descendant::Plans");

            if (d != null)
            {
                for (int i = 0; i < d.Count; i++)
                {
                    if (NewPlanName == d[i].Row["PlanName"].ToString())
                    {
                        xmlPlan = null;
                        throw new cSoukeyException("�Ѿ����ڴ˼ƻ���������ʧ��");
                        return false;
                    }
                }
            }

            xmlPlan.EditNodeValue("Plans", "PlanName", OldPlanName, "PlanName", NewPlanName);
            xmlPlan.Save();
            xmlPlan = null;

            return true;
        }

        #endregion
    }
}
