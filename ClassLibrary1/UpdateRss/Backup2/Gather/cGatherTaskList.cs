using System;
using System.Collections.Generic;
using System.Text;

///���ܣ��ɼ�������д���
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget.Gather
{
    //�ɼ�������й�����
    //�ɼ�������������ݲɼ������ִ��״̬���ֱ���
    //�����˸���״̬��cGatherTask����
    //�ڴ�����,Ĭ���˵�ǰ���Ŀ�ִ��������Ϊ10������
    

    public class cGatherTaskList
    {

        private List<cGatherTask> m_WaitingTaskList;
        private List<cGatherTask> m_RunningTaskList;
        private List<cGatherTask> m_StoppedTaskList;
        private List<cGatherTask> m_CompletedTaskList;
        private List<cGatherTaskSplit> m_WaitingWorkThread;
        private Queue<cGatherTask> m_FinishTaskQueue;
        private int m_MaxTask;

        public cGatherTaskList()
        {
            m_WaitingWorkThread = new List<cGatherTaskSplit>();   
            m_CompletedTaskList = new List<cGatherTask>();
            m_RunningTaskList = new List<cGatherTask>();
            m_StoppedTaskList = new List<cGatherTask>();
            m_WaitingTaskList = new List<cGatherTask>();
            m_FinishTaskQueue = new Queue<cGatherTask>();

            //�������Ϊ20
            m_MaxTask = 20;
        }


        private static int s_Waittime = 5000;
        /// <summary>
        /// �������Եȴ�ʱ��
        /// </summary>
        public static int Waittime
        {
            get { return cGatherTaskList.s_Waittime; }
            set { cGatherTaskList.s_Waittime = value; }
        }


        public int MaxTask
        {
            get { return m_MaxTask; }
            set { m_MaxTask = value; }
        }

        #region ������

        /// <summary>
        /// �ȴ����߳�
        /// </summary>
        internal List<cGatherTaskSplit> WaitingWorkThread
        {
            get { return m_WaitingWorkThread; }
            //set { m_WaitingWorkThread = value; }
        }
        /// <summary>
        /// �ȴ���ʼ���������
        /// </summary>
        internal List<cGatherTask> WaitingTaskList
        {
            get { return m_WaitingTaskList; }
            //set { m_WaitingTaskList = value; }
        }
        /// <summary>
        /// ����ִ�е��������
        /// </summary>
        internal List<cGatherTask> RunningTaskList
        {
            get { return m_RunningTaskList; }
            //set { m_RunningTaskList = value; }
        }
        /// <summary>
        /// ����ֹͣ״̬���������
        /// </summary>
        internal List<cGatherTask> StoppedTaskList
        {
            get { return m_StoppedTaskList; }
            //set { m_StoppedTaskList = value; }
        }
        /// <summary>
        /// ����ɵ��������
        /// </summary>
        internal List<cGatherTask> CompletedTaskList
        {
            get { return m_CompletedTaskList; }
            //set { m_CompletedTaskList = value; }
        }

        #endregion

        //���������
        private readonly Object m_listLock = new Object();

        //���ݲɼ������״̬�Զ�ά���ɼ������еĲɼ���������
        //��ǰ��Ϊ����ɼ�����(����/ʧ��/�ȴ�/���)���ȴ������е�
        //���񽫲��ϵ����ԣ����Զ�ִ��
        internal void AutoList(cGatherTask task)
        {
            // ����״̬����¼��������̶߳��д������˴���Ҫͬ��
            lock (m_listLock)
            {
                switch (task.TaskState)
                {
                    case cGlobalParas.TaskState.Aborted:
                        m_RunningTaskList.Remove(task);
                        m_StoppedTaskList.Remove(task);
                        m_CompletedTaskList.Remove(task);
                        m_WaitingTaskList.Remove(task);
                        break;
                    case cGlobalParas.TaskState.Started:
                        if (!m_RunningTaskList.Contains(task))
                        {
                            m_RunningTaskList.Add(task);
                        }
                        m_StoppedTaskList.Remove(task);
                        m_CompletedTaskList.Remove(task);
                        m_WaitingTaskList.Remove(task);
                        break;
                    case cGlobalParas.TaskState.Completed:

                        //������ɺ�ֱ�ӽ������ݵ���,������������ɶ���
                        //�������������,���񵼳���ֱ����taskcompleted�б�
                        //��������������Ŀ���.
                        if (!m_CompletedTaskList.Contains(task))
                        {
                            m_CompletedTaskList.Add(task);
                        }
                        m_RunningTaskList.Remove(task);
                        break;
                    case cGlobalParas.TaskState.UnStart:
                        if (!m_StoppedTaskList.Contains(task))
                        {
                            m_StoppedTaskList.Add(task);
                        }
                        m_WaitingTaskList.Remove (task );
                        m_CompletedTaskList.Remove(task);
                        m_RunningTaskList.Remove(task);
                        break;
                    case cGlobalParas.TaskState.Waiting :
                        if (!m_WaitingTaskList.Contains(task))
                        {
                            m_WaitingTaskList.Add(task);
                        }
                        m_StoppedTaskList.Remove(task);
                        m_CompletedTaskList.Remove(task);
                        m_RunningTaskList.Remove(task);
                        break;
                    default:
                        if (!m_StoppedTaskList.Contains(task))
                        {
                            m_StoppedTaskList.Add(task);
                        }
                        m_WaitingTaskList.Remove(task);
                        m_RunningTaskList.Remove(task);
                        break;
                }
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="task"></param>
        internal void FinishTask(cGatherTask task)
        {
            m_FinishTaskQueue.Enqueue(task);
        }

        /// <summary>
        /// ��������ɵ��߳�
        /// </summary>
        /// 
        internal void TaskDispose()
        {
            // �ͷ�������ɵ��߳�
            while (m_FinishTaskQueue.Count > 0)
            {
                cGatherTask dt = m_FinishTaskQueue.Dequeue();
                if (dt.TaskState == cGlobalParas.TaskState.Completed || dt.TaskState == cGlobalParas.TaskState.Stopped)
                {
                    dt.TaskSplit.Clear();
                }
            }
        }

        internal cGatherTaskSplit[] GetWaitingWorkThread()
        {
            return m_WaitingWorkThread.ToArray();
        }

        internal void AddWaitingWorkThread(cGatherTaskSplit dtc)
        {
            dtc.Waittime = cGatherTaskList.Waittime;
            m_WaitingWorkThread.Add(dtc);
        }

        internal void ReStartWaitingWorkThread(cGatherTaskSplit dtc)
        {
            m_WaitingWorkThread.Remove(dtc);
            dtc.ReStart();
        }

        /// <summary>
        /// ������������
        /// </summary>
        public void Start()
        {
            while (m_StoppedTaskList.Count > 0)
            {
                StartTask(m_StoppedTaskList[0]);
            }

        }
        /// <summary>
        /// ֹͣ��������
        /// </summary>
        public void Stop()
        {
            m_WaitingWorkThread.Clear();
            while (m_WaitingTaskList.Count > 0)
            {
                StopTask(m_WaitingTaskList[0]);
            }
            while (m_RunningTaskList.Count > 0)
            {
                StopTask(m_RunningTaskList[0]);
            }
        }

    
        /// <summary>
        /// ��տ���������
        /// </summary>
        public void Clear()
        {
            m_WaitingWorkThread.Clear();
            m_CompletedTaskList.Clear();
            m_RunningTaskList.Clear();
            m_StoppedTaskList.Clear();
            m_WaitingTaskList.Clear();
            m_FinishTaskQueue.Clear();
        }

        internal void AutoNext()
        {
            while (m_WaitingTaskList.Count > 0 && (m_MaxTask <= 0 || m_RunningTaskList.Count < m_MaxTask))
            {
                cGatherTask task = m_WaitingTaskList[0];
                task.Start();
            }
        }

        internal void StartTask(cGatherTask  task)
        {
            if (task.TaskState ==cGlobalParas.TaskState.UnStart || task.TaskState == cGlobalParas.TaskState.Stopped || task.TaskState == cGlobalParas.TaskState.Failed || task.TaskState == cGlobalParas.TaskState.Aborted)
            {
                if (m_MaxTask <= 0 || m_RunningTaskList.Count < m_MaxTask)
                {
                    task.Start();
                }
                else
                {
                    task.ReadyToStart();
                }
            }
        }

        internal void ReStartTask(cGatherTask task)
        {
            task.ResetTaskData();
            StartTask(task);
        }

        internal void StopTask(cGatherTask task)
        {
            if (task.TaskState == cGlobalParas.TaskState.Started || task.TaskState == cGlobalParas.TaskState.Waiting)
            {
                task.Stop();
            }
        }
  
        /// �Ƴ�����ȡ������
        internal void RemoveTask(cGatherTask task)
        {
            // ȡ������ ɾ���б��е���������
            if (task.State == cGlobalParas.TaskState.Running || task.State ==cGlobalParas.TaskState.Started )
            {
                task.Abort();
            }
            
            task.Remove();

            task.State = cGlobalParas.TaskState.Aborted;

            AutoList(task);
        }

        ///��ϵͳ�Զ�������������������������û�����������ֹͣ������
        ///������ϵͳ������Ʃ�磺ϵͳǿ���˳�
        internal void Abort()
        {
            m_WaitingWorkThread.Clear();
            while (m_WaitingTaskList.Count > 0)
            {
                if (m_WaitingTaskList[0].TaskState == cGlobalParas.TaskState.Waiting)
                    m_WaitingTaskList[0].Abort();
            }
            while (m_RunningTaskList.Count > 0)
            {
                if (m_RunningTaskList[0].TaskState == cGlobalParas.TaskState.Started)
                    m_RunningTaskList[0].Abort ();
            }

      
        }

        //ǿ�ƽ���ĳ������
        internal void Abort(cGatherTask task)
        {
            task.Abort();
        }

    }
}
