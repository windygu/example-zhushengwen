using System;
using System.Collections.Generic;
using System.Text;

///功能：采集任务控制 启动 停止 暂停 重置
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace SoukeyNetget.Gather
{
    //采集任务的控制
    public class cGatherControl : IDisposable
    {

        //private Queue<long> m_DCountQueue;
        //private Queue<int> m_DTtimeQueue;
        private int m_CompletedCount;
        private bool m_IsInitialized;
        private int m_LastTime;
        private bool m_Isbusying;
        private System.Threading.Timer m_GatherEngine;

        #region 类构造

        public cGatherControl()
        {
            m_TaskManage = new cGatherManage();
        }

        ~cGatherControl()
        {
            Dispose(false);
        }
        #endregion

        #region 属性
        private cGatherManage m_TaskManage;
        public cGatherManage TaskManage
        {
            get { return m_TaskManage; }
            set { m_TaskManage = value; }
        }
        #endregion

        #region 计时器
        /// 初始化timer计时器,当前采用了Threading.timer计时器
        /// 此计时器的作用是轮询当前的任务队列，看是否有已经完成的任务
        /// 及等待执行的任务，如果存在则执行它
        private void timerInit()
        {
            m_LastTime = System.Environment.TickCount;
            m_GatherEngine = new System.Threading.Timer(new System.Threading.TimerCallback(m_GatherEngine_CallBack), null, 0, 50);
            m_IsInitialized = true;
            m_Isbusying = false;
        }

        /// 处理采集数据的显示和日志显示
        private void m_GatherEngine_CallBack(object State)
        {
            if (!m_Isbusying)
            {
                m_Isbusying = true;

                // 在此 处理等待执行的工作线程
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

                // 处理 已完成的任务
                m_TaskManage.TaskListControl.TaskDispose();

                // 判断已完成任务数和总任务数相等则触发 所有任务完成 事件
                if (m_TaskManage.TaskListControl.CompletedTaskList.Count != m_CompletedCount)
                {
                    m_CompletedCount = m_TaskManage.TaskListControl.CompletedTaskList.Count;
                    //m_TaskManage.SaveTaskList();
                    if (m_CompletedCount == m_TaskManage.TaskList.Count)
                    {
                        onCompleted();
                    }
                }

                // 检查并开始等待的任务
                m_TaskManage.TaskListControl.AutoNext();

                // 处理所有代理事件
                m_TaskManage.EventProxy.DoEvents();

                m_Isbusying = false;
            }
        }

        #endregion

        #region 事件

        /// 全部采集完成事件
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

        #region 采集任务控制操作

        /// 增加运行区所有的任务
        public bool AddGatherTask(cTaskDataList taskDataList)
        {
            //根据运行区数据进行采集任务的添加
            //如果有任务加载出错，则忽略错误，继续加载，确保所有的任务都可以加载成功
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

        //增加单个任务
        public void AddGatherTask(cTaskData task)
        {
            m_TaskManage.Add(task);
        }

        
        /// 开始指定的采集任务
        public void Start(cGatherTask task)
        {
            if (!m_IsInitialized)
            {
                timerInit();
            }
            m_TaskManage.TaskListControl.StartTask(task);
        }
        
        /// 启动所有运行区的任务
        public void Start()
        {
            if (!m_IsInitialized)
            {
                timerInit();
            }
            m_TaskManage.TaskListControl.Start();
        }

        /// 停止所有运行区的任务
        public void Stop()
        {
            m_TaskManage.TaskListControl.Stop();
        }

        /// 停止指定的采集任务
        public void Stop(cGatherTask  task)
        {
            m_TaskManage.TaskListControl.StopTask(task);
        }
       
        /// 重新开始指定的采集任务
        public void ReStart(cGatherTask task)
        {
            m_TaskManage.TaskListControl.ReStartTask(task);
        }
   
        /// 删除指定的采集任务
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

        #region IDisposable 成员
        private bool m_disposed;
        /// <summary>
        /// 释放由 Download 的当前实例使用的所有资源
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
                    // 在此释放托管资源
                    //if (m_GatherWatch != null)
                    //{
                    //    m_GatherWatch.Dispose();
                    //}
                    if (m_GatherEngine != null)
                    {
                        m_GatherEngine.Dispose();
                    }
                }

                // 在此释放非托管资源

                m_disposed = true;
            }
        }


        #endregion
    }

    
}
