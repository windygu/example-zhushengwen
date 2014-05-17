using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using System.IO;

///功能：采集任务类别 管理
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace SoukeyNetget.Task
{
    class cTaskClass
    {
        cXmlIO xmlConfig;
        DataView TaskClass;

        //定义一个集合类
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

                //获取TaskClass节点
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

        //计算当前共有多少个TaskClass
        public int GetTaskClassCount()
        {
            int tCount = 0;

            if (TaskClass == null)
                tCount = 0;
            else
                tCount = TaskClass.Count;
            return tCount;
        }

        //根据指定的index返回TaskID
        public int GetTaskClassID(int index)
        {
            int TClassID = int.Parse (TaskClass[index].Row["id"].ToString());
            return TClassID;
        }

        //根据制定的index返回TaskClassName
        public string GetTaskClassName(int index)
        {
            string TClassName = TaskClass[index].Row["Name"].ToString();
            return TClassName;
        }

        ////根据指定的index返回TaskClassPath
        //public string GetTaskClassPathByIndex(int index)
        //{
        //    return "";
        //}

        //根据指定的ID返回TaskClassPath
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

        //根据指定的Task分类名称返回任务分类所存储的路径
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

        //判断任务分类是否存在
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

        //增加分类节点，如果添加成功，则返回添加成功后的分类节点ID
        //系统中存储的路径全部都是相对路径，不允许存储绝对路径
        //系统参数是绝对路径，绝对路径到相对路径的转换在方法在内部完成
        //系统对外看都是绝对路径
        public int AddTaskClass(string TaskClassName,string TaskClassPath)
        {
            //转换相对路径
            TaskClassPath = cTool.GetRelativePath(TaskClassPath);
 
            int tCount = GetTaskClassCount();

            //需要判断新建立的任务分类是否已经存在
            for (int i = 0; i < tCount; i++)
            {
                if (TaskClass[i].Row["Name"].ToString() == TaskClassName)
                {
                    throw new cSoukeyException("任务分类已经存在！");
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
            

            //建立物理的任务分类的目录和索引文件
            if (!System.IO.Directory.Exists(TaskClassPath))
            {
                System.IO.Directory.CreateDirectory(TaskClassPath);
            }

            Task.cTaskIndex tIndex = new Task.cTaskIndex();
            tIndex.NewIndexFile(TaskClassPath);
            tIndex = null;

            return MaxID;

        }

        //删除指定的分类文件
        public bool DelTaskClass(string TClassName)
        {
            //首先删除TaskClass.xml中的任务分类索引节点
            string FilePath = this.GetTaskClassPathByName(TClassName);
            xmlConfig.DeleteChildNodes("TaskClasses", "Name", TClassName);
            xmlConfig.Save();

            System.IO.Directory.Delete (FilePath ,true );
            //string FileName =FilePath   + "\\index.xml";
            //System.IO.File.Delete(FileName);
            return true;
        }

        //任务分类改名，任务分类改名就是新建一个分类，并把原有分类的任务都
        //迁移过来，并修改任务信息的任务，并删除原有内容
        public bool RenameTaskClass(string TClassName, string NewTClassName)
        {
            try
            {
                int OldTaskClassID=0;
                string OldPath = GetTaskClassPathByName(TClassName);
                string NewPath = OldPath.Substring(0, OldPath.IndexOf(TClassName)) + NewTClassName;

                //判断新的任务路径是否存在，如果存在则报错
                if (Directory.Exists (NewPath ))
                    throw new cSoukeyException("新任务分类的路径已经存在，请重新修改任务分类名称！");

                //转换相对路径
                string NewRelativePath = cTool.GetRelativePath(NewPath);

                int tCount = GetTaskClassCount();

                //需要判断新建立的任务分类是否已经存在
                for (int i = 0; i < tCount; i++)
                {
                    if (TaskClass[i].Row["Name"].ToString() == NewTClassName)
                    {
                        throw new cSoukeyException("任务分类已经存在！");
                    }

                     if (TaskClass[i].Row["Name"].ToString() == TClassName)
                    {
                         //获取原有分类的ID
                         OldTaskClassID=GetTaskClassID (i);
                    }
                }

                if (OldTaskClassID == 0)
                {
                    throw new cSoukeyException("未能找到需要修改分类的信息，名称修改失败！");
                }

                //开始修改任务分类下的所有任务的所属分类
                cTaskIndex xmlTasks = new cTaskIndex();
                xmlTasks.GetTaskDataByClass(TClassName);

                //开始初始化此分类下的任务
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

                //开始修改taskclass.xml文件中的任务分类索引信息
                xmlConfig.EditNodeValue("TaskClasses", "id", OldTaskClassID.ToString (), "Name", NewTClassName);
                xmlConfig.EditNodeValue("TaskClasses", "id", OldTaskClassID.ToString (), "Path", NewRelativePath);
                xmlConfig.Save();
                xmlConfig = null;

                //开始将修改任务分类的实际路径
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
