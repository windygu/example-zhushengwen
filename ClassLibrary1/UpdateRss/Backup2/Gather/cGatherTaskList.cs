using System;
using System.Collections.Generic;
using System.Text;

///功能：采集任务队列处理
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace SoukeyNetget.Gather
{
    //采集任务队列管理类
    //采集任务队列是依据采集任务的执行状态来分别建立
    //定义了各种状态的cGatherTask集合
    //在此类中,默认了当前最大的可执行任务数为10个任务
    

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

            //最大任务为20
            m_MaxTask = 20;
        }


        private static int s_Waittime = 5000;
        /// <summary>
        /// 错误重试等待时间
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

        #region 类属性

        /// <summary>
        /// 等待的线程
        /// </summary>
        internal List<cGatherTaskSplit> WaitingWorkThread
        {
            get { return m_WaitingWorkThread; }
            //set { m_WaitingWorkThread = value; }
        }
        /// <summary>
        /// 等待开始的任务队列
        /// </summary>
        internal List<cGatherTask> WaitingTaskList
        {
            get { return m_WaitingTaskList; }
            //set { m_WaitingTaskList = value; }
        }
        /// <summary>
        /// 正在执行的任务队列
        /// </summary>
        internal List<cGatherTask> RunningTaskList
        {
            get { return m_RunningTaskList; }
            //set { m_RunningTaskList = value; }
        }
        /// <summary>
        /// 处于停止状态的任务队列
        /// </summary>
        internal List<cGatherTask> StoppedTaskList
        {
            get { return m_StoppedTaskList; }
            //set { m_StoppedTaskList = value; }
        }
        /// <summary>
        /// 已完成的任务队列
        /// </summary>
        internal List<cGatherTask> CompletedTaskList
        {
            get { return m_CompletedTaskList; }
            //set { m_CompletedTaskList = value; }
        }

        #endregion

        //定义队列锁
        private readonly Object m_listLock = new Object();

        //根据采集任务的状态自动维护采集队列中的采集任务数据
        //当前分为多个采集队列(运行/失败/等待/完成)，等待队列中的
        //任务将不断的重试，即自动执行
        internal void AutoList(cGatherTask task)
        {
            // 任务状态变更事件在所有线程都有触发，此处需要同步
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

                        //任务完成后直接进行数据导出,不在向任务完成队列
                        //中添加任务数据,任务导出后直接由taskcompleted列表
                        //接手完成完成任务的控制.
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
        /// 结束任务
        /// </summary>
        /// <param name="task"></param>
        internal void FinishTask(cGatherTask task)
        {
            m_FinishTaskQueue.Enqueue(task);
        }

        /// <summary>
        /// 回收已完成的线程
        /// </summary>
        /// 
        internal void TaskDispose()
        {
            // 释放所有完成的线程
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
        /// 启动所有任务
        /// </summary>
        public void Start()
        {
            while (m_StoppedTaskList.Count > 0)
            {
                StartTask(m_StoppedTaskList[0]);
            }

        }
        /// <summary>
        /// 停止所有任务
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
        /// 清空控制器内容
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
  
        /// 移除任务（取消任务）
        internal void RemoveTask(cGatherTask task)
        {
            // 取消任务 删除列表中的所有数据
            if (task.State == cGlobalParas.TaskState.Running || task.State ==cGlobalParas.TaskState.Started )
            {
                task.Abort();
            }
            
            task.Remove();

            task.State = cGlobalParas.TaskState.Aborted;

            AutoList(task);
        }

        ///由系统自动触发结束任务操作，并不是用户触发了任务停止操作，
        ///而是由系统触发，譬如：系统强制退出
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

        //强制结束某个任务
        internal void Abort(cGatherTask task)
        {
            task.Abort();
        }

    }
}
