using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data ;

///功能：计划运行日志类
///完成时间：2009-8-21
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：主要就是记录任务运行的日志
///
///版本：01.10.00
///修订：无
namespace SoukeyNetget.Plan
{
    class cPlanRunLog
    {
        private cXmlIO m_PlanFile;
        private DataView m_dataLog;

        public cPlanRunLog()
        {
            m_dataLog = new DataView();
        }

        ~cPlanRunLog()
        {

        }

        public void InsertOnceLog(cGlobalParas.LogType lType, string PlanID, string PlanName, cGlobalParas.RunTaskType rType, string FileName, string Para)
        {
            if (!IsExist())
                NewLogFile();

            cXmlIO xmlconfig = new cXmlIO(Program.getPrjPath() + "tasks\\plan\\RunLog.xml");

            string strXml = "<LogType>" + lType + "</LogType>" +
                "<PlanID>" + PlanID + "</PlanID>" +
                "<PlanName>" + PlanName + "</PlanName>" +
                "<FileName>" + FileName + "</FileName>" +
                "<FilePara>" + Para + "</FilePara>" +
                "<TaskType>" + rType + "</TaskType>" +
                "<RunTime>" + DateTime.Now.ToString() + "</RunTime>";

            xmlconfig.InsertElement("Logs", "Log", strXml);
            xmlconfig.Save();
            xmlconfig = null;

        }

        public void OpenLogFile()
        {
            if (!IsExist())
                NewLogFile();

            m_PlanFile = new cXmlIO(Program.getPrjPath() + "tasks\\plan\\RunLog.xml");
        }

        public void CloseLogFile()
        {
            m_PlanFile = null;
        }

        public void InsertLog(cGlobalParas.LogType lType,string PlanID,string PlanName, cGlobalParas.RunTaskType rType, string FileName, string Para)
        {
            

            string strXml = "<LogType>" + ((int)lType).ToString () + "</LogType>" +
                "<PlanID>" + PlanID + "</PlanID>" +
                "<PlanName>" + PlanName + "</PlanName>" +
                "<FileName>" + FileName + "</FileName>" +
                "<FilePara>" + Para + "</FilePara>" +
                "<TaskType>" + ((int)rType).ToString () + "</TaskType>" +
                "<RunTime>" + DateTime.Now.ToString() + "</RunTime>";

            m_PlanFile.InsertElement("Logs", "Log", strXml);
            m_PlanFile.Save();

        }

        public void LoadLog()
        {
            if (!IsExist())
                return ;

            cXmlIO xmlConfig = new cXmlIO(Program.getPrjPath() + "tasks\\plan\\RunLog.xml");

            m_dataLog = xmlConfig.GetData("descendant::Logs");

            xmlConfig = null;
            
        }

        public int GetLogCount()
        {
            if (m_dataLog == null)
                return 0;
            else
                return m_dataLog.Count;
        }

        public string GetLogType(int index)
        {
            if (m_dataLog == null || m_dataLog.Count == 0)
                return "";
            else
                return cGlobalParas.ConvertName (int.Parse ( m_dataLog[index].Row["LogType"].ToString()));
        }

        public string GetPlanID(int index)
        {
            if (m_dataLog == null || m_dataLog.Count == 0)
                return "";
            else
                return m_dataLog[index].Row["PlanID"].ToString();
        }

        public string GetPlanName(int index)
        {
            if (m_dataLog == null || m_dataLog.Count == 0)
                return "";
            else
                return m_dataLog[index].Row["PlanName"].ToString();
        }

        public string GetFileName(int index)
        {
            if (m_dataLog == null || m_dataLog.Count == 0)
                return "";
            else
                if (int.Parse(m_dataLog[index].Row["TaskType"].ToString()) == (int)cGlobalParas.RunTaskType.DataTask)
                    return cGlobalParas.ConvertName (int.Parse ( m_dataLog[index].Row["FileName"].ToString()));
                else
                    return m_dataLog[index].Row["FileName"].ToString();
        }

        public string GetFilePara(int index)
        {
            if (m_dataLog == null || m_dataLog.Count == 0)
                return "";
            else
                return m_dataLog[index].Row["FilePara"].ToString();
        }

        public string GetTaskType(int index)
        {
            if (m_dataLog == null || m_dataLog.Count == 0)
                return "";
            else
                return cGlobalParas.ConvertName (int.Parse(m_dataLog[index].Row["TaskType"].ToString()));
        }

        public string GetRunTime(int index)
        {
            if (m_dataLog == null || m_dataLog.Count == 0)
                return "";
            else
                return m_dataLog[index].Row["RunTime"].ToString();
        }


        //清空日志信息
        public void DelLog()
        {
            //删除文件再重新建立一个
            File.Delete(Program.getPrjPath() + "tasks\\plan\\RunLog.xml");

            NewLogFile();
        }

        private void NewLogFile()
        {
            cXmlIO xmlConfig = new cXmlIO();

            string strXml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                       "<Logs>" +
                       "</Logs>";
            xmlConfig.NewXmlFile(Program.getPrjPath() + "tasks\\plan\\RunLog.xml", strXml);

            xmlConfig = null;

        }

        private bool IsExist()
        {
            return File.Exists(Program.getPrjPath() + "tasks\\plan\\RunLog.xml");
        }

    }
}
