using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using SoukeyNetget.Gather ;
using SoukeyNetget.Task;
using System.Security.Permissions;
using System.Reflection;
using SoukeyNetget.CustomControl;
using System.Runtime.InteropServices;
using SoukeyNetget.publish ;
//using System.Web;
using System.IO;
using SoukeyNetget.Plan;
using SoukeyNetget.Listener ;
using SoukeyNetget.Log;
using System.Resources;

///功能：Soukey采摘主界面处理（包括线程响应事件）
///完成时间：2009-3-2
///作者：一孑
///遗留问题：代码还未整理，可能看起来比较乱
///开发计划：无
///说明：
///版本：01.10.00
///修订：无
namespace SoukeyNetget
{

    public partial class frmMain : Form
    {

        private cGatherControl m_GatherControl;
        private cPublishControl m_PublishControl;
        private bool IsTimer = true;
        private string DelName="";
        private cGlobalParas.ExitPara m_ePara = cGlobalParas.ExitPara.Exit;

        private DataGridViewCellStyle m_RowStyleErr;

        private ToolTip tTip;

        private TreeNode Old_SelectedNode;

        private cListenControl m_ListenControl;

        private bool m_IsRunListen = false;

        private ResourceManager rm;

        //判断是否正在进行退出操作
        //private bool IsExitting = false;

        //是否保存系统日志标记,默认不保存
        private bool m_IsAutoSaveLog = false;

        #region 窗体初始化操作
        
        ////主窗体的初始化操作由外部来控制
        //private Thread m_InfoThread;

        public frmMain()
        {
            InitializeComponent();

            //加载主界面显示的资源文件
            rm = new ResourceManager("SoukeyNetget.Resources.globalUI", Assembly.GetExecutingAssembly());

            //加载托盘图标
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.ShowBalloonTip(2,rm.GetString ("TrayTitle"), rm.GetString ("TrayInfo3"), ToolTipIcon.Info); 
        }

