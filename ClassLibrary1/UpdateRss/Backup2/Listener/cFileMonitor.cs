using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace SoukeyNetget.Listener
{
    class cFileMonitor
    {
      
        private string m_MonitorFile;
        private FileSystemWatcher Fw;
        private System.Threading.Timer m_timer;
        private bool m_IsMonitoring=false;
        private int TimeoutMillis = 2000;
        private bool m_IsTrigger=false ;
        private cGlobalParas.MessageType mType=cGlobalParas.MessageType.ReloadPlan ;

        public cFileMonitor(string monitorFile)
        {
            m_MonitorFile = monitorFile;
         
        }

        //如果发生更改，则触发事件
        protected void OnChanged(object sender, FileSystemEventArgs e)   
        {
            m_IsTrigger = true;

            //重新设置定时器的触发间隔，并且仅仅触发一次
            m_timer.Change(TimeoutMillis, Timeout.Infinite);

            mType = cGlobalParas.MessageType.ReloadPlan;
        }

        //监控文件失败时间按，进行触发
        protected void OnError(object sender, ErrorEventArgs e)
        {
            m_IsTrigger = true;

            //重新设置定时器的触发间隔，并且仅仅触发一次
            m_timer.Change(TimeoutMillis, Timeout.Infinite);

            mType = cGlobalParas.MessageType.MonitorFileFaild;
        }

        public void Start()
        {
            string path = Path.GetDirectoryName(m_MonitorFile);
            string baseName = Path.GetFileName(m_MonitorFile);

            Fw = new FileSystemWatcher(path, baseName);
            Fw.NotifyFilter = NotifyFilters.LastWrite;
            Fw.Changed += this.OnChanged;
            Fw.Error += this.OnError;
            Fw.EnableRaisingEvents = true;

            //启动定时器
            if (m_timer == null)
            {
                //设置定时器的回调函数。此时定时器未启动
                m_timer = new System.Threading.Timer(new TimerCallback(m_OnWatchedFileChange_CallBack), null, Timeout.Infinite, Timeout.Infinite);
            }

            m_IsMonitoring = true;

        }

        public void Stop()
        {
            Fw = null;
            m_timer.Dispose();

            m_IsMonitoring = false;
        }

        private void m_OnWatchedFileChange_CallBack(object state)
        {

            if (m_IsTrigger == true)
            {
                e_ReloadPlanFile(this, new cCommandEventArgs(mType));
                m_IsTrigger = false;
            }

        }


        void timerTick(object sender, EventArgs e)
        {
            //UpdateBasedOnFileTime();
        }

        private void ClearTimer()
        {
            //if (null != timer)
            //{
            //    timer.Tick -= timerTick;
            //    timer.Dispose();
            //    timer = null;
            //}
        }

        private readonly Object m_eventLock = new Object();

        #region 事件
        ///
        private event EventHandler<cCommandEventArgs> e_ReloadPlanFile;
        internal event EventHandler<cCommandEventArgs> ReloadPlanFile
        {
            add { lock (m_eventLock) { e_ReloadPlanFile += value; } }
            remove { lock (m_eventLock) { e_ReloadPlanFile -= value; } }
        }

        #endregion


    }
}
