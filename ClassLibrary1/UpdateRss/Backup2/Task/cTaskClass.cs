using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using System.IO;

///���ܣ��ɼ�������� ����
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget.Task
{
    class cTaskClass
    {
        cXmlIO xmlConfig;
        DataView TaskClass;

        //����һ��������
        public List<cTaskIndex> Task
        {
            get { return Task; }
            set { Task = value; }
        }

        public cTaskClass()
        {
            try
            {
                xmlConfig = new cXmlIO(Program.getPrjPath () + "TaskClass.xml");

                //��ȡTaskClass�ڵ�
                TaskClass = xmlConfig.GetData("descendant::TaskClasses");
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        ~cTaskClass()
        {
            xmlConfig = null;
        }

        //���㵱ǰ���ж��ٸ�TaskClass
        public int GetTaskClassCount()
        {
            int tCount = 0;

            if (TaskClass == null)
                tCount = 0;
            else
                tCount = TaskClass.Count;
            return tCount;
        }

        //����ָ����index����TaskID
        public int GetTaskClassID(int index)
        {
            int TClassID = int.Parse (TaskClass[index].Row["id"].ToString());
            return TClassID;
        }

        //�����ƶ���index����TaskClassName
        public string GetTaskClassName(int index)
        {
            string TClassName = TaskClass[index].Row["Name"].ToString();
            return TClassName;
        }

        ////����ָ����index����TaskClassPath
        //public string GetTaskClassPathByIndex(int index)
        //{
        //    return "";
        //}

        //����ָ����ID����TaskClassPath
        public string GetTaskClassPathByID(int id)
        {
            int i = 0;

            for (i = 0; i < GetTaskClassCount();i++ )
            {
                if (int.Parse (TaskClass[i].Row["id"].ToString()) == id)
                {
                    string tClassPath =Program.getPrjPath () + TaskClass[i].Row["Path"].ToString();
                    return tClassPath;
                }
            }

            return "";
        }

        //����ָ����Task�������Ʒ�������������洢��·��
        public string GetTaskClassPathByName(String Name)
        {
            int i = 0;
            for (i = 0; i < GetTaskClassCount(); i++)
            {
                if (TaskClass[i].Row["Name"].ToString() == Name)
                {
                    string tClassPath = Program.getPrjPath() + TaskClass[i].Row["Path"].ToString();
                    return tClassPath;

                }
            }
            return "";
        }

        //�ж���������Ƿ����
        public bool IsExist(string TaskClassName)
        {
            bool isbool = false;

            for (int i = 0; i < TaskClass.Count; i++)
            {
                if (TaskClass[i].Row["Name"].ToString() == TaskClassName)
                {
                    isbool = true;
                    break;
                }
            }

            return isbool;
        }

        //���ӷ���ڵ㣬�����ӳɹ����򷵻���ӳɹ���ķ���ڵ�ID
        //ϵͳ�д洢��·��ȫ���������·����������洢����·��
        //ϵͳ�����Ǿ���·��������·�������·����ת���ڷ������ڲ����
        //ϵͳ���⿴���Ǿ���·��
        public int AddTaskClass(string TaskClassName,string TaskClassPath)
        {
            //ת�����·��
            TaskClassPath = cTool.GetRelativePath(TaskClassPath);
 
            int tCount = GetTaskClassCount();

            //��Ҫ�ж��½�������������Ƿ��Ѿ�����
            for (int i = 0; i < tCount; i++)
            {
                if (TaskClass[i].Row["Name"].ToString() == TaskClassName)
                {
                    throw new cSoukeyException("��������Ѿ����ڣ�");
                }
            }

            string strTaskClass = "";
            int MaxID=0;
            if (tCount > 0)
            {
                int index = TaskClass.Count - 1;
                MaxID = int.Parse(TaskClass[index].Row["id"].ToString()) + 1;
            }
            else
            {
            }

            strTaskClass = "<id>" + MaxID + "</id>";
            strTaskClass += "<Name>" + TaskClassName + "</Name>";
            strTaskClass += "<Path>" + TaskClassPath + "</Path>";
            xmlConfig.InsertElement("TaskConfig/TaskClasses", "TaskClass", strTaskClass);
            xmlConfig.Save();
            

            //�����������������Ŀ¼�������ļ�
            if (!System.IO.Directory.Exists(TaskClassPath))
            {
                System.IO.Directory.CreateDirectory(TaskClassPath);
            }

            Task.cTaskIndex tIndex = new Task.cTaskIndex();
            tIndex.NewIndexFile(TaskClassPath);
            tIndex = null;

            return MaxID;

        }

        //ɾ��ָ���ķ����ļ�
        public bool DelTaskClass(string TClassName)
        {
            //����ɾ��TaskClass.xml�е�������������ڵ�
            string FilePath = this.GetTaskClassPathByName(TClassName);
            xmlConfig.DeleteChildNodes("TaskClasses", "Name", TClassName);
            xmlConfig.Save();

            System.IO.Directory.Delete (FilePath ,true );
            //string FileName =FilePath   + "\\index.xml";
            //System.IO.File.Delete(FileName);
            return true;
        }

        //���������������������������½�һ�����࣬����ԭ�з��������
        //Ǩ�ƹ��������޸�������Ϣ�����񣬲�ɾ��ԭ������
        public bool RenameTaskClass(string TClassName, string NewTClassName)
        {
            try
            {
                int OldTaskClassID=0;
                string OldPath = GetTaskClassPathByName(TClassName);
                string NewPath = OldPath.Substring(0, OldPath.IndexOf(TClassName)) + NewTClassName;

                //�ж��µ�����·���Ƿ���ڣ���������򱨴�
                if (Directory.Exists (NewPath ))
                    throw new cSoukeyException("����������·���Ѿ����ڣ��������޸�����������ƣ�");

                //ת�����·��
                string NewRelativePath = cTool.GetRelativePath(NewPath);

                int tCount = GetTaskClassCount();

                //��Ҫ�ж��½�������������Ƿ��Ѿ�����
                for (int i = 0; i < tCount; i++)
                {
                    if (TaskClass[i].Row["Name"].ToString() == NewTClassName)
                    {
                        throw new cSoukeyException("��������Ѿ����ڣ�");
                    }

                     if (TaskClass[i].Row["Name"].ToString() == TClassName)
                    {
                         //��ȡԭ�з����ID
                         OldTaskClassID=GetTaskClassID (i);
                    }
                }

                if (OldTaskClassID == 0)
                {
                    throw new cSoukeyException("δ���ҵ���Ҫ�޸ķ������Ϣ�������޸�ʧ�ܣ�");
                }

                //��ʼ�޸���������µ������������������
                cTaskIndex xmlTasks = new cTaskIndex();
                xmlTasks.GetTaskDataByClass(TClassName);

                //��ʼ��ʼ���˷����µ�����
                int count = xmlTasks.GetTaskClassCount();

                cXmlIO txml;

                for (int i = 0; i < count; i++)
                {
                    txml = new cXmlIO(OldPath + "\\" + xmlTasks.GetTaskName(i) + ".xml");
                    txml.EditNodeValue("Task/BaseInfo/Class", NewTClassName);
                    txml.Save();
                    txml = null;
                }

                xmlTasks = null;

                //��ʼ�޸�taskclass.xml�ļ��е��������������Ϣ
                xmlConfig.EditNodeValue("TaskClasses", "id", OldTaskClassID.ToString (), "Name", NewTClassName);
                xmlConfig.EditNodeValue("TaskClasses", "id", OldTaskClassID.ToString (), "Path", NewRelativePath);
                xmlConfig.Save();
                xmlConfig = null;

                //��ʼ���޸���������ʵ��·��
                File.SetAttributes(OldPath, System.IO.FileAttributes.Normal);
                Directory.Move(OldPath, NewPath);
                //Directory.Delete(OldPath);

            }
            catch (System.Exception ex)
            {
                throw ex;
                return false;
            }

            return true;
        }
    }
}
