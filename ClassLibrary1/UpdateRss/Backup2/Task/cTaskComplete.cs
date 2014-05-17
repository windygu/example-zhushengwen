using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

///功能：采集任务完成索引文件管理
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace SoukeyNetget.Task
{
    class cTaskComplete
    {

        cXmlIO xmlConfig;
        DataView Tasks;

        public cTaskComplete()
        {
        }

        ~cTaskComplete()
        {
        }

      
        //返回的是已经完成的任务的集合信息
        public void LoadTaskData()
        {
            try
            {
                xmlConfig = new cXmlIO(Program.getPrjPath() + "Data\\index.xml");
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            //获取TaskClass节点
            Tasks = xmlConfig.GetData("Tasks");
        }

        //调用加载任务信息后,调用此方法返回一个任务的dataview
        public DataView GetTasks()
        {
            return Tasks;
        }

        //根据完成任务ID加载任务信息
        public void LoadSingleTask(Int64  TaskID)
        {
            try
            {
                xmlConfig = new cXmlIO(Program.getPrjPath() + "Data\\index.xml");
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            //获取TaskClass节点,尽管返回的也是DataView,但其中只包含一条记录
            //这样做是为了更好的兼容访问操作
            Tasks = xmlConfig.GetData("Tasks","TaskID",TaskID.ToString () );
        }


        //计算当前共有多少个任务已经完成
        public int GetTaskCount()
        {
            int tCount;

            if (Tasks == null)
            {
                tCount = 0;
            }
            else
            {
                tCount = Tasks.Count;
            }
            return tCount;
        }

        public Int64 GetTaskID(int index)
        {

            Int64 tid = 0;
            try
            {
                tid = Int64.Parse(Tasks[index].Row["TaskID"].ToString());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return tid;
        }

        public string GetTaskName(int index)
        {
            string TName = Tasks[index].Row["TaskName"].ToString();
            return TName;
        }

        public cGlobalParas.TaskType GetTaskType(int index)
        {
            cGlobalParas.TaskType tType = (cGlobalParas.TaskType)int.Parse(Tasks[index].Row["TaskType"].ToString());
            return tType;
        }

        public cGlobalParas.TaskRunType GetTaskRunType(int index)
        {
            cGlobalParas.TaskRunType TRunType =(cGlobalParas.TaskRunType)int.Parse (Tasks[index].Row["RunType"].ToString());
            return TRunType;
        }

        public string GetTempFile(int index)
        {
            string tempData = Tasks[index].Row["tempFile"].ToString();
            return tempData;
        }

        public string GetExportFile(int index)
        {
            string ExportFile = Tasks[index].Row["ExportFile"].ToString();
            return ExportFile;
        }

        public bool GetIsLogin(int index)
        {
            bool Isl = (Tasks[index].Row["IsLogin"].ToString() == "True" ? true : false);
            return Isl;
        }

        public cGlobalParas.PublishType GetPublishType(int index)
        {
            cGlobalParas.PublishType pType = (cGlobalParas.PublishType)int.Parse(Tasks[index].Row["PublishType"].ToString());
            return pType;
        }

        //返回此任务需要采集的网页地址数量
        public int GetUrlCount(int index)
        {
            int WebLinkCount;
            try
            {
                WebLinkCount = int.Parse(Tasks[index].Row["UrlCount"].ToString());
            }
            catch (System.Exception ex)
            {
                if (Tasks[index].Row["UrlCount"].ToString() == "")
                {
                    WebLinkCount = 0;
                }
                else
                {
                    throw ex;
                }
            }
            return WebLinkCount;
        }

        //返回此任务已经采集网页的地址数量
        public int GetGatheredUrlCount(int index)
        {
            int WebLinkCount;
            try
            {
                WebLinkCount = int.Parse(Tasks[index].Row["GatheredUrlCount"].ToString());
            }
            catch
            {
                WebLinkCount = 0;
            }
            return WebLinkCount;
        }
        
        //共有多少个已经完成的任务
        public int GetCount()
        {
            int RunCount;

            try
            {
                RunCount = Tasks.Count;
            }
            catch
            {
                RunCount = 0;
            }
            return RunCount;
        }

        public void NewTaskCompleteFile()
        {
            xmlConfig = new cXmlIO();
            string strXml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                      "<Tasks>" +
                      "</Tasks>";
            xmlConfig.NewXmlFile(Program.getPrjPath()  + "data\\index.xml", strXml);
        }

        public Int64 InsertTaskComplete(Int64 TaskID,cGlobalParas.GatherResult tSate)
        {
            ///首先判断存放任务执行的目录是否存在
            ///此目录是固定目录，存放在系统\\data
            string cPath = Program.getPrjPath() + "data";

            if (!System.IO.Directory.Exists(cPath))
            {
                System.IO.Directory.CreateDirectory(cPath);
            }

            ///先将此任务的摘要信息加载到index.xml文件中
            Task.cTaskRun t = new Task.cTaskRun();
            t.LoadSingleTask(TaskID);

            //开始构造xml节点内容
            LoadTaskData();
            //int MaxID=GetCount () + 1;

            string txml="";
            txml = "<TaskID>" + t.GetTaskID (0) + "</TaskID>";
            txml += "<TaskName>" + t.GetTaskName(0) + "</TaskName>";
            txml += "<TaskState>" + tSate + "</TaskState>";
            txml += "<TaskType>" + (int)t.GetTaskType(0) + "</TaskType>";
            txml += "<RunType>" + (int)t.GetTaskRunType(0) + "</RunType>";
            txml += "<ExportFile>" + t.GetExportFile(0) + "</ExportFile>";
            txml += "<tempFile>" + t.GetTempFile(0) + "</tempFile>";
            txml += "<UrlCount>" + t.GetUrlCount(0) + "</UrlCount>";
            txml += "<GatheredUrlCount>" + t.GetGatheredUrlCount(0) + "</GatheredUrlCount>";
            txml += "<IsLogin>" + t.GetIsLogin(0) + "</IsLogin>";
            txml += "<PublishType>" + (int)t.GetPublishType(0) + "</PublishType>";

            xmlConfig.InsertElement("Tasks", "Task", txml);
            xmlConfig.Save ();

            return TaskID;

        }

        public void DelTask(Int64  TaskID)
        {
            xmlConfig.DeleteChildNodes("Tasks", "TaskID", TaskID.ToString ());
            xmlConfig.Save();
        }
    }
}
