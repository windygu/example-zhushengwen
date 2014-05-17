using System;
using System.Collections.Generic;
using System.Text;
using SoukeyNetget.Listener;

///功能：发布任务管理 启动任务 响应事件 此功能实现的很简单
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：下一步需要完善发布功能模块，势必此功能要继续完善
///说明：无 
///版本：01.10.00
///修订：无
namespace SoukeyNetget.publish
{
    class cPublishManage
    {
        List<cPublishTask> m_ListPublish;
        private Gather.cEventProxy m_EventProxy;

        public cPublishManage()
        {
            m_ListPublish = new List<cPublishTask>();
            m_EventProxy = new Gather.cEventProxy();
        }

        ~cPublishManage()
        {
        }

        public List<cPublishTask> ListPublish
        {
            get { return m_ListPublish; }
        }

        public void AddPublishTask(cPublishTask pt)
        {
            //添加到对列中
            ListPublish.Add(pt);
            TaskInit(pt);

            //启动此任务
            pt.startPublic();
        }

        public void AddSaveTempDataTask(cPublishTask pt)
        {
            ListPublish.Add(pt);
            TaskTempSaveInit(pt);

            //启动此任务
            pt.startSaveTempData();
        }

        //注册临时存储任务的事件，系统自动执行，无需用户干预
        private void TaskTempSaveInit(cPublishTask pTask)
        {
            if (pTask.PublishManage.Equals(this))
            {
                pTask.PublishTempDataCompleted += this.onPublishTempDataCompleted;
            }
        }

        //注册发布任务的事件
        private void TaskInit(cPublishTask pTask)
        {

            if (pTask.PublishManage.Equals(this))
            {
                pTask.PublishCompleted  += this.onPublishCompleted;
                pTask.PublishFailed  += this.onPublishFailed;
                pTask.PublishStarted  += this.onPublishStarted;
                pTask.PublishError  += this.onPublishError;
                pTask.PublishTempDataCompleted += this.onPublishTempDataCompleted;
                pTask.PublishLog += this.onPublishLog;
                pTask.RunTask += this.onRunSoukeyTask;
            }
        }

        private void onPublishLog(object sender, PublishLogEventArgs e)
        {
            if (e_PublishLog != null && !e.Cancel)
            {
                e_PublishLog(sender, e);
            }

        }

        private void onPublishCompleted(object sender, PublishCompletedEventArgs e)
        {

            //从当前列表中删除此记录
            cPublishTask pt = (cPublishTask)sender;
            m_ListPublish.Remove(pt);
            pt = null;

            if (e_PublishCompleted != null && !e.Cancel)
            {
                e_PublishCompleted(sender, e);
            }

        }

        private void onPublishFailed(object sender, PublishFailedEventArgs e)
        {
            //从当前列表中删除此记录
            cPublishTask pt = (cPublishTask)sender;
            m_ListPublish.Remove(pt);
            pt = null;

            if (e_PublishFailed != null && !e.Cancel)
            {
                e_PublishFailed(sender, e);
            }

        }

        private void onPublishStarted(object sender, PublishStartedEventArgs e)
        {
            if (e_PublishStarted != null && !e.Cancel)
            {
                e_PublishStarted(sender, e);
            }
        }

        private void onPublishError(object sender, PublishErrorEventArgs e)
        { 
            //从当前列表中删除此记录
            cPublishTask pt = (cPublishTask)sender;
            m_ListPublish.Remove(pt);
            pt = null;

            if (e_PublishError != null && !e.Cancel)
            {
                e_PublishError(sender, e);
            }

        }

        private void onPublishTempDataCompleted(object sender, PublishTempDataCompletedEventArgs e)
        {

            //从当前列表中删除此记录，临时数据的保存也是作为一个发布任务来执行的
            //所以，保存完毕后，需要删除此任务
            cPublishTask pt = (cPublishTask)sender;
            m_ListPublish.Remove(pt);
            pt = null;

            if (e_PublishTempDataCompleted != null && !e.Cancel)
            {
                e_PublishTempDataCompleted(sender, e);
            }
        }

        private void onRunSoukeyTask(object sender, cRunTaskEventArgs e)
        {
            e_RunTask(this, new cRunTaskEventArgs(e.MessType, e.RunName, e.RunPara));
        }

        #region 事件

        /// 发布任务 完成事件
        private event EventHandler<PublishCompletedEventArgs> e_PublishCompleted;
        public event EventHandler<PublishCompletedEventArgs> PublishCompleted
        {
            add { e_PublishCompleted += value; }
            remove { e_PublishCompleted -= value; }
        }

        /// 发布任务 失败事件
        private event EventHandler<PublishFailedEventArgs> e_PublishFailed;
        public event EventHandler<PublishFailedEventArgs> PublishFailed
        {
            add { e_PublishFailed += value; }
            remove { e_PublishFailed -= value; }
        }

        /// 发布任务 开始采集事件
        private event EventHandler<PublishStartedEventArgs> e_PublishStarted;
        public event EventHandler<PublishStartedEventArgs> PublishStarted
        {
            add { e_PublishStarted += value; }
            remove { e_PublishStarted -= value; }
        }

        ///发布任务 错误事件
        private event EventHandler<PublishErrorEventArgs> e_PublishError;
        public event EventHandler<PublishErrorEventArgs> PublishError
        {
            add { e_PublishError += value; }
            remove { e_PublishError -= value; }
        }

        ///临时发布数据完成事件
        private event EventHandler<PublishTempDataCompletedEventArgs> e_PublishTempDataCompleted;
        public event EventHandler<PublishTempDataCompletedEventArgs> PublishTempDataCompleted
        {
            add { e_PublishTempDataCompleted += value; }
            remove { e_PublishTempDataCompleted -= value; }
        }

        //任务发布日志事件
        private event EventHandler<PublishLogEventArgs> e_PublishLog;
        public event EventHandler<PublishLogEventArgs> PublishLog
        {
            add { e_PublishLog += value; }
            remove { e_PublishLog -= value; }
        }

        /// <summary>
        /// 定义一个执行Soukey采摘任务的事件，用于响应触发器执行Soukey采摘任务时
        /// 的处理。
        /// </summary>
        private event EventHandler<cRunTaskEventArgs> e_RunTask;
        internal event EventHandler<cRunTaskEventArgs> RunTask
        {
            add { e_RunTask += value;  }
            remove { e_RunTask -= value;  }
        }
        #endregion
    }
}
