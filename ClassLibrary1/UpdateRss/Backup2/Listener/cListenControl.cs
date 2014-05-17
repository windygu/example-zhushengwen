using System;
using System.Collections.Generic;
using System.Text;

///���ܣ���������������Ҫ���ں�̨����ļ���
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ǰ��Ϊ��̨�����������ͨ��ʱ�������д�����Ĭ�ϼ��ʱ��Ϊ1���� 
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget.Listener
{
    class cListenControl
    {
        
        private cListenManage m_ListenManage;

        //���������б�־
        private bool m_IsRunning = true;

        //����һ���߳�����ִ�м�ز���

        #region ���������
        public cListenControl()
        {
            m_ListenManage = new cListenManage();
        }

        ~cListenControl()
        {
            
            m_ListenManage.Dispose();


        }

        #endregion

        #region ����������
        
        public cListenManage ListenManage
        {
            get { return m_ListenManage; }
            set { m_ListenManage = value; }
        }
        #endregion

        #region ��������
        //����������
        public void Start()
        {
            m_ListenManage.Start();
            m_IsRunning = true;
        }

        public void Stop()
        {
            m_ListenManage.Stop();
            m_IsRunning = false;
        }
        #endregion

        public bool IsRunning
        {
            get { return m_IsRunning; }
            set { m_IsRunning = value; }
        }
     
    }
}
