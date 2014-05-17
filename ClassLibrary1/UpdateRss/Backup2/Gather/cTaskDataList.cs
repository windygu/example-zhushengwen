using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data ;

///功能：采集任务队列数据
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace SoukeyNetget.Gather
{
    #region cTaskDataList 类代码

    //加载需要采集的任务信息
    //根据软件运行目录下的Tasks\\TaskRun.xml文件
    //进行加载,此文件存储着需要采集的任务

    public class cTaskDataList
    {
        private int m_TaskCount ;

        public cTaskDataList()
        {
            m_TaskDataList = new List<cTaskData>();
        }

        //初始化运行区的采集任务信息.
        public void LoadTaskRunData()
        {
            Task.cTaskRun t = new Task.cTaskRun();
            t.LoadTaskRunData();
            cTaskData tData;
            for (int i = 0; i < t.GetCount(); i++)
            {
                tData = new cTaskData();
                tData.TaskID = t.GetTaskID(i);
                tData.TaskName = t.GetTaskName(i);
                tData.TaskType = t.GetTaskType(i);
                tData.RunType = t.GetTaskRunType(i);
                tData.tempFileName = t.GetTempFile(i);
                tData.TaskState = t.GetTaskState(i);
                tData.ThreadCount = t.GetThreadCount(i);
                tData.UrlCount = t.GetUrlCount(i);
                tData.TrueUrlCount = t.GetTrueUrlCount(i);
                tData.GatheredUrlCount = 0;
                tData.GatheredTrueUrlCount = t.GetGatheredTrueUrlCount(i);
                tData.GatherErrUrlCount = 0;
                tData.GatheredTrueErrUrlCount = t.GetTrueErrUrlCount(i);
                m_TaskDataList.Add(tData);
                tData = null;
            }

            m_TaskCount = t.GetCount();
            t = null;
        }

        public int TaskCount
        {
            get { return m_TaskCount; }
        }

        public cTaskDataList(string FileName)
        {
            m_TaskDataList = new List<cTaskData>();
        }

        private List<cTaskData> m_TaskDataList;

        public List<cTaskData> TaskDataList
        {
            get { return m_TaskDataList; }
            set { m_TaskDataList = value; }
        }

        ~cTaskDataList()
        {

        }

    }

    #endregion

   

}
