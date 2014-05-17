using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using SoukeyNetget.Listener;

///功能：采集任务管理 管理队列 绑定 响应事件 控制任务 
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace SoukeyNetget.Gather
{
    //采集任务管理
    public class cGatherManage
    {

        private List<cGatherTask> m_list_Task;
        private cTaskDataList m_TaskDataList;
        private cGatherTaskList m_GatherTaskList;
        private cEventProxy m_EventProxy;

        #region 构造
        public cGatherManage()
        {
            m_list_Task = new List<cGatherTask>();
            m_TaskDataList = new cTaskDataList();
            m_GatherTaskList = new cGatherTaskList();
            m_EventProxy = new cEventProxy();
        }
        #endregion

        #region 静态变量

        /// 线程最大错误次数，当达到这个数量则自动停止任务，
        /// 前提条件是系统必须检测是断网，如果没有断网，则引发错误事件
        /// 但不停止任务执行
        private static int s_MaxErrorCount = 10;
        public static int MaxErrorCount
        {
            get { return cGatherManage.s_MaxErrorCount; }
            set { cGatherManage.s_MaxErrorCount = value; }
        }

        #endregion

        #region 属性

        /// 获取当前事件代理对象
        internal cEventProxy EventProxy
        {
            get { return m_EventProxy; }
        }

        /// 事件 线程同步锁
        private readonly Object m_taskListFileLock = new Object();

        /// 文件 线程同步锁
        private readonly Object m_taskFileLock = new Object();

        /// 获取当前 任务列表
        /// （包括所有状态的任务对象）
        public List<cGatherTask> TaskList
        {
            get { return m_list_Task; }
        }

        /// 获取当前 采集任务队列控制器
        public cGatherTaskList TaskListControl
        {
            get
            {
                if (m_GatherTaskList == null)
                {
                    m_GatherTaskList = new cGatherTaskList();
                }
                return m_GatherTaskList;
            }
        }

        #endregion

        //从当前的采集任务列表中,按照指定的TaskID查找
        //一个采集任务,并返回

        public cGatherTask FindTask(Int64 TaskID)
        {
            foreach (cGatherTask gt in m_list_Task )
            {
                if (gt.TaskID ==TaskID )
                {
                    return gt;
                }
            }
            return null;
        }

        #region 任务控制 增加任务 任务初始化
        /// 向采集任务队列中增加一个采集任务
        public void Add(cTaskData tData)
        {
            try
            {
                //新建一个采集任务,并把采集任务的数据传入此采集任务中
                cGatherTask gTask = new cGatherTask(this, tData);

                //初始化此采集任务,主要是注册此任务的相关事件
                TaskInit(gTask);

                //判断此任务是否已经加入此任务数据集合,如果没有加入,则加入集合
                if (!m_TaskDataList.TaskDataList.Contains(tData))
                {
                    m_TaskDataList.TaskDataList.Add(tData);
                }

                //将此采集任务添加到采集任务队列中
                m_list_Task.Add(gTask);

                //根据添加的任务状态,自动维护队列的信息
                m_GatherTaskList.AutoList(gTask);

                //如果任务增加后就是完成的任务，则需要出发完成的
                //事件
                if (gTask.TaskState == cGlobalParas.TaskState.Completed)
                {
                    e_TaskCompleted(gTask, new cTaskEventArgs(gTask.TaskID, gTask.TaskName, false));
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        private void TaskInit(cGatherTask gTask)
        {
            if (gTask.TaskManage.Equals(this))
            {
                if (!gTask.IsInitialized)
                {
                    gTask.TaskCompleted += this.onTaskCompleted;
                    gTask.TaskFailed += this.onTaskFailed;
                    gTask.TaskStopped += this.onTaskStopped;
                    gTask.TaskStarted += this.onTaskStarted;
                    gTask.TaskAborted += this.onTaskAborted;
                    gTask.Log += this.onLog;
                    gTask.GData += this.onGData;
                    gTask.TaskError += this.onTaskError;
                    gTask.TaskStateChanged += this.onTaskStateChanged;
                    gTask.TaskThreadInitialized += this.onTaskThreadInitialized;

                    gTask.RunTask += this.onRunSoukeyTask;

                    gTask.IsInitialized = true;
                }
            }
        }

        //任务强制终止，销毁事件关联，不让其返回任何信息
        public void TaskEventDispose(cGatherTask gTask)
        {
            if (gTask.TaskManage.Equals(this))
            {
                
                    gTask.TaskCompleted -= this.onTaskCompleted;
                    gTask.TaskFailed -= this.onTaskFailed;
                    gTask.TaskStopped -= this.onTaskStopped;
                    gTask.TaskStarted -= this.onTaskStarted;
                    gTask.TaskAborted -= this.onTaskAborted;
                    gTask.Log -= this.onLog;
                    gTask.GData -= this.onGData;
                    gTask.TaskError -= this.onTaskError;
                    gTask.TaskStateChanged -= this.onTaskStateChanged;
                    gTask.RunTask -= this.onRunSoukeyTask;
                    gTask.TaskThreadInitialized -= this.onTaskThreadInitialized;

               
            }
        }

        #endregion

        #region 事件

        /// 采集任务 完成事件
        private event EventHandler<cTaskEventArgs> e_TaskCompleted;
        public event EventHandler<cTaskEventArgs> TaskCompleted
        {
            add { e_TaskCompleted += value; }
            remove { e_TaskCompleted -= value; }
        }

        /// 采集任务 失败事件
        private event EventHandler<cTaskEventArgs> e_TaskFailed;
        public event EventHandler<cTaskEventArgs> TaskFailed
        {
            add { e_TaskFailed += value; }
            remove { e_TaskFailed -= value; }
        }

        /// 采集任务 开始采集事件
        private event EventHandler<cTaskEventArgs> e_TaskStarted;
        public event EventHandler<cTaskEventArgs> TaskStarted
        {
            add { e_TaskStarted += value; }
            remove { e_TaskStarted -= value; }
        }

        /// 采集任务 停止事件
        private event EventHandler<cTaskEventArgs> e_TaskStopped;
        public event EventHandler<cTaskEventArgs> TaskStopped
        {
            add { e_TaskStopped += value; }
            remove { e_TaskStopped -= value; }
        }

        /// 采集任务 取消事件
        private event EventHandler<cTaskEventArgs> e_TaskAborted;
        public event EventHandler<cTaskEventArgs> TaskAborted
        {
            add { e_TaskAborted += value; }
            remove { e_TaskAborted -= value; }
        }

        ///采集任务 错误事件
        private event EventHandler<TaskErrorEventArgs> e_TaskError;
        public event EventHandler<TaskErrorEventArgs> TaskError
        {
            add { e_TaskError += value; }
            remove { e_TaskError -= value; }
        }

        /// 采集任务状态 变更事件
        private event EventHandler<TaskStateChangedEventArgs> e_TaskStateChanged;

        public event EventHandler<TaskStateChangedEventArgs> TaskStateChanged
        {
            add { e_TaskStateChanged += value; }
            remove { e_TaskStateChanged -= value; }
        }

        /// 采集任务 初始化事件
        private event EventHandler<TaskInitializedEventArgs> e_TaskInitialized;
        public event EventHandler<TaskInitializedEventArgs> TaskInitialized
        {
            add { e_TaskInitialized += value; }
            remove { e_TaskInitialized -= value; }
        }

        /// <summary>
        /// 采集日志事件
        /// </summary>
        private event EventHandler<cGatherTaskLogArgs> e_Log;
        internal event EventHandler<cGatherTaskLogArgs> Log
        {
            add {  e_Log += value;  }
            remove {  e_Log -= value;  }
        }

        /// <summary>
        /// 采集数据事件
        /// </summary>
        private event EventHandler<cGatherDataEventArgs> e_GData;
        internal event EventHandler<cGatherDataEventArgs> GData
        {
            add {  e_GData += value;  }
            remove { e_GData -= value;  }
        }

        /// <summary>
        /// 触发器执行Soukey采摘任务时的事件响应
        /// </summary>
        private event EventHandler<cRunTaskEventArgs> e_RunTask;
        internal event EventHandler<cRunTaskEventArgs> RunTask
        {
            add {  e_RunTask += value;  }
            remove {  e_RunTask -= value; }
        }
        #endregion

        #region 事件处理

        /// <summary>
        /// 处理任务触发器响应执行Soukey采摘任务时的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onRunSoukeyTask(object sender, cRunTaskEventArgs e)
        {
            e_RunTask(this, new cRunTaskEventArgs(e.MessType, e.RunName, e.RunPara));
        }

        /// 处理 任务完成 事件
        private void onTaskCompleted(object sender, cTaskEventArgs e)
        {
            if (e_TaskCompleted != null && !e.Cancel)
            {
                e_TaskCompleted(sender, e);
            }

            // 将任务对象添加到已完成的任务队列，等待任务管理器处理
            m_GatherTaskList.FinishTask((cGatherTask)sender);

            //将完成的信息写入taskrun

        }
        /// <summary>
        /// 处理 任务失败 事件
        /// </summary>
        /// <param name="sender">触发事件的任务</param>
        /// <param name="e"></param>
        private void onTaskFailed(object sender, cTaskEventArgs e)
        {
            if (e_TaskFailed != null && !e.Cancel)
            {
                e_TaskFailed(sender, e);
            }
        }

        /// <summary>
        /// 采集错误 事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void onTaskError(object sender, TaskErrorEventArgs e)
        {
            // 加入重试队列
            //m_GatherTaskList.AddWaitingWorkThread(e.ErrorThread);

            if (e_TaskError != null)
            {
                e_TaskError(sender, e);
            }
        }

        private void onTaskStarted(object sender, cTaskEventArgs e)
        {
            if (e_TaskStarted != null && !e.Cancel)
            {
                e_TaskStarted(sender, e);
            }
        }

        private void onTaskStopped(object sender, cTaskEventArgs e)
        {
            if (e_TaskStopped != null && !e.Cancel)
            {
                e_TaskStopped(sender, e);
            }
        }

        private void onTaskAborted(object sender, cTaskEventArgs e)
        {
            cGatherTask task = (cGatherTask)sender;
            // 从任务列表删除
            m_list_Task.Remove(task);
            m_TaskDataList.TaskDataList.Remove(task.TaskData);

            if (e_TaskAborted != null && !e.Cancel)
            {
                e_TaskAborted(sender, e);
            }
        }

        /// <summary>
        /// 处理 任务状态变更 事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onTaskStateChanged(object sender, TaskStateChangedEventArgs e)
        {
            if (e_TaskStateChanged != null && !e.Cancel)
            {
                e_TaskStateChanged(sender, e);
            }

            // 重要：此处处理所有状态变更后任务队列的变更
            m_GatherTaskList.AutoList((cGatherTask)sender);
        }

        /// <summary>
        /// 处理任务线程初始化完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onTaskThreadInitialized(object sender, TaskInitializedEventArgs e)
        {
            if (e_TaskInitialized != null)
            {
                e_TaskInitialized(sender, e);
            }
            // 保存当前任务采集的状态
            //this.SaveTaskList();
            //this.SaveTask((cGatherTask)sender);
        }

        //处理日志事件
        public void onLog(object sender, cGatherTaskLogArgs e)
        {
            if (e_TaskStarted != null && !e.Cancel)
            {
                e_Log(sender, e);
            }
        }

        //处理数据事件
        public void onGData(object sender, cGatherDataEventArgs e)
        {
            if (e_TaskStarted != null && !e.Cancel)
            {
                e_GData(sender, e);
            }
        }

        #endregion
    }
}
