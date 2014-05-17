using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data ;

///���ܣ��ɼ������������
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget.Gather
{
    #region cTaskDataList �����

    //������Ҫ�ɼ���������Ϣ
    //�����������Ŀ¼�µ�Tasks\\TaskRun.xml�ļ�
    //���м���,���ļ��洢����Ҫ�ɼ�������

    public class cTaskDataList
    {
        private int m_TaskCount ;

        public cTaskDataList()
        {
            m_TaskDataList = new List<cTaskData>();
        }

        //��ʼ���������Ĳɼ�������Ϣ.
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
