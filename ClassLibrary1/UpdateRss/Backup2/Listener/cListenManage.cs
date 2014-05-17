using System;
using System.Collections.Generic;
using System.Text;
using SoukeyNetget.Plan;

///功能：监听管理器 
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace SoukeyNetget.Listener
{
    class cListenManage
    {
        private bool m_IsInitialized;

        private int m_LastTime;
        private bool m_Isbusying;
        private System.Threading.Timer m_ListenEngine;
        private bool m_IsRunning = false;
        private cCheckPlan m_checkPlan= new cCheckPlan();
        private cRunTask m_rTask= new cRunTask();
        

        public cListenManage()
        {
            //初始化运行任务类
            m_rTask.RunSoukeyTaskEvent += this.onRunSoukeyTask;

            //初始化任务检测类
            m_checkPlan.AddRunTaskEvent += this.OnAddRunTask ;
            m_checkPlan.ListenErrorEvent += this.OnListenError;


            timerInit();
        }

        ~cListenManage()
        {
            Dispose(false);
        }

        #region 计时器
        /// 初始化timer计时器,当前采用了Threading.timer计时器
        /// 此计时器主要是定期监控任务的下次执行时间，如果到了执行时间
        /// 则启动此任务，默认的监听时间间隔为1分钟
        private void timerInit()
        {
            m_LastTime = System.Environment.TickCount;
            m_ListenEngine = new System.Threading.Timer(new System.Threading.TimerCallback(m_ListenEngine_CallBack), null, 0, 6000);
            m_IsInitialized = true;
            m_Isbusying = false;
        }

        /// 处理需要启动的计划任务
        private void m_ListenEngine_CallBack(object State)
        {

            if (m_Isbusying == false && m_IsRunning == true)
            {
                m_Isbusying = true;

                try
                {
                    m_checkPlan.CheckPlan();
                }
                catch (System.Exception ex)
                {
                    e_ListenError(this, new cListenErrorEventArgs(ex.Message));
                }

                m_Isbusying = false;
            }
        }

        #endregion

        #region 启动 停止
        public void Start()
        {
            m_IsRunning = true;
        }

        public void Stop()
        {
            m_IsRunning = false;
        }
        #endregion

        #region 响应事件处理
        //相应新增运行任务事件
        private void OnAddRunTask(object sender, cAddRunTaskEventArgs e)
        {
            //任务增加后，会自动运行
            m_rTask.AddTask(e.RunTask);

            //判断此计划是否过期，如果过期，则修改计划的状态
            cPlans cp = new cPlans();
            cp.IfEnabled(Int64.Parse ( e.RunTask.PlanID),false);
            cp = null;
        }

        private void OnListenError(object sender, cListenErrorEventArgs e)
        {
            e_ListenError(this, new cListenErrorEventArgs(e.Message));
        }

        private void onRunSoukeyTask(object sender, cRunTaskEventArgs e)
        {
            e_RunTask(this,new cRunTaskEventArgs (e.MessType ,e.RunName ,e.RunPara ));
        }

        #endregion

        private readonly Object m_eventLock = new Object();

        #region 事件

        /// 采集任务完成事件
        private event EventHandler<cRunTaskEventArgs> e_RunTask;
        internal event EventHandler<cRunTaskEventArgs> RunTask
        {
            add { lock (m_eventLock) { e_RunTask += value; } }
            remove { lock (m_eventLock) { e_RunTask -= value; } }
        }

        private event EventHandler<cListenErrorEventArgs> e_ListenError;
        internal event EventHandler<cListenErrorEventArgs> ListenError
        {
            add { lock (m_eventLock) { e_ListenError += value; } }
            remove { lock (m_eventLock) { e_ListenError -= value; } }
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

                    if (m_ListenEngine != null)
                    {
                        m_ListenEngine.Dispose();
                    }
                }

                // 在此释放非托管资源

                m_disposed = true;
            }
        }


        #endregion
    }
}
