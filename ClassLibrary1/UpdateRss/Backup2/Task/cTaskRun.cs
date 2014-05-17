using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

///���ܣ��������������ļ�����
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget.Task
{

    ///��Ӧ�ó����Tasks�£����TaskRunn.xml����¼��ǰ�������е�������������ʱ����Ҫ���ش��ļ��е�������Ϣ
    ///������Ҫά����������ִ�е���Ϣ��ÿ�����������󣬶�����д���ļ������п��ƣ���������Ŀ������������������
    ///����ִ�У�����ά���Լ���״̬����Ҫ���ں�����չ�ķ���
    ///taskrun.xml��һ������̷߳��ʵ��ļ��������ڲ���֮ǰ��Ҫ���ж������̶߳���Ĳ�������
    ///�Ա��������Ϣ����ʧ�ܵ���������ԣ��Դ������������Ӧ�þ�����ͷŴ������Դ��
    class cTaskRun
    {

        cXmlIO xmlConfig;
        DataView Tasks;

        public cTaskRun()
        {

        }

        ~cTaskRun()
        {
        }

        //���ص�������������ļ�����Ϣ

        public void LoadTaskRunData()
        {
            try
            {
                xmlConfig = new cXmlIO(Program.getPrjPath() + "Tasks\\TaskRun.xml");
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

        //����ִ�е�����ID����������Ϣ
        public void LoadSingleTask(Int64 TaskID)
        {
            try
            {
                xmlConfig = new cXmlIO(Program.getPrjPath() + "Tasks\\TaskRun.xml");
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            //��ȡTaskClass�ڵ�,���ܷ��ص�Ҳ��DataView,������ֻ����һ����¼
            //��������Ϊ�˸��õļ��ݷ��ʲ���
            Tasks = xmlConfig.GetData("Tasks","TaskID",TaskID.ToString () );
        }

        //���㵱ǰ���ж��ٸ�������������
        public int GetTaskClassCount()
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

        #region �����ƶ��������Ż�ȡ�����������Ϣ

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
            string tempFile = Tasks[index].Row["tempFile"].ToString();
            return tempFile;
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

        public cGlobalParas.PublishType GetPublishType (int index)
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

        //���ش�����ʵ����Ҫ�ɼ�����ַ��������ֵ������δ����ǰ���ǲ�׼ȷ��
        public int GetTrueUrlCount(int index)
        {
            int WebLinkCount;
            try
            {
                WebLinkCount = int.Parse(Tasks[index].Row["TrueUrlCount"].ToString());
            }
            catch (System.Exception ex)
            {
                if (Tasks[index].Row["TrueUrlCount"].ToString() == "")
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

        public int GetGatheredTrueUrlCount(int index)
        {
            int WebLinkCount;
            try
            {
                WebLinkCount = int.Parse(Tasks[index].Row["GatheredTrueUrlCount"].ToString());
            }
            catch
            {
                WebLinkCount = 0;
            }
            return WebLinkCount;
        }

        public int GetErrUrlCount(int index)
        {
            int ErrUrlCount;
            try
            {
                ErrUrlCount = int.Parse(Tasks[index].Row["ErrUrlCount"].ToString());
            }
            catch
            {
                ErrUrlCount = 0;
            }
            return ErrUrlCount;
        }

        public int GetTrueErrUrlCount(int index)
        {
            int ErrUrlCount;
            try
            {
                ErrUrlCount = int.Parse(Tasks[index].Row["TrueErrUrlCount"].ToString());
            }
            catch
            {
                ErrUrlCount = 0;
            }
            return ErrUrlCount;
        }

        public int GetThreadCount(int index)
        {
            int ThreadCount;
            if (Tasks[index].Row["ThreadCount"].ToString() == null || Tasks[index].Row["ThreadCount"].ToString() == "")
            {
                ThreadCount = 0;
            }
            else
            {
                ThreadCount = int.Parse(Tasks[index].Row["ThreadCount"].ToString());
            }
            return ThreadCount;

        }

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

        ///�����������*****************************************************************************************
        ///���Ȳɼ�����������ɺ����ݲ�û�����������ת�Ƶ�����ɶ���
        ///��ζ���ڴ��л����ڴ����ݣ��������õȡ���taskrun��ȷ�Ѿ�ɾ����
        ///�����ݣ�����Ѿ���ͬ���ˣ����ԣ������������񣬾ͻᵼ������
        ///������ͻ�������޷�ִ�У�����maxid��Ҫ�����ڴ��е����ݽ�������
        ///ͬʱ������inserttaskrun��deltask�п��ܻ��ɶ���̲߳�����Ҳ����
        ///��ɲ�ͬ�����������Ҳ��Ҫ�������ǰgetnewid�ǲ�����ʱ���Ž��еģ��������Ǳ������ظ������⡣
        ///***********************************************************************



        public cGlobalParas.TaskState GetTaskState(int index)
        {
            cGlobalParas.TaskState TaskState;

            try
            {
                //�ڴ��ж�һ����������ļ��������ļ��Ƿ����
                //��������ڣ������ʧ��״̬��˵��
                string fName=Program.getPrjPath () + "tasks\\run\\task" + this.GetTaskID (index ) + ".xml";
                if (File.Exists(fName))
                {
                    TaskState = (cGlobalParas.TaskState)int.Parse(Tasks[index].Row["TaskState"].ToString());
                }
                else
                {
                    TaskState = cGlobalParas.TaskState.Failed;
                }
            }
            catch
            {
                TaskState = cGlobalParas.TaskState.UnStart;
            }

            return TaskState;
        }

        public long GetRunTime(int index)
        {
            long RunTime;

            try
            {
                RunTime = long.Parse(Tasks[index].Row["RunTime"].ToString());
            }
            catch
            {
                RunTime = 0;
            }
            return RunTime;
        }

        #endregion

        public Int64 GetNewID()
        {
            Int64 id=Int64.Parse ( DateTime.Now.ToFileTime().ToString ());

            return id;
        }

        public void NewTaskRunFile()
        {
            xmlConfig = new cXmlIO();
            string strXml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                      "<Tasks>" +
                      "</Tasks>";
            xmlConfig.NewXmlFile(Program.getPrjPath()  + "tasks\\taskrun.xml", strXml);
        }

        private readonly Object m_taskFileLock = new Object();

        public Int64 InsertTaskRun(string Path, string File)
        {
            ///�����жϴ������ִ�е�Ŀ¼�Ƿ����
            ///��Ŀ¼�ǹ̶�Ŀ¼�������ϵͳ\\Task\\run
            

            string RunPath = Program.getPrjPath() + "Tasks\\run";

            if (!System.IO.Directory.Exists(RunPath))
            {
                System.IO.Directory.CreateDirectory(RunPath);
            }

            ///�Ƚ��������ժҪ��Ϣ���ص�TaskRun.xml�ļ���
            Task.cTask t = new Task.cTask();
            t.LoadTask(Path + "\\" + File);

            //��ʼ����xml�ڵ�����
            LoadTaskRunData();
            Int64 maxID = GetNewID();

            string tRunxml = "";
            tRunxml = "<TaskID>" + maxID + "</TaskID>";
            tRunxml += "<TaskName>" + t.TaskName + "</TaskName>";
            tRunxml += "<TaskState>" + (int)cGlobalParas.TaskState.UnStart + "</TaskState>";
            tRunxml += "<TaskType>" + t.TaskType + "</TaskType>";
            tRunxml += "<RunType>" + t.RunType + "</RunType>";
            tRunxml += "<ExportFile>" + t.ExportFile + "</ExportFile>";
            tRunxml += "<tempFile>" + t.SavePath + "\\" + t.TaskName + "-" + maxID + ".xml" + "</tempFile>";
            tRunxml += "<StartDate>" + DateTime.Now + "</StartDate>";
            tRunxml += "<EndDate></EndDate>";
            tRunxml += "<ThreadCount>" + t.ThreadCount + "</ThreadCount>";
            tRunxml += "<UrlCount>" + t.UrlCount + "</UrlCount>";

            ///TrueUrlCount��ʾ����ɼ�����ַ�д��ڵ�����ַ������Ҫ�ɼ�����ַ���޷����ݹ�ʽ���˳�����
            ///��Ҫ�ɼ����񲻶�ִ�У����ϸ��ݲɼ��Ĺ�����м���ɼ���ַ��������������Ҫ�ٴμ�¼��ֵ
            ///��¼��ֵ��Ŀ����Ϊ�˿��Ը��õĸ��ٲɼ��Ľ��ȣ���Urlcount�����޸ģ���Ϊ��ֵҪ��������ֽ�
            ///ʹ�ã�����ı���UrlCount����ܵ�������ֽ�ʧ�ܣ�����Ӫ�����ʼ����ʱ�򣬴�ֵͬUrlCount����ֵ��
            ///������������Ӫʱά��
            tRunxml += "<TrueUrlCount>" + t.UrlCount + "</TrueUrlCount>";

            tRunxml += "<GatheredUrlCount>0</GatheredUrlCount>";
            tRunxml += "<GatheredTrueUrlCount>0</GatheredTrueUrlCount>";
            tRunxml += "<ErrUrlCount>0</ErrUrlCount>";
            tRunxml += "<TrueErrUrlCount>0</TrueErrUrlCount>";

            tRunxml += "<IsLogin>" + t.IsLogin + "</IsLogin>";
            tRunxml += "<PublishType>" + t.ExportType + "</PublishType>";

            xmlConfig.InsertElement("Tasks", "Task", tRunxml);
            xmlConfig.Save();
            xmlConfig = null;

            ///������������xml�ļ��ĸ�ʽ��Task�����ʽ��ȫһ�۸�����������ʽ��ȫ��ͬ
            ///������ʽ�ǰ���Task����ǰ�ļ���Taskrun�е�id����������������Ŀ����֧��ͬһ������
            ///���Խ����������ʵ����Ҳ���ǵ�����������е�ʱ���û�Ҳ�����޸Ĵ����������
            ///һ��ʵ����ʼ���С�
            System.IO.File.Copy(Path + "\\" + File, RunPath + "\\" + "Task" + maxID + ".xml", true);

            //�ļ�������ȥ����Ҫ�޸��ļ��е�TaskID�����Ǻ�TaskRun�е�TaskID����������
            //�ڼ����ļ���ʱ������,ϵͳ��ID����Ψһ����
            cXmlIO xmlFile;
            xmlFile = new cXmlIO(RunPath + "\\" + "Task" + maxID + ".xml");
            string tID = xmlFile.GetNodeValue("Task/BaseInfo/ID");
            xmlFile.EditNode("ID", tID, maxID.ToString());
            xmlFile.Save();
            xmlFile = null;
          
            return maxID ;

        }

        public void DelTask(Int64 TaskID)
        {
            
            xmlConfig.DeleteChildNodes("Tasks", "TaskID", TaskID.ToString());
            xmlConfig.Save();
            
        }
    }
}
