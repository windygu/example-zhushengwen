using System;
using System.Collections.Generic;
using System.Text;
using System.Threading ;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using SoukeyNetget.Task;

///���ܣ��ɼ����� �ֽ���������
///���ʱ�䣺2009-6-1
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget.Gather
{

    //���ǲɼ��������С��Ԫ,�����һ���ɼ��������,��һ��URL��ַ����
    //�����ж��������ǧ��URL��ַ,Ϊ���������,��URl�ɼ������˶��̵߳�
    //����ʽ,�������һ������Ҫ���в��,��ֵ������Ǵ������Ƶ��߳�
    //��,������ʼִ��.������Ҫ����ִ����ôһ����С��Ԫ������.

    public class cGatherTaskSplit : IDisposable 
    {
       
        private delegate void work();
        
        private Thread m_Thread;
        private bool m_ThreadRunning = false;

        #region �����࣬����ʼ���������

        ///�޸��˹��캯��
        public cGatherTaskSplit()
        {
            //m_TaskManage = TaskManage;
            //m_TaskID = TaskID;
            //m_gStartPos = StartPos;
            //m_gEndPos = EndPos;
            //m_SavePath = sPath;
            //m_Cookie = strCookie;
            //m_WebCode = webCode;
            //m_IsUrlEncode = IsUrlEncode;
            //m_UrlEncode = UrlEncode;
            m_TaskSplitData = new cTaskSplitData ();
            m_ThreadState = cGlobalParas.GatherThreadState.Stopped;

            //m_TaskType = TaskType;

            m_GatherData = new DataTable();
        }

        ////��ʾ�зֽ������
        //internal cGatherTaskSplit(cGatherManage TaskManage, Int64 TaskID, cGlobalParas.WebCode webCode, bool IsUrlEncode, string UrlEncode, string strCookie, string StartPos, string EndPos, string sPath, cTaskSplitData TaskSplitData,cGlobalParas.TaskType TaskType)
        //{
        //    m_TaskManage = TaskManage;
        //    m_TaskID = TaskID;
        //    m_gStartPos = StartPos;
        //    m_gEndPos = EndPos;
        //    m_SavePath = sPath;
        //    m_Cookie = strCookie;
        //    m_WebCode = webCode;
        //    m_IsUrlEncode = IsUrlEncode;
        //    m_UrlEncode = UrlEncode;
        //    m_TaskSplitData = TaskSplitData;
        //    //m_IsDataInitialized = true;
        //    m_ThreadState = cGlobalParas.GatherThreadState.Stopped;

        //    m_TaskType = TaskType;

        //    m_GatherData = new DataTable();
        //}

        public void UpdateCookie(string cookie)
        {
            m_Cookie = cookie;
        }

        ~cGatherTaskSplit()
        {
            Dispose(false);
        }

        #endregion


        #region ����


        /// <summary>
        /// �¼� �߳�ͬ����
        /// </summary>
        private readonly Object m_eventLock = new Object();
        /// <summary>
        /// ���� �߳�ͬ����
        /// </summary>
        private readonly Object m_mstreamLock = new Object();

      
        private int m_ErrorCount;
        public int ErrorCount
        {
            get { return m_ErrorCount; }
        }

        private bool m_IsInitialized;
        internal bool IsInitialized
        {
            get { return m_IsInitialized; }
            set { m_IsInitialized = value; }
        }

        private int m_Waittime;
        internal int Waittime
        {
            get { return m_Waittime; }
            set { m_Waittime = value; }
        }

        private Int64 m_TaskID;
        public Int64 TaskID
        {
            get { return m_TaskID; }
            set { m_TaskID = value; }
        }

        private string m_gStartPos;
        public string StartPos
        {
            get { return m_gStartPos; }
            set { m_gStartPos = value; }
        }

        private string m_gEndPos;
        public string EndPos
        {
            get { return m_gEndPos; }
            set { m_gEndPos = value; }
        }
        private string m_Cookie;
        public string Cookie
        {
            get { return m_Cookie; }
            set { m_Cookie = value; }
        }
        private cTaskSplitData m_TaskSplitData;
        public cTaskSplitData TaskSplitData
        {
            get { return m_TaskSplitData; }
            set { m_TaskSplitData = value; }
        }

        private cGlobalParas.TaskType m_TaskType;
        public cGlobalParas.TaskType TaskType
        {
            get { return m_TaskType; }
            set { m_TaskType = value; }
        }

        private cGlobalParas.WebCode m_WebCode;
        public cGlobalParas.WebCode WebCode
        {
            get { return m_WebCode; }
            set { m_WebCode = value; }
        }

        private bool m_IsUrlEncode;
        public bool IsUrlEncode
        {
            get { return m_IsUrlEncode; }
            set { m_IsUrlEncode = value; }
        }

        private string m_UrlEncode;
        public string UrlEncode
        {
            get { return m_UrlEncode; }
            set { m_UrlEncode = value; }
        }

        private cGatherManage m_TaskManage;
        public cGatherManage TaskManage
        {
            get { return m_TaskManage; }
            set { m_TaskManage = value; }
        }

        private DataTable m_GatherData;
        public DataTable GatherData
        {
            get { return m_GatherData; }
            set { m_GatherData = value; }
        }

        private string m_SavePath;
        public string SavePath
        {
            get { return m_SavePath; }
            set { m_SavePath = value; }
        }

        private int m_AgainNumber;
        public int AgainNumber
        {
            get { return m_AgainNumber; }
            set { m_AgainNumber = value; }
        }

        private bool m_Ignore404;
        public bool Ignore404
        {
            get { return m_Ignore404; }
            set { m_Ignore404 = value; }
        }

        private bool m_IsErrorLog;
        public bool IsErrorLog
        {
            get { return m_IsErrorLog; }
            set { m_IsErrorLog = value; }
        }

        //��δ��
        private bool m_IsDelRepRow;
        public bool IsDelRepRow
        {
            get { return m_IsDelRepRow; }
            set { m_IsDelRepRow = value; }
        }

         /// <summary>
        /// �ֽ�ɼ������Ƿ��Ѿ����
        /// </summary>
        public bool IsCompleted
        {
            //get { return m_TaskSplitData.GatheredUrlCount == m_TaskSplitData.UrlCount ; }
            get { return m_TaskSplitData.UrlCount > 1 && m_TaskSplitData.GatheredUrlCount + m_TaskSplitData.GatheredErrUrlCount  == m_TaskSplitData.TrueUrlCount; }
        }
        /// <summary>
        /// �ֽ�Ĳɼ������Ƿ��Ѿ��ɼ����
        /// </summary>
        public bool IsGathered
        {
            get { return m_TaskSplitData.UrlCount > 0 && m_TaskSplitData.GatheredUrlCount +m_TaskSplitData.GatheredErrUrlCount == m_TaskSplitData.TrueUrlCount ; }
        }
        /// <summary>
        /// ��ȡһ��״ֵ̬��ʾ��ǰ�߳��Ƿ��ںϷ�����״̬ [���߳��ڲ�ʹ��]
        /// </summary>
        private bool IsCurrentRunning
        {
            get { return ThreadState  ==cGlobalParas.GatherThreadState.Started  && Thread.CurrentThread.Equals(m_Thread); }
        }

        /// <summary>
        /// ��ȡ��ǰ�߳��Ƿ��Ѿ�ֹͣ��ע��IsThreadRunningֻ��һ����־
        /// ������ȷ���̵߳�״̬��ֻ�Ǹ����߳���Ҫֹͣ��
        /// IsStop�������ⲿ������˷ֽ������Ƿ�ֹͣʹ��
        /// </summary>
        public bool IsStop
        {
            get { return m_ThreadRunning ==false && this.IsCurrentRunning==false;  }
        }
        

        /// <summary>
        /// ��ǰ��Ĺ����߳�
        /// </summary>
        internal Thread WorkThread
        {
            get { return m_Thread; }
        }

        /// <summary>
        /// ��ȡ��ǰ�̵߳�ִ��״̬
        /// </summary>
        internal bool IsThreadAlive
        {
            get { return m_Thread != null && m_Thread.IsAlive; }
        }

        //public cTaskSplitData TaskSplitData
        //{
        //    get { return m_TaskSplitData; }
        //}
        /// <summary>
        /// �ֽ����������Ƿ��ѳ�ʼ��
        /// </summary>
        //public bool IsDataInitialized
        //{
        //    get { return m_IsDataInitialized; }
        //}

        /// <summary>
        /// ��ʼ�ɼ���ҳ��ַ������
        /// </summary>
        public int BeginIndex
        {
            get { return m_TaskSplitData.BeginIndex; }
        }

        /// <summary>
        /// �����ɼ���ҳ��ַ������
        /// </summary>
        public int EndIndex
        {
            get { return m_TaskSplitData.EndIndex ; }
        }

        /// <summary>
        /// ��ǰ���ڲɼ���ַ������
        /// </summary>
        public int CurIndex
        {
            get { return m_TaskSplitData.CurIndex; }
        }

        /// <summary>
        /// ��ǰ�ɼ���ַ
        /// </summary>
        public string CurUrl
        {
            get { return m_TaskSplitData.CurUrl; }
        }

        /// <summary>
        /// �Ѳɼ���ַ����
        /// </summary>
        public int GatheredUrlCount
        {
            get { return m_TaskSplitData.GatheredUrlCount; }
        }

        public int GatheredTrueUrlCount
        {
            get { return m_TaskSplitData.GatheredTrueUrlCount; }
        }

        /// <summary>
        /// �Ѳɼ����������ַ����
        /// </summary>
        public int GatherErrUrlCount
        {
            get { return m_TaskSplitData.GatheredErrUrlCount; }
        }

        public int GatheredTrueErrUrlCount
        {
            get { return m_TaskSplitData.GatheredTrueErrUrlCount; }
        }

        /// <summary>
        /// һ����Ҫ�ɼ�����ַ����
        /// </summary>
        public int UrlCount
        {
            get { return m_TaskSplitData.UrlCount; }
        }

        public int TrueUrlCount
        {
            get { return m_TaskSplitData.TrueUrlCount; }
        }

        #endregion


        #region �¼�
        /// <summary>
        /// �ɼ������ʼ���¼�
        /// </summary>
        private event EventHandler<TaskInitializedEventArgs> e_TaskInit;
        internal event EventHandler<TaskInitializedEventArgs> TaskInit
        {
            add { lock (m_eventLock) { e_TaskInit += value; } }
            remove { lock (m_eventLock) { e_TaskInit -= value; } }
        }

        /// <summary>
        /// �ֽ�����ɼ���ʼ�¼�
        /// </summary>
        private event EventHandler<cTaskEventArgs> e_Started;
        internal event EventHandler<cTaskEventArgs> Started
        {
            add { lock (m_eventLock) { e_Started += value; } }
            remove { lock (m_eventLock) { e_Started -= value; } }
        }

        /// <summary>
        /// �ֽ�����ֹͣ�¼�
        /// </summary>
        private event EventHandler<cTaskEventArgs> e_Stopped;
        internal event EventHandler<cTaskEventArgs> Stopped
        {
            add { lock (m_eventLock) { e_Stopped += value; } }
            remove { lock (m_eventLock) { e_Stopped -= value; } }
        }

        /// <summary>
        /// �ֽ�����ɼ�����¼�
        /// </summary>
        private event EventHandler<cTaskEventArgs> e_Completed;
        internal event EventHandler<cTaskEventArgs> Completed
        {
            add { lock (m_eventLock) { e_Completed += value; } }
            remove { lock (m_eventLock) { e_Completed -= value; } }
        }

        /// <summary>
        /// �ֽ�����ɼ������¼�
        /// </summary>       
        private event EventHandler<TaskThreadErrorEventArgs> e_Error;
        internal event EventHandler<TaskThreadErrorEventArgs> Error
        {
            add { lock (m_eventLock) { e_Error += value; } }
            remove { lock (m_eventLock) { e_Error -= value; } }
        }

        /// <summary>
        ///  �ֽ�����ɼ�ʧ���¼�
        /// </summary>
        private event EventHandler<cTaskEventArgs> e_Failed;
        internal event EventHandler<cTaskEventArgs> Failed
        {
            add { lock (m_eventLock) { e_Failed += value; } }
            remove { lock (m_eventLock) { e_Failed -= value; } }
        }

        /// <summary>
        /// д��־�¼�
        /// </summary>
        private event EventHandler<cGatherTaskLogArgs> e_Log;
        internal event EventHandler<cGatherTaskLogArgs> Log
        {
            add { lock (m_eventLock) { e_Log += value; } }
            remove { lock (m_eventLock) { e_Log -= value; } }
        }
        
        /// <summary>
        /// ���زɼ������¼�
        /// </summary>
        private event EventHandler<cGatherDataEventArgs> e_GData;
        internal event EventHandler<cGatherDataEventArgs> GData
        {
            add { lock (m_eventLock) { e_GData += value; } }
            remove { lock (m_eventLock) { e_GData -= value; } }
        }

        private event EventHandler<cGatherUrlCountArgs> e_GUrlCount;
        internal event EventHandler<cGatherUrlCountArgs> GUrlCount
        {
            add { lock (m_eventLock) { e_GUrlCount += value; } }
            remove { lock (m_eventLock) { e_GUrlCount -= value; } }
        }
        #endregion

        #region �¼���������
        //�¼�����������ͨ���ı�����״̬����ɵ�
        //ͨ����������״̬�ı�����������¼��������շ���������
        //������Ȩ��������

        /// <summary>
        /// ��ȡ��ǰ�߳�״̬
        /// </summary>    
        private cGlobalParas.GatherThreadState m_ThreadState;

        /// <summary>
        /// ����/��ȡ �߳�״̬ �����ڲ�ʹ�ã������¼���
        /// </summary>
        protected cGlobalParas.GatherThreadState ThreadState
        {
            get { return m_ThreadState; }
            set
            {
                m_ThreadState = value;
                // ע�⣬�����漰�߳�״̬������¼����ڴ˴���
                switch (m_ThreadState)
                {
                    case cGlobalParas.GatherThreadState.Completed:
                        if (e_Completed != null)
                        {
                            // ������ �ɼ�������� �¼�
                            m_TaskManage.EventProxy.AddEvent(delegate()
                            {
                                e_Completed(this, new cTaskEventArgs());
                            });
                        }
                        m_Thread = null;
                        break;
                    case cGlobalParas.GatherThreadState.Failed:
                        if (e_Failed != null)
                        {
                            // ������ �߳�ʧ�� �¼�
                            m_TaskManage.EventProxy.AddEvent(delegate()
                            {
                                e_Failed(this, new cTaskEventArgs());
                            });
                        }
                        m_Thread = null;
                        break;
                    case cGlobalParas.GatherThreadState.Started:
                        if (e_Started != null)
                        {
                            // ���� �߳̿�ʼ �¼�
                            e_Started(this, new cTaskEventArgs());
                        }
                        break;
                    case cGlobalParas.GatherThreadState.Stopped:
                        if (e_Stopped != null)
                        {
                            // ���� �߳�ֹͣ �¼�
                            e_Stopped(this, new cTaskEventArgs());
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// ��������������󣬲���ֹͣ�����������������
        /// ����Ҫ������Ϣ�����û���һ����ַ�ɼ�����
        /// </summary>
        /// <param name="exp"></param>
        private void onError(Exception exp)
        {
            if (this.IsCurrentRunning)
            {
                // ������Stopped�¼�
                //m_ThreadState = cGlobalParas.GatherThreadState.Stopped;

                if (e_Error != null)
                {
                    // ������ �̴߳��� �¼�
                    m_TaskManage.EventProxy.AddEvent(delegate()
                    {
                        m_ErrorCount++;
                        e_Error(this, new TaskThreadErrorEventArgs(exp));
                    });
                }

                //m_Thread = null;
            }
        }
        #endregion


        #region �߳̿��� ���� ֹͣ ���� ����

        /// <summary>
        /// �߳���
        /// </summary>
        private readonly Object m_threadLock = new Object();
        /// <summary>
        /// ���������߳�
        /// </summary>
        public void Start()
        {
            if (m_ThreadState != cGlobalParas.GatherThreadState.Started && !IsThreadAlive && !IsCompleted)
            {
                lock (m_threadLock)
                {
                    //�����߳����б�־����ʶ���߳�����
                    m_ThreadRunning = true; 

                    m_Thread = new Thread(this.ThreadWorkInit);

                    //�����߳�����,���ڵ���ʹ��
                    m_Thread.Name =m_TaskID.ToString() + "-" + m_TaskSplitData.BeginIndex.ToString ();

                    m_Thread.Start();
                    m_ThreadState = cGlobalParas.GatherThreadState.Started;
                }
            }
        }

        /// <summary>
        /// �������������߳�
        /// </summary>
        public void ReStart()
        {   // �������߳������
            Stop();
            Start();
        }

        /// <summary>
        /// ֹͣ��ǰ�߳�
        /// </summary>
        public void Stop()
        {   
            //�������߳������
            //����ֹͣ�̱߳�־���߳�ֹֻͣ�ܵȴ�һ����ַ�ɼ���ɺ�ֹͣ������
            //ǿ���жϣ�����ᶪʧ����

            ///ע�⣺�ڴ�ֻ�Ǵ���һ����ǣ��̲߳�û����������
            m_ThreadRunning = false;


            if (m_ThreadState == cGlobalParas.GatherThreadState.Started && IsThreadAlive)
            {
                lock (m_threadLock)
                {

                    //��ʼ����Ƿ������̶߳�����ɻ��˳�
                    //bool isStop = false;

                    //while (!isStop)
                    //{
                    //    isStop = true;

                    //    if (m_ThreadState == cGlobalParas.GatherThreadState.Started && IsThreadAlive)
                    //        isStop = false;

                        
                    //}
                    
                    m_Thread = null;
                    if (m_ThreadState == cGlobalParas.GatherThreadState.Started)
                    {
                        m_ThreadState = cGlobalParas.GatherThreadState.Stopped;
                    }
                }
            }
            else
            {

                m_Thread = null;
                if (m_ThreadState == cGlobalParas.GatherThreadState.Started)
                {
                    m_ThreadState = cGlobalParas.GatherThreadState.Stopped;
                }
            }

            m_ThreadState = cGlobalParas.GatherThreadState.Stopped;
        }

        public void Abort()
        {
            m_ThreadRunning = false;

            if (m_ThreadState == cGlobalParas.GatherThreadState.Started && IsThreadAlive)
            {
                lock (m_threadLock)
                {
                    if (m_ThreadState == cGlobalParas.GatherThreadState.Started && IsThreadAlive)
                    {
                        m_Thread.Abort();
                        m_Thread = null;
                        m_ThreadState = cGlobalParas.GatherThreadState.Aborted;
                    }
                    else
                    {
                        m_Thread = null;
                        if (m_ThreadState == cGlobalParas.GatherThreadState.Started)
                        {
                            m_ThreadState = cGlobalParas.GatherThreadState.Aborted; ;
                        }
                    }
                }
            }
            else
            {
                m_Thread = null;
                if (m_ThreadState == cGlobalParas.GatherThreadState.Started)
                {
                    m_ThreadState = cGlobalParas.GatherThreadState.Aborted; ;
                }
            }

            m_ThreadState = cGlobalParas.GatherThreadState.Aborted;
        }

        /// <summary>
        /// �����߳̿�Ϊδ��ʼ��״̬
        /// </summary>
        internal void Reset()
        {
            e_Completed = null;
            e_Error = null;
            e_Started = null;
            e_Stopped = null;
            e_TaskInit = null;
            m_IsInitialized = false;

            e_Log = null;
            e_GData = null;
        }

        #endregion

        #region �����̴߳����ɼ���ҳ���ݣ� ִ��һ���ɼ��ֽ�����

        /// <summary>
        /// �ɼ���ʼ���߳�
        /// </summary>
        private void ThreadWorkInit()
        {
            if (!m_IsInitialized)
            {
                ///����ǰ�Ĵ���ʽ��ֻҪ����������������Ѿ���ʼ����������Ϣ
                ///������һ�����δ���������Ǵ��е�������ַ��������ַ��Ҫ����ʵ��
                ///�Ľ�������ſ��Ի����������Ҫ�ɼ����ݵ���ַ����ô����Ҫ����Щʵʱ
                ///������������ַ�ٴθ����߳������������֣�������Ҫ���¶��������
                ///��ʼ�������������ǰδ���д����ر�����
                e_TaskInit(this, new TaskInitializedEventArgs(m_TaskID));
               
            }
            else if (GatheredUrlCount !=TrueUrlCount )
            {
                ThreadWork();
            }
        }

        /// <summary>
        /// �ɼ�����
        /// </summary>
        private void ThreadWork()
        {
            //cGatherWeb gWeb = new cGatherWeb();

            //gWeb.CutFlag =m_TaskSplitData.CutFlag ;

            bool IsSucceed = false;

            for (int i = 0; i < m_TaskSplitData.Weblink.Count; i++)
            {
                if (m_ThreadRunning == true)
                {
                    switch (m_TaskSplitData.Weblink[i].IsGathered)
                    {
                        case (int) cGlobalParas.UrlGatherResult.UnGather:

                            e_Log(this, new cGatherTaskLogArgs(m_TaskID,((int)cGlobalParas.LogType.Info).ToString() + "���ڲɼ���" + m_TaskSplitData.Weblink[i].Weblink + "\n",this.IsErrorLog ));

                            //�жϴ���ַ�Ƿ�Ϊ������ַ������ǵ�����ַ����Ҫ���Ƚ���Ҫ�ɼ�����ַ��ȡ����
                            //Ȼ����о�����ַ�Ĳɼ�
                            if (m_TaskSplitData.Weblink[i].IsNavigation == true)
                            {
                                IsSucceed = GatherNavigationUrl(m_TaskSplitData.Weblink[i].Weblink, m_TaskSplitData.Weblink[i].NavigRules , m_TaskSplitData.Weblink[i].IsNextpage, m_TaskSplitData.Weblink[i].NextPageRule);
                            }
                            else
                            {
                                IsSucceed=GatherSingleUrl(m_TaskSplitData.Weblink[i].Weblink, m_TaskSplitData.Weblink[i].IsNextpage, m_TaskSplitData.Weblink[i].NextPageRule);
                            }

                            //����ɼ�����������ֱ�ӵ�����onError���д�����
                            //�����ڴ��е�������ַ��һ�βɼ�����Զ����ַ�����ϵͳ
                            //ֹͣ���񣬶��ڶ����ַ�Ĳɼ��Ǿ��ǲɼ�����δ�ɹ�����Ȼ���ɹ�
                            //����Ҫ����GatheredUrlCount����������
                            if (IsSucceed == true)
                            {
                                //ÿ�ɼ����һ��Url������Ҫ�޸ĵ�ǰCurIndex��ֵ����ʾϵͳ�������У���
                                //����ȷ���˷ֽ������Ƿ��Ѿ����,���ұ�ʾ����ַ�Ѿ��ɼ����
                                m_TaskSplitData.CurIndex++;
                                e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.UrlCountAdd, 0));

                                m_TaskSplitData.Weblink[i].IsGathered = (int)cGlobalParas.UrlGatherResult.Succeed;

                                m_TaskSplitData.GatheredUrlCount++;
                            }
                            else
                            {
                                e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.ErrUrlCountAdd, 0));
                            }

                            break;

                        case (int)cGlobalParas.UrlGatherResult.Succeed:
                            m_TaskSplitData.CurIndex++;
                            m_TaskSplitData.GatheredUrlCount++;
                            m_TaskSplitData.GatheredTrueUrlCount++;
                            e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.UrlCountAdd, 0));
                            break;
                        case (int)cGlobalParas.UrlGatherResult.Error:
                            m_TaskSplitData.CurIndex++;
                            m_TaskSplitData.GatheredErrUrlCount++;
                            m_TaskSplitData.GatheredTrueErrUrlCount++;
                            e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.ErrUrlCountAdd, 0));
                            break;
                        case (int)cGlobalParas.UrlGatherResult.Gathered:
                            m_TaskSplitData.CurIndex++;
                            m_TaskSplitData.GatheredUrlCount++;
                            m_TaskSplitData.GatheredTrueUrlCount++;
                            e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.UrlCountAdd, 0));
                            break;
                    }
                }
                else if (m_ThreadRunning == false)
                {
                    //��ʾ�߳���Ҫ��ֹ���˳�forѭ��
                    break;
                }


            }
           

            //ÿ�ɼ����һ����ַ��Ҫ�жϵ�ǰ�߳��Ƿ��Ѿ�����
            //��Ҫ�ж��Ѳɼ�����ַ�Ͳɼ������������ַ
            if (UrlCount == GatheredUrlCount + GatherErrUrlCount && UrlCount != GatherErrUrlCount)
            {
                ThreadState = cGlobalParas.GatherThreadState.Completed;
            }
            else if (UrlCount == GatherErrUrlCount)
            {
                //��ʾ�ɼ�ʧ�ܣ�������һ��������ԣ�һ���̵߳���ȫʧ�ܲ�������
                //����Ҳ�ɼ���ȫʧ�ܣ���������İ�ȫʧ����Ҫ������ɼ������ж�
                //���ԣ��ڴ˻��Ƿ����߳��������ʱ�䣬����ʧ��ʱ�䡣
                ThreadState = cGlobalParas.GatherThreadState.Completed;
            }
            else if (UrlCount<GatheredUrlCount + GatherErrUrlCount)
            {
                ThreadState = cGlobalParas.GatherThreadState.Completed;
            }
            else
            {
                ThreadState = cGlobalParas.GatherThreadState.Stopped;
            }

            m_ThreadRunning = false;

        }

        //���ڲɼ�һ����ҳ������
        private bool GatherSingleUrl(string Url,bool IsNext,string NextRule)
        {
            cGatherWeb gWeb = new cGatherWeb();
            DataTable tmpData;
            string NextUrl=Url ;
            string Old_Url = NextUrl;

            //gWeb.CutFlag = m_TaskSplitData.CutFlag;

            bool IsAjax = false;

            if (m_TaskType == cGlobalParas.TaskType.AjaxHtmlByUrl)
                IsAjax = true;


            try
            {
                if (IsNext)
                {
                    do
                    {
                        Url = NextUrl;
                        Old_Url = NextUrl;

                        e_Log(this, new cGatherTaskLogArgs(m_TaskID, ((int)cGlobalParas.LogType.Info).ToString() + "���ڲɼ���" + Url + "\n", this.IsErrorLog));

                        if (m_IsUrlEncode == true)
                        {
                            Url = cTool.UrlEncode(Url,(cGlobalParas.WebCode)int.Parse ( m_UrlEncode));
                        }

                        //tmpData = gWeb.GetGatherData(Url, m_WebCode, m_Cookie, m_gStartPos, m_gEndPos,m_SavePath,IsAjax );
                        tmpData = GetGatherData(Url, m_WebCode, m_Cookie, m_gStartPos, m_gEndPos, m_SavePath, IsAjax);

                        if (tmpData != null)
                        {
                            m_GatherData.Merge(tmpData);
                        }

                        //������־���ɼ����ݵ��¼�
                        if (tmpData == null || tmpData.Rows.Count == 0)
                        {
                        }
                        else
                        {
                            e_GData(this, new cGatherDataEventArgs(m_TaskID, tmpData));
                        }
                        e_Log(this, new cGatherTaskLogArgs(m_TaskID, ((int)cGlobalParas.LogType.Info).ToString() + "�ɼ���ɣ�" + Url + "\n", this.IsErrorLog));


                        e_Log(this, new cGatherTaskLogArgs(m_TaskID, ((int)cGlobalParas.LogType.Info).ToString() + "��ʼ������һҳ�����ȡ��һҳ��ַ\n", this.IsErrorLog));
                        
                        
                        string webSource = gWeb.GetHtml(Url, m_WebCode, m_Cookie, "", "", true, IsAjax);
                        

                        string NRule="((?<=href=[\'|\"])\\S[^#+$<>\\s]*(?=[\'|\"]))[^<]*(?<=" + NextRule + ")";
                        Match charSetMatch = Regex.Match(webSource, NRule, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string strNext = charSetMatch.Groups[1].Value;

                        if (strNext != "")
                        {
                            //�жϻ�ȡ�ĵ�ַ�Ƿ�Ϊ��Ե�ַ
                            if (strNext.Substring(0, 1) == "/")
                            {
                                string PreUrl = Url;
                                PreUrl = PreUrl.Substring(7, PreUrl.Length - 7);
                                PreUrl = PreUrl.Substring(0, PreUrl.IndexOf("/"));
                                PreUrl = "http://" + PreUrl;
                                strNext = PreUrl + strNext;
                            }
                            else if (strNext.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase))
                            {
                                //NextUrl = strNext;
                            }
                            else if (strNext.StartsWith("?", StringComparison.CurrentCultureIgnoreCase))
                            {
                                Match aa = Regex.Match(Url, @".*(?=\?)");
                                string PreUrl = aa.Groups[0].Value.ToString();
                                strNext = PreUrl + strNext;
                            }
                            else
                            {
                                Match aa = Regex.Match(Url, ".*/");
                                string PreUrl = aa.Groups[0].Value.ToString();
                                strNext = PreUrl + strNext;
                            }

                            e_Log(this, new cGatherTaskLogArgs(m_TaskID, ((int)cGlobalParas.LogType.Info).ToString() + "��һҳ��ַ��ȡ�ɹ���" + NextUrl + "\n", this.IsErrorLog));

                        }
                        else
                        {
                            e_Log(this, new cGatherTaskLogArgs(m_TaskID, ((int)cGlobalParas.LogType.Info).ToString() + "�Ѿ�������ҳ" + NextUrl + "\n", this.IsErrorLog));

                        }

                        NextUrl = strNext;

                        
                    }
                    while (NextUrl != "" && Old_Url != NextUrl);
                }
                else
                {

                    if (m_IsUrlEncode == true)
                    {
                        Url = cTool.UrlEncode(Url, (cGlobalParas.WebCode)int.Parse(m_UrlEncode));
                    }

                    //tmpData = gWeb.GetGatherData(Url, m_WebCode, m_Cookie, m_gStartPos, m_gEndPos, m_SavePath, IsAjax);
                    tmpData = GetGatherData(Url, m_WebCode, m_Cookie, m_gStartPos, m_gEndPos, m_SavePath, IsAjax);

                    if (tmpData != null)
                    {
                        m_GatherData.Merge(tmpData);
                    }

                    //������־���ɼ����ݵ��¼�
                    if (tmpData == null || tmpData.Rows.Count == 0)
                    {
                    }
                    else
                    {
                        e_GData(this, new cGatherDataEventArgs(m_TaskID, tmpData));
                    }
                    e_Log(this, new cGatherTaskLogArgs(m_TaskID, ((int)cGlobalParas.LogType.Info).ToString() + "�ɼ���ɣ�" + Url + "\n", this.IsErrorLog));
                }
                

                //�����ɼ���ַ�����¼�
                e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.Gathered, 0));

                m_TaskSplitData.GatheredTrueUrlCount++;

            }
            catch (System.Exception ex)
            {
                e_Log(this, new cGatherTaskLogArgs(m_TaskID, ((int)cGlobalParas.LogType.Error).ToString() + Url + "�ɼ���������" + ex.Message + "\n", this.IsErrorLog));
                e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.Err, 0));
                e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.ErrUrlCountAdd, 0));
                m_TaskSplitData.GatheredTrueErrUrlCount++;
                m_TaskSplitData.GatheredErrUrlCount++;
                onError(ex);
                return false;
            }

            gWeb = null;
            tmpData = null;

            return true;
        }

        ///���ǲɼ����е����������ַ���ݵ����
        ///���������Ϊ���ࣺһ����һҳ�ĵ������򣻶���ҳ�浼����
        ///�˷��������ַ����Ҫ������һҳ�Ĺ���Ȼ�����ParseGatherNavigationUrl
        ///����ҳ�浼��������
        private bool GatherNavigationUrl(string Url, List<Task.cNavigRule> nRules, bool IsNext, string NextRule)
        {
            cGatherWeb gWeb = new cGatherWeb();
            //gWeb.CutFlag = m_TaskSplitData.CutFlag;
            string NextUrl = Url;
            string Old_Url = NextUrl;
            bool IsSucceed = false;

            try
            {

                if (IsNext)
                {
                    do
                    {
                        if (m_ThreadRunning == true)
                        {
                            Url = NextUrl;
                            Old_Url = NextUrl;

                            e_Log(this, new cGatherTaskLogArgs(m_TaskID, ((int)cGlobalParas.LogType.Info).ToString() + "���ڲɼ���" + Url + "\n", this.IsErrorLog));

                            IsSucceed = ParseGatherNavigationUrl(Url,nRules) ; //, NagRule, IsOppPath);

                            e_Log(this, new cGatherTaskLogArgs(m_TaskID, ((int)cGlobalParas.LogType.Info).ToString() + "�ɼ���ɣ�" + Url + "\n", this.IsErrorLog));

                            e_Log(this, new cGatherTaskLogArgs(m_TaskID, ((int)cGlobalParas.LogType.Info).ToString() + "��ʼ������һҳ�����ȡ��һҳ��ַ\n", this.IsErrorLog));

                            bool IsAjax = false;

                            if (m_TaskType == cGlobalParas.TaskType.AjaxHtmlByUrl)
                                IsAjax = true;

                            string webSource = gWeb.GetHtml(Url, m_WebCode, m_Cookie, "", "",true,IsAjax );

                            string NRule = "((?<=href=[\'|\"])\\S[^#+$<>\\s]*(?=[\'|\"]))[^<]*(?<=" + NextRule + ")";
                            Match charSetMatch = Regex.Match(webSource, NRule, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                            string strNext = charSetMatch.Groups[1].Value;

                            if (strNext != "")
                            {
                                //�жϻ�ȡ�ĵ�ַ�Ƿ�Ϊ��Ե�ַ
                                if (strNext.Substring(0, 1) == "/")
                                {
                                    string PreUrl = Url;
                                    PreUrl = PreUrl.Substring(7, PreUrl.Length - 7);
                                    PreUrl = PreUrl.Substring(0, PreUrl.IndexOf("/"));
                                    PreUrl = "http://" + PreUrl;
                                    strNext = PreUrl + strNext;
                                }
                                else if (strNext.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    NextUrl = strNext;
                                }
                                else if (strNext.StartsWith("?", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    Match aa = Regex.Match(Url, @".*(?=\?)");
                                    string PreUrl = aa.Groups[0].Value.ToString();
                                    strNext = PreUrl + strNext;
                                }
                                else
                                {
                                    Match aa = Regex.Match(Url, ".*/");
                                    string PreUrl = aa.Groups[0].Value.ToString();
                                    strNext = PreUrl + strNext;
                                }

                                e_Log(this, new cGatherTaskLogArgs(m_TaskID, ((int)cGlobalParas.LogType.Info).ToString() + "��һҳ��ַ��ȡ�ɹ���" + NextUrl + "\n", this.IsErrorLog));
                            }
                            else
                            {
                                e_Log(this, new cGatherTaskLogArgs(m_TaskID, ((int)cGlobalParas.LogType.Info).ToString() + "�Ѿ�������ҳ" + "\n", this.IsErrorLog));
                            }

                            NextUrl = strNext;

                        }
                        else if (m_ThreadRunning == false)
                        {
                            //��ʶҪ����ֹ�̣߳�ֹͣ�����˳�doѭ����ǰ��������
                            if (NextUrl == "" || Old_Url == NextUrl)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                            //break;
                        }

                    }
                    while (NextUrl != "" && Old_Url != NextUrl);
                }
                else
                {
                    IsSucceed = ParseGatherNavigationUrl(Url, nRules); //, NagRule, IsOppPath);
                }
            }
            catch (System.Exception ex)
            {
                e_Log(this, new cGatherTaskLogArgs(m_TaskID, ((int)cGlobalParas.LogType.Error).ToString() + Url + "�ɼ���������" + ex.Message + "\n", this.IsErrorLog));
                e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.Err, 0));
                e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.ErrUrlCountAdd, 0));
                m_TaskSplitData.GatheredTrueErrUrlCount++;
                m_TaskSplitData.GatheredErrUrlCount++;
                onError(ex);
                return false;
            }

            gWeb = null;

            return IsSucceed;
        }

        //���ڲɼ���Ҫ��������ҳ���ڴ˴�����ҳ����
        private bool ParseGatherNavigationUrl(string Url, List<Task.cNavigRule> nRules)
        {
            Task.cUrlAnalyze u = new Task.cUrlAnalyze();
            List<string> gUrls;
            bool IsSucceed = false;

            e_Log(this, new cGatherTaskLogArgs(m_TaskID, ((int)cGlobalParas.LogType.Info).ToString() + "��ʼ���ݵ��������ȡ��ҳ��ַ����ȴ�......\n�����㼶Ϊ��" + nRules.Count + " ��\n", this.IsErrorLog));

            gUrls = u.ParseUrlRule(Url, nRules,m_WebCode ,m_Cookie );

            u = null;
            if (gUrls == null || gUrls.Count == 0)
            {
                e_Log(this, new cGatherTaskLogArgs(m_TaskID, ((int)cGlobalParas.LogType.Info).ToString() + Url + " ��������ʧ�ܣ��п��������ڵ����������ô���Ҳ�п�������������������ɣ�������������ݣ���Ӱ��ϵͳ�����ݵĲɼ�\n", this.IsErrorLog));
                return false;
            }

            e_Log(this, new cGatherTaskLogArgs(m_TaskID, ((int)cGlobalParas.LogType.Info).ToString() + "�ɹ����ݵ��������ȡ" + gUrls.Count + "����ַ\n", this.IsErrorLog));

            //����ʵ�ʲɼ���ַ���������ǵ���ҳ�棬����ʵ�ʲɼ���ַ���������˱仯
            //ͨ���¼�������������Ĳɼ�����������ͬʱ����������Ĳɼ�����
            //ע�⣬������ʵ�ʲɼ���ַ������������������ַ��������������ֵ������ά�����Ե�ҵ���߼�����

            //ϵͳ���������񵼺��ķֽ��������ʱ���Ѿ��޸�����Ҫ�ɼ���������������ԣ���Ҫ�����������ʵ�ʲɼ���ַ������
            //ͬʱ���败����Ӧ���¼��޸���������Ĳɼ���ַ������
            m_TaskSplitData.TrueUrlCount += gUrls.Count - 1;
            e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.ReIni, gUrls.Count));

            for (int j = 0; j < gUrls.Count; j++)
            {
                if (m_ThreadRunning == true)
                {
                    try
                    {
                        if (string.Compare(gUrls[j].Substring(0, 4), "http", true) != 0)
                        {
                            string PreUrl = Url;

                            if (gUrls[j].Substring(0, 1) == "/")
                            {
                                PreUrl = PreUrl.Substring(7, PreUrl.Length - 7);
                                PreUrl = PreUrl.Substring(0, PreUrl.IndexOf("/"));
                                PreUrl = "http://" + PreUrl;
                            }
                            else
                            {
                                Match aa = Regex.Match(PreUrl, ".*/");
                                PreUrl = aa.Groups[0].Value.ToString();
                            }

                           IsSucceed= GatherParsedUrl(PreUrl + gUrls[j].ToString());
                        }
                        else
                        {
                           IsSucceed= GatherParsedUrl(gUrls[j].ToString());
                        }


                        //�����ɼ���ַ�����¼�
                        e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.Gathered, 0));
                        m_TaskSplitData.GatheredTrueUrlCount++;

                    }
                    catch (System.Exception ex)
                    {
                        e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.Err, 0));
                        m_TaskSplitData.GatheredTrueErrUrlCount++;
                        onError(ex);
                    }
                }
                else if (m_ThreadRunning == false)
                {
                    //��ʶҪ����ֹ�̣߳�ֹͣ�����˳�forѭ����ǰ��������
                    if (j == gUrls.Count)
                    {
                        //��ʾ���ǲɼ������
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                    //break;
                }

            }

            return true;
        }

        //���ڲɼ�������ҳ�ֽ��ĵ�����ַ
        private bool GatherParsedUrl(string Url)
        {
            cGatherWeb gWeb = new cGatherWeb();
            DataTable tmpData=null;

            gWeb.CutFlag = m_TaskSplitData.CutFlag;

            bool IsAjax = false;

            if (m_TaskType == cGlobalParas.TaskType.AjaxHtmlByUrl)
                IsAjax = true;

            try
            {
                e_Log(this, new cGatherTaskLogArgs(m_TaskID, ((int)cGlobalParas.LogType.Info).ToString() + "���ڲɼ���" + Url + "\n", this.IsErrorLog));

                if (m_IsUrlEncode == true)
                {
                    Url = cTool.UrlEncode(Url, (cGlobalParas.WebCode)int.Parse(m_UrlEncode));
                }

                //tmpData = gWeb.GetGatherData(Url, m_WebCode, m_Cookie, m_gStartPos, m_gEndPos, m_SavePath, IsAjax);
                tmpData = GetGatherData(Url, m_WebCode, m_Cookie, m_gStartPos, m_gEndPos, m_SavePath, IsAjax);

                if (tmpData != null)
                {
                    m_GatherData.Merge(tmpData);
                }

                //������־���ɼ����ݵ��¼�
                if (tmpData == null || tmpData.Rows.Count == 0)
                {
                }
                else
                {
                    e_GData(this, new cGatherDataEventArgs(m_TaskID, tmpData));
                }
                if (tmpData == null)
                {
                    e_Log(this, new cGatherTaskLogArgs(m_TaskID, ((int)cGlobalParas.LogType.Error).ToString() + Url + " �˵�ַ�����ݣ�" + "\n", this.IsErrorLog));
                }
                else
                {
                    e_Log(this, new cGatherTaskLogArgs(m_TaskID, ((int)cGlobalParas.LogType.Info).ToString() + "�ɼ���ɣ�" + Url + "\n", this.IsErrorLog));
                }
                tmpData = null;

            }
            catch (System.Exception ex)
            {
                e_Log(this, new cGatherTaskLogArgs(m_TaskID, ((int)cGlobalParas.LogType.Error).ToString() + Url + "�ɼ���������" + ex.Message + "\n", this.IsErrorLog));
                onError(ex);
                return false  ;
            }

            gWeb = null;

            return true;

        }

        #endregion

        //����һ��ͨѶ�Ľӿڷ����������ɼ�����Ĵ���������Ҫ�ɼ�����ҳ�����ô˷���
        //�ɴ˷�������cGatherWeb.GetGatherData�����η�����Ŀ����Ϊ�˿��Դ����������

        private DataTable GetGatherData(string Url, cGlobalParas.WebCode webCode, string cookie, string startPos, string endPos, string sPath, bool IsAjax)
        {
            cGatherWeb gWeb = new cGatherWeb();
            gWeb.CutFlag = m_TaskSplitData.CutFlag;

            DataTable tmpData ;
            int AgainTime = 0;

            GatherAgain:

            try
            {
                tmpData = gWeb.GetGatherData(Url, m_WebCode, m_Cookie, m_gStartPos, m_gEndPos, m_SavePath, IsAjax);
            }
            catch (System.Exception ex)
            {
                AgainTime++;
                
                if (AgainTime > m_AgainNumber)
                {
                    if (m_IsErrorLog == true)
                    {
                        //���������־
                    }

                    throw ex;
                }
                else
                {
                    if (m_Ignore404 == true && ex.Message.Contains ("404"))
                    {
                        if (m_IsErrorLog == true)
                        {
                            //���������־
                        }

                        throw ex;
                    }
                    else
                    {
                        e_Log(this, new cGatherTaskLogArgs(m_TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "��ַ��" + Url + "���ʷ�����������Ϣ��" + ex.Message + "���ȴ�3������\n", this.IsErrorLog));
                       
                        Thread.Sleep(3000);

                        e_Log(this, new cGatherTaskLogArgs(m_TaskID, ((int)cGlobalParas.LogType.Warning).ToString() + Url + "���ڽ��е�" + AgainTime + "������\n", this.IsErrorLog));

                        //��������
                        goto GatherAgain;
                    }
                }
            }

            return tmpData;
        }

        #region ��������ֽ����ݣ����ⲿ����

        /// <summary>
        /// ���÷ֽ����������,�ֽ������������Ҫ��������ʼ�ɼ�
        /// ����ҳ����,��ֹ�ɼ�����ҳ����,һ����Ҫ�ɼ�����ҳ����
        /// </summary>
        /// <param name="beginIndex">��ʼ�Ĳɼ���ҳ��ַ</param>
        /// <param name="endIndex">��ֹ�Ĳɼ���ҳ��ַ</param>
        public void SetSplitData(int beginIndex, int endIndex,List<Task.cWebLink> tUrl,List<Task.cWebpageCutFlag> tCutFlag)
        {
            lock (m_mstreamLock)
            {
                m_TaskSplitData.BeginIndex = beginIndex;
                m_TaskSplitData.CurIndex = beginIndex;
                m_TaskSplitData.EndIndex = endIndex;
                m_TaskSplitData.Weblink = tUrl;
                m_TaskSplitData.TrueUrlCount = tUrl.Count;
                m_TaskSplitData.CutFlag = tCutFlag;
                //m_IsDataInitialized = true;
            }
        }
       
        #endregion

        #region IDisposable ��Ա
        private bool m_disposed;
        /// <summary>
        /// �ͷ��� �ɼ� �ĵ�ǰʵ��ʹ�õ�������Դ
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

                m_disposed = true;
            }
        }

        #endregion

    }
}
