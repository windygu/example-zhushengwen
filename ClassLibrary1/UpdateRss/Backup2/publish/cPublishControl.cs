using System;
using System.Collections.Generic;
using System.Text;

///功能：发布任务入口 事件定义
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace SoukeyNetget.publish
{
    class cPublishControl : IDisposable 
    {

        //发布做的简单了，实际应该按照采集的模式来进行，可以进行各种监控
        //并且如果发布的 数据量大的时候可以多线程发布，但总感觉这种发布模式
        //实用性不是很大，所以就先做一个发布功能，可以让系统跑起来，感觉一下
        //试用的效果,后面会慢慢进行修改.

        private cPublishManage m_PublishManage;

        public cPublishControl()
        {
            m_PublishManage = new cPublishManage();
        }

        ~cPublishControl()
        {
        }

        public cPublishManage PublishManage
        {
            get { return m_PublishManage; }
        }
        
        //增加发布任务,用于发布数据，同时启动此任务
        public void startPublish(cPublishTask pT)
        {
            m_PublishManage.AddPublishTask(pT );
        }

        //增加发布任务,用于发布临时采集的数据，同时启动此任务
        public void startSaveTempData(cPublishTask pT)
        {
            m_PublishManage.AddSaveTempDataTask(pT);
        }

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
                 
                  
                }

                // 在此释放非托管资源

                m_disposed = true;
            }
        }


        #endregion

    }




}
