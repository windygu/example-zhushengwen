using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

///���ܣ��ɼ�������������ļ�����
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
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

      
        //���ص����Ѿ���ɵ�����ļ�����Ϣ
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

            //��ȡTaskClass�ڵ�
            Tasks = xmlConfig.GetData("Tasks");
        }

        //���ü���������Ϣ��,���ô˷�������һ�������dataview
        public DataView GetTasks()
        {
            return Tasks;
        }

        //�����������ID����������Ϣ
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

            //��ȡTaskClass�ڵ�,���ܷ��ص�Ҳ��DataView,������ֻ����һ����¼
            //��������Ϊ�˸��õļ��ݷ��ʲ���
            Tasks = xmlConfig.GetData("Tasks","TaskID",TaskID.ToString () );
        }


        //���㵱ǰ���ж��ٸ������Ѿ����
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

        //���ش�������Ҫ�ɼ�����ҳ��ַ����
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

        //���ش������Ѿ��ɼ���ҳ�ĵ�ַ����
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
        
        //���ж��ٸ��Ѿ���ɵ�����
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
            ///�����жϴ������ִ�е�Ŀ¼�Ƿ����
            ///��Ŀ¼�ǹ̶�Ŀ¼�������ϵͳ\\data
            string cPath = Program.getPrjPath() + "data";

            if (!System.IO.Directory.Exists(cPath))
            {
                System.IO.Directory.CreateDirectory(cPath);
            }

            ///�Ƚ��������ժҪ��Ϣ���ص�index.xml�ļ���
            Task.cTaskRun t = new Task.cTaskRun();
            t.LoadSingleTask(TaskID);

            //��ʼ����xml�ڵ�����
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
