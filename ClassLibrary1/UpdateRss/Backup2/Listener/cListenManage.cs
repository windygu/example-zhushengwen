using System;
using System.Collections.Generic;
using System.Text;
using SoukeyNetget.Plan;

///���ܣ����������� 
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
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
            //��ʼ������������
            m_rTask.RunSoukeyTaskEvent += this.onRunSoukeyTask;

            //��ʼ����������
            m_checkPlan.AddRunTaskEvent += this.OnAddRunTask ;
            m_checkPlan.ListenErrorEvent += this.OnListenError;


            timerInit();
        }

        ~cListenManage()
        {
            Dispose(false);
        }

        #region ��ʱ��
        /// ��ʼ��timer��ʱ��,��ǰ������Threading.timer��ʱ��
        /// �˼�ʱ����Ҫ�Ƕ��ڼ��������´�ִ��ʱ�䣬�������ִ��ʱ��
        /// ������������Ĭ�ϵļ���ʱ����Ϊ1����
        private void timerInit()
        {
            m_LastTime = System.Environment.TickCount;
            m_ListenEngine = new System.Threading.Timer(new System.Threading.TimerCallback(m_ListenEngine_CallBack), null, 0, 6000);
            m_IsInitialized = true;
            m_Isbusying = false;
        }

        /// ������Ҫ�����ļƻ�����
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

        #region ���� ֹͣ
        public void Start()
        {
            m_IsRunning = true;
        }

        public void Stop()
        {
            m_IsRunning = false;
        }
        #endregion

        #region ��Ӧ�¼�����
        //��Ӧ�������������¼�
        private void OnAddRunTask(object sender, cAddRunTaskEventArgs e)
        {
            //�������Ӻ󣬻��Զ�����
            m_rTask.AddTask(e.RunTask);

            //�жϴ˼ƻ��Ƿ���ڣ�������ڣ����޸ļƻ���״̬
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

        #region �¼�

        /// �ɼ���������¼�
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

                    if (m_ListenEngine != null)
                    {
                        m_ListenEngine.Dispose();
                    }
                }

                // �ڴ��ͷŷ��й���Դ

                m_disposed = true;
            }
        }


        #endregion
    }
}
