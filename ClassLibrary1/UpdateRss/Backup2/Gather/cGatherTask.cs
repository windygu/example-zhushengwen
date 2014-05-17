using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using SoukeyNetget.Gather;
using System.Xml;
using System.Threading;
using SoukeyNetget.Task;
using SoukeyNetget.Log;
using SoukeyNetget.Listener;
using SoukeyNetget.Plan;

///功能：采集任务处理
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace SoukeyNetget.Gather
{
    ///采集任务类,根据一个采集任务(即一个Task.xml文件)

    public class cGatherTask
    {
        private cTaskData m_TaskData;
        private cGlobalParas.TaskState m_State;
        private List<cGatherTaskSplit> m_list_GatherTaskSplit;
        private bool m_IsDataInitialized;
        private bool m_IsInitialized;
        private cGatherManage m_TaskManage;

        private bool m_ThreadsRunning = false;

        #region 构造 析构 

        /// 初始化采集任务对象,并根据任务数据确定此任务是否
        /// 由多个线程来完成,如果是,则进行任务分解
        internal cGatherTask(cGatherManage taskManage, cTaskData taskData)
        {
            m_TaskManage = taskManage;
            m_TaskData = taskData;
            m_State = TaskData.TaskState;

            m_list_GatherTaskSplit = new List<cGatherTaskSplit>();
            
            //当任务数据传进来之后,直接对当前任务进行任务分解,
            //是否需要多线程进行,并初始化相关数据内容
            SplitTask();

            //开始初始化任务
            TaskInit();
        }
        #endregion


        #region 属性

        /// <summary>
        /// 事件 线程同步锁
        /// </summary>
        private readonly Object m_eventLock = new Object();
        /// <summary>
        /// 文件 线程同步锁
        /// </summary>
        private readonly Object m_fstreamLock = new Object();
        public cGatherManage TaskManage
        {
            get { return m_TaskManage; }
        }

        public bool IsInitialized
        {
            get { return m_IsInitialized; }
            set { m_IsInitialized = value; }
        }

        public bool ThreadsRunning
        {
            get { return m_ThreadsRunning; }
        }

        /// <summary>
        /// 设置/获取 当前任务数据对象
        /// </summary>
        internal cTaskData TaskData
        {
            get { return m_TaskData; }
        }
        /// <summary>
        /// 获取采集网址的个数
        /// </summary>
        public int UrlCount
        {
            get { return m_TaskData.UrlCount; }
        }

        /// <summary>
        /// 获取实际需要采集的网址数量
        /// </summary>
        public int TrueUrlCount
        {
            get { return m_TaskData.TrueUrlCount; }
        }

        /// <summary>
        /// 设置/获取 单任务采集的线程数
        /// </summary>
        public int ThreadCount
        {
            //get { return m_TaskData.ThreadCount; }
            set { m_TaskData.ThreadCount = value; }
        }
        /// <summary>
        /// 获取已完成采集任务的数量
        /// </summary>
        public int GatheredUrlCount
        {
            get { return m_TaskData.GatheredUrlCount; }
        }

        public int GatheredTrueUrlCount
        {
            get { return m_TaskData.GatheredTrueUrlCount; }
        }

        /// <summary>
        /// 获取采集失败网址的数量
        /// </summary>
        public int GatherErrUrlCount
        {
            get { return m_TaskData.GatherErrUrlCount; }
        }

        public int GatheredTrueErrUrlCount
        {
            get { return m_TaskData.GatheredTrueErrUrlCount; }
        }

        /// <summary>
        /// 设置/获取 任务ID
        /// </summary>
        public Int64 TaskID
        {
            get { return m_TaskData.TaskID; }
            //set { m_TaskData.TaskID = value; }
        }

        /// <summary>
        /// 设置/获取 任务名
        /// </summary>
        public string TaskName
        {
            get { return m_TaskData.TaskName; }
            //set { m_TaskData.TaskName = value; }
        }
        /// <summary>
        /// 获取网页的地址 集合
        /// </summary>
        public List<Task.cWebLink> Weblink
        {
            get { return m_TaskData.Weblink; }
        }

        public List<Task.cWebpageCutFlag> GatherFlag
        {
            get { return  m_TaskData.CutFlag; }
        }

        public string TempFile
        {
            get { return m_TaskData.tempFileName; }
        }

        public cGlobalParas.PublishType  PublishType
        {
            get { return m_TaskData.PublishType; }
        }

        public bool IsLogin
        {
            get { return m_TaskData.IsLogin; }
        }

        public string ExportFileName
        {
            get { return m_TaskData.ExportFileName; }
        }

        public cGlobalParas.TaskType TaskType
        {
            get { return m_TaskData.TaskType; }
        }
       
        /// <summary>
        /// 获取采集任务的运行类型
        /// </summary>
        public cGlobalParas.TaskRunType RunType
        {
            get { return m_TaskData.RunType; }
        }
        /// <summary>
        /// 是否已经采集完成
        /// </summary>
        public bool IsCompleted
        {
            get { return GatheredUrlCount ==UrlCount ; }
        }

        /// <summary>
        /// 分解任务类 结合
        /// </summary>
        public List<cGatherTaskSplit> TaskSplit
        {
            get { return m_list_GatherTaskSplit; }
            set { m_list_GatherTaskSplit = value; }
        }

        #endregion


        #region 事件触发 任务状态触发
        /// 任务状态改变的事件触发
        /// /// 设置/获取 任务状态 （仅内部使用，触发事件）
        /// 
        public cGlobalParas.TaskState TaskState
        {
            get { return m_State; }
        }

        public cGlobalParas.TaskState State
        {
            get { return m_State; }
            set
            {
                cGlobalParas.TaskState tmp = m_State;
                m_State = value;
                TaskStateChangedEventArgs evt = null;

                if (e_TaskStateChanged != null)
                {
                    
                    evt = new TaskStateChangedEventArgs(TaskID, tmp, m_State);
                    e_TaskStateChanged(this, evt);
                }

                // 注意，所以涉及任务状态变更的事件都在此处理
                bool cancel = (evt != null && evt.Cancel);

                switch (m_State)
                {
                    case cGlobalParas.TaskState.Aborted:
                        // 触发 任务强制停止取消 事件
                        //任务强制停止，任然保存数据，但有可能会丢失数据因为在此系统要推出，系统会忽略
                        //所有错误
                        if (e_TaskAborted != null)
                        {
                            Save();
                            e_TaskAborted(this, new cTaskEventArgs(TaskID,TaskName,cancel));
                        }
                        break;
                    case cGlobalParas.TaskState.Completed:
                        // 触发 任务完成 事件
                        if (e_TaskCompleted != null)
                        {
                            //当任务停止后，开始保存任务的执行状态
                            Save();

                            e_TaskCompleted(this, new cTaskEventArgs(TaskID, TaskName, cancel));
                        }
                        break;
                    case cGlobalParas.TaskState.Failed:
                        // 触发 任务失败 事件
                        if (e_TaskFailed != null)
                        {
                            //任务失败
                            Save();
                            e_TaskFailed(this, new cTaskEventArgs(TaskID, TaskName, cancel));
                        }
                        break;
                    case cGlobalParas.TaskState.Started:
                        // 触发 任务开始 事件
                        m_TaskManage.EventProxy.AddEvent(delegate()
                        {
                            if (e_TaskStarted != null)
                            {
                                e_TaskStarted(this, new cTaskEventArgs(TaskID, TaskName, cancel));
                            }
                        });
                        break;
                    case cGlobalParas.TaskState.Stopped:
                        m_TaskManage.EventProxy.AddEvent(delegate()
                        {
                            //WriteToFile();
                            // 触发 任务停止 事件
                            if (e_TaskStopped != null)
                            {
                                //当任务停止后，开始保存任务的执行状态
                                Save();

                                e_TaskStopped(this, new cTaskEventArgs(TaskID, TaskName, cancel));
                            }
                        });
                        break;
                    case cGlobalParas.TaskState.Waiting:
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region 事件
        
        /// 采集任务完成事件
        private event EventHandler<cTaskEventArgs> e_TaskCompleted;
        internal event EventHandler<cTaskEventArgs> TaskCompleted
        {
            add { lock (m_eventLock) { e_TaskCompleted += value; } }
            remove { lock (m_eventLock) { e_TaskCompleted -= value; } }
        }

        /// 采集任务采集失败事件
        private event EventHandler<cTaskEventArgs> e_TaskFailed;
        internal event EventHandler<cTaskEventArgs> TaskFailed
        {
            add { lock (m_eventLock) { e_TaskFailed += value; } }
            remove { lock (m_eventLock) { e_TaskFailed -= value; } }
        }
        
        /// 采集任务开始事件
        private event EventHandler<cTaskEventArgs> e_TaskStarted;
        internal event EventHandler<cTaskEventArgs> TaskStarted
        {
            add { lock (m_eventLock) {  e_TaskStarted += value; } }
            remove { lock (m_eventLock) {  e_TaskStarted -= value; } }
        }

        /// 采集任务停止事件
        private event EventHandler<cTaskEventArgs> e_TaskStopped;
        internal event EventHandler<cTaskEventArgs> TaskStopped
        {
            add { lock (m_eventLock) {  e_TaskStopped += value; } }
            remove { lock (m_eventLock) {  e_TaskStopped -= value; } }
        }

        /// 采集任务取消事件
        private event EventHandler<cTaskEventArgs> e_TaskAborted;
        internal event EventHandler<cTaskEventArgs> TaskAborted
        {
            add { lock (m_eventLock) { e_TaskAborted += value; } }
            remove { lock (m_eventLock) { e_TaskAborted -= value; } }
        }

        /// 采集任务错误事件
        private event EventHandler<TaskErrorEventArgs> e_TaskError;
        internal event EventHandler<TaskErrorEventArgs> TaskError
        {
            add { lock (m_eventLock) {  e_TaskError += value; } }
            remove { lock (m_eventLock) {  e_TaskError -= value; } }
        }

        /// 任务状态变更事件,每当任务状态发生变更时进行处理,
        /// 并触发界面此事件,用于界面状态的改变
        private event EventHandler<TaskStateChangedEventArgs> e_TaskStateChanged;
        internal event EventHandler<TaskStateChangedEventArgs> TaskStateChanged
        {
            add { lock (m_eventLock) {  e_TaskStateChanged += value; } }
            remove { lock (m_eventLock) {  e_TaskStateChanged -= value; } }
        }


        /// 采集任务分解初始化完成事件
        private event EventHandler<TaskInitializedEventArgs> e_TaskThreadInitialized;
        internal event EventHandler<TaskInitializedEventArgs> TaskThreadInitialized
        {
            add { lock (m_eventLock) {  e_TaskThreadInitialized += value; } }
            remove { lock (m_eventLock) { e_TaskThreadInitialized -= value; } }
        }

        /// <summary>
        /// 采集日志事件
        /// </summary>
        private event EventHandler<cGatherTaskLogArgs> e_Log;
        internal event EventHandler<cGatherTaskLogArgs> Log
        {
            add { e_Log += value; }
            remove { e_Log -= value; }
        }

        /// <summary>
        /// 采集数据事件
        /// </summary>
        private event EventHandler<cGatherDataEventArgs> e_GData;
        internal event EventHandler<cGatherDataEventArgs> GData
        {
            add { e_GData += value; }
            remove { e_GData -= value; }
        }

        /// <summary>
        /// 定义一个执行Soukey采摘任务的事件，用于响应触发器执行Soukey采摘任务时
        /// 的处理。
        /// </summary>
        private event EventHandler<cRunTaskEventArgs> e_RunTask;
        internal event EventHandler<cRunTaskEventArgs> RunTask
        {
            add { lock (m_eventLock) { e_RunTask += value; } }
            remove { lock (m_eventLock) { e_RunTask -= value; } }
        }

        #endregion

        #region 任务控制 启动 停止 重启 取消

        /// 开始任务
        public void Start()
        {
            // 确保位初始化的任务先进行初始化（包括从文件读取的任务信息）
            if (m_State !=cGlobalParas.TaskState.Started && m_TaskManage != null)
            {
                m_TaskData.GatheredUrlCount = 0;
                m_TaskData.GatherErrUrlCount = 0;

                //m_TaskData.GatheredTrueUrlCount = 0;
                //m_TaskData.GatheredTrueErrUrlCount = 0;

                m_ThreadsRunning = true;

                TaskInit();
                StartAll();
            }
        }

        /// 启动所有采集任务线程 如果没有分解任务,则启动的单个线程进行采集
        private void StartAll()
        {
            foreach (cGatherTaskSplit dtc in m_list_GatherTaskSplit)
            {
                dtc.TaskSplitData.TrueUrlCount = dtc.TaskSplitData.UrlCount;
                dtc.TaskSplitData.GatheredUrlCount = 0;
                dtc.TaskSplitData.GatheredTrueUrlCount = 0;
                dtc.TaskSplitData.GatheredErrUrlCount = 0;
                dtc.TaskSplitData.GatheredTrueErrUrlCount = 0;
                
                dtc.Start();
            }
            State = cGlobalParas.TaskState.Started;
        }

        /// 任务准备就绪（等待开始）
        public void ReadyToStart()
        {
            if (m_State != cGlobalParas.TaskState.Started && m_State != cGlobalParas.TaskState.Completed)
            {
                State = cGlobalParas.TaskState.Waiting;
            }
        }

        /// 停止任务
        public void Stop()
        {
            m_ThreadsRunning = false;

            foreach (cGatherTaskSplit dtc in m_list_GatherTaskSplit)
            {
                dtc.Stop();
            }

            //开始检测是否所有线程都以完成或退出
            bool isStop = false;

            while (!isStop)
            {
                isStop = true;

                foreach (cGatherTaskSplit dtc in m_list_GatherTaskSplit)
                {
                    //if (dtc.WorkThread.ThreadState == cGlobalParas.GatherThreadState.Started && dtc.WorkThread.IsAlive )
                    
                    if (dtc.IsStop ==false )
                        isStop = false;
                }
               
                Thread.Sleep(200);
            }

            State = cGlobalParas.TaskState.Stopped;
        }

        //停止任务，此停止任务与Stop不同的是，强制停止所有工作线程
        //Stop是属于执行完一个完整工作后停止，而Abort不论是否执行到
        //何种状态，必须强制停止，此种方式会导致数据丢失
        /// 取消任务（移除任务）
        public void Abort()
        {
            m_ThreadsRunning = false;

            foreach (cGatherTaskSplit dtc in m_list_GatherTaskSplit)
            {
                dtc.Abort();
            }
            State = cGlobalParas.TaskState.Aborted;
        }

        ///保存运行的任务，主要是保存当前运行的状态
        ///任务保存需要同时保存taskrun.xml，主要是保存采集数量
        ///注意，如果进行暂存后，任务的链接地址会发生变化，因为在任务新建时，任务链接地址有可能带有一定得参数
        ///但任务一旦开始执行，带有参数的网址就会进行解析，同时是按照解析后的网址进行是否采集的标识，所以，再次
        ///保存后，链接地址会很多
        public void Save()
        {
            string FileName = Program.getPrjPath() + "tasks\\run\\task" + this.TaskID + ".xml"; 
            string runFileindex= Program.getPrjPath() + "tasks\\taskrun.xml";

            //开始保存文件
            string tXml="";

            for (int i = 0; i < m_TaskData.Weblink.Count ; i++)
            {
                tXml += "<WebLink>";
                tXml += "<Url>" + cTool.ReplaceTrans(m_TaskData.Weblink[i].Weblink.ToString ()) + "</Url>";
                tXml += "<IsNag>" + m_TaskData.Weblink[i].IsNavigation + "</IsNag>";
                //tXml += "<IsOppPath>" + m_TaskData.Weblink[i].IsOppPath + "</IsOppPath>";
                //tXml += "<NagRule>" + cTool.ReplaceTrans(m_TaskData.Weblink[i].NagRule) + "</NagRule>";
                tXml += "<IsNextPage>" + m_TaskData.Weblink[i].IsNextpage + "</IsNextPage>";
                tXml += "<NextPageRule>" + cTool.ReplaceTrans(m_TaskData.Weblink[i].NextPageRule) + "</NextPageRule>";
                tXml += "<IsGathered>" + (int)m_TaskData.Weblink[i].IsGathered + "</IsGathered>";

                //保存采集地地址是否需要导航
                //插入此网址的导航规则
                if (m_TaskData.Weblink[i].IsNavigation == true)
                {
                    tXml += "<NavigationRules>";
                    for (int j = 0; j < m_TaskData.Weblink[i].NavigRules.Count; j++)
                    {
                        tXml += "<Rule>";
                        tXml += "<Url>" + cTool.ReplaceTrans(m_TaskData.Weblink[i].NavigRules[j].Url) + "</Url>";
                        tXml += "<Level>" + m_TaskData.Weblink[i].NavigRules[j].Level + "</Level>";
                        tXml += "<NagRule>" + cTool.ReplaceTrans(m_TaskData.Weblink[i].NavigRules[j].NavigRule) + "</NagRule>";
                        tXml += "</Rule>";
                    }
                    tXml += "</NavigationRules>";
                }

                tXml += "</WebLink>";
            }

            cXmlIO cxml = new cXmlIO(FileName);
            cxml.DeleteNode("WebLinks");
            cxml.InsertElement("Task", "WebLinks", tXml);
            cxml.Save();

            cxml = null;

            cxml = new cXmlIO(runFileindex);
            cxml.EditTaskrunValue(this.TaskID.ToString(), cGlobalParas.TaskState.Stopped , this.GatheredUrlCount.ToString(),this.GatheredTrueUrlCount.ToString () , this.GatherErrUrlCount.ToString(),this.GatheredTrueErrUrlCount.ToString () ,this.TrueUrlCount.ToString () );
            cxml.Save();
            cxml = null;
        }

        
        #endregion

        #region  类方法 内部使用

        //根据指定的任务ID对当前的任务进行分解，如果有导航页，也需要在此进行
        //分解
        //并初始化此任务的关键数据
        private void SplitTask()
        {
            cGatherTaskSplit dtc;
            List<Task.cWebLink> tWeblink;
            Task.cTask t= new Task.cTask();
            //m_TaskData.TaskID = e.TaskID;

            //根据指定的TaskID加载任务地址信息
            try
            {
                t.LoadTask(Int64.Parse (m_TaskData.TaskID.ToString ()));
            }
            catch (System.Exception )
            {
                //调试实体文件加载失败，有可能是文件丢失所造成
                //但还是需要加载一个空信息，以便界面可以显示此丢失的任务
                //这样用户可以通过界面操作删除此任务内容，这是一个针对
                //丢失文件的处理手段
                m_TaskData.SavePath = "";
                m_TaskData.TaskDemo = "";
                m_TaskData.StartPos = "";
                m_TaskData.EndPos = "";
                m_TaskData.Cookie = "";
                m_TaskData.WebCode =cGlobalParas.WebCode.auto ;
                m_TaskData.IsLogin = false ;
                m_TaskData.LoginUrl = "";
                m_TaskData.PublishType = cGlobalParas.PublishType.NoPublish ;
                m_TaskData.IsUrlEncode =false ;
                m_TaskData.UrlEncode = "";
                m_TaskData.Weblink = null;
                m_TaskData.CutFlag = null;

                return;

            }

            ////加载页面的采集起始位置和终止位置
            ///此两项数据不在taskrun中存储，是在任务的xml文件中存储
            ///但m_TaskData是按照taskrun来加载的数据，所以无法加载此两
            ///项值和采集页面的规则及网址。
            ///为什么从taskrun中加载，是因为在索引taskrun的时候可以显示界面
            ///信息，所以就共用了一个加载信息的内容
            m_TaskData.SavePath = t.SavePath;
            m_TaskData.TaskDemo =t.TaskDemo ;
            m_TaskData.StartPos = t.StartPos;
            m_TaskData.EndPos = t.EndPos;
            m_TaskData.Cookie = t.Cookie;
            m_TaskData.WebCode = (cGlobalParas.WebCode) int.Parse ( t.WebCode);
            m_TaskData.IsLogin = t.IsLogin;
            m_TaskData.LoginUrl = t.LoginUrl;
            m_TaskData.PublishType = (cGlobalParas.PublishType) int.Parse (t.ExportType);
            m_TaskData.IsUrlEncode = t.IsUrlEncode;
            m_TaskData.UrlEncode = t.UrlEncode;

            m_TaskData.GatherAgainNumber = t.GatherAgainNumber;
            m_TaskData.IsIgnore404 = t.IsIgnore404;
            m_TaskData.IsErrorLog = t.IsErrorLog;
            m_TaskData.IsDelRepRow = t.IsDelRepRow;
            m_TaskData.IsTrigger = t.IsTrigger;
            if (t.IsTrigger == true)
            {
                m_TaskData.TriggerType = t.TriggerType;
                m_TaskData.TriggerTask = t.TriggerTask;
            }

            ////加载网页地址数据及采集标志数据
            ////再次去处理如果带有参数的网址,则需要进行分解
            ////确保加载的网址肯定是一个有效的网址
            ////注意,此时由于有可能分解任务信息,所以,网址数量在此会发生变化,所以,最终还需修改网址数据
            Task.cWebLink w;
            Task.cUrlAnalyze u = new Task.cUrlAnalyze();

            for (int i = 0; i < t.WebpageLink.Count; i++)
            {
                if (Regex.IsMatch(t.WebpageLink[i].Weblink.ToString(), "{.*}"))
                {
                    List<string> Urls;

                    if (m_TaskData.IsUrlEncode == true)
                    {
                        Urls = u.SplitWebUrl(t.WebpageLink[i].Weblink.ToString());
                    }
                    else
                    {
                        Urls = u.SplitWebUrl(t.WebpageLink[i].Weblink.ToString());
                    }
                    
                    //开始添加m_TaskData.weblink数据
                    for (int j=0;j<Urls.Count ;j++)
                    {
                        w = new Task.cWebLink();
                        w.IsGathered = t.WebpageLink[i].IsGathered;
                        w.IsNavigation = t.WebpageLink[i].IsNavigation;
                        w.IsNextpage = t.WebpageLink[i].IsNextpage;
                        w.NextPageRule = t.WebpageLink[i].NextPageRule;
                        w.Weblink = Urls[j].ToString();

                        //加载导航数据
                        if (t.WebpageLink[i].IsNavigation == true)
                        {
                            w.NavigRules = t.WebpageLink[i].NavigRules;
                        }

                        m_TaskData.Weblink.Add(w);
                        w = null;
                    }

                }
                else
                {
                    m_TaskData.Weblink.Add(t.WebpageLink[i]);
                }

            }

            u=null;

            m_TaskData.CutFlag = t.WebpageCutFlag;

            string sPath = m_TaskData.SavePath + "\\" + m_TaskData.TaskName + "_file";

            //重新初始化UrlCount
            //m_TaskData.UrlCount = m_TaskData.Weblink.Count;

            //开始进行任务分块,但此任务的Url数必须大于线程数,且线程数>1
            if (m_TaskData.UrlCount > m_TaskData.ThreadCount && m_TaskData.ThreadCount > 1)
            {
                int SplitUrlCount = (int)Math.Ceiling((decimal)m_TaskData.UrlCount / (decimal)m_TaskData.ThreadCount);

                //设置每个分解任务的起始Url索引和终止的Url索引
                int StartIndex = 0;
                int EndIndex = 0;
                int j = 0;

                //for (int i = 1; i <= SplitUrlCount; i++)
                for (int i = 1; i <= m_TaskData.ThreadCount; i++)
                {
                    StartIndex = EndIndex;
                    if (i == m_TaskData.ThreadCount)
                    {
                        EndIndex = m_TaskData.Weblink.Count;
                    }
                    else
                    {
                        //EndIndex = i * m_TaskData.ThreadCount;
                        EndIndex = i * SplitUrlCount;
                    }

                    //初始化分解采集任务类
                    dtc = new cGatherTaskSplit();
                    dtc.TaskManage =m_TaskManage;
                    dtc.TaskID =m_TaskData.TaskID;
                    dtc.WebCode =m_TaskData.WebCode;
                    dtc.IsUrlEncode =m_TaskData.IsUrlEncode;
                    dtc.UrlEncode =m_TaskData.UrlEncode;
                    dtc.Cookie = m_TaskData.Cookie;
                    dtc.StartPos =m_TaskData.StartPos;
                    dtc.EndPos =m_TaskData.EndPos;
                    dtc.SavePath =sPath;
                    dtc.AgainNumber = m_TaskData.GatherAgainNumber;
                    dtc.Ignore404 = m_TaskData.IsIgnore404;
                    dtc.IsErrorLog = m_TaskData.IsErrorLog;

                    tWeblink = new List<Task.cWebLink>();

                    for (j = StartIndex; j < EndIndex; j++)
                    {
                        tWeblink.Add(m_TaskData.Weblink[j]);

                    }

                    //初始化分解的子任务数据
                    dtc.SetSplitData(StartIndex, EndIndex - 1, tWeblink, m_TaskData.CutFlag);

                    m_TaskData.TaskSplitData.Add(dtc.TaskSplitData);

                    tWeblink = null;
                    dtc = null;

                }

            }
            else
            {
                //初始化分解采集任务类
                dtc = new cGatherTaskSplit();
                dtc.TaskManage = m_TaskManage;
                dtc.TaskID = m_TaskData.TaskID;
                dtc.WebCode = m_TaskData.WebCode;
                dtc.IsUrlEncode = m_TaskData.IsUrlEncode;
                dtc.UrlEncode = m_TaskData.UrlEncode;
                dtc.Cookie = m_TaskData.Cookie;
                dtc.StartPos = m_TaskData.StartPos;
                dtc.EndPos = m_TaskData.EndPos;
                dtc.SavePath = sPath;
                dtc.AgainNumber = m_TaskData.GatherAgainNumber;
                dtc.Ignore404 = m_TaskData.IsIgnore404;
                dtc.IsErrorLog = m_TaskData.IsErrorLog;


                dtc.SetSplitData(0, m_TaskData.UrlCount - 1, m_TaskData.Weblink, m_TaskData.CutFlag);
                m_TaskData.TaskSplitData.Add(dtc.TaskSplitData);
                //m_list_GatherTaskSplit.Add(dtc);
            }

            t = null;
            dtc = null;
        }

        //更新cookie的值，cookie的值会根据用户的输入情况发生变化
        public void UpdateCookie(string cookie)
        {
            this.TaskData.Cookie = cookie;

            foreach (cGatherTaskSplit tp in m_list_GatherTaskSplit)
            {
                tp.UpdateCookie(cookie);
            }

        }

        /// 初始化采集任务线程
        private void TaskInit()
        {
            string sPath = m_TaskData.SavePath + "\\" + m_TaskData.TaskName + "_file";

            ///任务初始化分为两种情况，一种是未启动执行的任务，一种是已经启动但未执行完毕的任务
            ///
            //m_TaskData.GatheredUrlCount = 0;
            //m_TaskData.GatherErrUrlCount = 0;
            //m_TaskData.TrueUrlCount = m_TaskData.UrlCount;

            if (!m_IsDataInitialized)
            {
                if (m_list_GatherTaskSplit.Count > 0)
                {   // 清理可能存在的子线程
                    foreach (cGatherTaskSplit dtc in m_list_GatherTaskSplit)
                    {
                        dtc.Stop();
                    }
                    m_list_GatherTaskSplit.Clear();
                }

                if (IsCompleted)
                {   
                    // 修改此采集任务的状态为已采集完成,设置为状态为已完成，需要出发事件
                    m_State = cGlobalParas.TaskState.Completed;

                    //m_State = cGlobalParas.TaskState.Completed;

                    //e_TaskCompleted(this, new cTaskEventArgs(m_TaskData.TaskID, false));
                }
                else
                {
                    cGatherTaskSplit dtc;

                    if (m_TaskData.TaskSplitData.Count  > 0)
                    {   

                        foreach (cTaskSplitData configData in m_TaskData.TaskSplitData)
                        {
                            dtc = new cGatherTaskSplit();
                            dtc.TaskManage = m_TaskManage;
                            dtc.TaskID = m_TaskData.TaskID;
                            dtc.WebCode = m_TaskData.WebCode;
                            dtc.IsUrlEncode = m_TaskData.IsUrlEncode;
                            dtc.UrlEncode = m_TaskData.UrlEncode;
                            dtc.Cookie = m_TaskData.Cookie;
                            dtc.StartPos = m_TaskData.StartPos;
                            dtc.EndPos = m_TaskData.EndPos;
                            dtc.SavePath = sPath;
                            dtc.AgainNumber = m_TaskData.GatherAgainNumber;
                            dtc.Ignore404 = m_TaskData.IsIgnore404;
                            dtc.IsErrorLog = m_TaskData.IsErrorLog;

                            dtc.TaskSplitData = configData;

                            m_list_GatherTaskSplit.Add(dtc);

                            dtc = null;
                        }

                    }
                    else
                    {
                        dtc = new cGatherTaskSplit();
                        dtc.TaskManage = m_TaskManage;
                        dtc.TaskID = m_TaskData.TaskID;
                        dtc.WebCode = m_TaskData.WebCode;
                        dtc.IsUrlEncode = m_TaskData.IsUrlEncode;
                        dtc.UrlEncode = m_TaskData.UrlEncode;
                        dtc.Cookie = m_TaskData.Cookie;
                        dtc.StartPos = m_TaskData.StartPos;
                        dtc.EndPos = m_TaskData.EndPos;
                        dtc.SavePath = sPath;
                        dtc.AgainNumber = m_TaskData.GatherAgainNumber;
                        dtc.Ignore404 = m_TaskData.IsIgnore404;
                        dtc.IsErrorLog = m_TaskData.IsErrorLog;


                        // 新任务，则新建子线程
                        m_list_GatherTaskSplit.Add(dtc);

                        dtc = null;
                    }


                    foreach (cGatherTaskSplit TaskSplit in m_list_GatherTaskSplit)
                    {   
                        // 初始化所有子线程
                        TaskEventInit(TaskSplit);
                    }
                }

                m_IsDataInitialized = true;
            }
        }

        //将分解任务事件进行绑定
        private void TaskEventInit(cGatherTaskSplit dtc)
        {
            if (!dtc.IsInitialized)
            {
                // 绑定 初始化事件、完成事件
                dtc.TaskInit += this.TaskWorkThreadInit;
                dtc.Completed += this.TaskWorkThreadCompleted;
                dtc.GUrlCount += this.onGUrlCount;
                dtc.Log += this.onLog;
                dtc.GData += this.onGData;
                dtc.Error += this.TaskThreadError;
                dtc.IsInitialized = true;
            }
        }

        /// 重置任务为未初始化状态
        internal void ResetTaskState()
        {
            e_TaskCompleted = null;
            e_TaskStarted = null;
            e_TaskError = null;
            e_TaskStateChanged = null;
            e_TaskStopped = null;
            e_TaskFailed = null;
            e_TaskAborted = null;
            e_TaskThreadInitialized = null;
            this.State = cGlobalParas.TaskState.UnStart;

            m_IsInitialized = false;

            e_Log = null;
            e_GData = null;
        }

        /// 重置采集任务为未启动状态
        internal void ResetTaskData()
        {
            // 停止任务
            //Stop();

            m_TaskData.GatheredUrlCount = 0;
            m_TaskData.GatherErrUrlCount = 0;

            m_TaskData.GatheredTrueUrlCount = 0;
            m_TaskData.GatheredTrueErrUrlCount = 0;

            //修改taskrun文件中，此文件索引的采集地址和出错地址为0
            string runFileindex = Program.getPrjPath() + "tasks\\taskrun.xml";
            cXmlIO cxml = new cXmlIO(runFileindex);
            cxml = new cXmlIO(runFileindex);

            //还原数据需要将实际需要采集的网址数量初始化为UrlCount
            cxml.EditTaskrunValue(this.TaskID.ToString(),cGlobalParas.TaskState.UnStart , "0","0","0","0",m_TaskData.UrlCount.ToString () );
            cxml.Save();
            cxml = null;

            string tXml = "";

            for (int i = 0; i < m_TaskData.Weblink.Count; i++)
            {
                tXml += "<WebLink>";
                tXml += "<Url>" + cTool.ReplaceTrans(m_TaskData.Weblink[i].Weblink.ToString()) + "</Url>";
                tXml += "<IsNag>" + m_TaskData.Weblink[i].IsNavigation + "</IsNag>";
                tXml += "<IsNextPage>" + m_TaskData.Weblink[i].IsNextpage + "</IsNextPage>";
                tXml += "<NextPageRule>" + cTool.ReplaceTrans(m_TaskData.Weblink[i].NextPageRule) + "</NextPageRule>";
                tXml += "<IsGathered>" + (int)cGlobalParas.UrlGatherResult.UnGather + "</IsGathered>";

                if (m_TaskData.Weblink[i].IsNavigation == true)
                {
                    tXml += "<NavigationRules>";
                    for (int j = 0; j < m_TaskData.Weblink[i].NavigRules.Count; j++)
                    {
                        tXml += "<Rule>";
                        tXml += "<Url>" + m_TaskData.Weblink[i].NavigRules[j].Url + "</Url>";
                        tXml += "<Level>" + m_TaskData.Weblink[i].NavigRules[j].Level + "</Level>";
                        tXml += "<NagRule>" + m_TaskData.Weblink[i].NavigRules[j].NavigRule + "</NagRule>";
                        tXml += "</Rule>";
                    }
                    tXml += "</NavigationRules>";
                }

                tXml += "</WebLink>";

                m_TaskData.Weblink[i].IsGathered = (int) cGlobalParas.UrlGatherResult.UnGather ;
 
            }

            string FileName = Program.getPrjPath() + "tasks\\run\\task" + m_TaskData.TaskID + ".xml";
            cXmlIO cxml1 = new cXmlIO(FileName);
            cxml1.DeleteNode("WebLinks");
            cxml1.InsertElement("Task", "WebLinks", tXml);
            cxml1.Save();
            cxml1 = null;

            //删除临时存储的采集数据xml文件
            string tmpFileName = m_TaskData.SavePath + "\\" + m_TaskData.TaskName + "-" + m_TaskData.TaskID + ".xml";
            if (File.Exists(tmpFileName))
            {
                File.Delete(tmpFileName);
            }

            Task.cTaskRun t = new Task.cTaskRun();
            t.LoadSingleTask(m_TaskData.TaskID);
            m_TaskData.UrlCount = t.GetUrlCount(0);
            t = null;

            //m_TaskData.TaskSplitData.Clear ();
            //m_IsDataInitialized = false;
        }

        /// 清理任务数据
        internal void Remove()
        {
            ResetTaskState();
        }

        //重置任务
        public void ResetTask()
        {
            ResetTaskData();
        }

        #endregion

        #region 公有方法

        /// 将所有缓存数据写入文件
        public void WriteToFile()
        {

        }


        #endregion

        #region  响应分解采集任务(子线程)事件

        /// 任务初始化,由分解任务触发,
        private void TaskWorkThreadInit(object sender, TaskInitializedEventArgs e)
        {
            cGatherTaskSplit dtc = (cGatherTaskSplit)sender;
            m_TaskData.TaskID  =e.TaskID ;

            if (e_TaskThreadInitialized != null)
            {
                // 代理触发 任务初始化 事件
                m_TaskManage.EventProxy.AddEvent(delegate()
                {
                    e_TaskThreadInitialized(this, new TaskInitializedEventArgs(m_TaskData.TaskID));
                });
            }
            
        }

        /// 分解采集任务 线程完成 事件处理 判断的是独立线程，每个线程完成后
        /// 都需要触发任务完成事件，交由任务继续判断，如果完成则调用任务完成
        /// 事件，告诉程序此任务已经完成
        private void TaskWorkThreadCompleted(object sender, cTaskEventArgs e)
        {

            cGatherTaskSplit dtc = (cGatherTaskSplit)sender;
            if (dtc.UrlCount  == dtc.GatherErrUrlCount +dtc.GatheredUrlCount )
            {  
                // 任务采集完成
                onTaskCompleted();
            }
        }

        /// 当某个线程采集完成后，会调用任务完成事件进行检测，如果任务完成则触发任务
        /// 完成事件。但在此判断时需要注意，如果任务采集失败的数量和采集数量相等，则判断
        /// 任务失败。且每次调用此事件时，都需要做一次检测，对每个子线程都检测一遍，看是否
        /// 存在已经完成，但未触发完成事件的自线程。
        private void onTaskCompleted()
        {
            if (m_TaskData.UrlCount == (m_TaskData.GatheredUrlCount + m_TaskData.GatherErrUrlCount) && m_State != cGlobalParas.TaskState.Completed)
            {
                if (m_TaskData.TrueUrlCount == m_TaskData.GatherErrUrlCount)
                {
                    //如果全部采集都发生了错误，则此任务为失败
                    State = cGlobalParas.TaskState.Failed ;
                }
                else
                {
                    // 设置为完成状态，触发任务完成事件
                    State = cGlobalParas.TaskState.Completed;
                }

                //无论失败还是成功都要进行触发器的触发
                if (m_TaskData.IsTrigger == true && m_TaskData.TriggerType == ((int)cGlobalParas.TriggerType.GatheredRun).ToString ())
                {
                    cRunTask rt = new cRunTask();
                    rt.RunSoukeyTaskEvent += this.onRunSoukeyTask;

                    cTaskPlan p;
                    
                    for (int i=0;i<m_TaskData.TriggerTask.Count ;i++)
                    {
                        p=new cTaskPlan ();

                        p.RunTaskType =m_TaskData.TriggerTask[i].RunTaskType ;
                        p.RunTaskName =m_TaskData.TriggerTask[i].RunTaskName ;
                        p.RunTaskPara =m_TaskData.TriggerTask[i].RunTaskPara ;

                        rt.AddTask (p);

                    }

                    rt.RunSoukeyTaskEvent -= this.onRunSoukeyTask;
                    rt = null;
                    
                }
            }
        }

        //处理触发器执行Soukey采摘任务
        private void onRunSoukeyTask(object sender, cRunTaskEventArgs e)
        {
            e_RunTask(this, new cRunTaskEventArgs(e.MessType, e.RunName, e.RunPara));
        }

        /// 处理 分解采集任务 错误事件
        private void TaskThreadError(object sender, TaskThreadErrorEventArgs e)
        {
            //当采集发生错误后，系统首先需要检测当前是否连接网络
            //如果没有连接网络，即无Internet，则系统停止此任务执行
            if (cTool.IsLinkInternet() == false)
            {
                Stop();

                m_State = cGlobalParas.TaskState.Failed;

                if (e_TaskFailed != null)
                {
                    e_TaskFailed(this, new cTaskEventArgs(TaskID, TaskName, false));
                }

                return;

            }


            cGatherTaskSplit gt = (cGatherTaskSplit)sender;

            //如果出错调用此事件,也表示完成了一个网址的采集,但是出错了


            //一个线程发生错误并不允许停止整个任务执行，即便所有线程都发生促务
            //也需要保障任务执行，只是把任务出错信息写入日志

            //if (gt.ErrorCount >= cGatherManage.MaxErrorCount)
            //{   
                // 达到最大错误数，停止当前线程
                //bool failed = true;

                // 如果当前任务所有的线程都停止了，则判断为任务失败
                //foreach (cGatherTaskSplit dtc in m_list_GatherTaskSplit)
                //{
                //    if (!gt.Equals(dtc) && dtc.IsThreadAlive)
                //    {
                //        failed = false;
                //        break;
                //    }
                //}
                //if (failed)
                //{
                //    State = cGlobalParas.TaskState.Failed;
                //    return;
                //}
            //}
            

           

            if (e_TaskError != null)
            {
                e_TaskError(this, new TaskErrorEventArgs(gt, e.Error));
            }
            //}
        }

        //处理日志事件
        public void onLog(object sender, cGatherTaskLogArgs e)
        {
            if (e_TaskStarted != null && !e.Cancel)
            {
                e_Log(sender, e);
            }

            //在此处理是否写入错误数据到日志的请求
            if (e.IsSaveErrorLog == true)
            {
                cErrLog eLog = new cErrLog();
                eLog.WriteLog(this.TaskName, cGlobalParas.LogType.GatherError, e.strLog);
                eLog = null;
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

        //采集网址计数事件,每调用此事件一次,则加一,表示采集完成一个网址
        public void onGUrlCount(object sender,cGatherUrlCountArgs e)
        {

            switch (e.UType)
            {
                case cGlobalParas.UpdateUrlCountType .Gathered :
                    m_TaskData.GatheredTrueUrlCount++;

                    break;
                case cGlobalParas.UpdateUrlCountType.Err :
                    m_TaskData.GatheredTrueErrUrlCount++;
                    break;
                case cGlobalParas.UpdateUrlCountType.ReIni :
                    m_TaskData.TrueUrlCount += e.TrueUrlCount - 1;
                    break;
                case cGlobalParas.UpdateUrlCountType.ErrUrlCountAdd :
                    m_TaskData.GatherErrUrlCount++;
                    break;
                case cGlobalParas.UpdateUrlCountType.UrlCountAdd :
                    m_TaskData.GatheredUrlCount++;
                    break;
            }
        }
        #endregion

        
    }
}
