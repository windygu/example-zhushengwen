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

        //����������ģ��򴥷��¼�
        protected void OnChanged(object sender, FileSystemEventArgs e)   
        {
            m_IsTrigger = true;

            //�������ö�ʱ���Ĵ�����������ҽ�������һ��
            m_timer.Change(TimeoutMillis, Timeout.Infinite);

            mType = cGlobalParas.MessageType.ReloadPlan;
        }

        //����ļ�ʧ��ʱ�䰴�����д���
        protected void OnError(object sender, ErrorEventArgs e)
        {
            m_IsTrigger = true;

            //�������ö�ʱ���Ĵ�����������ҽ�������һ��
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

            //������ʱ��
            if (m_timer == null)
            {
                //���ö�ʱ���Ļص���������ʱ��ʱ��δ����
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

        #region �¼�
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