        //此方法主要进行界面初始化的操作，无参数
        //此方法主要是初始化界面显示的内容,包括
        //树形结构,默认节点,网页打开默认的页面等等
        public void IniForm()
        {
            this.treeMenu.ExpandAll();
            TreeNode newNode;
            int i = 0;

            //写日志启动的时间
            ExportLog(DateTime.Now + "：" + rm.GetString("InfoStart"));

            //设置datagridview加载错误信息的现实样式
            SetRowErrStyle();

            //设置Tooltip信息
            SetToolTip();

            //初始化网页，固定地址，具体跳转由此页面来控制
            this.webBrowser.Navigate("http://www.yijie.net/softini.html");

            try
            {
                //开始初始化树形结构,取xml中的数据,读取任务分类
                Task.cTaskClass xmlTClass = new Task.cTaskClass();

                int TClassCount = xmlTClass.GetTaskClassCount();

                for (i = 0; i < TClassCount; i++)
                {
                    newNode = new TreeNode();
                    newNode.Tag = xmlTClass.GetTaskClassPathByID(xmlTClass.GetTaskClassID(i));
                    newNode.Name = "C" + xmlTClass.GetTaskClassID(i);
                    newNode.Text = xmlTClass.GetTaskClassName(i);
                    newNode.ImageIndex = 0;
                    newNode.SelectedImageIndex = 0;
                    //this.treeMenu.SelectedNode = newNode;
                    this.treeMenu.Nodes["nodTaskClass"].Nodes.Add(newNode);
                    newNode = null;
                }
                xmlTClass = null;
            }
            catch (System.Exception )
            {
                MessageBox.Show(rm.GetString ("Error14"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            //根据当前显示的语言，设置菜单的默认选项
            try
            {
                cXmlSConfig Config = new cXmlSConfig();
                cGlobalParas.CurLanguage cl = Config.CurrentLanguage;
                switch (cl)
                {
                    case cGlobalParas.CurLanguage.Auto:
                        this.toolmenuAuto.Checked = true;
                        break;
                    case cGlobalParas.CurLanguage.enUS:
                        this.toolmenuAuto.Checked = false;
                        this.toolmenuEnglish.Checked = true;
                        this.toolmenuCHS.Checked = false;
                        break;
                    case cGlobalParas.CurLanguage.zhCN:
                        this.toolmenuAuto.Checked = false;
                        this.toolmenuCHS.Checked = true;
                        this.toolmenuEnglish.Checked = false;
                        break;
                    default:
                        break;
                }
                Config = null;
            }
            catch (System.Exception)
            {

            }

            //将任务根节点赋路径值
            this.treeMenu.Nodes["nodTaskClass"].Tag = Program.getPrjPath() + "Tasks";

            //设置默认选择的树形结构节点为“正在运行”
            TreeNode SelectNode = new TreeNode();
            SelectNode = this.treeMenu.Nodes[0].Nodes[0];
            this.treeMenu.SelectedNode = SelectNode;
            SelectNode = null;


            //设置删除项为树形结构
            DelName = this.treeMenu.Name;

        }

        //此方法主要是初始化系统对象,包括需要初始化消息事件,初始化采集
        //任务控制器,并加载运行区的数据,如果运行区无数据,则初始化一个空
        //的对象
        public void UserIni()
        {

            //初始化一个采集任务的控制器,采集任务由此控制器来负责采集任务
            //管理
            m_GatherControl = new cGatherControl();

            //采集控制器事件绑定,绑定后,页面可以响应采集任务的相关事件
            m_GatherControl.TaskManage.TaskCompleted += tManage_Completed;
            m_GatherControl.TaskManage.TaskStarted += tManage_TaskStart;
            m_GatherControl.TaskManage.TaskInitialized += tManage_TaskInitialized;
            m_GatherControl.TaskManage.TaskStateChanged += tManage_TaskStateChanged;
            m_GatherControl.TaskManage.TaskStopped += tManage_TaskStop;
            m_GatherControl.TaskManage.TaskError += tManage_TaskError;
            m_GatherControl.TaskManage.TaskFailed += tManage_TaskFailed;
            m_GatherControl.TaskManage.TaskAborted += tManage_TaskAbort;
            m_GatherControl.TaskManage.Log += tManage_Log;
            m_GatherControl.TaskManage.GData += tManage_GData;

            m_GatherControl.TaskManage.RunTask += this.On_RunSoukeyTask;

            m_GatherControl.Completed += m_Gather_Completed;

            //加载运行区的数据,运行区的数据主要是根据taskrun.xml(默认在Tasks\\TaskRun.xml)文件中
            //的内容进行加载,

            //首先判断TaskRun.xml文件是否存在,不存在则建立一个
            if (!System.IO.File.Exists(Program.getPrjPath() + "Tasks\\taskrun.xml"))
            {
                CreateTaskRun();
            }

            cTaskDataList gList = new cTaskDataList();
            
            
            //根据加载的运行区的任务信息,开始初始化采集任务
            try
            {
                gList.LoadTaskRunData();
                bool IsAddRTaskSucceed=m_GatherControl.AddGatherTask(gList);

                if (IsAddRTaskSucceed==false )
                    MessageBox.Show(rm.GetString("Error23") , rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Error15") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           

            //开始加载正在导出的任务信息
            m_PublishControl = new cPublishControl();

            //注册发布任务的事件
            m_PublishControl.PublishManage.PublishCompleted += this.Publish_Complete;
            m_PublishControl.PublishManage.PublishError += this.Publish_Error;
            m_PublishControl.PublishManage.PublishFailed += this.Publish_Failed;
            m_PublishControl.PublishManage.PublishStarted  += this.Publish_Started;
            m_PublishControl.PublishManage.PublishTempDataCompleted += this.Publish_TempDataCompleted;
            m_PublishControl.PublishManage.PublishLog += this.Publish_Log;
            m_PublishControl.PublishManage.RunTask += On_RunSoukeyTask;


            //根据选择的“正在运行”树形节点，加载相应的信息
            try
            {
                LoadRunTask();
            }
            catch (System.IO.IOException)
            {
                if (MessageBox.Show(rm.GetString("Quaere3"), rm.GetString ("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    CreateTaskRun();
                }
            }
            catch (System.Exception)
            {
                MessageBox.Show(rm.GetString ("Error16"),rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

            SetDataShow();

            //加载是否自动保存系统日志标志
            try
            {
                cXmlSConfig Config = new cXmlSConfig();
                m_IsAutoSaveLog = Config.AutoSaveLog;
                Config = null;
            }
            catch (System.Exception)
            {
                //表示配置文件出错，但需要继续加载
            }

            //启动时间器用于更新任务显示的进度
            this.timer1.Enabled = true;

            //更新状态条信息
            UpdateStatebarTask();

        }

        #endregion

        #region 监听器处理，启动 停止 响应事件

        /// 启动监听器，用于监听计划任务是否可以执行
        public void StartListen()
        {
            m_ListenControl = new cListenControl();
            m_ListenControl.ListenManage.RunTask += this.On_RunSoukeyTask;
            m_ListenControl.ListenManage.ListenError += this.On_ListenError;

            try
            {
                m_ListenControl.Start();
            }
            catch (System.Exception ex)
            {
                ExportLog(rm.GetString ("Info9") + ex.Message);
            }

            m_IsRunListen = true;
        }

        public void StopListen()
        {
            try
            {
                if (m_ListenControl.IsRunning == true)
                {
                    m_ListenControl.Stop();
                    m_ListenControl.ListenManage.RunTask -= this.On_RunSoukeyTask;
                    m_ListenControl.ListenManage.ListenError -= this.On_ListenError;
                    m_ListenControl = null;
                }
            }
            catch(System.Exception ex)
            {
                ExportLog(rm.GetString ("Info10") + ex.Message);
            }
            m_IsRunListen = false;
        }

        //处理启动任务事件，由监听管理类通过事件进行触发
        private void On_RunSoukeyTask(object sender,cRunTaskEventArgs e)
        {
            
            string tClassName=e.RunName.Substring(0, e.RunName.IndexOf("\\"));
            string TaskName=e.RunName.Substring(e.RunName.IndexOf("\\")+1, e.RunName.Length - e.RunName.IndexOf("\\")-1);
            cGatherTask t = AddRunTask(tClassName, TaskName);

            if (t == null)
                return;

            Int64 TaskID = t.TaskID;

            //增加Tab标签
            InvokeMethod(this, "AddTab", new object[] { TaskID, TaskName });

            //启动此任务
            m_GatherControl.Start(t);
           
            //任务启动成功显示消息
            InvokeMethod(this, "ShowInfo", new object[] {TaskName ,rm.GetString ("TaskStarted")  });

            t = null;
        }

        private void On_ListenError(object sender, cListenErrorEventArgs e)
        {
            InvokeMethod(this, "ExportLog", new object[] { e.Message });
        }
        #endregion

        #region 菜单 工具条 树形结构 listview 等控件的 响应事件


        private void rmenuAddTaskClass_Click(object sender, EventArgs e)
        {
            NewTaskClass();
        }

        private void toolNewTask_ButtonClick(object sender, EventArgs e)
        {
            NewTask();
        }

        //启动任务，当前设计是只能启动一个任务，不支持启动多个任务
        private void toolStartTask_Click(object sender, EventArgs e)
        {

            StartMultiTask();
            
        }

        private void StartMultiTask()
        {
            if (this.dataTask.SelectedRows.Count == 0)
                return;

            for (int index=0; index < this.dataTask.SelectedRows.Count; index++)
            {
                if ((cGlobalParas.TaskState)this.dataTask.SelectedRows[index].Cells[2].Value == cGlobalParas.TaskState.Failed)
                {
                    continue;
                }
                else
                {
                    StartTask(Int64.Parse(this.dataTask.SelectedRows[index].Cells[1].Value.ToString()), this.dataTask.SelectedRows[index].Cells[4].Value.ToString(), index);
                }
            }
        }

        private void StartTask(Int64 TaskID,string TaskName,int SelectedIndex)
        {
            cGatherTask t = null;

            //判断当前选择的树节点
            if (this.treeMenu.SelectedNode.Name == "nodRunning" && this.dataTask.SelectedCells.Count != 0)
            {
                //执行正在执行的任务
                t = m_GatherControl.TaskManage.FindTask(TaskID);
            }
            else if (this.treeMenu.SelectedNode.Name.Substring(0, 1) == "C" || this.treeMenu.SelectedNode.Name == "nodTaskClass")
            {
                ///如果是选择的任务分类节点，点击此按钮首先先将此任务加载到运行区，然后调用
                ///starttask方法，启动任务。
                string tClassName = "";
                if (this.treeMenu.SelectedNode.Name == "nodTaskClass")
                    tClassName = "";
                else
                    tClassName = this.treeMenu.SelectedNode.Text;

                t = AddRunTask(tClassName,TaskName);

                //如果是新增的任务，则传进来的TaskID是任务的编号，并不是任务执行的编号（即不是由时间自动产生的任务）
                //是一个递增的整数，所以，需要重新更新传入的TaskID

                if (t == null)
                {
                    //表示启动任务被用户中断，也有可能是因为错误造成
                    return;
                }

                TaskID = t.TaskID;
            }

            //判断此任务是否需要登录，如果需要登录则需要用户干预
            if (t.TaskData.IsLogin == true)
            {
                frmBrowser f = new frmBrowser(t.TaskData.LoginUrl);
                f.Owner = this;
                f.rCookie = new frmBrowser.ReturnCookie(GetCookie);
                f.getFlag = 0;
                if (f.ShowDialog() == DialogResult.Cancel)
                {
                    f.Dispose();

                    //string strLog = (int)cGlobalParas.LogType.Info + "您取消了Cookie获取操作，任务被迫中断！";
                    //string conName = "sCon" + t.TaskID;
                    //string pageName = "page" + t.TaskID;

                    //this.tabControl1.TabPages[pageName].Controls[conName].Controls[1].Controls[0].Text = strLog;

                    MessageBox.Show(rm.GetString("Info11"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return;
                }
                f.Dispose();

                t.UpdateCookie(this.Cookie);

            }

            //任务成功启动后，需要建立TabPage用于显示此任务的日志及采集数据的信息
            if (this.treeMenu.SelectedNode.Name.ToString() == "nodRunning" && (int.Parse(this.dataTask.SelectedRows[SelectedIndex].Cells[7].Value.ToString()) + int.Parse(this.dataTask.SelectedRows[SelectedIndex].Cells[8].Value.ToString())) > 0)
            {
                BrowserData(TaskID,TaskName);
            }
            else
            {
                AddTab(TaskID, TaskName);
            }

            //启动此任务
            m_GatherControl.Start(t);

           
            //任务启动成功显示消息
            ShowInfo(TaskName,rm.GetString("TaskStarted"));

            t = null;
        }

        //通过用户登录获取cookie
        private string m_Cookie;
        private string Cookie
        {
            get { return m_Cookie; }
            set { m_Cookie = value; }
        }

        private void GetCookie(string strCookie)
        {
            this.Cookie= strCookie;
        }

        private void treeMenu_AfterSelect(object sender, TreeViewEventArgs e)
        {

            //switch (e.Node.Name)
            //{
            //    case "nodRunning":   //运行区的任务
            //        try
            //        {
            //            LoadRunTask(e.Node.Name);
            //        }
            //        catch (System.IO.IOException)
            //        {
            //            if (MessageBox.Show("任务运行监控文件丢失，请问是否根据正在运行的任务情况自动创建？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //            {
            //                CreateTaskRun();
            //            }
            //        }
            //        catch (System.Exception)
            //        {
            //            MessageBox.Show("加载的任务运行监控文件非法，请检查此文件“" + Program.getPrjPath() + "tasks\\taskrun.xml" + "”，如果格式非法，请通过Windows文件浏览器删除，并重新点击此节点由系统自动建立！", "系统信息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            //        }

            //        SetDataShow();

            //        //启动时间器用于更新任务显示的进度
            //        this.timer1.Enabled = true;

            //        break;

            //    case "nodPublish":

            //        LoadPublishTask();

            //        //启动时间器用于更新任务显示的进度
            //        this.timer1.Enabled = true;

            //        SetDataShow();

            //        break;

            //    case "nodComplete":    //已经完成采集的任务

            //        try
            //        {
            //            LoadCompleteTask();
            //        }
            //        catch (System.IO.IOException)
            //        {
            //            if (MessageBox.Show("已完成任务索引文件丢失，需要重新建立，但已经完成的任务信息将会丢失，数据内容不会丢失，默认存储在：" + Program.getPrjPath() + "Data" + "目录下，是否建立？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //            {
            //                CreateTaskComplete();
            //            }
            //        }
            //        catch (System.Exception)
            //        {
            //            MessageBox.Show("加载的已完成任务索引文件非法，请检查此文件“" + Program.getPrjPath() + "data\\index.xml" + "”，如果格式非法，请通过Windows文件浏览器删除，并重新点击此节点由系统自动建立！", "系统信息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            //        };

            //        SetDataShow();

            //        this.timer1.Enabled = false;

            //        break;

            //    default:
            //        try
            //        {
            //            LoadOther(e.Node);
            //        }
            //        catch (System.IO.IOException)
            //        {
            //            if (MessageBox.Show(this.treeMenu.SelectedNode.Text + "分类下的索引文件丢失，请问是否自动创建？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //            {
            //                CreateTaskIndex(this.treeMenu.SelectedNode.Tag.ToString());
            //            }
            //        }
            //        catch (System.Exception)
            //        {
            //            MessageBox.Show("加载的任务分类索引文件非法，请检查此文件“" + e.Node.Tag.ToString() + "\\index.xml" + "”，如果格式非法，请通过Windows文件浏览器删除，并重新点击此节点由系统自动建立！", "系统信息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            //        }

            //        SetDataHide();

            //        this.timer1.Enabled = false;
            //        break;
            //}

            ////无论点击树形结构的菜单都将按钮置为不可用
            //this.toolStartTask.Enabled = false;
            //this.toolRestartTask.Enabled = false;
            //this.toolStopTask.Enabled = false;
            //this.toolExportData.Enabled = false;
            //this.toolBrowserData.Enabled = false;

            ////置删除按钮为有效
            //DelName = this.treeMenu.Name;
            //this.toolDelTask.Enabled = true;

            //UpdateStatebarTaskState("当前显示： " + e.Node.Text);

        }

        //树形结构的设置确实很不合理，不过真的是懒的改了，下一个版本会修正
        //因为下一个版本会扩充树形结构的应用，不得不改了，呵呵
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (this.treeMenu.SelectedNode.Name.Substring(0, 1) == "C")
            {
                //this.rmmenuStopTask.Enabled = false;
                this.rmenuAddTaskClass.Enabled = true;
                this.rmenuDelTaskClass.Enabled = true;

                this.rmenuRenameTaskClass.Enabled = true;
            }
            else
            {
                //this.rmmenuStopTask.Enabled = true;
                this.rmenuAddTaskClass.Enabled = true;
                this.rmenuDelTaskClass.Enabled = false;

                this.rmenuRenameTaskClass.Enabled = false;
            }
                  
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            if (this.dataTask.SelectedRows.Count  == 0)
            {
                this.rmmenuStartTask.Enabled = false;
                this.rmmenuStopTask.Enabled = false;
                this.rmmenuRestartTask.Enabled = false;
                this.rmmenuNewTask.Enabled = true;
                this.rmmenuEditTask.Enabled = false;
                this.rmmenuDelTask.Enabled = false;
                this.rmenuBrowserData.Enabled = false;

                this.rmmenuRenameTask.Enabled = false;

                this.rmenuAddPlan.Enabled = true;

                this.rmenuCopyTask.Visible = false;
                this.rmenuPasteTask.Visible = false;
                this.toolStripSeparator18.Visible = false;

                if (this.treeMenu.SelectedNode.Name.Substring(0, 1) == "C" || this.treeMenu.SelectedNode.Name == "nodTaskClass")
                {
                    if (IsClipboardSoukeyData())
                    {

                        this.rmenuPasteTask.Visible = true;
                        this.rmenuPasteTask.Enabled = true;
                    }
                    else
                    {
                        this.rmenuPasteTask.Visible = false;
                    }
                }
                else
                {
                    this.rmenuPasteTask.Visible = false;

                }
                return;
            }

            cGlobalParas.TaskState tState = (cGlobalParas.TaskState)this.dataTask.SelectedCells[2].Value;

            switch (this.treeMenu.SelectedNode.Name)
            {
                case "nodRunning":

                    

                    if (int.Parse(dataTask.SelectedCells[7].Value.ToString()) > 0)
                        this.rmenuBrowserData.Enabled = true;
                    else
                        this.rmenuBrowserData.Enabled = false;

                    switch (tState)
                    {
                        case cGlobalParas.TaskState.Started:
                    
                            this.rmmenuStartTask.Enabled = false;
                            this.rmmenuStopTask.Enabled = true;
                            this.rmmenuRestartTask.Enabled = true;
                            break ;
                        case cGlobalParas.TaskState.Failed:
                    
                            this.rmmenuStartTask.Enabled = false;
                            this.rmmenuStopTask.Enabled = false;
                            this.rmmenuRestartTask.Enabled = false;
                            this.rmenuBrowserData.Enabled = false;
                            break ;
                        case cGlobalParas.TaskState.Running :

                            this.rmmenuStartTask.Enabled = false;
                            this.rmmenuStopTask.Enabled = true;
                            this.rmmenuRestartTask.Enabled = false;
                            break;
                        default:
                    
                            this.rmmenuStartTask.Enabled = true;
                            this.rmmenuStopTask.Enabled = false;
                            this.rmmenuRestartTask.Enabled = true;
                            break;
                    }

                    this.rmmenuRenameTask.Enabled = false;

                    this.rmmenuNewTask.Enabled = true ;
                    this.rmmenuEditTask.Enabled = false;
                    this.rmmenuDelTask.Enabled = true;

                    this.rmenuAddPlan.Enabled = true;

                    this.rmenuCopyTask.Visible = false;
                    this.rmenuPasteTask.Visible = false;
                    this.toolStripSeparator18.Visible = false;


                    break;

                case "nodComplete":
                    this.rmmenuStartTask.Enabled = false;
                    this.rmmenuStopTask.Enabled = false;
                    this.rmmenuRestartTask.Enabled = false;
                    this.rmmenuNewTask.Enabled = true;
                    this.rmmenuDelTask.Enabled = true;
                    this.rmmenuEditTask.Enabled = false;
                    if (this.dataTask.Rows.Count == 0)
                    {
                        this.rmenuBrowserData.Enabled = false;
                        this.rmmenuDelTask.Enabled = false;
                    }
                    else
                    {
                        this.rmenuBrowserData.Enabled = true;
                        this.rmmenuDelTask.Enabled = true;
                    }

                    this.rmenuAddPlan.Enabled = true;

                    this.rmmenuRenameTask.Enabled = false;

                    this.rmenuCopyTask.Visible = false;
                    this.rmenuPasteTask.Visible = false;
                    this.toolStripSeparator18.Visible = false;


                    break;
                case "nodPublish":
                    this.rmmenuStartTask.Enabled = false;
                    this.rmmenuStopTask.Enabled = false;
                    this.rmmenuRestartTask.Enabled = false;
                    this.rmmenuNewTask.Enabled = false;
                    this.rmmenuEditTask.Enabled = false;
                    this.rmmenuDelTask.Enabled = false;
                    this.rmenuBrowserData.Enabled = true;

                    this.rmenuAddPlan.Enabled = true;

                    this.rmmenuRenameTask.Enabled = false;

                    this.rmenuCopyTask.Visible = false;
                    this.rmenuPasteTask.Visible = false;
                    this.toolStripSeparator18.Visible = false;


                    break;
                case "nodPlanRunning":

                    this.rmmenuStartTask.Enabled = false;
                    this.rmmenuStopTask.Enabled = false;
                    this.rmmenuRestartTask.Enabled = false;
                    this.rmmenuNewTask.Enabled = true;
                    this.rmmenuEditTask.Enabled = true ;
                    this.rmmenuDelTask.Enabled = true ;
                    this.rmenuBrowserData.Enabled = false;

                    this.rmenuAddPlan.Enabled = true;

                    this.rmmenuRenameTask.Enabled = true;

                    this.rmenuCopyTask.Visible = false;
                    this.rmenuPasteTask.Visible = false;
                    this.toolStripSeparator18.Visible = false;

                    break;
                case "nodPlanCompleted":

                    this.rmmenuStartTask.Enabled = false;
                    this.rmmenuStopTask.Enabled = false;
                    this.rmmenuRestartTask.Enabled = false;
                    this.rmenuBrowserData.Enabled = false;

                    this.rmenuAddPlan.Enabled = true;
                    this.rmmenuEditTask.Enabled = false ;
                    this.rmmenuDelTask.Enabled = true;

                    this.rmmenuRenameTask.Enabled = false;

                    this.rmenuCopyTask.Visible = false;
                    this.rmenuPasteTask.Visible = false;
                    this.toolStripSeparator18.Visible = false;

                    break;
                default :
                    if (tState == cGlobalParas.TaskState.Failed)
                    {
                        this.rmmenuStartTask.Enabled = false;
                        this.rmmenuEditTask.Enabled = false;
                    }
                    else
                    {
                        this.rmmenuStartTask.Enabled = true;
                        this.rmmenuEditTask.Enabled = true;
                    }

                    this.rmmenuStopTask.Enabled = false;
                    this.rmmenuRestartTask.Enabled = false;
                    this.rmmenuNewTask.Enabled = true;
                    this.rmmenuDelTask.Enabled = true;
                    this.rmenuBrowserData.Enabled = false;

                    this.rmenuAddPlan.Enabled = true;

                    this.rmmenuRenameTask.Enabled = true ;

                    this.rmenuCopyTask.Visible = true;
                    this.rmenuPasteTask.Visible = true;
                    this.toolStripSeparator18.Visible = true;

                    if (!IsClipboardSoukeyData())
                    {
                        this.rmenuPasteTask.Enabled = false;
                    }
                    else
                    {
                        this.rmenuPasteTask.Enabled = true;
                    }

                    break;
            }
        }

        private void rmenuDelTaskClass_Click(object sender, EventArgs e)
        {
            
                DelTaskClass();
            
        }

        private void treeMenu_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    try
                    {
                        DelTaskClass();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return;
                    }
                    break;
                case Keys.F2:
                    if (this.treeMenu.SelectedNode.Name.Substring(0, 1) == "C")
                        this.treeMenu.SelectedNode.BeginEdit();

                    break;

            }

        }

        private void rmmenuNewTask_Click(object sender, EventArgs e)
        {
            NewTask();
        }

        private void rmmenuEditTask_Click(object sender, EventArgs e)
        {
            switch (this.treeMenu.SelectedNode.Name)
            {
                case "nodSnap":
                    break;
                case "nodRunning":
                    
                    break;
                case "nodPublish":
                    break;
                case "nodComplete":
                   
                    break;
                case "nodTaskPlan":
                    
                    break;
                case "nodPlanRunning":
                    EditPlan();
                    break;
                case "nodPlanCompleted":
                   
                    break;
                case "nodTaskClass":
                    EditTask();
                    break;
                default:
                   EditTask();
                   break;
            }
            

           
        }

        private void rmmenuDelTask_Click(object sender, EventArgs e)
        {

            this.dataTask.Focus();
            SendKeys.Send("{Del}");
           
        }

        private void myListData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (DelTask() == true)
                {
                    //this.myListData.Items.Remove(this.myListData.SelectedItems[0]);
                }
            }
        }


        #endregion

        #region 界面控件事件 所调用的方法
        private void NewTaskClass()
        {
            frmTaskClass frmTClass = new frmTaskClass();
            frmTClass.RTaskClass = new frmTaskClass.ReturnTaskClass(AddTaskClassNode);
            frmTClass.ShowDialog();
            frmTClass.Dispose();
        }

        //当添加任务分类后，根据新添加的信息，开始添加任务分类树形结构
        private void AddTaskClassNode(int TaskClassID, string TaskClassName, string TaskClassPath)
        {
            TreeNode newNode = new TreeNode();
            newNode.Tag = TaskClassPath;
            newNode.Name = "C" + TaskClassID;
            newNode.Text = TaskClassName;
            newNode.ImageIndex = 0;
            newNode.SelectedImageIndex = 0;
            this.treeMenu.Nodes["nodTaskClass"].Nodes.Add(newNode);
            
            this.treeMenu.SelectedNode = newNode;

            treeMenu_NodeMouseClick(this.treeMenu, new TreeNodeMouseClickEventArgs(this.treeMenu.SelectedNode, MouseButtons.Left, 0, 0, 0));

        }

        private void NewTask()
        {
            string TClass = "";
            if (this.treeMenu.SelectedNode.Name.Substring(0, 1) == "C")
            {
                //表示选择的是分类节点
                TClass = this.treeMenu.SelectedNode.Text;
            }
            frmTask fTask = new frmTask();
            fTask.NewTask(TClass);
            fTask.FormState = cGlobalParas.FormState.New;
            fTask.RShowWizard = ShowTaskWizard;
            fTask.rTClass = refreshNode;
            fTask.ShowDialog();
            fTask.Dispose();

            //刷新一下当前所显示的节点信息，因为已经新增了一个任务

        }

        private void ShowTaskWizard(bool IsShow)
        {
            if (IsShow == true)
            {
                string TClass = "";
                if (this.treeMenu.SelectedNode.Name.Substring(0, 1) == "C")
                {
                    //表示选择的是分类节点
                    TClass = this.treeMenu.SelectedNode.Text;
                }

                frmAddTaskWizard ft = new frmAddTaskWizard();
                ft.NewTask(TClass);
                ft.FormState = cGlobalParas.FormState.New;
                ft.rTClass = refreshNode;
                ft.ShowDialog();
                ft.Dispose();
            }
        }

        private void refreshNode(string TClass)
        {
            if (TClass == "")
            {
                TreeNode SelectNode = new TreeNode();
                SelectNode = this.treeMenu.Nodes[1];
                this.treeMenu.SelectedNode = SelectNode;

                LoadTask(this.treeMenu.Nodes["nodTaskClass"]);
                return;
            }
            
            foreach (TreeNode a in this.treeMenu.Nodes["nodTaskClass"].Nodes)
            {
                if (a.Text ==TClass)
                {
                    this.treeMenu.SelectedNode = a;

                    LoadTask (a);
                    break ;
                }
            }

        }

        private void CreateTaskIndex(string tPath)
        {
            Task.cTaskIndex tIndex = new Task.cTaskIndex();
            tIndex.NewIndexFile(tPath);
            tIndex = null;

        }

        private void CreateTaskRun()
        {
            Task.cTaskRun tRun = new Task.cTaskRun();
            tRun.NewTaskRunFile();
            tRun = null;
        }

        private void CreateTaskComplete()
        {
            Task.cTaskComplete t = new Task.cTaskComplete();
            t.NewTaskCompleteFile();
            t = null;
        }

        private void LoadPublishTask()
        {
            ShowPublishTask();

            ///任务发布做的很简单，当任务采集完成后，自动启动开始进行数据的发布，
            /// 不允许进行人工干预，当前认为此种发布方式不具备太大的实用性，所以
            /// 当前的作为是一种临时的做法，后期会逐步完善，希望可以找到合适的发布
            /// 方式
            ///需要发布的数据不进行本地文件的保存，直接保存在m_PublishControl中

            foreach (cPublishTask t in m_PublishControl.PublishManage.ListPublish)
            {
                dataTask.Rows.Add(imageList1.Images["export"], t.TaskData.TaskID , cGlobalParas.TaskState.Publishing, this.treeMenu.SelectedNode.Name,
                    t.TaskData.TaskName, t.PublishedCount, t.Count, (t.Count ==0? 0:t.PublishedCount * 100 / t.Count),
                                   cGlobalParas.ConvertName ((int) t.PublishType));
                                  
            }

            this.dataTask.Sort(this.dataTask.Columns[4], ListSortDirection.Ascending);

            this.dataTask.ClearSelection();

        }

        private void LoadCompleteTask()
        {
            ShowCompletedTask();

            //从完成的任务中加载
            cTaskComplete t = new cTaskComplete();
            t.LoadTaskData();

            for (int i = 0; i < t.GetCount(); i++)
            {
                dataTask.Rows.Add(imageList1.Images["OK"],t.GetTaskID (i), cGlobalParas.TaskState.Completed, this.treeMenu.SelectedNode.Name,
                                   t.GetTaskName (i), cGlobalParas.ConvertName((int)t.GetTaskType (i)),
                                   //t.GetGatheredUrlCount(i), t.GetUrlCount(i) - t.GetGatheredUrlCount(i),
                                   t.GetUrlCount(i), cGlobalParas.ConvertName((int)t.GetTaskRunType(i)),
                                   cGlobalParas.ConvertName((int)t.GetPublishType(i)) );
            }

            this.dataTask.Sort(this.dataTask.Columns[4], ListSortDirection.Ascending);

            this.dataTask.ClearSelection();

        }

        //加载正在执行的任务，正在执行的任务记录在应用程序目录下的RunningTask.xml文件中
        private void LoadRunTask()
        {

            ShowRunTask();

            //开始初始化正在运行的任务
            //从m_TaskControl中读取
            //每次加载会加载正在运行、等待、停止队列中的任务
            List<cGatherTask> taskList=new List<cGatherTask>();
            taskList.AddRange(m_GatherControl.TaskManage.TaskListControl.RunningTaskList);
            taskList.AddRange(m_GatherControl.TaskManage.TaskListControl.StoppedTaskList);
            taskList.AddRange(m_GatherControl.TaskManage.TaskListControl.WaitingTaskList);
            //taskList.AddRange(m_GatherControl.TaskManage.TaskListControl.CompletedTaskList);

            for (int i = 0; i < taskList.Count ; i++)
            {
                try
                {
                    switch (taskList[i].State)
                    {
                        case cGlobalParas.TaskState.Started:
                            dataTask.Rows.Add(imageList1.Images["started"], taskList[i].TaskID, taskList[i].State, this.treeMenu.SelectedNode.Name,
                                taskList[i].TaskName, cGlobalParas.ConvertName((int)taskList[i].TaskType), (taskList[i].IsLogin == true ? rm.GetString ("Logon") : rm.GetString ("NoLogon")),
                                taskList[i].GatheredTrueUrlCount, taskList[i].GatheredTrueErrUrlCount, taskList[i].TrueUrlCount, (taskList[i].GatheredTrueUrlCount + taskList[i].GatheredTrueErrUrlCount) * 100 / taskList[i].TrueUrlCount, cGlobalParas.ConvertName((int)taskList[i].RunType),
                                cGlobalParas.ConvertName((int)taskList[i].PublishType));
                            break;

                        case cGlobalParas.TaskState.Stopped:
                            if ((taskList[i].GatheredTrueUrlCount + taskList[i].GatheredTrueErrUrlCount) > 0)
                            {
                                dataTask.Rows.Add(imageList1.Images["pause"], taskList[i].TaskID, taskList[i].State, this.treeMenu.SelectedNode.Name,
                                    taskList[i].TaskName, cGlobalParas.ConvertName((int)taskList[i].TaskType), (taskList[i].IsLogin == true ? rm.GetString("Logon") : rm.GetString("NoLogon")),
                                    taskList[i].GatheredTrueUrlCount, taskList[i].GatheredTrueErrUrlCount, taskList[i].TrueUrlCount, (taskList[i].GatheredTrueUrlCount + taskList[i].GatheredTrueErrUrlCount) * 100 / taskList[i].TrueUrlCount, cGlobalParas.ConvertName((int)taskList[i].RunType),
                                    cGlobalParas.ConvertName((int)taskList[i].PublishType));
                            }
                            else
                            {
                                dataTask.Rows.Add(imageList1.Images["stop"], taskList[i].TaskID, taskList[i].State, this.treeMenu.SelectedNode.Name,
                                    taskList[i].TaskName, cGlobalParas.ConvertName((int)taskList[i].TaskType), (taskList[i].IsLogin == true ? rm.GetString("Logon") : rm.GetString("NoLogon")),
                                    taskList[i].GatheredTrueUrlCount, taskList[i].GatheredTrueErrUrlCount, taskList[i].TrueUrlCount, (taskList[i].GatheredTrueUrlCount + taskList[i].GatheredTrueErrUrlCount) * 100 / taskList[i].TrueUrlCount, cGlobalParas.ConvertName((int)taskList[i].RunType),
                                    cGlobalParas.ConvertName((int)taskList[i].PublishType));
                            }
                            break;
                        case cGlobalParas.TaskState.UnStart:
                            dataTask.Rows.Add(imageList1.Images["stop"], taskList[i].TaskID, taskList[i].State, this.treeMenu.SelectedNode.Name,
                                taskList[i].TaskName, cGlobalParas.ConvertName((int)taskList[i].TaskType), (taskList[i].IsLogin == true ? rm.GetString("Logon") : rm.GetString("NoLogon")),
                                taskList[i].GatheredTrueUrlCount, taskList[i].GatheredTrueErrUrlCount, taskList[i].TrueUrlCount, (taskList[i].GatheredTrueUrlCount + taskList[i].GatheredTrueErrUrlCount) * 100 / taskList[i].TrueUrlCount, cGlobalParas.ConvertName((int)taskList[i].RunType),
                                cGlobalParas.ConvertName((int)taskList[i].PublishType));
                            break;
                        case cGlobalParas.TaskState.Failed:
                            dataTask.Rows.Add(imageList1.Images["error"], taskList[i].TaskID, taskList[i].State, this.treeMenu.SelectedNode.Name,
                                taskList[i].TaskName, "", "",
                                "0", "0", "0", "0", "",
                               rm.GetString ("Info14"));
                            dataTask.Rows[dataTask.Rows.Count - 1].DefaultCellStyle = this.m_RowStyleErr;
                            break;
                        default:
                            dataTask.Rows.Add(imageList1.Images["stop"], taskList[i].TaskID, taskList[i].State, this.treeMenu.SelectedNode.Name,
                                taskList[i].TaskName, cGlobalParas.ConvertName((int)taskList[i].TaskType), (taskList[i].IsLogin == true ? rm.GetString("Logon") : rm.GetString("NoLogon")),
                                taskList[i].GatheredTrueUrlCount, taskList[i].GatheredTrueErrUrlCount, taskList[i].TrueUrlCount, (taskList[i].GatheredTrueUrlCount + taskList[i].GatheredTrueErrUrlCount) * 100 / (taskList[i].TrueUrlCount==0 ? 1:taskList[i].TrueUrlCount), cGlobalParas.ConvertName((int)taskList[i].RunType),
                                cGlobalParas.ConvertName((int)taskList[i].PublishType));
                            break;
                    }
                }
                catch (System.Exception ex)
                {
                    //捕获错误，不做处理，让信息继续加载
                    ExportLog(ex.Message);

                }
            }

            this.dataTask.Sort(this.dataTask.Columns[4], ListSortDirection.Ascending);

            this.dataTask.ClearSelection();

            taskList = null;

        }

        //加载任务执行计划
        private void LoadTaskPlan()
        {
            ShowTaskPlan();

            cPlans p = new cPlans();

            p.LoadPlans();

            string runPlan = "";

            int count = 0;

            if (p.Plans == null || p.Plans.Count == 0)
            {
                count = 0;
            }
            else
            {
                count = p.Plans.Count;
            }

            for (int i = 0; i < count; i++)
            {

                switch (p.Plans[i].RunTaskPlanType)
                {
                    case (int)cGlobalParas.RunTaskPlanType .Ones :
                        runPlan =  p.Plans[i].RunOnesTime ;
                        break;
                    case (int)cGlobalParas.RunTaskPlanType.DayOnes :
                        runPlan = rm.GetString ("Everyday") + " " + p.Plans[i].RunDayTime;
                        break;
                    case (int)cGlobalParas.RunTaskPlanType.DayTwice :
                        runPlan = rm.GetString ("AM") + " " + p.Plans[i].RunAMTime + "  " + rm.GetString ("PM") + " " + p.Plans[i].RunPMTime + " " + rm.GetString("Run"); ;
                        break;
                    case (int)cGlobalParas.RunTaskPlanType.Weekly :
                        runPlan = rm.GetString ("Weekly") + " ";

                        string rWeekly = p.Plans[i].RunWeekly;
                        foreach (string sc in rWeekly.Split(','))
                        {
                            string ss = sc.Trim();
                            switch (ss)
                            {
                                case "0":
                                    runPlan += rm.GetString ("W0") + " ";
                                    break;
                                case "1":
                                    runPlan += rm.GetString("W1") + " ";
                                    break;
                                case "2":
                                    runPlan += rm.GetString("W2") + " ";
                                    break;
                                case "3":
                                    runPlan += rm.GetString("W3") + " ";
                                    break;
                                case "4":
                                    runPlan += rm.GetString("W4") + " ";
                                    break;
                                case "5":
                                    runPlan += rm.GetString("W5") + " ";
                                    break;
                                case "6":
                                    runPlan += rm.GetString("W6") + " ";
                                    break;
                                default :
                                    break;
                            }
                        }

                        runPlan += " " + p.Plans[i].RunWeeklyTime ;
                        break;
                    case (int)cGlobalParas.RunTaskPlanType.Custom :
                        runPlan = rm.GetString ("Info15") + " " + p.Plans[i].FirstRunTime + " " + rm.GetString ("Info16") + " " + p.Plans[i].RunInterval ;

                        break;
                }

                if (p.Plans[i].IsDisabled == true)
                {
                    string strDisabled = "";
                    if (p.Plans[i].DisabledType ==(int) cGlobalParas.PlanDisabledType.RunTime)
                        strDisabled = rm.GetString ("Run") + p.Plans[i].DisabledTime ;
                    else
                        strDisabled = rm.GetString ("Info17") + p.Plans[i].DisabledDateTime ;

                    if ( p.Plans[i].PlanState==(int)cGlobalParas.PlanState.Enabled )

                        dataTask.Rows.Add(imageList1.Images["taskplan"], p.Plans[i].PlanID, p.Plans[i].PlanState, this.treeMenu.SelectedNode.Name,
                            p.Plans[i].PlanName, cGlobalParas.ConvertName(p.Plans[i].PlanState), cGlobalParas.ConvertName(p.Plans[i].DisabledType ),
                            strDisabled, runPlan,
                            p.Plans[i].NextRunTime, p.Plans[i].PlanRemark);
                    else
                        dataTask.Rows.Add(imageList1.Images["disabledplan"], p.Plans[i].PlanID, p.Plans[i].PlanState, this.treeMenu.SelectedNode.Name,
                            p.Plans[i].PlanName, cGlobalParas.ConvertName(p.Plans[i].PlanState), cGlobalParas.ConvertName(p.Plans[i].DisabledType),
                            strDisabled, runPlan,
                            p.Plans[i].NextRunTime, p.Plans[i].PlanRemark);
            }
                else
                {
                    dataTask.Rows.Add(imageList1.Images["taskplan"], p.Plans[i].PlanID, p.Plans[i].PlanState, this.treeMenu.SelectedNode.Name,
                         p.Plans[i].PlanName, cGlobalParas.ConvertName(p.Plans[i].PlanState), rm.GetString("Info18"),
                        "", runPlan,
                        p.Plans[i].NextRunTime, p.Plans[i].PlanRemark);
                }
               
            }

            p = null;

            this.dataTask.Sort(this.dataTask.Columns[4], ListSortDirection.Ascending);

            this.dataTask.ClearSelection();

        }

        //此部分的数据是根据当前已经完成的导出任务
        //实时产生的数据
        private void LoadExportDataTask(string mNode)
        {
            //this.myListData.Items.Clear();
        }

        private void LoadOther(TreeNode mNode)
        {
            //任务分类作为一个特殊的分类进行处理
            //此节点所有的内容全部为默认，不提供用户可操作的功能
            if (mNode.Name.Substring(0, 1) == "C" || mNode.Name == "nodTaskClass")
            {
                //表示加载的是任务信息
                LoadTask(mNode);
            }
            else
            {

            }
        }

        ///加载系统默认的任务，此任务无法删除，是Soukey采摘 最新版本软件下载
        private void LoadSoukeyTask()
        {
            
        }

        private void LoadTask(TreeNode  mNode)
        {
            ShowTaskInfo();


            Task.cTaskIndex xmlTasks = new Task.cTaskIndex();

            if (mNode.Name == "nodTaskClass")
            {
                xmlTasks.GetTaskDataByClass();
            }
            else
            {
                //因可能无法保证id的唯一性，所以，所有的内容全部都按照名称索取
                string TaskClassName = mNode.Text;
                xmlTasks.GetTaskDataByClass(TaskClassName);
            }

            //开始初始化此分类下的任务
            int count = xmlTasks.GetTaskClassCount();

            for (int i = 0; i < count; i++)
            {
                if (xmlTasks.GetTaskState(i) == cGlobalParas.TaskState.Failed)
                {
                    
                    dataTask.Rows.Add(imageList1.Images["error"], xmlTasks.GetTaskID(i), xmlTasks.GetTaskState(i), this.treeMenu.SelectedNode.Name, xmlTasks.GetTaskName(i),
                        "", "",
                       "","",
                       "任务加载失败，请删除后重建！");
                    dataTask.Rows[dataTask.Rows.Count - 1].DefaultCellStyle = this.m_RowStyleErr;

                    
                }
                else
                {
                    dataTask.Rows.Add(imageList1.Images["task"], xmlTasks.GetTaskID(i), xmlTasks.GetTaskState(i), this.treeMenu.SelectedNode.Name, xmlTasks.GetTaskName(i),
                        cGlobalParas.ConvertName(int.Parse(xmlTasks.GetTaskType(i).ToString())), (xmlTasks.GetIsLogin(i) == true ? rm.GetString("Logon") : rm.GetString("NoLogon")),
                       xmlTasks.GetWebLinkCount(i).ToString(), cGlobalParas.ConvertName(int.Parse(xmlTasks.GetTaskRunType(i).ToString())),
                       cGlobalParas.ConvertName((int)xmlTasks.GetPublishType(i)));
                }
            }
            xmlTasks = null;


            this.dataTask.Sort(this.dataTask.Columns[4], ListSortDirection.Ascending);

            this.dataTask.ClearSelection();

        }

        //加载任务，可以加载任务信息和正在运行的任务
        //注意，如果加载的是运行区的任务，则不可以进行任务的升级操作
        //当前判断的条件是：如果是编辑任务则判断任务版本是否符合当前版本
        //约束，如果不是，则进行任务升级。如果是浏览任务，则无论是否满足
        //版本约束，都不进行升级操作
        private void LoadTaskInfo(string FilePath, string FileName ,cGlobalParas.FormState fState)
        {

            frmTask ft=null ;

            LoadAgain:

            try
            {
                ft = new frmTask();
                ft.EditTask(FilePath, FileName);
                ft.FormState = fState;
                ft.RShowWizard = ShowTaskWizard;
                ft.rTClass = refreshNode;
                ft.ShowDialog();
                ft.Dispose();
            }
            catch (cSoukeyException)
            {
                if (fState == cGlobalParas.FormState.Browser)
                {
                    MessageBox.Show(rm.GetString("Info19"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ft = null;
                    return;
                }

                if (MessageBox.Show(rm.GetString("Quaere4"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    ft = null;

                    return;
                }
                else
                {
                    ft = null;
                    
                    frmUpgradeTask fu = new frmUpgradeTask(FilePath + "\\" + FileName);
                    fu.ShowDialog();
                    fu.Dispose();
                    fu = null;
                    return;

                }
            }

                
        }

        private void LoadPlanLog()
        {
            ShowPlanLog();

            cPlanRunLog rLog = new cPlanRunLog();

            rLog.LoadLog();

            int Count = rLog.GetLogCount();

            for (int i = 0; i < Count; i++)
            {
                dataTask.Rows.Add(imageList1.Images["log"], rLog.GetPlanID(i), cGlobalParas.ConvertID ( rLog.GetLogType(i)), this.treeMenu.SelectedNode.Name,
                        rLog.GetPlanName (i),rLog.GetTaskType (i), 
                        rLog.GetFileName (i),
                       rLog.GetFilePara (i),rLog.GetRunTime (i));
            }

            rLog = null;

            this.dataTask.Sort(this.dataTask.Columns[4], ListSortDirection.Ascending);

            this.dataTask.ClearSelection();
        }

        private void DelTaskClass()
        {
            if (this.treeMenu.SelectedNode.Name.Substring(0, 1) != "C")
            {
                if (this.treeMenu.SelectedNode.Name == "nodPlanCompleted")
                {
                    DelPlanLog();
                    return;
                }
                else
                {
                    MessageBox.Show(rm.GetString("Info20") + this.treeMenu.SelectedNode.Text, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            

            if (MessageBox.Show(rm.GetString ("Info21") + this.treeMenu.SelectedNode.Text   + "\r\n" + rm.GetString ("Info22"),
               rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    Task.cTaskClass tClass = new Task.cTaskClass();
                    
                    if (tClass.DelTaskClass(this.treeMenu.SelectedNode.Text))
                    {
                        tClass = null;
                    }
               
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(rm.GetString("Info23") + ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }

                this.treeMenu.Nodes.Remove(this.treeMenu.SelectedNode);

                treeMenu_NodeMouseClick(this.treeMenu, new TreeNodeMouseClickEventArgs(this.treeMenu.SelectedNode, MouseButtons.Left, 0, 0, 0));
            }


           
        }

        private bool DelPlan()
        {
            if (this.dataTask.SelectedRows.Count == 0)
            {
                return false;
            }

            if (this.dataTask.SelectedRows.Count == 1)
            {
                if (MessageBox.Show(rm.GetString ("Info24") + this.dataTask.SelectedCells[4].Value.ToString() + "\r\n"
                    + rm.GetString("Info25"), rm.GetString("MessageboxQuaere"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return false;
                }
            }
            else
            {
                if (MessageBox.Show(rm.GetString("Info26"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return false;
                }
            }

            

            try
            {
                cPlans ps = new cPlans();

                for (int index = 0; index < this.dataTask.SelectedRows.Count; index++)
                {
                    ps.DelPlan (this.dataTask.SelectedRows[index].Cells[1].Value.ToString());
                }

                ps = null;

                return true;

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Info27") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
           
        }

        private bool DelTask()
        {
            if (this.dataTask.SelectedRows.Count ==0)
            {
                return false;
            }

     
            if (this.dataTask.SelectedRows.Count ==1)
            {
                if (MessageBox.Show(rm.GetString ("Info28") + this.dataTask.SelectedCells[4].Value + "\r\n"
                    + rm.GetString("Quaere5"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return false;
                }
            }
            else
            {
                if (MessageBox.Show(rm.GetString("Quaere6"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return false;
                }
            }

            //表示选择的是任务节点
            try
            {
                for (int index=0; index < this.dataTask.SelectedRows.Count; index++)
                {
                    Task.cTask t = new Task.cTask();
                    t.DeleTask(this.treeMenu.SelectedNode.Tag.ToString(), this.dataTask.SelectedRows[index].Cells[4].Value.ToString());
                    t = null;

                }
                    return true;
                
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false ;
            }
           

        }

        private bool DelPlanLog()
        {
            if (this.dataTask.SelectedRows.Count == 0)
            {
                return false;
            }

            if (MessageBox.Show(rm.GetString("Quaere7"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return false;
            }

            cPlanRunLog rLog = new cPlanRunLog();
            rLog.DelLog();
            rLog = null;

            //清空显示
            try
            {
                this.dataTask.Rows.Clear();
            }
            catch (System.Exception)
            {
            }

            return true;
        }
        
        /// <summary>
        /// 删除正在运行的任务
        /// </summary>
        private bool DelRunTask()
        {
            if (this.dataTask.SelectedRows.Count == 0)
                return false ;

            if (this.dataTask.SelectedRows.Count == 1)
            {
                if (MessageBox.Show(rm.GetString ("Info29") + this.dataTask.SelectedCells[4].Value.ToString() + "\r\n" +
                    rm.GetString("Quaere9"), rm.GetString("MessageboxQuaere"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return false;
                }
            }
            else
            {
                if (MessageBox.Show(rm.GetString("Quaere8"), rm.GetString("MessageboxQuaere"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return false;
                }
            }

            for (int index = 0; index < this.dataTask.SelectedRows.Count; index++)
            {
                
                cGatherTask t = m_GatherControl.TaskManage.FindTask(Int64.Parse(this.dataTask.SelectedRows[index].Cells[1].Value.ToString()));

                m_GatherControl.Remove(t);

                Int64 TaskID = Int64.Parse(this.dataTask.SelectedRows[index].Cells[1].Value.ToString());

                t = null;

                //删除taskrun节点
                cTaskRun tr = new cTaskRun();
                tr.LoadTaskRunData();
                tr.DelTask(TaskID);
                tr = null;

                ////删除已经加载到采集任务控制器中的任务
                //m_GatherControl.TaskManage.TaskListControl.DelTask(t);

                //删除run中的任务实例文件
                string FileName = Program.getPrjPath() + "Tasks\\run\\" + "Task" + TaskID + ".xml";
                System.IO.File.Delete(FileName);
                
                tr = null;

            }

            return true;

            //删除Datagridview中选中的数据

            //while(this.dataTask.SelectedRows.Count>0)
            //{
            //    this.dataTask.Rows.Remove(this.dataTask.SelectedRows[0]);
            //}

          
        }

        //删除已经完成的任务
        private bool DelCompletedTask()
        {
            if (this.dataTask.SelectedRows.Count == 0)
                return false;
            if (this.dataTask.SelectedRows.Count == 1)
            {
                if (MessageBox.Show(rm.GetString ("Info29") + this.dataTask.SelectedCells[4].Value.ToString() + "\r\n" +
                    rm.GetString("Quaere10"), rm.GetString("MessageboxQuaere"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return false;
                }
            }
            else
            {
                if (MessageBox.Show(rm.GetString ("Quaere11"), 
                    rm.GetString("MessageboxQuaere"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return false;
                }
            }

            for (int index = 0; index < this.dataTask.SelectedRows.Count; index++)
            {
                Int64 TaskID = Int64.Parse(this.dataTask.SelectedRows [index].Cells[1].Value.ToString());
                string TaskName = this.dataTask.SelectedRows[index].Cells[4].Value.ToString();

                //删除taskcomplete节点
                cTaskComplete tc = new cTaskComplete();
                tc.LoadTaskData();
                tc.DelTask(TaskID);
                tc = null;

                //删除run中的任务实例文件
                string FileName = Program.getPrjPath() + "data\\" + TaskName + "-" + TaskID + ".xml";
                System.IO.File.Delete(FileName);
            }

            return true;

            //while (this.dataTask.SelectedRows.Count > 0)
            //{
            //    this.dataTask.Rows.Remove(this.dataTask.SelectedRows[0]);
            //}
           
        }

        #endregion

        #region 窗体控制

        private void frmMain_Resize(object sender, EventArgs e)
        {
            //根据窗体变化来调整控件布局
            //this.toolStrip2.Width = 300;
            //this.toolStrip2.Left = this.Width - this.toolStrip2.Width;
        }

         #endregion

    
        #region 事件处理

        public event EventHandler<cTaskEventArgs> Completed
        {
            add
            {
                m_GatherControl.Completed += value;
            }
            remove
            {
                m_GatherControl.Completed -= value;
            }
        }

        public event EventHandler<cTaskEventArgs> TaskCompleted
        {
            add
            {
                m_GatherControl.TaskManage.TaskCompleted += value;
            }
            remove
            {
                m_GatherControl.TaskManage.TaskCompleted -= value;
            }
        }

        private void tManage_Completed(object sender, cTaskEventArgs e)
        {
            //任务执行完毕后，需要将任务移至已经完成的节点中，
            //在此如果选择的是nodRunning则删除datagridview的内容
            //然后添加到完成队列中

            try
            {
                cGatherTask t = (cGatherTask)sender;
                InvokeMethod(this, "ShowInfo", new object[] { e.TaskName,  rm.GetString("TaskGCompleted")});

                //任务完成后，无论是否发布都调用此方法，因为要进行临时数据保存
                InvokeMethod(this, "UpdateTaskPublish", new object[] { e.TaskID, t.TaskData.IsDelRepRow });

                t = null;

                InvokeMethod(this, "UpdateStatebarTask", null);

            }
            catch (System.Exception ex)
            {
                InvokeMethod(this, "ExportLog", new object[] { ex.Message });
            }


        }

        //保存采集任务的临时数据
        public void SaveGatherTempData(Int64 TaskID)
        {

            //将此任务添加到发布队列中

            string conName = "sCon" + TaskID;
            string pageName = "page" + TaskID;

            if (this.tabControl1.TabPages[pageName] == null)
            {
                return;
            }

            DataTable d = (DataTable)((DataGridView)this.tabControl1.TabPages[pageName].Controls[conName].Controls[0].Controls[0]).DataSource;

            cPublishTask pt = new cPublishTask(m_PublishControl.PublishManage, TaskID, d);
            m_PublishControl.startSaveTempData (pt);

        }

        //处理任务采集完成的工作，注意任务无论是否发布都要执行
        //此方法，因为无论任务是否发布都需要进行临时数据保存
        //如果不发布，则不进行数据发布任务的启动。
        public void UpdateTaskPublish(Int64 TaskID,bool IsDelRepRow)
        {
            //将此任务添加到发布队列中

            string conName = "sCon" + TaskID;
            string pageName = "page" + TaskID;

            if (this.tabControl1.TabPages[pageName]== null)
            {
                //表示采集任务是没有定义采集规则的，
                //所以，会导致任务直接完成，并未建立
                //任务输出的tab页
                UpdateTaskComplete(TaskID);
            }
            else
            {
                //表示采集有数据
                DataTable d;

                if (IsDelRepRow == true)
                {
                    //去除重复行
                    DataTable d1 = (DataTable)((DataGridView)this.tabControl1.TabPages[pageName].Controls[conName].Controls[0].Controls[0]).DataSource;
                    string[] strComuns = new string[d1.Columns.Count];

                    for (int m = 0; m < d1.Columns.Count; m++)
                    {
                        strComuns[m] = d1.Columns[m].ColumnName;
                    }

                    DataView dv = new DataView(d1);

                    d = dv.ToTable(true, strComuns);
                }
                else
                {
                    d = (DataTable)((DataGridView)this.tabControl1.TabPages[pageName].Controls[conName].Controls[0].Controls[0]).DataSource;
                }

                cPublishTask pt = new cPublishTask(m_PublishControl.PublishManage, TaskID, d);
                m_PublishControl.startSaveTempData(pt);


                if (pt.PublishType == cGlobalParas.PublishType.NoPublish)
                {
                    //如果是不发布数据，则需要通过此方法完成任务采集后的处理工作
                    //删除taskrun中的数据等等，此部分如果是实现了任务数据发布，则
                    //由任务发布结束后触发事件完成，但在此需要手工完成
                    UpdateTaskComplete(TaskID);
                }
                else
                {
                    m_PublishControl.startPublish(pt);
                }
            }

            #region 更新界面显示
            if (this.treeMenu.SelectedNode.Name == "nodRunning")
            {
                for (int i = 0; i < this.dataTask.Rows.Count; i++)
                {
                    if (this.dataTask.Rows[i].Cells[1].Value.ToString() == TaskID.ToString())
                    {
                        this.dataTask.Rows.Remove(this.dataTask.Rows[i]);
                        break;
                    }
                }
            }
            else if (this.treeMenu.SelectedNode.Name == "nodPublish")
            {
                //重新加载已完成任务的信息
                LoadPublishTask();
            }
            else if (this.treeMenu.SelectedNode.Name == "nodComplete")
            {
                //重新加载已完成任务的信息
                LoadCompleteTask();
            }
#endregion

        }


        //处理任务采集完成后的工作，首先如果选择的正在运行的节点，则
        //删除此节点,然后从taskrun数据中删除,然后在删除实际的文件
        public void UpdateTaskComplete(Int64 TaskID)
        {
            //将已经完成的任务添加到完成任务的索引文件中
            Task.cTaskComplete t = new Task.cTaskComplete();
            t.InsertTaskComplete(TaskID, cGlobalParas.GatherResult.GatherSucceed );
            t = null;

            //删除taskrun节点
            cTaskRun tr = new cTaskRun();
            tr.LoadTaskRunData();
            tr.DelTask(TaskID);

            //删除run中的任务实例文件
            string FileName = Program.getPrjPath() + "Tasks\\run\\" + "Task" + TaskID + ".xml";
            System.IO.File.Delete(FileName);

        }

        public void UpdateRunTaskState(Int64 TaskID, cGlobalParas.TaskState tState)
        {

            if (this.treeMenu.SelectedNode.Name == "nodRunning")
            {
                for (int i = 0; i < this.dataTask.Rows.Count; i++)
                {
                    if (this.dataTask.Rows[i].Cells[1].Value.ToString() == TaskID.ToString())
                    {
                        switch (tState)
                        {
                            case cGlobalParas.TaskState.Running:
                                this.dataTask.Rows[i].Cells[0].Value = imageList1.Images["started"];
                                break;
                            case cGlobalParas.TaskState.Stopped:
                                if ((int.Parse(dataTask.Rows[i].Cells[7].Value.ToString()) + int.Parse(dataTask.Rows[i].Cells[8].Value.ToString())) > 0)
                                {
                                    this.dataTask.Rows[i].Cells[0].Value= imageList1.Images["pause"];
                                }
                                else
                                {
                                    this.dataTask.Rows[i].Cells[0].Value = imageList1.Images["stop"];
                                }
                                break;
                            default:
                                this.dataTask.Rows[i].Cells[0].Value = imageList1.Images["stop"];
                                break;
                        }
                        this.dataTask.Rows[i].Cells[2].Value = tState;
                        break;
                    }
                }
               
            }

        }

        private void tManage_TaskStart(object sender, cTaskEventArgs e)
        {
            //如果任务启动，则修改任务的图标，此事件是由点击按钮后任务
            //启动进行触发

            try
            {
                InvokeMethod(this, "UpdateRunTaskState", new object[] { e.TaskID, cGlobalParas.TaskState.Running });

                SetValue(this.toolStrip1.Items["toolStartTask"], "Enabled", false);
                SetValue(this.toolStrip1.Items["toolRestartTask"], "Enabled", false);
                SetValue(this.toolStrip1.Items["toolStopTask"], "Enabled", false);

                UpdateStatebarTask();
            }
            catch (System.Exception ex)
            {
                InvokeMethod(this, "ExportLog", new object[] { ex.Message });
            }
        }

        private void tManage_TaskInitialized(object sender, TaskInitializedEventArgs e)
        {
            //暂不做任何处理

            try
            {
                UpdateStatebarTask();
            }
            catch (System.Exception ex)
            {
                InvokeMethod(this, "ExportLog", new object[] { ex.Message });
            }

        }

        private void tManage_TaskStateChanged(object sender, TaskStateChangedEventArgs e)
        {
            try
            {
                InvokeMethod(this, "SetTaskShowState", new object[] { e.TaskID, e.NewState });

                UpdateStatebarTask();
            }
            catch (System.Exception ex)
            {
                InvokeMethod(this, "ExportLog", new object[] { ex.Message });
            }
 
        }

        private void tManage_TaskStop(object sender, cTaskEventArgs e)
        {

           //如果任务启动，则修改任务的图标，此事件是由点击按钮后任务
            //启动进行触发

            try
            {

                InvokeMethod(this, "UpdateRunTaskState", new object[] { e.TaskID, cGlobalParas.TaskState.Stopped });

                SetValue(this.toolStrip1.Items["toolStartTask"], "Enabled", false);
                SetValue(this.toolStrip1.Items["toolRestartTask"], "Enabled", false);
                SetValue(this.toolStrip1.Items["toolStopTask"], "Enabled", false);


                //在此处处理任务由于用户中断，需要进行的必要保存工作
                //任务中断后，系统需保存已经采集完成的数据，保存已经采集的网址记录，
                //确保下次运行任务时，可以直接进行，即类似下载的断点功能操作

                SaveGatherTempData(e.TaskID);


                UpdateStatebarTask();
            }
            catch (System.Exception ex)
            {
                InvokeMethod(this, "ExportLog", new object[] { ex.Message });
            }
            
           
        }

        //单个Url采集发生错误，不进行界面响应，记录日志即可，日志由其他事件记录完成
        //当错误达到一定的数量后，会由后台线程触发任务失败的事件，由任务失败事件完成
        //临时数据的存储
        private void tManage_TaskError(object sender, TaskErrorEventArgs e)
        {
            //cGatherTask t = (cGatherTask)sender;
            ////不需要通过窗口通知告诉用户，直接写入采集日子，注销下面代码
            ////InvokeMethod(this, "ShowInfo", new object[] {rm.GetString("TaskGError"), t.TaskName });
           

            //InvokeMethod(this, "UpdateStatebarTask", null);

            //InvokeMethod(this, "SaveGatherTempData", new object[] { t.TaskID });

            //t = null;
        }

        private void tManage_TaskFailed(object sender, cTaskEventArgs e)
        {
            try
            {
                InvokeMethod(this, "ShowInfo", new object[] { e.TaskName ,rm.GetString ( "TaskGFailed")});

                InvokeMethod(this, "SaveGatherTempData", new object[] { e.TaskID });

                InvokeMethod(this, "UpdateStatebarTask", null);
            }
            catch (System.Exception ex)
            {
                InvokeMethod(this, "ExportLog", new object[] { ex.Message });
            }
        }

        private void tManage_TaskAbort(object sender, cTaskEventArgs e)
        {

            //InvokeMethod(this, "SaveGatherTempData", new object[] { e.TaskID });

            //if (m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count > 0)
            //{
            //    IsExitting = true ;
            //}
            //else
            //{
            //    IsExitting = false;
            //}

        }

        private void m_Gather_Completed(object sender, EventArgs e)
        {
            //任务采集完成，则启动消息通知窗体，通知用户


        }

        //写日志事件
        private void tManage_Log(object sender, cGatherTaskLogArgs e)
        {
            //写日志
            Int64 TaskID = e.TaskID;
            string strLog = e.strLog;
            string conName="sCon" + TaskID ;
            string pageName="page" + TaskID ;

            try
            {
                SetValue(this.tabControl1.TabPages[pageName].Controls[conName].Controls[1].Controls[0], "Text", strLog);
            }
            catch (System.Exception ex)
            {
                InvokeMethod(this, "ExportLog", new object[] { ex.Message });
            }

        }

        //写数据事件
        private void tManage_GData(object sender, cGatherDataEventArgs e)
        {
            try
            {
                //写采集数据到界面Datagridview
                Int64 TaskID = e.TaskID;
                DataTable gData = e.gData;
                string conName = "sCon" + TaskID;
                string pageName = "page" + TaskID;

                SetValue(this.tabControl1.TabPages[pageName].Controls[conName].Controls[0].Controls[0], "gData", gData);
            }
            catch (System.Exception ex)
            {
                InvokeMethod(this, "ExportLog", new object[] { ex.Message });
            }
        }

        #endregion

        #region 委托代理 用于后台线程调用 配置UI线程的方法、属性

        delegate void bindvalue(object Instance, string Property, object value);
        delegate object invokemethod(object Instance, string Method, object[] parameters);
        delegate object invokepmethod(object Instance, string Property, string Method, object[] parameters);
        delegate object invokechailmethod(object InstanceInvokeRequired, object Instance, string Method, object[] parameters);

        /// <summary>
        /// 委托设置对象属性
        /// </summary>
        /// <param name="Instance">对象</param>
        /// <param name="Property">属性名</param>
        /// <param name="value">属性值</param>
        private void SetValue(object Instance, string Property, object value)
        {
            Type iType = Instance.GetType();
            object inst;

            if (iType.Name.ToString() == "ToolStripButton")
            {
                inst = this.toolStrip1;
            }
            else
            {
                inst = Instance;
            }

            bool a = (bool)GetPropertyValue(inst, "InvokeRequired");
            
            if (a)
            {
                bindvalue d = new bindvalue(SetValue);
                this.Invoke(d, new object[] { Instance, Property, value });
            }
            else
            {
                SetPropertyValue(Instance, Property, value);
            }
        }
        /// <summary>
        /// 委托执行实例的方法，方法必须都是Public 否则会出错
        /// </summary>
        /// <param name="Instance">类实例</param>
        /// <param name="Method">方法名</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>返回值</returns>
        private object InvokeMethod(object Instance, string Method, object[] parameters)
        {
            if ((bool)GetPropertyValue(Instance, "InvokeRequired"))
            {
                invokemethod d = new invokemethod(InvokeMethod);
                return this.Invoke(d, new object[] { Instance, Method, parameters });
            }
            else
            {
                return MethodInvoke(Instance, Method, parameters);
            }
        }

        /// <summary>
        /// 委托执行实例的方法
        /// </summary>
        /// <param name="InstanceInvokeRequired">窗体控件对象</param>
        /// <param name="Instance">需要执行方法的对象</param>
        /// <param name="Method">方法名</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>返回值</returns>
        private object InvokeChailMethod(object InstanceInvokeRequired, object Instance, string Method, object[] parameters)
        {
            if ((bool)GetPropertyValue(InstanceInvokeRequired, "InvokeRequired"))
            {
                invokechailmethod d = new invokechailmethod(InvokeChailMethod);
                return this.Invoke(d, new object[] { InstanceInvokeRequired, Instance, Method, parameters });
            }
            else
            {
                return MethodInvoke(Instance, Method, parameters);
            }
        }
        /// <summary>
        /// 委托执行实例的属性的方法
        /// </summary>
        /// <param name="Instance">类实例</param>
        /// <param name="Property">属性名</param>
        /// <param name="Method">方法名</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>返回值</returns>
        private object InvokePMethod(object Instance, string Property, string Method, object[] parameters)
        {
            if ((bool)GetPropertyValue(Instance, "InvokeRequired"))
            {
                invokepmethod d = new invokepmethod(InvokePMethod);
                return this.Invoke(d, new object[] { Instance, Property, Method, parameters });
            }
            else
            {
                return MethodInvoke(GetPropertyValue(Instance, Property), Method, parameters);
            }
        }
        /// <summary>
        /// 获取实例的属性值
        /// </summary>
        /// <param name="ClassInstance">类实例</param>
        /// <param name="PropertyName">属性名</param>
        /// <returns>属性值</returns>
        private static object GetPropertyValue(object ClassInstance, string PropertyName)
        {
            Type myType = ClassInstance.GetType();
            PropertyInfo myPropertyInfo = myType.GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return myPropertyInfo.GetValue(ClassInstance, null);
        }
        /// <summary>
        /// 设置实例的属性值
        /// </summary>
        /// <param name="ClassInstance">类实例</param>
        /// <param name="PropertyName">属性名</param>
        private static void SetPropertyValue(object ClassInstance, string PropertyName, object PropertyValue)
        {
            Type myType = ClassInstance.GetType();
            PropertyInfo myPropertyInfo = myType.GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            myPropertyInfo.SetValue(ClassInstance, PropertyValue, null);
        }

        /// <summary>
        /// 执行实例的方法
        /// </summary>
        /// <param name="ClassInstance">类实例</param>
        /// <param name="MethodName">方法名</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>返回值</returns>
        private static object MethodInvoke(object ClassInstance, string MethodName, object[] parameters)
        {
            if (parameters == null)
            {
                parameters = new object[0];
            }
            Type myType = ClassInstance.GetType();
            Type[] types = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; ++i)
            {
                types[i] = parameters[i].GetType();
            }
            MethodInfo myMethodInfo = myType.GetMethod(MethodName, types);
            return myMethodInfo.Invoke(ClassInstance, parameters);
        }

        #endregion

        #region 任务管理 启动 停止 
        //启动采集任务
        private cGatherTask AddRunTask(string tClassName, string tName)
        {

            //将选择的任务添加到运行区
            //首先判断此任务是否已经添加到运行区,
            //如果已经添加到运行区则需要询问是否再起一个运行实例
            bool IsExist = false;

            //开始初始化正在运行的任务
            Task.cTaskRun xmlTasks = new Task.cTaskRun();
            xmlTasks.LoadTaskRunData();
            for (int i=0 ;i<xmlTasks.GetCount() ;i++)
            {
                if (xmlTasks.GetTaskName(i) == tName)
                {
                    IsExist = true;
                    break;
                }
            }
            xmlTasks = null;

            if (IsExist == true)
            {
                //if (MessageBox.Show("您选择启动的任务已经在运行区存在或者有相同名称的任务已经在运行区，您是否确认此任务需要运行或需要此任务运行第二个实例？",
                //    "系统询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                //{
                //    return null;
                //}

                if (cTool.MyMessageBox(rm.GetString("Quaere12"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return null;
                }

            }

            cTaskRun tr = new cTaskRun();
            cTaskClass tc = new cTaskClass();
            cTaskData tData=new cTaskData ();
            
            string tPath="";

            if (tClassName == "")
            {
                tPath = Program.getPrjPath() + "tasks";
            }
            else
            {
                tPath = tc.GetTaskClassPathByName(tClassName);
            }

            tc=null;

            string tFileName = tName + ".xml";

            //获取最大的执行ID
            Int64 NewID = tr.InsertTaskRun(tPath, tFileName);

            tr.LoadSingleTask(NewID);

            tData = new cTaskData();
            tData.TaskID = tr.GetTaskID(0);
            tData.TaskName = tr.GetTaskName(0);
            tData.TaskType = tr.GetTaskType(0);
            tData.RunType = tr.GetTaskRunType(0);
            tData.tempFileName = tr.GetTempFile(0);
            tData.TaskState = tr.GetTaskState(0);
            tData.UrlCount = tr.GetUrlCount(0);
            tData.TrueUrlCount = tr.GetTrueUrlCount(0);
            tData.ThreadCount = tr.GetThreadCount(0);
            tData.GatheredUrlCount = tr.GetGatheredUrlCount(0);
            tData.GatherErrUrlCount = tr.GetErrUrlCount(0);

            //添加任务到运行区
            m_GatherControl.AddGatherTask(tData);

            tData = null;

            //任务添加到运行区后,需要再添加到任务执行列表中
            tr = null;

            return  m_GatherControl.TaskManage.FindTask(NewID);

        }

        private void StopMultiTask()
        {
            if (this.dataTask.SelectedRows.Count == 0)
                return;

            for (int index = 0; index < this.dataTask.SelectedRows.Count; index++)
            {
                if ((cGlobalParas.TaskState)this.dataTask.SelectedRows[index].Cells[2].Value == cGlobalParas.TaskState.Failed)
                {
                    continue;
                }
                else
                {
                    StopTask(Int64.Parse(this.dataTask.SelectedRows[index].Cells[1].Value.ToString()));
                }
            }
        }

        private void StopTask(Int64 TaskID)
        {
            cGatherTask t = null;

            //判断当前选择的树节点
            if (this.treeMenu.SelectedNode.Name == "nodRunning" )
            {
                //执行正在执行的任务
                t = m_GatherControl.TaskManage.FindTask(TaskID);

                //停止此任务
                m_GatherControl.Stop(t);

                //任务启动成功显示消息
                ShowInfo(t.TaskName,rm.GetString ( "TaskStoped"));

            }
        }

        #endregion

        #region 托盘的处理
        private void notifyIcon1_MouseMove(object sender, MouseEventArgs e)
        {
            //显示当前任务队列内容
            string s="";

            try
            {
                s = rm.GetString ("TrayTitle").ToString () + "\n";
                s +=rm.GetString ("TrayInfo1").ToString () + ": " + m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count + "\n";
                s +=rm.GetString ("TrayInfo2").ToString () + ": " + m_PublishControl.PublishManage.ListPublish.Count;

                if (s.Length > 64)
                {
                    s = rm.GetString("TrayInfo1").ToString() + ": " + m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count + "\n";
                    s += rm.GetString("TrayInfo2").ToString() + ": " + m_PublishControl.PublishManage.ListPublish.Count;

                }
            }
            catch (System.Exception)
            {
                //捕获错误但不处理，让其继续显示信息
            }

          
            this.notifyIcon1.Text = s;
            

        }
        #endregion

        #region 设置datagridview的列表头

        //设置显示任务数据Datalistview的列表头
        private void ShowRunTask()
        {
            try
            {
                this.dataTask.Columns.Clear();
                this.dataTask.Rows.Clear();
            }
            catch (System.Exception)
            {
            }


            #region 此部分为固定显示 任务类型的任务都必须固定显示此列
            DataGridViewImageColumn tStateImg = new DataGridViewImageColumn();
            tStateImg.HeaderText = rm.GetString("GridState");
            tStateImg.Width = 40;
            tStateImg.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            tStateImg.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(0, tStateImg);

            //任务编号,不显示此列
            DataGridViewTextBoxColumn tID = new DataGridViewTextBoxColumn();
            tID.Name = rm.GetString("GridTaskID");
            tID.Width = 0;
            tID.Visible = false;
            this.dataTask.Columns.Insert(1, tID);

            //任务状态,不显示此列
            DataGridViewTextBoxColumn tState = new DataGridViewTextBoxColumn();
            tState.Name = rm.GetString("GridState");
            tState.Width = 0;
            tState.Visible = false;
            this.dataTask.Columns.Insert(2, tState);

            //用于通过判断Datagridview的数据就可知道当前树形结构选择的节点
            //用于控制(更新)界面显示状态
            DataGridViewTextBoxColumn tTreeNode = new DataGridViewTextBoxColumn();
            tTreeNode.HeaderText = "treeMenuName";
            tTreeNode.Visible = false;
            this.dataTask.Columns.Insert(3, tTreeNode);

            #endregion

            DataGridViewTextBoxColumn tName = new DataGridViewTextBoxColumn();
            tName.HeaderText = rm.GetString("GridTaskName");
            tName.Width = 150;
            this.dataTask.Columns.Insert(4, tName);

            DataGridViewTextBoxColumn tType = new DataGridViewTextBoxColumn();
            tType.HeaderText = rm.GetString("GridTaskType");
            tType.Width = 80;
            this.dataTask.Columns.Insert(5, tType);

            DataGridViewTextBoxColumn Islogin = new DataGridViewTextBoxColumn();
            Islogin.HeaderText = rm.GetString("GridIsLogon");
            Islogin.Width = 80;
            this.dataTask.Columns.Insert(6, Islogin);

            DataGridViewTextBoxColumn GatheredUrlCount = new DataGridViewTextBoxColumn();
            GatheredUrlCount.HeaderText = rm.GetString("GridCompleteCount");
            GatheredUrlCount.Width = 50;
            GatheredUrlCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(7, GatheredUrlCount);

            DataGridViewTextBoxColumn GatheredErrUrlCount = new DataGridViewTextBoxColumn();
            GatheredErrUrlCount.HeaderText = rm.GetString("GridErrorCount");
            GatheredErrUrlCount.Width = 50;
            GatheredErrUrlCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(8, GatheredErrUrlCount);

            DataGridViewTextBoxColumn tUrlCount = new DataGridViewTextBoxColumn();
            tUrlCount.HeaderText = rm.GetString("GridUrlCount");
            tUrlCount.Width = 50;
            tUrlCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(9, tUrlCount);

             
            DataGridViewProgressBarColumn tPro = new DataGridViewProgressBarColumn();
            tPro.HeaderText = rm.GetString("GridProcess");
            tPro.Width = 120;
            this.dataTask.Columns.Insert(10, tPro);

            DataGridViewTextBoxColumn tRunType = new DataGridViewTextBoxColumn();
            tRunType.HeaderText = rm.GetString("GridTaskRunType");
            tRunType.Width = 120;
            this.dataTask.Columns.Insert(11, tRunType);

            DataGridViewTextBoxColumn tExportFile = new DataGridViewTextBoxColumn();
            tExportFile.HeaderText = rm.GetString("GridExportType");
            tExportFile.Width = 1900;
            this.dataTask.Columns.Insert(12, tExportFile);

        }

        private void ShowPublishTask()
        {
            try
            {
                this.dataTask.Columns.Clear();
                this.dataTask.Rows.Clear();
            }
            catch (System.Exception )
            {
            }
          

            #region 此部分为固定显示 任务类型的任务都必须固定显示此列
            DataGridViewImageColumn tStateImg = new DataGridViewImageColumn();
            tStateImg.HeaderText = rm.GetString("GridState");
            tStateImg.Width = 40;
            tStateImg.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            tStateImg.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(0, tStateImg);

            //任务编号,不显示此列
            DataGridViewTextBoxColumn tID = new DataGridViewTextBoxColumn();
            tID.Name = rm.GetString("GridTaskID");
            tID.Width = 0;
            tID.Visible = false;
            this.dataTask.Columns.Insert(1, tID);

            //任务状态,不显示此列
            DataGridViewTextBoxColumn tState = new DataGridViewTextBoxColumn();
            tState.Name = rm.GetString("GridState");
            tState.Width = 0;
            tState.Visible = false;
            this.dataTask.Columns.Insert(2, tState);

            //用于通过判断Datagridview的数据就可知道当前树形结构选择的节点
            //用于控制(更新)界面显示状态
            DataGridViewTextBoxColumn tTreeNode = new DataGridViewTextBoxColumn();
            tTreeNode.HeaderText = "treeMenuName";
            tTreeNode.Visible = false;
            this.dataTask.Columns.Insert(3, tTreeNode);

            #endregion

            DataGridViewTextBoxColumn tName = new DataGridViewTextBoxColumn();
            tName.HeaderText = rm.GetString("GridTaskName");
            tName.Width = 150;
            this.dataTask.Columns.Insert(4, tName);

            DataGridViewTextBoxColumn PublishedCount = new DataGridViewTextBoxColumn();
            PublishedCount.HeaderText = rm.GetString("GridExportedCount");
            PublishedCount.Width = 50;
            PublishedCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(5, PublishedCount);

            DataGridViewTextBoxColumn Count = new DataGridViewTextBoxColumn();
            Count.HeaderText = rm.GetString("GridUrlCount");
            Count.Width = 50;
            Count.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(6, Count);


            DataGridViewProgressBarColumn tPro = new DataGridViewProgressBarColumn();
            tPro.HeaderText = rm.GetString("GridProcess");
            tPro.Width = 120;
            this.dataTask.Columns.Insert(7, tPro);


            DataGridViewTextBoxColumn PublishType = new DataGridViewTextBoxColumn();
            PublishType.HeaderText = rm.GetString("GridExportType");
            PublishType.Width = 1900;
            this.dataTask.Columns.Insert(8, PublishType);
        }

        private void ShowCompletedTask()
        {
            try
            {
                this.dataTask.Columns.Clear();
                this.dataTask.Rows.Clear();
            }
            catch (System.Exception)
            {
            }

            #region 此部分为固定显示
            DataGridViewImageColumn tStateImg = new DataGridViewImageColumn();
            tStateImg.HeaderText = rm.GetString("GridState");
            tStateImg.Width = 40;
            tStateImg.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            tStateImg.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(0, tStateImg);

            //任务编号,不显示此列
            DataGridViewTextBoxColumn tID = new DataGridViewTextBoxColumn();
            tID.Name = rm.GetString("GridTaskID");
            tID.Width = 0;
            tID.Visible = false;
            this.dataTask.Columns.Insert(1, tID);

            //任务状态,不显示此列
            DataGridViewTextBoxColumn tState= new DataGridViewTextBoxColumn();
            tState.Name = rm.GetString("GridState");
            tState.Width = 0;
            tState.Visible = false;
            this.dataTask.Columns.Insert(2, tState);

            //用于通过判断Datagridview的数据就可知道当前树形结构选择的节点
            //用于控制(更新)界面显示状态
            DataGridViewTextBoxColumn tTreeNode = new DataGridViewTextBoxColumn();
            tTreeNode.HeaderText = "treeMenuName";
            tTreeNode.Visible = false;
            this.dataTask.Columns.Insert(3, tTreeNode);
            #endregion

            DataGridViewTextBoxColumn tName = new DataGridViewTextBoxColumn();
            tName.HeaderText = rm.GetString("GridTaskName");
            tName.Width = 150;
            this.dataTask.Columns.Insert(4, tName);

            DataGridViewTextBoxColumn tType = new DataGridViewTextBoxColumn();
            tType.HeaderText = rm.GetString("GridTaskType");
            tType.Width = 180;
            this.dataTask.Columns.Insert(5, tType);

            //DataGridViewTextBoxColumn gatherUrlCount = new DataGridViewTextBoxColumn();
            //gatherUrlCount.HeaderText = "成功采集";
            //gatherUrlCount.Width = 80;
            //gatherUrlCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //this.dataTask.Columns.Insert(6, gatherUrlCount);

            //DataGridViewTextBoxColumn errUrlCount = new DataGridViewTextBoxColumn();
            //errUrlCount.HeaderText = "失败数量";
            //errUrlCount.Width = 80;
            //errUrlCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //this.dataTask.Columns.Insert(7, errUrlCount);

            DataGridViewTextBoxColumn tUrlCount = new DataGridViewTextBoxColumn();
            tUrlCount.HeaderText = rm.GetString("GridUrlCount");
            tUrlCount.Width = 80;
            tUrlCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(6, tUrlCount);

            DataGridViewTextBoxColumn tPro = new DataGridViewTextBoxColumn();
            tPro.HeaderText = rm.GetString("GridTaskRunType");
            tPro.Width = 120;
            this.dataTask.Columns.Insert(7, tPro);

            DataGridViewTextBoxColumn tExportFile = new DataGridViewTextBoxColumn();
            tExportFile.HeaderText = rm.GetString("GridExportType");
            tExportFile.Width = 1900;
            this.dataTask.Columns.Insert(8, tExportFile);

        }

        private void ShowTaskInfo()
        {
            try
            {
                this.dataTask.Columns.Clear();
                this.dataTask.Rows.Clear();
            }
            catch (System.Exception)
            {
            }

            #region 比部分为固定显示
            DataGridViewImageColumn tStateImg = new DataGridViewImageColumn();
            tStateImg.HeaderText = rm.GetString("GridState");
            tStateImg.Width = 40;
            tStateImg.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            tStateImg.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(0, tStateImg);

            DataGridViewTextBoxColumn tID = new DataGridViewTextBoxColumn();
            tID.Name = rm.GetString("GridTaskID");
            tID.Width = 0;
            tID.Visible = false;
            this.dataTask.Columns.Insert(1, tID);

            //任务状态,不显示此列
            DataGridViewTextBoxColumn tState = new DataGridViewTextBoxColumn();
            tState.Name = rm.GetString("GridState");
            tState.Width = 0;
            tState.Visible = false;
            this.dataTask.Columns.Insert(2, tState);

            //用于通过判断Datagridview的数据就可知道当前树形结构选择的节点
            //用于控制(更新)界面显示状态
            DataGridViewTextBoxColumn tTreeNode = new DataGridViewTextBoxColumn();
            tTreeNode.HeaderText = "treeMenuName";
            tTreeNode.Visible = false;
            this.dataTask.Columns.Insert(3, tTreeNode);

            #endregion

            DataGridViewTextBoxColumn tName = new DataGridViewTextBoxColumn();
            tName.HeaderText = rm.GetString("GridTaskName");
            tName.Width = 150;
            this.dataTask.Columns.Insert(4, tName);

            DataGridViewTextBoxColumn tType = new DataGridViewTextBoxColumn();
            tType.HeaderText = rm.GetString("GridTaskType");
            tType.Width = 80;
            this.dataTask.Columns.Insert(5, tType);

            DataGridViewTextBoxColumn tLogin = new DataGridViewTextBoxColumn();
            tLogin.HeaderText = rm.GetString("GridIsLogon");
            tLogin.Width = 80;
            this.dataTask.Columns.Insert(6, tLogin);

            DataGridViewTextBoxColumn tUrlCount = new DataGridViewTextBoxColumn();
            tUrlCount.HeaderText = rm.GetString("GridUrlCount");
            tUrlCount.Width = 80;
            tUrlCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
       
            this.dataTask.Columns.Insert(7, tUrlCount);

            DataGridViewTextBoxColumn tRunType = new DataGridViewTextBoxColumn();
            tRunType.HeaderText = rm.GetString("GridTaskRunType");
            tRunType.Width = 120;
            this.dataTask.Columns.Insert(8, tRunType);

            DataGridViewTextBoxColumn tExportFile = new DataGridViewTextBoxColumn();
            tExportFile.HeaderText = rm.GetString("GridExportType");
            tExportFile.Width = 1900;

            this.dataTask.Columns.Insert(9, tExportFile);

            
        }

        private void ShowTaskPlan()
        {
            try
            {
                this.dataTask.Columns.Clear();
                this.dataTask.Rows.Clear();
            }
            catch (System.Exception)
            {
            }

            #region 比部分为固定显示
            DataGridViewImageColumn tStateImg = new DataGridViewImageColumn();
            tStateImg.HeaderText = rm.GetString("GridState");
            tStateImg.Width = 40;
            tStateImg.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            tStateImg.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(0, tStateImg);

            DataGridViewTextBoxColumn tID = new DataGridViewTextBoxColumn();
            tID.Name = rm.GetString("GridID");
            tID.Width = 0;
            tID.Visible = false;
            this.dataTask.Columns.Insert(1, tID);

            //任务状态,不显示此列
            DataGridViewTextBoxColumn tState = new DataGridViewTextBoxColumn();
            tState.Name = rm.GetString("GridState");
            tState.Width = 0;
            tState.Visible = false;
            this.dataTask.Columns.Insert(2, tState);

            //用于通过判断Datagridview的数据就可知道当前树形结构选择的节点
            //用于控制(更新)界面显示状态
            DataGridViewTextBoxColumn tTreeNode = new DataGridViewTextBoxColumn();
            tTreeNode.HeaderText = "treeMenuName";
            tTreeNode.Visible = false;
            this.dataTask.Columns.Insert(3, tTreeNode);

            #endregion

            DataGridViewTextBoxColumn tName = new DataGridViewTextBoxColumn();
            tName.HeaderText = rm.GetString("GridPlanName");
            tName.Width = 150;
            this.dataTask.Columns.Insert(4, tName);

            //任务状态,不显示此列
            DataGridViewTextBoxColumn tState1 = new DataGridViewTextBoxColumn();
            tState1.Name = rm.GetString("GridState");
            tState1.Width = 80;
            this.dataTask.Columns.Insert(5, tState1);

            DataGridViewTextBoxColumn tIsDisabled = new DataGridViewTextBoxColumn();
            tIsDisabled.HeaderText = rm.GetString("GridIsOvertime");
            tIsDisabled.Width = 120;
            this.dataTask.Columns.Insert(6, tIsDisabled);

            DataGridViewTextBoxColumn tDisabledRule = new DataGridViewTextBoxColumn();
            tDisabledRule.HeaderText = rm.GetString("GridOvertimeRule");
            tDisabledRule.Width = 180;
            this.dataTask.Columns.Insert(7, tDisabledRule);

            DataGridViewTextBoxColumn tPlanning = new DataGridViewTextBoxColumn();
            tPlanning.HeaderText = rm.GetString("GridRunPlan");
            tPlanning.Width = 180;
            this.dataTask.Columns.Insert(8, tPlanning);

            DataGridViewTextBoxColumn RunDate = new DataGridViewTextBoxColumn();
            RunDate.HeaderText = rm.GetString("GridNextTime");
            RunDate.Width = 160;
            RunDate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(9, RunDate);


            DataGridViewTextBoxColumn tRemark = new DataGridViewTextBoxColumn();
            tRemark.HeaderText = rm.GetString("GridRemark");
            tRemark.Width = 380;
            tRemark.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(10, tRemark);
    
        }

        //显示计划日志
        private void ShowPlanLog()
        {
            try
            {
                this.dataTask.Columns.Clear();
                this.dataTask.Rows.Clear();
            }
            catch (System.Exception)
            {
            }

            #region 比部分为固定显示 实际在显示日志时此部分可以省略，但为了保证系统操作的统一性，还是加上了，默认值参看下面注释
            DataGridViewImageColumn tStateImg = new DataGridViewImageColumn();
            tStateImg.HeaderText = rm.GetString("GridState");
            tStateImg.Width = 40;
            tStateImg.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            tStateImg.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(0, tStateImg);

            //日志编号，永远为1
            DataGridViewTextBoxColumn tID = new DataGridViewTextBoxColumn();
            tID.Name = rm.GetString("GridID");
            tID.Width = 0;
            tID.Visible = false;
            this.dataTask.Columns.Insert(1, tID);

            //显示日志类型
            DataGridViewTextBoxColumn tState = new DataGridViewTextBoxColumn();
            tState.Name = rm.GetString("GridState");
            tState.Width = 0;
            tState.Visible = false;
            this.dataTask.Columns.Insert(2, tState);

            //用于通过判断Datagridview的数据就可知道当前树形结构选择的节点
            //用于控制(更新)界面显示状态
            DataGridViewTextBoxColumn tTreeNode = new DataGridViewTextBoxColumn();
            tTreeNode.HeaderText = "treeMenuName";
            tTreeNode.Visible = false;
            this.dataTask.Columns.Insert(3, tTreeNode);

            #endregion
            
            DataGridViewTextBoxColumn pName = new DataGridViewTextBoxColumn();
            pName.Name = rm.GetString("GridPlanName");
            pName.Width = 220;
            this.dataTask.Columns.Insert(4, pName);

            DataGridViewTextBoxColumn pType = new DataGridViewTextBoxColumn();
            pType.HeaderText = rm.GetString("GridTaskType");
            pType.Width = 150;
            this.dataTask.Columns.Insert(5, pType);

            DataGridViewTextBoxColumn tName = new DataGridViewTextBoxColumn();
            tName.Name = rm.GetString("GridTaskName");
            tName.Width = 120;
            this.dataTask.Columns.Insert(6, tName);


            DataGridViewTextBoxColumn tPara = new DataGridViewTextBoxColumn();
            tPara.HeaderText = rm.GetString("GridPara");
            tPara.Width = 120;
            this.dataTask.Columns.Insert(7, tPara);

            DataGridViewTextBoxColumn pRunTime = new DataGridViewTextBoxColumn();
            pRunTime.HeaderText = rm.GetString("GridRunPlan");
            pRunTime.Width = 580;
            this.dataTask.Columns.Insert(8, pRunTime);
        }

        #endregion

        #region 界面操作控制

        private void dataTask_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            SetToolbarState();
           
        }

        ///根据菜单选择及点击的数据内容，自动控制工具栏的按钮状态
        ///因为DataGridView支持多选，所以可能存在多种按钮状态的情况
        ///针对这种情况，系统按照最后选择的内容进行按钮状态设置

        private void SetToolbarState()
        {
            if (this.dataTask.SelectedRows.Count == 0)
            {
                ResetToolState();
                return;
            }

            cGlobalParas.TaskState tState = (cGlobalParas.TaskState)this.dataTask.SelectedCells[2].Value;

            try
            {
                switch (this.treeMenu.SelectedNode.Name)
                {
                    case "nodRunning":

                        switch (tState)
                        {
                            case cGlobalParas.TaskState.Started:

                                this.toolStartTask.Enabled = false;
                                this.toolStopTask.Enabled = true;
                                this.toolRestartTask.Enabled = false;
                                break;

                            case cGlobalParas.TaskState.Running:

                                this.toolStartTask.Enabled = false;
                                this.toolStopTask.Enabled = true;
                                this.toolRestartTask.Enabled = false;
                                break;

                            case cGlobalParas.TaskState.Failed:

                                this.toolStartTask.Enabled = false;
                                this.toolStopTask.Enabled = false;
                                this.toolRestartTask.Enabled = false;
                                break;

                            default:

                                this.toolStartTask.Enabled = true;
                                this.toolStopTask.Enabled = false;
                                this.toolRestartTask.Enabled = true;
                                break;

                        }

                        UpdateStatebarTaskState(tState);


                        //只要有内容就可以删除
                        this.toolDelTask.Enabled = true;

                        if (int.Parse(this.dataTask.SelectedCells[9].Value.ToString()) > 0)
                            this.toolBrowserData.Enabled = true;
                        else
                            this.toolBrowserData.Enabled = false;

                        //t = null;

                        this.toolCopyTask.Enabled = false;
                        this.toolPasteTask.Enabled = false;

                        break;
                    case "nodPublish":

                        this.toolStartTask.Enabled = false;
                        this.toolRestartTask.Enabled = true;
                        this.toolStopTask.Enabled = false;
                        this.toolDelTask.Enabled = false;
                        this.toolBrowserData.Enabled = false;

                        this.toolCopyTask.Enabled = false;
                        this.toolPasteTask.Enabled = false;


                        UpdateStatebarTaskState(cGlobalParas.TaskState.Publishing);
                        break;
                    case "nodComplete":
                        this.toolStartTask.Enabled = false;
                        this.toolRestartTask.Enabled = false;
                        this.toolStopTask.Enabled = false;
                        this.toolDelTask.Enabled = true;
                        this.toolBrowserData.Enabled = true;

                        this.toolCopyTask.Enabled = false;
                        this.toolPasteTask.Enabled = false;

                        UpdateStatebarTaskState(cGlobalParas.TaskState.Completed);

                        break;

                    case "nodTaskClass":

                        //只要有内容就可以删除
                        this.toolDelTask.Enabled = true;

                        if (tState == cGlobalParas.TaskState.Failed)
                            this.toolStartTask.Enabled = false;
                        else
                            this.toolStartTask.Enabled = true;

                        this.toolRestartTask.Enabled = false;
                        this.toolStopTask.Enabled = false;

                        this.toolCopyTask.Enabled = false;
                        this.toolPasteTask.Enabled = false;

                        break;

                    default:

                        //只要有内容就可以删除
                        this.toolDelTask.Enabled = true;

                        if (this.treeMenu.SelectedNode.Name.Substring(0, 1) == "C")
                        {
                            if (tState == cGlobalParas.TaskState.Failed)
                                this.toolStartTask.Enabled = false;
                            else
                                this.toolStartTask.Enabled = true;

                            this.toolRestartTask.Enabled = false;
                            this.toolStopTask.Enabled = false;

                            this.toolCopyTask.Enabled = true ;

                            if (!IsClipboardSoukeyData())
                            {
                                this.toolPasteTask.Enabled = false ;
                            }
                            else
                            {
                                this.toolPasteTask.Enabled = true;
                            }
                            

                        }
                        else
                        {
                            this.toolCopyTask.Enabled = false;
                            this.toolPasteTask.Enabled = false;

                        }

                        break;
                }
            }
            catch (System.Exception  )
            {

            }
        }

        private void ResetToolState()
        {
            //如果grid为空，则置按钮为原始状态
            this.toolStartTask.Enabled = false;
            this.toolStopTask.Enabled = false;
            this.toolRestartTask.Enabled = false;
            this.toolBrowserData.Enabled = false;
            this.toolExportData.Enabled = false;
            this.toolDelTask.Enabled = false;
            this.toolCopyTask.Enabled = false;

            if (this.treeMenu.SelectedNode.Name.Substring(0, 1) == "C" || this.treeMenu.SelectedNode.Name == "nodTaskClass")
            {
                if (!IsClipboardSoukeyData())
                {
                    this.toolPasteTask.Enabled = false;
                }
                else
                {
                    this.toolPasteTask.Enabled = true;
                }
            }
            else
            {
                this.toolPasteTask.Enabled = false;
            }
        }

        private void dataTask_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    switch (this.treeMenu.SelectedNode.Name)
                    {
                        case "nodSnap":
                            break;
                        case "nodRunning":
                            if (DelRunTask() == false)
                            {
                                e.SuppressKeyPress = true;
                                return;
                            }
                            break;
                        case "nodPublish":
                            break;
                        case "nodComplete":
                            if (DelCompletedTask() == false)
                            {
                                e.SuppressKeyPress = true;
                                return;
                            }
                            break;
                        case "nodTaskPlan":
                            break;
                        case "nodPlanRunning":
                            if (DelPlan() == false)
                            {
                                e.SuppressKeyPress = true;
                                return;
                            }
                            break;
                        case "nodPlanCompleted":
                            if (DelPlanLog() == false)
                            {
                                e.SuppressKeyPress = true;
                                return;
                            }
                            
                            break;
                        case "nodTaskClass":
                            if (DelTask() == false)
                            {
                                e.SuppressKeyPress = true;
                                return;
                            }
                            break;
                        default:
                            if (DelTask() == false)
                            {
                                e.SuppressKeyPress = true;
                                return;
                            }
                            break;
                    }
                    break;
                case Keys.F2 :
                    if (this.treeMenu.SelectedNode.Name.Substring(0, 1) == "C" || this.treeMenu.SelectedNode.Name == "nodTaskClass"
                        || this.treeMenu.SelectedNode.Name=="nodPlanRunning")
                    {
                        this.dataTask.CurrentCell = this.dataTask[4, this.dataTask.CurrentCell.RowIndex];

                        this.dataTask.BeginEdit(true);
                    }

                    break;
                default :
                    if (this.treeMenu.SelectedNode.Name.Substring(0, 1) == "C" || this.treeMenu.SelectedNode.Name == "nodTaskClass")
                    {
                        if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.C )
                        {
                            CopyTask();
                        }
                        else if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.V )
                        {
                            PasteTask();
                        }
                    }
                    return;

            }
        }

        private void SetDataShow()
        {
            splitContainer2.SplitterDistance = 150;
        }

        private void SetInfoShow()
        {
            splitContainer3.SplitterDistance = 200;
        }

        private void SetInfoHide()
        {
            splitContainer3.SplitterDistance = 0;
        }

        public void SetTaskShowState(Int64 TaskID, cGlobalParas.TaskState tState)
        {
           

            //查找当前的列表显示的任务
            //首先判断当前选中的树形节点是否是运行区的节点
            if (this.dataTask.Rows.Count > 0 && this.dataTask.Rows[0].Cells[3].Value.ToString() == "nodRunning")
            {
                for (int i = 0; i < this.dataTask.Rows.Count; i++)
                {
                    bool IsSetToolbutState = false;

                    if (this.dataTask.Rows[i].Cells[1].Value.ToString() == TaskID.ToString())
                    {
                        if (i == this.dataTask.CurrentRow.Index)
                        {
                            IsSetToolbutState = true;
                        }

                        switch (tState)
                        {
                            case cGlobalParas.TaskState.Started:

                                break;
                            case cGlobalParas.TaskState.Failed:
                                this.dataTask.Rows[i].Cells[0].Value = imageList1.Images["error"];
                                if (IsSetToolbutState == true)
                                {
                                    SetValue(this.toolStrip1.Items["toolStartTask"], "Enabled", true);
                                    SetValue(this.toolStrip1.Items["toolRestartTask"], "Enabled", false);
                                    SetValue(this.toolStrip1.Items["toolStopTask"], "Enabled", false);

                                }

                                break;
                            case cGlobalParas.TaskState.Completed:
                                this.dataTask.Rows[i].Cells[0].Value = imageList1.Images["stop"];
                                if (IsSetToolbutState == true)
                                {
                                    SetValue(this.toolStrip1.Items["toolStartTask"], "Enabled", true);
                                    SetValue(this.toolStrip1.Items["toolRestartTask"], "Enabled", false);
                                    SetValue(this.toolStrip1.Items["toolStopTask"], "Enabled", false);
                                }
                                
                                break;
                            case cGlobalParas.TaskState.UnStart:

                                break;
                            case cGlobalParas.TaskState.Pause:

                                break;
                            default:

                                break;
                        }

                        break;
                    }
                }
            }

        }

        private void toolStopTask_Click(object sender, EventArgs e)
        {
            StopMultiTask();
        }

        private void dataTask_DoubleClick(object sender, EventArgs e)
        {
            switch (this.treeMenu.SelectedNode.Name)
            {
                case "nodSnap":
                    break;
                case "nodRunning":
                    EditTask();
                    break;
                case "nodPublish":
                    break;
                case "nodComplete":
                    break;
                case "nodTaskPlan":
                    break;
                case "nodPlanRunning":
                    EditPlan();
                    break;
                case "nodPlanCompleted":
                    break ;
                case "nodTaskClass":
                    EditTask();
                    break;
                default :
                    EditTask();
                    break;
            }
            
        }

        private void EditPlan()
        {
            if (this.dataTask.SelectedRows.Count == 0)
                return;

            try
            {
                Int64 pid=Int64.Parse (this.dataTask.SelectedCells[1].Value.ToString());

                frmTaskPlan fp = new frmTaskPlan();
                fp.LoadPlan(pid); 
                fp.FormState = cGlobalParas.FormState.Edit;
                fp.ShowDialog();
                fp.Dispose();

                LoadTaskPlan();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Info30") + ex.Message, rm.GetString ("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditTask()
        {

            if (this.dataTask.SelectedRows.Count == 0)
                return;

            if (this.treeMenu.SelectedNode.Name.Substring(0, 1) == "C" || this.treeMenu.SelectedNode.Name == "nodTaskClass")
            {
                //表示选择的是任务节点
                try
                {
                    string Filename = this.dataTask.SelectedCells[4].Value.ToString() + ".xml";
                    string tPath = this.treeMenu.SelectedNode.Tag.ToString();

                    LoadTaskInfo(tPath, Filename, cGlobalParas.FormState.Edit);
                }
                catch (System.IO.IOException)
                {
                    MessageBox.Show(rm.GetString("Info31"), rm.GetString ("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(rm.GetString ("Info32") + ex.Message + "\r\n" +
                        rm.GetString("Info33"), rm.GetString ("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
            }
            else if (this.treeMenu.SelectedNode.Name == "nodRunning")
            {
                if (MessageBox.Show(rm.GetString("Quaere13"), rm.GetString ("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }

                string Filename = "Task" + this.dataTask.SelectedCells[1].Value.ToString() + ".xml";
                string tPath = Program.getPrjPath() + "Tasks\\run\\";

                LoadTaskInfo(tPath, Filename, cGlobalParas.FormState.Browser);
                
            }
        }

        private void toolExportData_ButtonClick(object sender, EventArgs e)
        {
            ExportTxt();
        }

        private void toolExportTxt_Click(object sender, EventArgs e)
        {
            ExportTxt();
        }

        private void toolExportExcel_Click(object sender, EventArgs e)
        {
            ExportExcel();
        }

        private void toolAbout_Click(object sender, EventArgs e)
        {
            frmAbout f = new frmAbout();
            f.ShowDialog();
            f.Dispose();
        }

        private void toolmenuNewTask_Click(object sender, EventArgs e)
        {
            NewTask();
        }

        private void toolmenuNewTaskClass_Click(object sender, EventArgs e)
        {
            NewTaskClass();
        }

        private void dataTask_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataTask.Rows.Count != 0 && this.dataTask.SelectedCells.Count != 0 && this.dataTask.SelectedCells[1].Value.ToString()!="")
            {
                Int64 TaskID = Int64.Parse(this.dataTask.SelectedCells[1].Value.ToString());
                string pageName = "page" + TaskID;

                for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
                {
                    if (this.tabControl1.TabPages[i].Name == pageName)
                    {
                        this.tabControl1.SelectedIndex = i;
                        break;
                    }
                }

                //设置按钮状态
                SetToolbarState();
            }

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolDelTask_Click(object sender, EventArgs e)
        {
            //首先判断当前焦点的控件
            //然后再确定删除的是分类还是任务
            if (DelName == "treeMenu")
            {
                //删除的是分类
               DelTaskClass();
                
            }
            else if (DelName == "dataTask")
            {
                //删除的是任务，但需要判断删除的是何种任务
                this.dataTask.Focus();
                SendKeys.Send("{Del}");
            }

        }

        private void rMenuExportTxt_Click(object sender, EventArgs e)
        {
            //首先判断当前需要导出数据的Tab及datagridview
            ExportTxt();

        }

        private void rMenuExportExcel_Click(object sender, EventArgs e)
        {
            ExportExcel();
        }

        private void rMenuCloseTabPage_Click(object sender, EventArgs e)
        {
            string TaskID = this.tabControl1.SelectedTab.Name.Substring(4, this.tabControl1.SelectedTab.Name.Length - 4);
            for (int index=0; index < this.dataTask.Rows.Count; index++)
            {
                if (this.dataTask.Rows [index].Cells [1].Value.ToString () ==TaskID )
                {
                    if ((cGlobalParas.TaskState)this.dataTask.Rows[index].Cells[2].Value == cGlobalParas.TaskState.Running)
                    {
                        MessageBox.Show(rm.GetString("Info34"), rm.GetString ("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                        
                    }
                    break;
                }
            }
            this.tabControl1.TabPages.Remove(this.tabControl1.SelectedTab);
        }

        //private void rMenuCopy_Click(object sender, EventArgs e)
        //{
        //    DataGridView tmp = (DataGridView)this.tabControl1.SelectedTab.Controls[0].Controls[0].Controls[0];
        //    Clipboard.SetDataObject(tmp.GetClipboardContent());
        //    tmp = null;
        //}

        private void dataTask_MouseDown(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Right)
            //{

            //    if (this.dataTask.Rows.Count == 0)
            //    {
            //        return;
            //    }

            //    DataGridView.HitTestInfo hittest = this.dataTask.HitTest(e.X, e.Y);

            //    this.dataTask.ClearSelection();
            //    if (hittest.Type == DataGridViewHitTestType.Cell)  // && e.Button == MouseButtons.Right)
            //    {
            //        this.dataTask.Rows[hittest.RowIndex].Selected = true;
            //    }
            //    else
            //    {
            //        this.dataTask.Rows[this.dataTask.Rows.Count - 1].Selected = true;
            //    }

               
            //}

        }

        private void Tab_MouseDown(object sender, MouseEventArgs e)
        {
            //先取消选择
            DataGridView d = (DataGridView)sender;
            d.ClearSelection();

            if (d.Rows.Count == 0)
            {
                return;
            }

            DataGridView.HitTestInfo hittest = d.HitTest(e.X, e.Y);
            if (hittest.Type == DataGridViewHitTestType.Cell && e.Button == MouseButtons.Right)
            {
                d.Rows[hittest.RowIndex].Selected = true;
            }
            else
            {
                d.Rows[d.Rows.Count - 1].Selected = true;
            }
        }

        private void contextMenuStrip4_Opening(object sender, CancelEventArgs e)
        {
            if (this.tabControl1.SelectedIndex == 0)
            {
                e.Cancel = true;
                return;
            }

            DataGridView tmp = (DataGridView)this.tabControl1.SelectedTab.Controls[0].Controls[0].Controls[0];

            if (tmp.Rows.Count == 0)
            {
                this.rMenuExportTxt.Enabled = false;
                this.rMenuExportExcel.Enabled = false;
            }
            else
            {
                this.rMenuExportTxt.Enabled = true;
                this.rMenuExportExcel.Enabled = true;
            }

            tmp = null;
        }

        #endregion

        #region 自动添加控件用于显示任务执行的结果

        public void AddTab(Int64 TaskID,string TaskName)
        {
          
            bool IsExist = false;
            int j = 0;

            //判断此任务是否已经添加了Tab页
            for (j=0;j<this.tabControl1.TabPages.Count ;j++)
            {
                if (this.tabControl1.TabPages[j].Name == "page" + TaskID .ToString ())
                {
                    IsExist = true;
                    break ;
                }
            }

            if (IsExist == true)
            {
                this.tabControl1.SelectedTab = this.tabControl1.TabPages[j];
                return;
            }

          
            TabPage tPage = new TabPage();
            tPage.Name = "page" + TaskID.ToString();

            //附件任务名称的信息
            tPage.Tag = TaskName;

            tPage.Text = TaskName;
            tPage.ImageIndex = 0;

            SplitContainer sc = new SplitContainer();
            sc.Name = "sCon" + TaskID.ToString();
            sc.Orientation = Orientation.Horizontal;
            sc.Dock = DockStyle.Fill;

            cMyDataGridView d = new cMyDataGridView();
            d.Name = "grid" + TaskID.ToString();
            d.TaskRunID = TaskID;
            d.TaskName = TaskName;
            d.Dock = DockStyle.Fill;
            d.MouseDown += this.Tab_MouseDown;
            d.ContextMenuStrip = this.contextMenuStrip4;
            sc.Panel1.Controls.Add(d);

            cMyTextLog r = new cMyTextLog();
            r.Name = "tLog" + TaskID.ToString();
            r.ReadOnly = true;
            r.BorderStyle = BorderStyle.FixedSingle;
            r.BackColor = Color.White;
            r.DetectUrls = false;
            r.WordWrap = false;
            r.Dock = DockStyle.Fill;
            r.ContextMenuStrip = this.contextMenuStrip5;
            sc.Panel2.Controls.Add(r);

            tPage.Controls.Add(sc);
          
            this.tabControl1.TabPages.Add(tPage);
            this.tabControl1.SelectedTab = tPage;
          
        }

        #endregion


        #region 时间控制器
        ///定时刷新界面显示的信息，主要显示各个正在执行的任务的进度
        ///任务启动(完成)后状态的刷新,状态栏的信息刷新
        private void timer1_Tick(object sender, EventArgs e)
        {
            int proI = 0;

            if (IsTimer == true)
            {
                IsTimer = false;

                if (this.treeMenu.SelectedNode.Name == "nodRunning" )
                {
                    //如果当前选中则开始更新
                    //按照m_GatherControl.TaskManage.TaskList进行更新
                    for (int i = 0; i < m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count; i++)
                    {
                        for (int j = 0; j < this.dataTask.Rows.Count; j++)
                        {
                            if (m_GatherControl.TaskManage.TaskListControl.RunningTaskList[i].TaskID.ToString() == this.dataTask.Rows[j].Cells[1].Value.ToString())
                            {
                                try
                                {
                                    this.dataTask.Rows[j].Cells[7].Value = m_GatherControl.TaskManage.TaskListControl.RunningTaskList[i].GatheredTrueUrlCount;
                                    this.dataTask.Rows[j].Cells[8].Value = m_GatherControl.TaskManage.TaskListControl.RunningTaskList[i].GatheredTrueErrUrlCount;
                                    this.dataTask.Rows[j].Cells[9].Value = m_GatherControl.TaskManage.TaskListControl.RunningTaskList[i].TrueUrlCount;
                                    proI = ((int)m_GatherControl.TaskManage.TaskListControl.RunningTaskList[i].GatheredTrueUrlCount + (int)m_GatherControl.TaskManage.TaskListControl.RunningTaskList[i].GatheredTrueErrUrlCount) * 100 / m_GatherControl.TaskManage.TaskListControl.RunningTaskList[i].TrueUrlCount;
                                    this.dataTask.Rows[j].Cells[10].Value = proI;
                                }
                                catch (System.Exception)
                                {
                                    //捕获错误不处理
                                }
                            }
                        }
                    }
                }
                else if (this.treeMenu.SelectedNode.Name == "nodPublish" )
                {
                    //如果当前选中则开始更新
                    //按照m_GatherControl.TaskManage.TaskList进行更新
                    for (int i = 0; i < m_PublishControl.PublishManage.ListPublish.Count ; i++)
                    {
                        for (int j = 0; j < this.dataTask.Rows.Count; j++)
                        {
                            if (m_PublishControl.PublishManage.ListPublish[i].TaskID.ToString() == this.dataTask.Rows[j].Cells[1].Value.ToString())
                            {
                                try
                                {
                                    this.dataTask.Rows[j].Cells[5].Value = m_PublishControl.PublishManage.ListPublish[i].PublishedCount;
                                    if (m_PublishControl.PublishManage.ListPublish[i].Count == 0)
                                        proI = 0;
                                    else
                                        proI = (int)m_PublishControl.PublishManage.ListPublish[i].PublishedCount * 100 / m_PublishControl.PublishManage.ListPublish[i].Count;
                                    this.dataTask.Rows[j].Cells[7].Value = proI;
                                }
                                catch (System.Exception)
                                {
                                    //捕获错误不处理
                                }
                            }
                        }
                    }
                    
                }
                
                IsTimer = true;
            }

        }
        #endregion

        #region MSN 通知模式 动态消息提示框操作
        //定义一个集合来控制如果有多个消息窗口同时显示的情况

        //private List<frmNotifyWindow> frmInfos = new List<frmNotifyWindow>();

        public void ShowInfo(string Title,string Info)
        {

            ExportLog(DateTime.Now + "：" + Info  + Title);

            this.notifyIcon1.ShowBalloonTip(2, Title, Info, ToolTipIcon.Info); 

        }

        #endregion


        #region 此部分代码用于手工导出数据（已经从网上采集下来的数据）
        //定义一个代理，用于在导出数据时进行进度条的刷新
        delegate void ShowProgressDelegate(int totalMessages, int messagesSoFar, bool statusDone);

        //手动导出数据同一时间只能导出一个任务，不能进行多任务的数据导出
        private bool IsExportData()
        {
            if (this.PrograBarTxt.Visible == true)
            {
                return false;
            }
            return true;
        }


        private void ExportTxt()
        {
            if (IsExportData() == false)
            {
                MessageBox.Show(rm.GetString("Info35"), rm.GetString ("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string FileName;

            this.saveFileDialog1.OverwritePrompt = true;
            this.saveFileDialog1.Title = rm.GetString ("Info36");
            saveFileDialog1.InitialDirectory = Program.getPrjPath();
            saveFileDialog1.Filter = "Text Files(*.txt)|*.txt|All Files(*.*)|*.*";

            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileName = this.saveFileDialog1.FileName;
            }
            else
            {
                return;
            }
            Application.DoEvents();

            ExportData(FileName , cGlobalParas.PublishType.PublishTxt);

        }

        private void ExportExcel()
        {
            if (IsExportData() == false)
            {
                MessageBox.Show(rm.GetString("Info35"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string FileName;

            this.saveFileDialog1.OverwritePrompt = true;
            this.saveFileDialog1.Title = rm.GetString("Info36");
            saveFileDialog1.InitialDirectory = Program.getPrjPath();
            saveFileDialog1.Filter = "Excel Files(*.xls)|*.xls|All Files(*.*)|*.*";

            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileName = this.saveFileDialog1.FileName;
            }
            else
            {
                return;
            }
            Application.DoEvents();

            ExportData(FileName , cGlobalParas.PublishType.PublishExcel);
           
        }

        private void ExportData(string FileName,cGlobalParas.PublishType pType)
        {
            cExport eTxt = new cExport();
            string tName = this.tabControl1.SelectedTab.Tag.ToString();
            DataGridView tmp = (DataGridView)this.tabControl1.SelectedTab.Controls[0].Controls[0].Controls[0];
            this.PrograBarTxt.Visible = true;
            this.PrograBarTxt.Text =rm.GetString ("Info37") + "：" + tName + " " + "0/0";
            this.ExportProbar.Visible = true;

            //定义一个后台线程用于导出数据操作
            ShowProgressDelegate showProgress = new ShowProgressDelegate(ShowProgress);

            cExport eExcel = new cExport(this, showProgress, pType, FileName, (DataTable)tmp.DataSource);
            Thread t = new Thread(new ThreadStart(eExcel.RunProcess));
            t.IsBackground = true;
            t.Start();
            ShowInfo( rm.GetString("Info38"),rm.GetString("Info37"));

            tName = null;
        }

        private void ShowProgress(int total, int messagesSoFar, bool done)
        {
            if (this.ExportProbar.Maximum != total)
            {
                this.ExportProbar.Maximum = total;
            }
            
            ExportProbar.Value = messagesSoFar;
            this.PrograBarTxt.Text = this.PrograBarTxt.Text.Substring(0, this.PrograBarTxt.Text.IndexOf(" ")) + " " + messagesSoFar + "/" + total;

            if (done)
            {
                ShowInfo(rm.GetString("Info39"),rm.GetString("Info37"));
                this.ExportProbar.Visible = false;
                this.PrograBarTxt.Visible = false;
            }
        }

        #endregion


        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            cXmlSConfig m_config=null;
            
            //初始化参数配置信息
            try
            {
                m_config = new cXmlSConfig();
                if (m_config.ExitIsShow == true)
                {
                    frmClose fc = new frmClose();
                    fc.RExitPara = new frmClose.ReturnExitPara(GetExitPara);
                    if (fc.ShowDialog() == DialogResult.Cancel)
                    {
                        fc.Dispose();
                        e.Cancel = true;
                        return;
                    }
                    else
                    {
                        if (m_ePara == cGlobalParas.ExitPara.MinForm)
                        {
                            fc.Dispose();
                            e.Cancel = true;
                            this.Hide();
                            return;
                        }
                    }
                }
                else
                {
                    //判断是直接退出还是最小化窗体
                    if (m_config.ExitSelected == 0)
                    {
                        this.Hide();
                        e.Cancel = true;
                        return;
                    }
                }

                m_config = null;
            }
            catch (System.Exception)
            {
                MessageBox.Show(rm.GetString ("Info40"), rm.GetString ("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                frmClose fc = new frmClose();
                fc.RExitPara = new frmClose.ReturnExitPara(GetExitPara);
                if (fc.ShowDialog() == DialogResult.Cancel)
                {
                    fc.Dispose();
                    e.Cancel = true;
                    return;
                }
                else
                {
                    if (m_ePara == cGlobalParas.ExitPara.MinForm)
                    {
                        fc.Dispose();
                        e.Cancel = true;
                        this.Hide();
                        return;
                    }
                }
            }

            //判断是否存在运行的任务
            if (m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count != 0)
            {
                if (MessageBox.Show(rm.GetString("Quaere13"), rm.GetString ("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
                else
                {
                    while (m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count > 0)
                    {
                        Int64 TaskID=m_GatherControl.TaskManage.TaskListControl.RunningTaskList[0].TaskID ;

                        //销毁事件关联
                        m_GatherControl.TaskManage.TaskEventDispose(m_GatherControl.TaskManage.TaskListControl.RunningTaskList[0]);

                        m_GatherControl.Abort(m_GatherControl.TaskManage.TaskListControl.RunningTaskList[0]);

                        SaveGatherTempData(TaskID);
                    }
                }
            }
            
            //开始销毁关于采集任务及发布任务的所有资源
            m_GatherControl.Dispose();
            m_PublishControl.Dispose();

        }

        private void GetExitPara(cGlobalParas.ExitPara ePara)
        {
            m_ePara = ePara;
        }

        private void toolRestartTask_Click(object sender, EventArgs e)
        {
            ResetMultiTask();
        }

        private void ResetMultiTask()
        {
            if (this.dataTask.SelectedRows.Count == 0)
                return;
            if (this.dataTask.SelectedRows.Count == 1)
            {
                if (MessageBox.Show(rm.GetString ("Info41") + this.dataTask.SelectedCells[4].Value.ToString() + "\r\n" +
                    rm.GetString ("Quaere16"),
                    rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }
            else
            {
                if (MessageBox.Show(rm.GetString ("Quaere15"),rm.GetString ("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }

            for (int index = 0; index < this.dataTask.SelectedRows.Count; index++)
            {
                if ((cGlobalParas.TaskState)this.dataTask.SelectedRows[index].Cells[2].Value == cGlobalParas.TaskState.Failed)
                {
                    continue;
                }
                else
                {
                    try
                    {
                        ResetTask(Int64.Parse(this.dataTask.SelectedRows[index].Cells[1].Value.ToString()));
                        this.dataTask.SelectedRows[index].Cells[0].Value = imageList1.Images["stop"];
                        this.dataTask.SelectedRows[index].Cells[7].Value = "0";
                        this.dataTask.SelectedRows[index].Cells[8].Value = "0";
                        this.dataTask.SelectedRows[index].Cells[10].Value = "0";
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(this.dataTask.SelectedRows[index].Cells[4].Value.ToString() + rm.GetString("Info42") + "：" + ex.Message, rm.GetString ("MessageboxQuaere"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    } 
                }

            }

        }

        private void ResetTask(Int64 TaskID)
        {
            //重置任务 将指定的任务恢复成为默认的状态

           
                cGatherTask t = m_GatherControl.TaskManage.FindTask(TaskID);
                t.ResetTask();

                //删除Tabpage
                string pageName = "page" + TaskID;

                for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
                {
                    if (this.tabControl1.TabPages[i].Name == pageName)
                    {
                        this.tabControl1.TabPages.Remove(this.tabControl1.TabPages[i]);
                        break;
                    }
                }

                t = null;
           
        }

        private void rMenuDelRow_Click(object sender, EventArgs e)
        {
            DataGridView tmp = (DataGridView)this.tabControl1.SelectedTab.Controls[0].Controls[0].Controls[0];
            tmp.Rows.Remove(tmp.SelectedRows[0]);
            tmp = null;
        }

        private void rmmenuStartTask_Click(object sender, EventArgs e)
        {
            StartMultiTask();
        }

        private void rmmenuStopTask_Click(object sender, EventArgs e)
        {
            StopMultiTask();
        }

        private void treeMenu_Enter(object sender, EventArgs e)
        {
            DelName = this.treeMenu.Name;
        }

        private void dataTask_Enter(object sender, EventArgs e)
        {
            DelName = this.dataTask.Name;

            ResetToolState();
        }

        private void rmmenuRestartTask_Click(object sender, EventArgs e)
        {
            ResetMultiTask();
        }

        public void UpdateStatebarTask()
        {
            string s = rm.GetString ("State1");

            try
            {
                //s += m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count + m_GatherControl.TaskManage.TaskListControl.StoppedTaskList.Count + m_PublishControl.PublishManage.ListPublish.Count + "-个任务  ";
                s += m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count + "-" +  rm.GetString ("State2") + "  ";
                s += m_GatherControl.TaskManage.TaskListControl.StoppedTaskList.Count + "-" + rm.GetString("State3") + "  ";
                s += m_PublishControl.PublishManage.ListPublish.Count + "-" + rm.GetString("State4");

                this.toolStripStatusLabel2.Text = s;
            }
            catch (System.Exception)
            {
                //捕获错误不处理
            }

        }

        private void UpdateStatebarTaskState(string Info)
        {
            this.StateInfo.Text = Info;
        }

        private void UpdateStatebarTaskState(cGlobalParas.TaskState tState)
        {

            switch (tState)
            {
                case cGlobalParas.TaskState .UnStart :
                    this.StateInfo.Text = rm.GetString ("Label7");
                    break;
                case cGlobalParas.TaskState .Stopped :
                    this.StateInfo.Text = rm.GetString ("Label8");
                    break;
                case cGlobalParas.TaskState.Completed :
                    this.StateInfo.Text = rm.GetString ("Label9");
                    break;
                case cGlobalParas.TaskState.Failed :
                    this.StateInfo.Text = rm.GetString ("Label10");
                    break;
                case cGlobalParas.TaskState.Pause :
                    this.StateInfo.Text = rm.GetString ("Label11");
                    break;
                case cGlobalParas.TaskState.Running :
                    this.StateInfo.Text = rm.GetString ("Label12");
                    break;
                case cGlobalParas.TaskState.Started :
                    this.StateInfo.Text = rm.GetString ("Label13");
                    break;
                case cGlobalParas.TaskState.Waiting :
                    this.StateInfo.Text = rm.GetString ("Label14");
                    break;
                case cGlobalParas.TaskState.Publishing :
                    this.StateInfo.Text = rm.GetString ("Label15");
                    break;
                default:
                    break;
            }
        }
       

        //每半分钟进行一次刷新，查看当前的网络状态
        //如果网络状态发生改变需要网络连接状态
        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                if (cTool.IsLinkInternet() == false)
                {
                    this.staIsInternet.Image = Bitmap.FromFile(Program.getPrjPath() + "img\\a08.gif");

                    this.staIsInternet.Text = rm.GetString ("State6");

                    //如果检测到离线则停止当前正在运行的任务



                }
                else
                {
                    this.staIsInternet.Image = Bitmap.FromFile(Program.getPrjPath() + "img\\a07.gif");
                    this.staIsInternet.Text = rm.GetString ("State5");

                    //如果检测到在线，则启动已经停止的需要采集的任务
                }
            }
            catch (System.Exception )
            {
                //捕获错误不进行处理
            }
        }


        #region 发布任务的事件处理
        private void Publish_Complete(object sender, PublishCompletedEventArgs e)
        {
            InvokeMethod(this, "ShowInfo", new object[] { e.TaskName, rm.GetString("TaskPulished")});

            InvokeMethod(this, "UpdateTaskPublished", new object[] { e.TaskID ,cGlobalParas.GatherResult.PublishSuccees});

            InvokeMethod(this, "UpdateStatebarTask", null);

        }

        private void Publish_Started(object sender, PublishStartedEventArgs e)
        {
            InvokeMethod(this, "ShowInfo", new object[] { rm.GetString("TaskPublishing"), e.TaskName});

            InvokeMethod(this, "UpdateStatebarTask", null);
        }

        private void Publish_Error(object sender, PublishErrorEventArgs e)
        {
            InvokeMethod(this, "UpdateTaskPublished", new object[] { e.TaskID ,cGlobalParas.GatherResult.PublishFailed });

            InvokeMethod(this, "ShowInfo", new object[] {e.TaskName, rm.GetString("TaskPublishFailed")});

            InvokeMethod(this, "UpdateStatebarTask", null);

        }

        private void Publish_Failed(object sender, PublishFailedEventArgs e)
        {
            InvokeMethod(this, "UpdateTaskPublished", new object[] { e.TaskID ,cGlobalParas.GatherResult.PublishFailed });

            InvokeMethod(this, "ShowInfo", new object[] { e.TaskName, rm.GetString("TaskPublishFailed")});

            InvokeMethod(this, "UpdateStatebarTask", null);
        }

        private void Publish_TempDataCompleted(object sender, PublishTempDataCompletedEventArgs e)
        {

        }

        private void Publish_Log(object sender, PublishLogEventArgs e)
        {
            //写发布任务的日志
            Int64 TaskID = e.TaskID;
            string strLog = e.strLog;
            string conName = "sCon" + TaskID;
            string pageName = "page" + TaskID;

            SetValue(this.tabControl1.TabPages[pageName].Controls[conName].Controls[1].Controls[0], "Text", strLog);
        }

        #endregion


        //处理任务发布完成的工作
        public void UpdateTaskPublished(Int64 TaskID,cGlobalParas.GatherResult tState)
        {
            //将已经完成发布的任务添加到完成任务的索引文件中
            Task.cTaskComplete t = new Task.cTaskComplete();
            t.InsertTaskComplete(TaskID, tState);
            t = null;

            //删除taskrun节点
            cTaskRun tr = new cTaskRun();
            tr.LoadTaskRunData();
            tr.DelTask(TaskID);

            //修改Tab页的名称


            //删除run中的任务实例文件
            string FileName = Program.getPrjPath() + "Tasks\\run\\" + "Task" + TaskID + ".xml";
            System.IO.File.Delete(FileName);

            if (this.treeMenu.SelectedNode.Name == "nodPublish")
            {
                for (int i = 0; i < this.dataTask.Rows.Count; i++)
                {
                    if (this.dataTask.Rows[i].Cells[1].Value.ToString() == TaskID.ToString())
                    {
                        this.dataTask.Rows.Remove(this.dataTask.Rows[i]);
                        break;
                    }
                }
            }
            else if (this.treeMenu.SelectedNode.Name == "nodComplete")
            {
                //重新加载已完成任务的信息
                LoadCompleteTask();
            }
        }

        private void toolMenuVisityijie_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.yijie.net"); 
        }


        private void MenuOpenMainfrm_Click(object sender, EventArgs e)
        {
            this.Visible = true;

            this.WindowState = FormWindowState.Maximized;
            this.Activate();
        }

        private void MenuCloseSystem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BrowserMultiData()
        {
            if (this.dataTask.SelectedRows.Count == 0)
                return;

            for (int index = 0; index < this.dataTask.SelectedRows.Count; index++)
            {
                if ((cGlobalParas.TaskState)this.dataTask.SelectedRows[index].Cells[2].Value == cGlobalParas.TaskState.Failed)
                {
                    continue;
                }
                else
                {
                    BrowserData(Int64.Parse(this.dataTask.SelectedRows[index].Cells[1].Value.ToString()), this.dataTask.SelectedRows[index].Cells[4].Value.ToString());
                }
            }
        }

        private void BrowserData(Int64 TaskID,string TaskName)
        {
            if (this.dataTask.Rows.Count != 0)
            {
                //Int64 TaskID = Int64.Parse (this.dataTask.SelectedCells[1].Value.ToString ());
                DataTable tmp = new DataTable();
                string dFile = "";

                //判断是浏览的那些数据：正在运行还是采集完成

                if (this.treeMenu.SelectedNode.Name == "nodRunning")
                {
                    cTaskRun tr = new cTaskRun();
                    tr.LoadSingleTask(TaskID);
                    dFile=tr.GetTempFile(0);
                    tr = null;
                }
                else if (this.treeMenu.SelectedNode.Name == "nodComplete")
                {
                    cTaskComplete tc = new cTaskComplete();
                    tc.LoadSingleTask(TaskID);

                    dFile = tc.GetTempFile(0);
                    tc = null;
                }

                string conName = "sCon" + TaskID;
                string pageName = "page" + TaskID;

                AddTab(TaskID, TaskName);

                if (File.Exists(dFile))
                {
                    try
                    {
                        tmp.ReadXml(dFile);
                    }
                    catch (System.Exception)
                    {
                        //有可能在保存数据时发生了错误，因此需要忽略错误，直接进行,但需要通过系统信息显示错误
                        ExportLog(DateTime.Now +  "：" + TaskName + rm.GetString ("Info43"));

                    }
                }

                ((cMyDataGridView)(this.tabControl1.TabPages[pageName].Controls[conName].Controls[0].Controls[0])).gData = tmp;

                if (tmp.Rows.Count == 0)
                {
                    this.toolExportData.Enabled = false;
                }
                else
                {
                    this.toolExportData.Enabled = true;
                }
                tmp = null;
            }
        }



        private void toolBrowserData_Click(object sender, EventArgs e)
        {
            BrowserMultiData();
        }

        private void rmenuBrowserData_Click(object sender, EventArgs e)
        {
            BrowserMultiData();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Visible = true;

                this.WindowState = FormWindowState.Maximized;

                this.Activate();

            }
        }

        private void ImportTask()
        {
            //支持同时导入多个任务

            this.openFileDialog1.Title = rm.GetString ("Info44");

            openFileDialog1.InitialDirectory = Program.getPrjPath() + "tasks";
            openFileDialog1.Filter = "Soukey Task Files(*.xml)|*.xml";


            if (this.openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            string FileName = this.openFileDialog1.FileName;
            string TaskClass = "";
            string NewFileName = "";

            //验证任务格式是否正确
            try
            {
                Task.cTask t = new Task.cTask();
                t.LoadTask(FileName);

                if (t.TaskName != "")
                {
                    NewFileName = t.TaskName + ".xml";
                }

                if (t.TaskClass != "")
                {
                    TaskClass = t.TaskClass.ToString();
                }

                //根据任务的分类导入指定的目录
                string TaskPath = Program.getPrjPath() + "tasks\\";
                if (TaskClass != "")
                {
                    TaskPath += TaskClass ;
                }

                if (NewFileName == "")
                {
                    NewFileName = "task" + DateTime.Now.ToFileTime().ToString() + ".xml";
                }


                if (FileName == TaskPath + NewFileName)
                {
                    MessageBox.Show(rm.GetString("Info45"), rm.GetString ("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Task.cTaskClass cTClass = new Task.cTaskClass();

                if (!cTClass.IsExist(TaskClass) && TaskClass != "")
                {
                    //表示是一个新任务分类
                    int TaskClassID = cTClass.AddTaskClass(TaskClass, TaskPath);
                    cTClass = null;

                    //建立树形结构的任务分类节点
                    TreeNode newNode = new TreeNode();
                    newNode.Tag = TaskPath;
                    newNode.Name = "C" + TaskClassID;
                    newNode.Text = TaskClass;
                    newNode.ImageIndex = 0;
                    newNode.SelectedImageIndex = 0;
                    this.treeMenu.Nodes["nodTaskClass"].Nodes.Add(newNode);
                    newNode = null;

                }

                try
                {
                    System.IO.File.Copy(FileName, TaskPath + "\\" + NewFileName);
                }
                catch (System.IO.IOException)
                {
                    t = null;
                    MessageBox.Show(rm.GetString("Info46"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                
                //插入任务索引文件
                t.InsertTaskIndex(TaskPath);

                t = null;

                MessageBox.Show(rm.GetString ("Info47") + TaskClass, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);



            }
            catch (cSoukeyException )
            {
                if (MessageBox.Show(rm.GetString("Quaere17"),  rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    frmUpgradeTask fu = new frmUpgradeTask(FileName);
                    fu.ShowDialog();
                    fu.Dispose();
                    fu = null;
                }

                return;
            }

            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Info48") + ex.Message, rm.GetString ("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void treeMenu_DragEnter(object sender, DragEventArgs e)
        {
            Old_SelectedNode = this.treeMenu.SelectedNode;
            this.treeMenu.Focus ();
            e.Effect = DragDropEffects.Copy;
        }

        private void treeMenu_DragDrop(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(typeof(DataGridViewRow)))
            {
                Point p = this.treeMenu.PointToClient(new Point(e.X, e.Y));
                TreeViewHitTestInfo index = this.treeMenu.HitTest(p);

                if (index.Node != null)
                {
                    if (index.Node.Name.Substring(0, 1) == "C" || index.Node.Name == "nodTaskClass")
                    {
                        DataGridViewRow drv = (DataGridViewRow)e.Data.GetData(typeof(DataGridViewRow));
                        

                        string TaskName = drv.Cells[4].Value.ToString();

                        string oldName="";

                        if (Old_SelectedNode.Name == "nodTaskClass")
                            OldName = "";
                        else
                            oldName = Old_SelectedNode.Text;

                        string NewName="";
                        if (index.Node.Name == "nodTaskClass")
                            NewName = "";
                        else
                            NewName = index.Node.Text;

                        if (oldName == NewName)
                        {
                            this.treeMenu.SelectedNode = Old_SelectedNode;
                            return;
                        }

                        cTask t = new cTask();

                        try
                        {
                            t.ChangeTaskClass(TaskName, oldName, NewName);
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show(rm.GetString("Info49") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            t = null;
                            this.treeMenu.SelectedNode = Old_SelectedNode;
                            return;
                        }

                        t = null;

                        this.dataTask.Rows.Remove(drv);
                    }
                    else
                    {
                        MessageBox.Show(rm.GetString ("Info50") + index.Node.Text, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                this.treeMenu.SelectedNode = Old_SelectedNode;
            }
        }

        private void dataTask_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dataTask_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.dataTask.SelectedRows.Count == 0)
            {
                return;
            }

            //SetToolbarState();

            //判断是否为左键点击，如果是则需要启动拖放操作
            if (((e.Button & MouseButtons.Left) == MouseButtons.Left && this.treeMenu.SelectedNode.Name == "nodTaskClass") ||
                ((e.Button & MouseButtons.Left) == MouseButtons.Left && this.treeMenu.SelectedNode.Name.Substring(0, 1) == "C"))
            {

                DataGridViewRow dragData = (DataGridViewRow)this.dataTask.SelectedRows[0];
                //Size dragSize = SystemInformation.DragSize;
                //Rectangle dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
                this.dataTask.DoDragDrop(dragData, DragDropEffects.Copy);
            }

        }

        private void toolMenuDownloadTask_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.yijie.net/downloadTask.html"); 
        }

        private void toolMenuAbout_Click(object sender, EventArgs e)
        {
            frmAbout f = new frmAbout();
            f.ShowDialog();
            f.Dispose();
        }

        private void sToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.yijie.net"); 
        }

        private void menuMailto_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:feiw@163.com"); 
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedIndex == 0)
            {
                this.toolExportData.Enabled = false;
                return;
            }

            DataGridView tmp = (DataGridView)this.tabControl1.SelectedTab.Controls[0].Controls[0].Controls[0];
            if (tmp.Rows.Count > 0)
                this.toolExportData.Enabled = true;
            else
                this.toolExportData.Enabled = false;


        }

        //设置加载错误的gridrows的现实样式
        private void SetRowErrStyle()
        {
            this.m_RowStyleErr = new DataGridViewCellStyle();
            this.m_RowStyleErr.Font = new Font(DefaultFont,FontStyle.Italic);
            this.m_RowStyleErr.ForeColor = Color.Red;
        }

        private void toolMenuProgram_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://sourceforge.net/projects/soukeygetdata"); 
        }

        private void toolImportTask_Click(object sender, EventArgs e)
        {
            ImportTask();
        }

        private void toolManageDict_Click(object sender, EventArgs e)
        {
            frmDict dfrm = new frmDict();
            dfrm.ShowDialog();
            dfrm.Dispose();
        }

        private void cmdCloseInfo_Click(object sender, EventArgs e)
        {
            this.splitContainer3.SplitterDistance = this.splitContainer3.Height ;
            this.toolLookInfo.Checked = false;
        }

        private void toolLookInfo_Click(object sender, EventArgs e)
        {
            if (this.splitContainer3.Panel2.Height > 1)
            {
                this.splitContainer3.SplitterDistance = this.splitContainer3.Height;
                this.toolLookInfo.Checked = false;
            }
            else
            {
                this.splitContainer3.SplitterDistance = 480;
                this.toolLookInfo.Checked = true;
            }
        }

        public void ExportLog(string str)
        {
            this.txtLog.Text = str + "\r\n" + this.txtLog.Text ;

            if (m_IsAutoSaveLog == true)
            {
                try
                {
                    cSystemLog sl = new cSystemLog();
                    sl.WriteLog(str);
                    sl = null;
                }
                catch (System.Exception ex)
                {
                    this.txtLog.Text = rm.GetString ("Info51") + ex.Message + "\r\n" + this.txtLog.Text;
                }
            }


        }
     
        //设置窗体现实的tooltip信息
        private void SetToolTip()
        {
            tTip  = new ToolTip();
            this.tTip.SetToolTip  (this.cmdCloseInfo ,rm.GetString ("Info52"));
            this.tTip.SetToolTip(this.treeMenu, rm.GetString ("Info53"));
            //this.
        }

        private void tabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point pt = new Point(e.X, e.Y);
                Rectangle recTab = new Rectangle();
                for (int i = 0; i < tabControl1.TabCount; i++)
                {
                    recTab = tabControl1.GetTabRect(i);
                    if (recTab.Contains(pt))
                        this.tabControl1.SelectedIndex = i;
                }
            }
        }

        private void treeMenu_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DataGridViewRow)))
            {
                Point p = this.treeMenu.PointToClient(new Point(e.X, e.Y));
                TreeViewHitTestInfo index = this.treeMenu.HitTest(p);

                if (index.Node != null)
                {
                    if (index.Node.Name.Substring(0, 1) == "C" || index.Node.Name == "nodTaskClass")
                    {
                        e.Effect = DragDropEffects.Copy;
                        this.treeMenu.SelectedNode = index.Node;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                    }
                }
            }
        }

        private void treeMenu_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                treeMenuMouseClick(e.Node);
        }

        private void treeMenuMouseClick(TreeNode eNode)
        {
            this.treeMenu.SelectedNode = eNode;

            //控制节点是否可以编辑，只能编辑任务分类
            if (this.treeMenu.SelectedNode.Name.Substring(0, 1) == "C" || this.treeMenu.SelectedNode.Name == "nodTaskClass"
                || this.treeMenu.SelectedNode.Name=="nodPlanRunning") 
            {
                if (this.treeMenu.SelectedNode.Name.Substring(0, 1) == "C")
                    this.treeMenu.LabelEdit = true;
                else
                    this.treeMenu.LabelEdit = false;

                this.dataTask.ReadOnly = false;
            }
            else
            {
                this.treeMenu.LabelEdit = false;
                this.dataTask.ReadOnly = true;
            }

            switch (eNode.Name)
            {
                case "nodRunning":   //运行区的任务
                    try
                    {
                        LoadRunTask();
                    }
                    catch (System.IO.IOException)
                    {
                        if (MessageBox.Show(rm.GetString("Quaere18"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            CreateTaskRun();
                        }
                    }
                    catch (System.Exception)
                    {
                        MessageBox.Show(rm.GetString("Info54") + Program.getPrjPath() + "tasks\\taskrun.xml", rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }

                    //启动时间器用于更新任务显示的进度
                    this.timer1.Enabled = true;
                    
                    break;

                case "nodPublish":

                    LoadPublishTask();

                    //启动时间器用于更新任务显示的进度
                    this.timer1.Enabled = true;

                    break;

                case "nodComplete":    //已经完成采集的任务

                    try
                    {
                        LoadCompleteTask();
                    }
                    catch (System.IO.IOException)
                    {
                        if (MessageBox.Show(rm.GetString ("Quaere19") + Program.getPrjPath() + "Data", rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            CreateTaskComplete();
                        }
                    }
                    catch (System.Exception)
                    {
                        MessageBox.Show(rm.GetString("Info55") + Program.getPrjPath() + "data\\index.xml", rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    };

                    this.timer1.Enabled = false;

                    break;

                case "nodSnap":
                    try
                    {
                        this.dataTask.Columns.Clear();
                        this.dataTask.Rows.Clear();
                    }
                    catch (System.Exception)
                    {
                    }

                    break;
                case "nodSoukey":
                    try
                    {
                        LoadSoukeyTask();
                    }
                    catch (System.Exception)
                    {
                        MessageBox.Show(rm.GetString("Info56"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }

                    break;
                case "nodTaskPlan":
                    try
                    {
                        this.dataTask.Columns.Clear();
                        this.dataTask.Rows.Clear();
                    }
                    catch (System.Exception)
                    {
                    }

                    break;
                case "nodPlanRunning":
                    try
                    {
                        LoadTaskPlan();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(rm.GetString("Info57") + ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }

                    break;
                case "nodPlanCompleted":
                    try
                    {
                        LoadPlanLog();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show( rm.GetString("Info58") + ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    break;
                default:
                    try
                    {
                        LoadOther(eNode);
                    }
                    catch (System.IO.IOException)
                    {
                        if (MessageBox.Show(this.treeMenu.SelectedNode.Text + rm.GetString("Quaere20"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            CreateTaskIndex(this.treeMenu.SelectedNode.Tag.ToString());
                        }
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(rm.GetString ("Info59") + ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }

                    this.timer1.Enabled = false;

                    break;
            }

            //无论点击树形结构的菜单都将按钮置为不可用
            this.toolStartTask.Enabled = false;
            this.toolRestartTask.Enabled = false;
            this.toolStopTask.Enabled = false;
            this.toolExportData.Enabled = false;
            this.toolBrowserData.Enabled = false;

            this.toolCopyTask.Enabled = false;
            this.toolPasteTask.Enabled = false;

            //置删除按钮为有效
            DelName = this.treeMenu.Name;
            this.toolDelTask.Enabled = true;

            UpdateStatebarTaskState(rm.GetString ("State7") + " " + eNode.Text);

        }

        private void treeMenu_DragLeave(object sender, EventArgs e)
        {
            this.treeMenu.SelectedNode = Old_SelectedNode;
        }

        private void toolMenuUpdate_Click(object sender, EventArgs e)
        {
            frmUpdate fu = new frmUpdate();
            fu.ShowDialog();
            fu.Dispose();
        }

        private void rmenuSaveLog_Click(object sender, EventArgs e)
        {
            string FileName = "";

            this.saveFileDialog1.OverwritePrompt = true;
            this.saveFileDialog1.Title = rm.GetString ("Info60");
            saveFileDialog1.InitialDirectory = Program.getPrjPath();
            saveFileDialog1.Filter = "Text Files(*.txt)|*.txt|All Files(*.*)|*.*";

            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileName = this.saveFileDialog1.FileName;
            }
            else
            {
                return;
            }
            Application.DoEvents();

            RichTextBox tmp = (RichTextBox)this.tabControl1.SelectedTab.Controls[0].Controls[1].Controls[0];

            tmp.SaveFile(FileName, RichTextBoxStreamType.TextTextOleObjs);

            ShowInfo(rm.GetString("LogSave"), rm.GetString ("Task") + "：" + this.tabControl1.SelectedTab.Text + " " + rm.GetString("LogSaveSuccess"));

        }

        private void toolWebbrowser_Click(object sender, EventArgs e)
        {
            frmBrowser fweb = new frmBrowser();
            fweb.getFlag = 4;
            fweb.ShowDialog();
            fweb.Dispose();
        }

        private void toolMenuExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolMenuNewTaskPlan_Click(object sender, EventArgs e)
        {
            NewPlan();
        }

        private void NewPlan()
        {
            frmTaskPlan fTaskPlan = new frmTaskPlan();
            fTaskPlan.FormState = cGlobalParas.FormState.New;
            fTaskPlan.ShowDialog();
            fTaskPlan.Dispose();

            //判断如果是选择的任务计划节点，则进行刷新
            if (this.treeMenu.SelectedNode.Name == "nodPlanRunning")
            {
                LoadTaskPlan();
            }
        }

        private void menuAddTask_Click(object sender, EventArgs e)
        {
            NewTask();
        }

        private void menuAddTaskPlan_Click(object sender, EventArgs e)
        {
            NewPlan();
        }

        private void rmenuAddPlan_Click(object sender, EventArgs e)
        {
            NewPlan();
        }

        private void rmenuDelPlan_Click(object sender, EventArgs e)
        {
            DelPlan();
        }

        private void toolUpgradeTask_Click(object sender, EventArgs e)
        {
            frmUpgradeTask fUt = new frmUpgradeTask();
            fUt.ShowDialog();
            fUt.Dispose();
            
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            
            cXmlSConfig m_config=null;
            
            //初始化参数配置信息
            try
            {
                m_config = new cXmlSConfig();

                if (m_config.IsFirstRun == true)
                {
                    frmHelpInfo fh = new frmHelpInfo();
                    fh.ShowDialog();
                    fh.Dispose();
                    fh = null;
                    m_config.IsFirstRun = false;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString ("Info61") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            m_config = null;
 
        }

        private void toolmenuConfig_Click(object sender, EventArgs e)
        {
            frmConfig fc = new frmConfig();
            fc.ShowDialog();
            fc.Dispose();
        }

        private void rmenuRenameTaskClass_Click(object sender, EventArgs e)
        {
            if (this.treeMenu.SelectedNode.Name.Substring(0, 1) == "C")
                this.treeMenu.SelectedNode.BeginEdit();

        }

        private void toolPasteT_Click(object sender, EventArgs e)
        {
            PasteTask();
        }

        private void treeMenu_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (this.treeMenu.SelectedNode.Name.Substring(0, 1) != "C" || e.Label == "" || e.Label == null)
            {
                e.CancelEdit = true;
                return;
            }

            //定义一个修改分类名称的委托
            delegateRenameTaskClass sd   =new delegateRenameTaskClass(this.RenameTaskClass); 

            //开始调用函数,可以带参数 
            IAsyncResult ir = sd.BeginInvoke(e.Node.Text, e.Label  , null, null); 

            //显示等待的窗口 
            frmWaiting fWait = new frmWaiting(rm.GetString("Info62"));
            fWait.Text = rm.GetString ("Info62");

            fWait.Show(this); 
            //刷新这个等待的窗口 
            Application.DoEvents(); 

            //循环检测是否完成了异步的操作 
            while   (true) 
            { 
                if   (ir.IsCompleted) 
                { 
                //完成了操作则关闭窗口 
                    fWait.Close(); 
                    break; 
                } 
            } 

            //取函数的返回值 
            bool  retValue   =   sd.EndInvoke(ir);

            if (retValue == false)
                e.CancelEdit = true ;

        }


        //修改任务分类名称的代理 
        private delegate bool delegateRenameTaskClass(string OldName,string NewName);
        private bool RenameTaskClass(string OldName, string NewName)
        {
            Task.cTaskClass tClass = new Task.cTaskClass();

            try
            {
                tClass.RenameTaskClass(OldName, NewName);
            }
            catch (System.Exception ex)
            {
                tClass = null;
                MessageBox.Show(rm.GetString("Info63") + ex.Message, rm.GetString ("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            tClass = null;

            return true;
        }

        private void rmmenuRenameTask_Click(object sender, EventArgs e)
        {
            this.dataTask.CurrentCell=this.dataTask[4,this.dataTask.CurrentCell.RowIndex];

            this.dataTask.BeginEdit(true);
        }

        //定义一个代理用于修改任务的名称
        private delegate bool delegateRenameTaskName(string TaskClass,string OldName, string NewName);
        private bool RenameTaskName(string TaskClass,string OldName, string NewName)
        {
            cTask t = new cTask();

            try
            {
                t.RenameTask(TaskClass, OldName, NewName);
            }
            catch (System.Exception ex)
            {
                t = null;
                MessageBox.Show(rm.GetString("Info64") + ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            t = null;

            return true;
        }

        //定义一个代理用于修改计划的名称
        private delegate bool delegateRenamePlanName(string OldName, string NewName);
        private bool RenamePlanName(string OldName, string NewName)
        {
            cPlans p = new cPlans();

            try
            {
                p.RenamePlanName ( OldName, NewName);
            }
            catch (System.Exception ex)
            {
                p = null;
                MessageBox.Show(rm.GetString("Info65") + ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            p = null;

            return true;
        }

        //定义一个全局变量，用于存储将要修改任务的名称或计划的名称
        private string OldName = "";

        private void dataTask_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataTask.CurrentCell.Value == null)
            {
                this.dataTask.CurrentCell.Value = OldName;
                return;
            }
            else if (this.dataTask.CurrentCell.Value.ToString().Trim() == "" || this.dataTask.CurrentCell.Value.ToString().Trim() == OldName)
            {
                this.dataTask.CurrentCell.Value = OldName;
                return;

            }
            

            //判断修改的是任务的名称还是计划的名称
            if (this.treeMenu.SelectedNode.Name == "nodPlanRunning")
            {
                //定义一个修改计划名称的委托
                delegateRenamePlanName sd = new delegateRenamePlanName(this.RenamePlanName);

                //开始调用函数,可以带参数 
                IAsyncResult ir = sd.BeginInvoke(OldName, this.dataTask.CurrentCell.Value.ToString(), null, null);

                //显示等待的窗口 
                frmWaiting fWait = new frmWaiting(rm.GetString("Info66"));
                fWait.Text = rm.GetString("Info66");

                fWait.Show(this);
                //刷新这个等待的窗口 
                //Application.DoEvents();

                //循环检测是否完成了异步的操作 
                while (true)
                {
                    if (ir.IsCompleted)
                    {
                        //完成了操作则关闭窗口
                        fWait.Close();
                        break;
                    }
                }

                //取函数的返回值 
                bool retValue = sd.EndInvoke(ir);

                if (retValue == false)
                    this.dataTask.CurrentCell.Value = OldName;
            
            }
            else
            {

                //定义一个修改分类名称的委托
                delegateRenameTaskName sd = new delegateRenameTaskName(this.RenameTaskName);
                IAsyncResult ir;

                //开始调用函数,可以带参数 
                if (this.treeMenu.SelectedNode.Name == "nodTaskClass")
                    ir = sd.BeginInvoke("", OldName, this.dataTask.CurrentCell.Value.ToString(), null, null);
                else
                    ir = sd.BeginInvoke(this.treeMenu.SelectedNode.Text, OldName, this.dataTask.CurrentCell.Value.ToString(), null, null);
               
                //显示等待的窗口 
                frmWaiting fWait = new frmWaiting(rm.GetString("Info67"));
                fWait.Text = rm.GetString("Info67");

                fWait.Show(this);
                //刷新这个等待的窗口 
                Application.DoEvents();

                //循环检测是否完成了异步的操作 
                while (true)
                {
                    if (ir.IsCompleted)
                    {
                        //完成了操作则关闭窗口
                        fWait.Close();
                        break;
                    }
                }

                //取函数的返回值 
                bool retValue = sd.EndInvoke(ir);

                if (retValue == false)
                    this.dataTask.CurrentCell.Value = OldName;
            
            }

        }

        private void dataTask_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (this.dataTask.CurrentCell.Value == null)
                OldName = "";
            else
                OldName = this.dataTask.CurrentCell.Value.ToString();
        }

        private void CopyTask()
        {
            if (this.treeMenu.SelectedNode.Name == "nodTaskClass")
            {
                Clipboard.SetDataObject("SoukeyNetGetTask:" + "" + "/" + this.dataTask.SelectedCells[4].Value.ToString());
            }
            else
            {
                Clipboard.SetDataObject("SoukeyNetGetTask:" + this.treeMenu.SelectedNode.Text + "/" + this.dataTask.SelectedCells[4].Value.ToString());
            }
            this.toolPasteTask.Enabled = true; 
        }

        private void PasteTask()
        {
            IDataObject cdata;
            cdata = Clipboard.GetDataObject();

            string TaskClass;
            string TaskName;

            //判断数据是否为文本
            if (IsClipboardSoukeyData())
            {
                string tInfo = cdata.GetData(DataFormats.Text).ToString();
                tInfo = tInfo.Substring(17, tInfo.Length - 17);

                //尝试分解获取的文本，有可能剪贴板中的信息不是Soukey采摘的信息
                if (tInfo.IndexOf("/") >= 0)
                {
                    try
                    {
                        bool IsTaskClass = false;
                        bool IsTaskName = false;

                        TaskClass = tInfo.Substring(0, tInfo.IndexOf("/") );
                        TaskName = tInfo.Substring(tInfo.IndexOf("/") + 1, tInfo.Length - tInfo.IndexOf("/") - 1);

                        if (TaskClass == "" && TaskName == "")
                            return;

                        //信息分解后再次验证指定的任务分类是否存在
                        cTaskClass tc = new cTaskClass();
                        int TClassCount = tc.GetTaskClassCount();

                        for (int i = 0; i < TClassCount; i++)
                        {
                            //taskclass为空表示是在分类的根节点下
                            if (tc.GetTaskClassName(i) == TaskClass || TaskClass=="")
                            {
                                IsTaskClass = true;
                                break;
                            }
                        }

                        if (IsTaskClass == false)
                        {
                            throw new cSoukeyException(rm.GetString("Error17"));
                            return;
                        }

                        string tClassPath = tc.GetTaskClassPathByName(TaskClass);

                        tc = null;

                        //粘贴任务操作
                        string NewTClass="";

                        if (this.treeMenu.SelectedNode.Name == "nodTaskClass")
                            NewTClass = "";
                        else
                            NewTClass = this.treeMenu.SelectedNode.Text;

                        cTask t = new cTask();

                        t.CopyTask(TaskName, TaskClass, NewTClass);

                        //增加datagridview的行，表示拷贝成功

                        this.dataTask.Rows.Add(imageList1.Images["task"], t.TaskID, cGlobalParas.TaskState.UnStart, this.treeMenu.SelectedNode.Name, t.TaskName,
                            cGlobalParas.ConvertName(int.Parse(t.TaskType)), (t.IsLogin == true ? rm.GetString("Logon") : rm.GetString("NoLogon")),
                           t.WebpageLink.Count.ToString(), cGlobalParas.ConvertName(int.Parse(t.RunType)),
                           cGlobalParas.ConvertName(int.Parse ( t.ExportType)));

                        t = null;
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(rm.GetString("Info68") + ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
            }
        }

        private void toolCopyTask_Click(object sender, EventArgs e)
        {
            CopyTask();
        }

        private void rmenuCopyTask_Click(object sender, EventArgs e)
        {
            CopyTask();
        }

        private void rmenuPasteTask_Click(object sender, EventArgs e)
        {
            PasteTask();
        }

        private void dataTask_CopyTask(object sender, CopyTaskEventArgs e)
        {
            CopyTask();
        }

        private void dataTask_PasteTask(object sender, PasteTaskEventArgs e)
        {
            PasteTask();
        }

        private bool IsClipboardSoukeyData()
        {
            //判断数据是否为文本
            if (Clipboard.ContainsData(DataFormats.Text))
            {
                IDataObject cdata;
                cdata = Clipboard.GetDataObject();
                if (cdata.GetDataPresent(DataFormats.Text))
                {
                    string tInfo = cdata.GetData(DataFormats.Text).ToString();
                    if (tInfo.Length > 18)
                    {
                        if (tInfo.Substring(0, 17) == "SoukeyNetGetTask:")
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void toolUrlEncoding_Click(object sender, EventArgs e)
        {
            frmUrlEncoding f = new frmUrlEncoding();
            f.Show(this);
            //Application.DoEvents();
            //f.Dispose();
        }

        private void toolmenuAuto_Click(object sender, EventArgs e)
        {
            try
            {
                cXmlSConfig Config = new cXmlSConfig();
                Config.CurrentLanguage =cGlobalParas.CurLanguage.Auto ;
                Config = null;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show (ex.Message ,rm.GetString ("MessageboxError"),MessageBoxButtons.OK ,MessageBoxIcon.Error );
                return ;
            }

            this.toolmenuAuto.Checked = true;
            this.toolmenuEnglish.Checked = false;
            this.toolmenuCHS.Checked = false;

            MessageBox.Show(rm.GetString("Info113"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void toolmenuEnglish_Click(object sender, EventArgs e)
        {
             try
            {
                cXmlSConfig Config = new cXmlSConfig();
                Config.CurrentLanguage = cGlobalParas.CurLanguage.enUS;
                Config = null;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show (ex.Message ,rm.GetString ("MessageboxError"),MessageBoxButtons.OK ,MessageBoxIcon.Error );
                return ;
            }

            this.toolmenuAuto.Checked = false;
            this.toolmenuEnglish.Checked = true;
            this.toolmenuCHS.Checked = false;

            MessageBox.Show(rm.GetString("Info113"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void toolmenuCHS_Click(object sender, EventArgs e)
        {
             try
            {
                cXmlSConfig Config = new cXmlSConfig();
                Config.CurrentLanguage = cGlobalParas.CurLanguage.zhCN;
                Config = null;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show (ex.Message ,rm.GetString ("MessageboxError"),MessageBoxButtons.OK ,MessageBoxIcon.Error );
                return ;
            }

            this.toolmenuAuto.Checked = false;
            this.toolmenuEnglish.Checked = false;
            this.toolmenuCHS.Checked = true;

            MessageBox.Show(rm.GetString("Info113"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dataTask_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (this.dataTask.CurrentCell.ColumnIndex == 0)
            {

                if (e.Control is TextBox)
                {

                    TextBox tb = e.Control as TextBox;

                    tb.KeyPress -= new KeyPressEventHandler(tb_KeyPress);

                    tb.KeyPress += new KeyPressEventHandler(tb_KeyPress);

                }

            }
        }

        void tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar)))
            {

                Keys key = (Keys)e.KeyChar;

                if (!(key == Keys.Back || key == Keys.Delete))
                {
                    e.Handled = true;
                }
            }
        }

    }

}