using System;
using System.Collections.Generic;
using System.Text;

///功能：监听控制器，主要用于后台服务的监听
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：当前作为后台服务监听，仅通过时间对其进行触发，默认间隔时间为1分钟 
///版本：01.10.00
///修订：无
namespace SoukeyNetget.Listener
{
    class cListenControl
    {
        
        private cListenManage m_ListenManage;

        //监听器运行标志
        private bool m_IsRunning = true;

        //定义一个线程用于执行监控操作

        #region 构造和析构
        public cListenControl()
        {
            m_ListenManage = new cListenManage();
        }

        ~cListenControl()
        {
            
            m_ListenManage.Dispose();


        }

        #endregion

        #region 监听管理器
        
        public cListenManage ListenManage
        {
            get { return m_ListenManage; }
            set { m_ListenManage = value; }
        }
        #endregion

        #region 监听控制
        //启动控制器
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
