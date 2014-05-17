using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

///���ܣ����������ļ�����
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget.Task
{

    //TaskIndex��������������ժҪ��Ϣ���࣬��Ҫ���ڶ�ҳ���ժҪ���ݽ�����ʾ
    //��������Ŀ���ǿ��Կ�����������ͬʱ����û���ɾ��������Ҳ�ɶԴ˽�������
    //ÿһ��������඼�洢��һ��·���£��ڴ�·����Ĭ�Ͻ���һ��TaskIndex.xml�ļ�����¼��������
    //��������������,Ӧ���ǰ���һ������ļ�����,���½����������Ĺ��ܲ�ֳ���,
    //�����ھ���������,��һ���������.
    class cTaskIndex
    {
        cXmlIO xmlConfig;
        DataView Tasks;

        #region ���������
        public cTaskIndex()
        {
            xmlConfig = new cXmlIO();
        }

        public cTaskIndex (string xmlFile)
        {
            xmlConfig =new cXmlIO (xmlFile );
        }

        ~cTaskIndex ()
        {
            xmlConfig =null;
        }

        #endregion

        #region �½� �½�һ��index�ļ�,���ڴ��ļ����½�һ��������Ϣ
        public void NewIndexFile(string Path)
        {

            string  strXml="<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                       "<TaskIndex>" +
                       "</TaskIndex>";
            xmlConfig.NewXmlFile(Path + "\\index.xml",strXml );
            
        }

        public void InsertTaskIndex(string strXml)
        {
            xmlConfig.InsertElement("TaskIndex", "Task", strXml);
            xmlConfig.Save();
        }

        public void DeleTaskIndex(string TaskName)
        {
            xmlConfig.DeleteChildNodes("TaskIndex","Name",TaskName);
            xmlConfig.Save();
        }


        #endregion

        #region ����ָ�����������,����index.xml�ļ�,������ָ����index����������Ϣ


        //��ȡ��������Ŀ¼�µ�������������Ŀ¼ΪӦ�ó���·��\\tasks��
        //�˷���Ϊ�̶����ݣ����ṩ�û��ɲ������κδ���

        public void GetTaskDataByClass()
        {

            string ClassPath = Program.getPrjPath() + "tasks";
            m_TaskPath = ClassPath;

            xmlConfig = new cXmlIO(ClassPath + "\\index.xml");

            //��ȡTaskClass�ڵ�
            Tasks = xmlConfig.GetData("TaskIndex");
        }

        public void GetTaskDataByClass(int ClassID)
        {
            Task.cTaskClass tClass = new Task.cTaskClass();

            string ClassPath = tClass.GetTaskClassPathByID(ClassID);
            m_TaskPath = ClassPath;

            tClass = null;

            xmlConfig = new cXmlIO(ClassPath + "\\index.xml");

            //��ȡTaskClass�ڵ�
            Tasks = xmlConfig.GetData("TaskIndex");


        }

        private string m_TaskPath;
        private string TaskPath
        {
            get { return m_TaskPath; }
        }

        public void GetTaskDataByClass(string ClassName)
        {
            string ClassPath ;

            if (ClassName == "")
            {
                ClassPath = Program.getPrjPath() + "Tasks";
            }
            else
            {
                Task.cTaskClass tClass = new Task.cTaskClass();

                ClassPath = tClass.GetTaskClassPathByName(ClassName) ;

                tClass = null;
            }

            m_TaskPath = ClassPath;

            xmlConfig = new cXmlIO(ClassPath + "\\index.xml");

            //��ȡTaskClass�ڵ�
            Tasks = xmlConfig.GetData("TaskIndex");


        }

        //���㵱ǰ���ж��ٸ�TaskClass
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

        public int GetTaskID(int index)
        {
            int tid = int.Parse (Tasks[index].Row["id"].ToString());
            return tid;
        }

        public string GetTaskName(int index)
        {
            string TName = Tasks[index].Row["Name"].ToString();
            return TName;
        }



        public string GetTaskType(int index)
        {
            string TType = Tasks[index].Row["Type"].ToString();
            return TType;
        }

        public string GetTaskRunType(int index)
        {
            string TRunType = Tasks[index].Row["RunType"].ToString();
            return TRunType;
        }

        public string GetExportFile(int index)
        {
            string ExportFile = Tasks[index].Row["ExportFile"].ToString();
            return ExportFile;
        }

        public cGlobalParas.TaskState GetTaskState(int index)
        {
            string fName = TaskPath + "\\" + GetTaskName(index) + ".xml";
            if (File.Exists(fName))
            {
                return cGlobalParas.TaskState.UnStart;
            }
            else
            {
                return cGlobalParas.TaskState.Failed;
            }
        }

        public int  GetWebLinkCount(int index)
        {
            int WebLinkCount;
            try
            {
                WebLinkCount = int.Parse(Tasks[index].Row["WebLinkCount"].ToString());
            }
            catch
            {
                WebLinkCount = 0;
            }
            return WebLinkCount;
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

        #endregion



    }
}
