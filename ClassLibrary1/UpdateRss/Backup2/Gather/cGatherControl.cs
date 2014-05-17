using System;
using System.Collections.Generic;
using System.Text;

///���ܣ��ɼ�������� ���� ֹͣ ��ͣ ����
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget.Gather
{
    //�ɼ�����Ŀ���
    public class cGatherControl : IDisposable
    {

        //private Queue<long> m_DCountQueue;
        //private Queue<int> m_DTtimeQueue;
        private int m_CompletedCount;
        private bool m_IsInitialized;
        private int m_LastTime;
        private bool m_Isbusying;
        private System.Threading.Timer m_GatherEngine;

        #region �๹��

        public cGatherControl()
        {
            m_TaskManage = new cGatherManage();
        }

        ~cGatherControl()
        {
            Dispose(false);
        }
        #endregion

        #region ����
        private cGatherManage m_TaskManage;
        public cGatherManage TaskManage
        {
            get { return m_TaskManage; }
            set { m_TaskManage = value; }
        }
        #endregion

        #region ��ʱ��
        /// ��ʼ��timer��ʱ��,��ǰ������Threading.timer��ʱ��
        /// �˼�ʱ������������ѯ��ǰ��������У����Ƿ����Ѿ���ɵ�����
        /// ���ȴ�ִ�е��������������ִ����
        private void timerInit()
        {
            m_LastTime = System.Environment.TickCount;
            m_GatherEngine = new System.Threading.Timer(new System.Threading.TimerCallback(m_GatherEngine_CallBack), null, 0, 50);
            m_IsInitialized = true;
            m_Isbusying = false;
        }

        /// ����ɼ����ݵ���ʾ����־��ʾ
        private void m_GatherEngine_CallBack(object State)
        {
            if (!m_Isbusying)
            {
                m_Isbusying = true;

                // �ڴ� ����ȴ�ִ�еĹ����߳�
                cGatherTaskSplit[] tmpList = m_TaskManage.TaskListControl.GetWaitingWorkThread();
                foreach (cGatherTaskSplit dtc in tmpList)
                {
                    if (dtc.Waittime <= 0)
                    {
                        m_TaskManage.TaskListControl.ReStartWaitingWorkThread(dtc);
                    }
                    else
                    {
                        dtc.Waittime -= 10;
                    }
                }

                // ���� ����ɵ�����
                m_TaskManage.TaskListControl.TaskDispose();

                // �ж��������������������������򴥷� ����������� �¼�
                if (m_TaskManage.TaskListControl.CompletedTaskList.Count != m_CompletedCount)
                {
                    m_CompletedCount = m_TaskManage.TaskListControl.CompletedTaskList.Count;
                    //m_TaskManage.SaveTaskList();
                    if (m_CompletedCount == m_TaskManage.TaskList.Count)
                    {
                        onCompleted();
                    }
                }

                // ��鲢��ʼ�ȴ�������
                m_TaskManage.TaskListControl.AutoNext();

                // �������д����¼�
                m_TaskManage.EventProxy.DoEvents();

                m_Isbusying = false;
            }
        }

        #endregion

        #region �¼�

        /// ȫ���ɼ�����¼�
        private event EventHandler<cTaskEventArgs> e_Completed;
        public event EventHandler<cTaskEventArgs> Completed
        {
            add { e_Completed += value; }
            remove { e_Completed -= value; }
        }

        private void onCompleted()
        {
            if (e_Completed != null)
            {
                e_Completed.Invoke(this, new cTaskEventArgs());
            }
        }
        #endregion

        #region �ɼ�������Ʋ���

        /// �������������е�����
        public bool AddGatherTask(cTaskDataList taskDataList)
        {
            //�������������ݽ��вɼ���������
            //�����������س�������Դ��󣬼������أ�ȷ�����е����񶼿��Լ��سɹ�
            bool IsSucceed = true;

            for (int i=0 ;i<taskDataList.TaskCount;i++)
            {
                try
                {
                    m_TaskManage.Add(taskDataList.TaskDataList[i]);
                }
                catch (System.Exception ex)
                {
                    IsSucceed = false;
                }
            }

            return IsSucceed;
        }

        //���ӵ�������
        public void AddGatherTask(cTaskData task)
        {
            m_TaskManage.Add(task);
        }

        
        /// ��ʼָ���Ĳɼ�����
        public void Start(cGatherTask task)
        {
            if (!m_IsInitialized)
            {
                timerInit();
            }
            m_TaskManage.TaskListControl.StartTask(task);
        }
        
        /// ��������������������
        public void Start()
        {
            if (!m_IsInitialized)
            {
                timerInit();
            }
            m_TaskManage.TaskListControl.Start();
        }

        /// ֹͣ����������������
        public void Stop()
        {
            m_TaskManage.TaskListControl.Stop();
        }

        /// ָֹͣ���Ĳɼ�����
        public void Stop(cGatherTask  task)
        {
            m_TaskManage.TaskListControl.StopTask(task);
        }
       
        /// ���¿�ʼָ���Ĳɼ�����
        public void ReStart(cGatherTask task)
        {
            m_TaskManage.TaskListControl.ReStartTask(task);
        }
   
        /// ɾ��ָ���Ĳɼ�����
        public void Remove(cGatherTask task)
        {
            m_TaskManage.TaskListControl.RemoveTask(task);
        }

        public void Abort()
        {
            while(m_TaskManage.TaskListControl.RunningTaskList.Count>0)
            {
                Abort(m_TaskManage.TaskListControl.RunningTaskList[0]);
            }

        }

        public void Abort(cGatherTask task)
        {
            m_TaskManage.TaskListControl.Abort(task);
            m_TaskManage.TaskListControl.AutoList(task);
        }

        #endregion

        #region IDisposable ��Ա
        private bool m_disposed;
        /// <summary>
        /// �ͷ��� Download �ĵ�ǰʵ��ʹ�õ�������Դ
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    // �ڴ��ͷ��й���Դ
                    //if (m_GatherWatch != null)
                    //{
                    //    m_GatherWatch.Dispose();
                    //}
                    if (m_GatherEngine != null)
                    {
                        m_GatherEngine.Dispose();
                    }
                }

                // �ڴ��ͷŷ��й���Դ

                m_disposed = true;
            }
        }


        #endregion
    }

    
}
