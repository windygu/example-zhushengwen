using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Microsoft.Office;
using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using System.Web;
using SoukeyNetget.Log;
using SoukeyNetget.Listener;
using SoukeyNetget.Plan;

///功能：发布任务
///完成时间：2009-7-21
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.01.00
///修订：增加了发布类别，支持数据库、web及文件发布
namespace SoukeyNetget.publish
{
    class cPublishTask
    {
        private cPublishTaskData m_pTaskData;
        private cPublishManage m_PublishManage;

        public cPublishTask(cPublishManage taskManage,Int64 TaskID,System.Data.DataTable dData)
        {
            m_PublishManage = taskManage;
            m_pTaskData = new cPublishTaskData();

            //初始化任务数据
            LoadTaskInfo(TaskID, dData);

        }

        ~cPublishTask()
        {

        }

        #region 属性

        public Int64 TaskID
        {
            get { return m_pTaskData.TaskID; }
        }

        public cPublishTaskData TaskData
        {
            get { return m_pTaskData; }
        }

        public cPublishManage PublishManage
        {
            get { return m_PublishManage; }
        }

        public string FileName
        {
            get { return m_pTaskData.FileName; }
        }

        public cGlobalParas.PublishType PublishType
        {
            get { return m_pTaskData.PublishType; }
        }

        public int Count
        {
            get { return m_pTaskData.Count; }
        }

        private int m_PublishedCount;
        public int PublishedCount
        {
            get { return m_PublishedCount; }
            set { m_PublishedCount = value; }
        }

        #endregion

        //根据提供的taskid加载任务信息
        //数据不应该是传进来,是读取文件的,但现在不支持事务处理,所以传进来
        private void LoadTaskInfo(Int64 TaskID, System.Data.DataTable dData)
        {
            //DataTable dt = new DataTable();
            Task.cTask t = new Task.cTask();

            t.LoadTask(Program.getPrjPath () + "tasks\\run\\task" + TaskID + ".xml");

            string FileName = t.SavePath  + "\\" + t.TaskName + "-" + t.TaskID + ".xml"; 
         
            m_pTaskData.TaskID =t.TaskID ;
            m_pTaskData.TaskName =t.TaskName ;
            //m_pTaskData.DataPwd =t.DataPwd ;
            m_pTaskData.ExportFile = t.ExportFile;
            m_pTaskData.DataSource =t.DataSource ;
            //m_pTaskData.DataUser =t.DataUser ;
            m_pTaskData.FileName = FileName;
            
            //dt.ReadXml(FileName);
            //需要保存的或者导出的数据还是传入，因为需要临时数据的保存
            //下一版需要将临时数据保存和发布数据进行分离
            m_pTaskData.PublishData = dData ;
            m_pTaskData.PublishData.TableName = t.TaskName + "-" + t.TaskID + ".xml"; 

            m_pTaskData.PublishType =(cGlobalParas.PublishType)(int.Parse (t.ExportType ));
            m_pTaskData.DataTableName =t.DataTableName ;

            m_pTaskData.InsertSql = t.InsertSql;
            m_pTaskData.ExportUrl = t.ExportUrl;
            m_pTaskData.ExportUrlCode = t.ExportUrlCode;
            m_pTaskData.ExportCookie = t.ExportCookie;

            m_pTaskData.IsErrorLog = t.IsErrorLog;

            m_pTaskData.IsTrigger = t.IsTrigger;

            m_pTaskData.IsExportHeader = t.IsExportHeader;

            if (t.IsTrigger == true)
            {
                m_pTaskData.TriggerType = t.TriggerType;
                m_pTaskData.TriggerTask = t.TriggerTask;
            }

            t=null;
        }

        #region 事件定义
        private Thread m_Thread;
        private readonly Object m_eventLock = new Object();

        /// 采集任务完成事件
        private event EventHandler<PublishCompletedEventArgs> e_PublishCompleted;
        internal event EventHandler<PublishCompletedEventArgs> PublishCompleted
        {
            add { lock (m_eventLock) { e_PublishCompleted += value; } }
            remove { lock (m_eventLock) { e_PublishCompleted -= value; } }
        }

        /// 采集任务采集失败事件
        private event EventHandler<PublishFailedEventArgs> e_PublishFailed;
        internal event EventHandler<PublishFailedEventArgs> PublishFailed
        {
            add { lock (m_eventLock) { e_PublishFailed += value; } }
            remove { lock (m_eventLock) { e_PublishFailed -= value; } }
        }

        /// 采集任务开始事件
        private event EventHandler<PublishStartedEventArgs> e_PublishStarted;
        internal event EventHandler<PublishStartedEventArgs> PublishStarted
        {
            add { lock (m_eventLock) { e_PublishStarted += value; } }
            remove { lock (m_eventLock) { e_PublishStarted -= value; } }
        }

        /// 采集任务错误事件
        private event EventHandler<PublishErrorEventArgs> e_PublishError;
        internal event EventHandler<PublishErrorEventArgs> PublishError
        {
            add { lock (m_eventLock) { e_PublishError += value; } }
            remove { lock (m_eventLock) { e_PublishError -= value; } }
        }

        /// 临时数据发布完成时间
        private event EventHandler<PublishTempDataCompletedEventArgs> e_PublishTempDataCompleted;
        internal event EventHandler<PublishTempDataCompletedEventArgs> PublishTempDataCompleted
        {
            add { lock (m_eventLock) { e_PublishTempDataCompleted += value; } }
            remove { lock (m_eventLock) { e_PublishTempDataCompleted -= value; } }
        }

        //发布日志事件
        private event EventHandler<PublishLogEventArgs> e_PublishLog;
        internal event EventHandler<PublishLogEventArgs> PublishLog
        {
            add { lock (m_eventLock) { e_PublishLog += value; } }
            remove { lock (m_eventLock) { e_PublishLog -= value; } }
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

        private readonly Object m_threadLock = new Object();

        #region 用于临时存储发布数据
        //此方法用于临时发布数据使用，即如果用户终端了操作
        //需调用此方法对已经采集的数据进行临时发布，当前
        //仅临时进行保存
        public void startSaveTempData()
        {
            lock (m_threadLock)
            {
                m_Thread = new Thread(this.SaveTempData);

                //定义线程名称,用于调试使用
                m_Thread.Name = FileName;

                m_Thread.Start();
            }
        }

        private readonly Object m_fileLock = new Object();

        private void SaveTempData()
        {
            //无论数据是否发布，都需要保存采集下来的数据
            //保存到本地磁盘
            try
            {
                if (File.Exists(m_pTaskData.FileName))
                {
                    lock (m_fileLock)
                    {
                        File.Delete(m_pTaskData.FileName);
                    }
                }

                m_pTaskData.PublishData.WriteXml(m_pTaskData.FileName, XmlWriteMode.WriteSchema);

                //触发临时数据发布成功事件
                e_PublishTempDataCompleted(this, new PublishTempDataCompletedEventArgs(this.TaskData.TaskID, this.TaskData.TaskName));
            }
            catch (System.Exception ex)
            {
                //存储临时数据时，有可能导致多个线程访问的失败操作，当前并没有
                //加锁控制，加锁控制在下一版提供
                if (e_PublishLog != null)
                {
                    WriteLog(ex.Message);
                    e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "任务：" + this.TaskData.TaskName + "临时存储失败，错误信息为：" + ex.Message + "\n"));

                }
                
                if (e_PublishError != null)
                {
                    e_PublishError(this, new PublishErrorEventArgs(this.TaskData.TaskID, this.TaskData.TaskName, ex));
                }

                
            }
        }
        #endregion

        //此方法用于采集任务完成后，数据的发布操作
        public void startPublic()
        {

            lock (m_threadLock)
            {
                m_Thread = new Thread(this.ThreadWorkInit);

                //定义线程名称,用于调试使用
                m_Thread.Name = FileName;

                m_Thread.Start();
            }
        }

        private void ThreadWorkInit()
        {
            //判断当前的数据文件是否存在

            //开始导出
            ThreadWork();
           
        }

        private void ThreadWork()
        {
            //无论数据是否发布，都需要保存采集下来的数据
            //保存到本地磁盘，此项工作由SaveTempData（）替代
            //这样做的目的是不触发消息事件，放到后台执行
            //if (File.Exists(m_pTaskData.FileName))
            //{
            //    File.Delete(m_pTaskData.FileName);
            //}
            //m_pTaskData.PublishData.WriteXml(m_pTaskData.FileName, XmlWriteMode.WriteSchema);

            try
            {
                ExportData();
            }
            catch (System.Exception ex)
            {
                e_PublishError(this, new PublishErrorEventArgs(this.TaskData.TaskID, this.TaskData.TaskName, ex));
            }
        }

        #region 导出数据 支持文本 Excel Access
        private bool ExportData()
        {
            //触发发布启动事件
            PublishStartedEventArgs evt = new PublishStartedEventArgs(this.TaskData.TaskID, this.TaskData.TaskName);
            e_PublishStarted(this,evt);

            switch (this.PublishType)
            {
                case cGlobalParas.PublishType.PublishAccess :
                    ExportAccess();
                    break;
                case cGlobalParas.PublishType.PublishExcel :
                    ExportExcel();
                    break;
                case cGlobalParas.PublishType.PublishTxt :
                    ExportTxt();
                    break;
                case cGlobalParas.PublishType.PublishMSSql :
                    ExportMSSql();
                    break;
                case cGlobalParas.PublishType.PublishMySql :
                    ExportMySql();
                    break;
                case cGlobalParas.PublishType.PublishWeb :
                    ExportWeb();
                    break;
                default :
                    break;
            }

            PublishCompletedEventArgs evt1 = new PublishCompletedEventArgs(this.TaskData.TaskID, this.TaskData.TaskName);
            
            e_PublishCompleted(this, evt1);
            
            TriggerRun();

            return true;
        }

        private bool ConnectSqlServer()
        {
            try
            {
                string strDataBase = "Server=.;DataBase=Library;Uid=" + m_pTaskData.DataUser + ";pwd=" + m_pTaskData.DataPwd  + ";";
                SqlConnection conn = new SqlConnection(strDataBase);
                conn.Open();
            }
            catch (System.Exception ex)
            {
                throw ex;
                
            }
            return true;
        }

        private string getCreateTablesql(cGlobalParas.DatabaseType dType ,string Encoding)
        {
            string strsql = "";

            strsql = "create table " + this.m_pTaskData.DataTableName + "(";
            for (int i=0;i<m_pTaskData.PublishData.Columns.Count ;i++)
            {
                switch (dType)
                {
                    case cGlobalParas.DatabaseType.Access:
                        strsql += m_pTaskData.PublishData.Columns[i].ColumnName + " " + "text" + ",";
                        break;
                    case cGlobalParas.DatabaseType.MSSqlServer:
                        strsql += m_pTaskData.PublishData.Columns[i].ColumnName + " " + "text" + ",";
                        break;
                    case cGlobalParas.DatabaseType.MySql:
                        strsql += m_pTaskData.PublishData.Columns[i].ColumnName + " " + "text" + ",";
                        break;
                    default:
                        strsql += m_pTaskData.PublishData.Columns[i].ColumnName + " " + "text" + ",";
                        break;
                }
            }
            strsql = strsql.Substring(0, strsql.Length - 1);
            strsql += ")";

            //如果是mysql数据库，需要根据连接串的字符集进行数据表的建立
            if (dType == cGlobalParas.DatabaseType.MySql)
            {
                if (Encoding == "" || Encoding == null)
                    Encoding = "utf8";

                strsql += " CHARACTER SET " + Encoding + " ";
            }

            return strsql;
        }

        private void ExportAccess()
        {
            bool IsTable = false;

            OleDbConnection conn = new OleDbConnection();

            string connectionstring = m_pTaskData.DataSource;

            //判断是否为新建表
            string tName = m_pTaskData.DataTableName;

            conn.ConnectionString = connectionstring;

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                if (e_PublishLog != null)
                {
                    WriteLog(ex.Message);
                    e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "任务：" + this.TaskData.TaskName + "发布Access失败，错误信息为：" + ex.Message + "\n"));
                }

                return;
            }

            System.Data.DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {
                if (r[3].ToString() == "TABLE")
                {
                    if (r[2].ToString() == tName)
                    {
                        IsTable = true;
                        break;
                    }
                }

            }

            if (IsTable == false)
            {
                //需要建立新表，建立新表的时候采用ado.net新建行的方式进行数据增加
                string CreateTablesql = getCreateTablesql(cGlobalParas.DatabaseType.Access,"" );

                OleDbCommand com = new OleDbCommand();
                com.Connection = conn;
                com.CommandText = CreateTablesql;
                com.CommandType = CommandType.Text;
                try
                {
                    int result = com.ExecuteNonQuery();
                }
                catch (System.Data.OleDb.OleDbException ex)
                {
                    if (ex.ErrorCode != -2147217900)
                    {
                        if (e_PublishLog != null)
                        {
                            WriteLog(ex.Message);
                            e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "任务：" + this.TaskData.TaskName + "发布MySql失败，错误信息为：" + ex.Message + "\n"));
                        }

                        throw ex;
                    }
                }

                try
                {

                    System.Data.OleDb.OleDbDataAdapter da = new System.Data.OleDb.OleDbDataAdapter("SELECT * FROM " + tName, conn);
                    System.Data.OleDb.OleDbCommandBuilder builder = new System.Data.OleDb.OleDbCommandBuilder(da);

                    DataSet ds = new DataSet();
                    da.Fill(ds, m_pTaskData.DataTableName);

                    for (int i = 0; i < m_pTaskData.PublishData.Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].NewRow();
                        for (int j = 0; j < m_pTaskData.PublishData.Columns.Count; j++)
                        {
                            dr[j] = m_pTaskData.PublishData.Rows[i][j].ToString();
                        }
                        ds.Tables[0].Rows.Add(dr);
                    }
                    int m = da.Update(ds.Tables[0]);
                }
                catch (System.Exception ex)
                {
                    if (e_PublishLog != null)
                    {
                        WriteLog(ex.Message);
                        e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "任务：" + this.TaskData.TaskName + "发布Access失败，错误信息为：" + ex.Message + "\n"));
                    }

                    return;
                }

            }
            else
            {
                try
                {
                    //无需建立新表，需要采用sql语句的方式进行，但需要替换sql语句中的内容
                    System.Data.OleDb.OleDbCommand cm = new System.Data.OleDb.OleDbCommand();
                    cm.Connection = conn;
                    cm.CommandType = CommandType.Text;

                    //开始拼sql语句
                    string sql = "";

                    for (int i = 0; i < m_pTaskData.PublishData.Rows.Count; i++)
                    {
                        sql = m_pTaskData.InsertSql;

                        for (int j = 0; j < m_pTaskData.PublishData.Columns.Count; j++)
                        {
                            string strPara = "{" + m_pTaskData.PublishData.Columns[j].ColumnName + "}";
                            //string strParaValue = m_pTaskData.PublishData.Rows[i][j].ToString();
                            string strParaValue = m_pTaskData.PublishData.Rows[i][j].ToString().Replace("\"", "\"\"");
                            sql = sql.Replace(strPara, strParaValue);
                        }

                        cm.CommandText = sql;
                        cm.ExecuteNonQuery();
                    }
                }
                catch (System.Exception ex)
                {
                    if (e_PublishLog != null)
                    {
                        WriteLog(ex.Message);
                        e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "任务：" + this.TaskData.TaskName + "发布Access失败，错误信息为：" + ex.Message + "\n"));
                    }

                    return;
                }

            }
           

            conn.Close();
        }

        private void ExportExcel()
        {
            string TaskName=m_pTaskData.TaskName ;
            string FileName = m_pTaskData.ExportFile;
            System.Data.DataTable gData= m_pTaskData.PublishData;

            //判断目录根据结果并创建
            cTool.CreateDirectory(FileName);
            
            // 定义要使用的Excel 组件接口
            // 定义Application 对象,此对象表示整个Excel 程序

            FileStream myStream = File.Open(FileName, FileMode.Create, FileAccess.Write, FileShare.Write);
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gb2312"));
            string str = "";
            string tempStr = "";
            int i = 0;
            int Count = 0;

            try
            {
                if (m_pTaskData.IsExportHeader == true)
                {
                    //写标题 
                    for (i = 0; i < gData.Columns.Count; i++)
                    {
                        str += "\t";
                        str += gData.Columns[i].ColumnName;
                    }

                    sw.WriteLine(str);
                }

                Count = gData.Rows.Count;
                //写内容 
                for (i = 0; i < gData.Rows.Count; i++)
                {
                    for (int j = 0; j < gData.Columns.Count; j++)
                    {

                        tempStr += "\t";
                        tempStr += gData.Rows[i][j];
                    }
                    sw.WriteLine(tempStr);
                    tempStr = "";

                }


                sw.Close();
                myStream.Close();

            }
            catch (Exception ex)
            {
                if (e_PublishLog != null)
                {
                    WriteLog(ex.Message);
                    e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "任务：" + this.TaskData.TaskName + "发布" + FileName + "失败，错误信息为：" + ex.Message + "\n"));
                }

                return;
            }
            finally
            {
                sw.Close();
                myStream.Close();

            }

            return ;
        }

        private void ExportTxt()
        {
            string TaskName = m_pTaskData.TaskName;
            string FileName = m_pTaskData.ExportFile;
            System.Data.DataTable gData = m_pTaskData.PublishData;

            //判断目录根据结果并创建
            cTool.CreateDirectory(FileName);


            FileStream myStream = File.Open(FileName, FileMode.Create, FileAccess.Write, FileShare.Write);
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gb2312"));
            string str = "";
            string tempStr = "";

            try
            {
                if (this.m_pTaskData.IsExportHeader == true)
                {
                    //写标题 
                    for (int i = 0; i < gData.Columns.Count; i++)
                    {
                        str += "\t";
                        str += gData.Columns[i].ColumnName;
                    }

                    sw.WriteLine(str);
                }

                //写内容 
                for (int i = 0; i < gData.Rows.Count; i++)
                {
                    for (int j = 0; j < gData.Columns.Count; j++)
                    {

                        tempStr += "\t";
                        tempStr += gData.Rows[i][j];
                    }
                    sw.WriteLine(tempStr);
                    tempStr = "";
                }


                sw.Close();
                myStream.Close();

            }
            catch (Exception ex)
            {
                if (e_PublishLog != null)
                {
                    WriteLog(ex.Message);
                    e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "任务：" + this.TaskData.TaskName + "发布" + FileName + "失败，错误信息为：" + ex.Message + "\n"));
                }

                return ;
            }
            finally
            {
                sw.Close();
                myStream.Close();
            }


            return ;

        }

        private void ExportMSSql()
        {
            bool IsTable = false;

            SqlConnection conn = new SqlConnection();

            string connectionstring = m_pTaskData.DataSource;

            //判断是否为新建表
            string tName = m_pTaskData.DataTableName;

            conn.ConnectionString = connectionstring;

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                if (e_PublishLog != null)
                {
                    WriteLog(ex.Message);
                    e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "任务：" + this.TaskData.TaskName + "发布MSSql失败，错误信息为：" + ex.Message + "\n"));
                }

                throw ex;
            }

            System.Data.DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {
                if (r[2].ToString()==tName )
                {
                    IsTable = true;
                    break;
                }
            }

            if (IsTable == false)
            {
                //需要建立新表，建立新表的时候采用ado.net新建行的方式进行数据增加
                string CreateTablesql = getCreateTablesql(cGlobalParas.DatabaseType.MSSqlServer,"");

                SqlCommand com = new SqlCommand();
                com.Connection = conn;
                com.CommandText = CreateTablesql;
                com.CommandType = CommandType.Text;
                try
                {
                    int result = com.ExecuteNonQuery();
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    if (ex.ErrorCode != -2147217900)
                    {
                        if (e_PublishLog != null)
                        {
                            WriteLog(ex.Message);
                            e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "任务：" + this.TaskData.TaskName + "发布MSSql失败，错误信息为：" + ex.Message + "\n"));
                        }

                        throw ex;
                    }
                }

            //    try
            //    {

            //        System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter("SELECT * FROM " + tName, conn);
            //        System.Data.SqlClient.SqlCommandBuilder builder = new System.Data.SqlClient.SqlCommandBuilder(da);

            //        DataSet ds = new DataSet();
            //        da.Fill(ds, m_pTaskData.DataTableName);

            //        for (int i = 0; i < m_pTaskData.PublishData.Rows.Count; i++)
            //        {
            //            DataRow dr = ds.Tables[0].NewRow();
            //            for (int j = 0; j < m_pTaskData.PublishData.Columns.Count; j++)
            //            {
            //                dr[j] = m_pTaskData.PublishData.Rows[i][j].ToString();
            //            }
            //            ds.Tables[0].Rows.Add(dr);
            //        }
            //        int m = da.Update(ds.Tables[0]);
            //    }
            //    catch (System.Exception ex)
            //    {
            //        if (e_PublishLog != null)
            //        {
            //            e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "任务：" + this.TaskData.TaskName + "发布MSSql失败，错误信息为：" + ex.Message + "\n"));
            //        }

            //        return;
            //    }

            }
            //else
            //{
                //无需建立新表，需要采用sql语句的方式进行，但需要替换sql语句中的内容
                SqlCommand cm = new SqlCommand();
                cm.Connection = conn;
                cm.CommandType = CommandType.Text;

                //开始拼sql语句
                string strInsertSql = m_pTaskData.InsertSql;

                //需要将双引号替换成单引号
                //strInsertSql = strInsertSql.Replace("\"", "'");

                string sql = "";

                for (int i = 0; i < m_pTaskData.PublishData.Rows.Count; i++)
                {
                    sql = strInsertSql;

                    for (int j = 0; j < m_pTaskData.PublishData.Columns.Count; j++)
                    {
                        string strPara = "{" + m_pTaskData.PublishData.Columns[j].ColumnName + "}";
                        //string strParaValue = m_pTaskData.PublishData.Rows[i][j].ToString();
                        string strParaValue = m_pTaskData.PublishData.Rows[i][j].ToString().Replace("\"", "\"\"");
                        sql = sql.Replace(strPara, strParaValue);
                    }
                    try
                    {
                        cm.CommandText = sql;
                        cm.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
                    {
                        if (e_PublishLog != null)
                        {
                            WriteLog(ex.Message);
                            e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "任务：" + this.TaskData.TaskName + "发布MSSql失败，Sql语句为：" + sql.ToString() + " 错误信息为：" + ex.Message + "\n"));
                        }

                    }
                }
            //}
            conn.Close();
        }

        private void ExportMySql()
        {
            bool IsTable = false;

            MySqlConnection conn = new MySqlConnection();

            string connectionstring = m_pTaskData.DataSource ;

            //判断是否为新建表
            string tName = m_pTaskData.DataTableName;

            conn.ConnectionString = connectionstring;

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                if (e_PublishLog != null)
                {
                    WriteLog(ex.Message);
                    e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "任务：" + this.TaskData.TaskName + "发布MySql失败，错误信息为：" + ex.Message + "\n"));
                }
                throw ex;
            }

            System.Data.DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {
                if (string.Compare (r[2].ToString (),tName ,true )==0)
                {
                    IsTable = true;
                    break;
                }
            }

            if (IsTable == false)
            {
                //通过连接字符串把编码获取出来，根据编码进行数据表的建立
                string strMatch = "(?<=character set=)[^\\s]*(?=[\\s;])";
                Match s = Regex.Match(connectionstring, strMatch, RegexOptions.IgnoreCase);
                string Encoding = s.Groups[0].Value;

                //需要建立新表，建立新表的时候采用ado.net新建行的方式进行数据增加
                string CreateTablesql = getCreateTablesql(cGlobalParas.DatabaseType.MySql,Encoding);

                MySqlCommand com = new MySqlCommand();
                com.Connection = conn;
                com.CommandText = CreateTablesql;
                com.CommandType = CommandType.Text;
                try
                {
                    int result = com.ExecuteNonQuery();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    if (ex.ErrorCode != -2147217900)
                    {
                        if (e_PublishLog != null)
                        {
                            WriteLog(ex.Message);
                            e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "任务：" + this.TaskData.TaskName + "发布MySql失败，错误信息为：" + ex.Message + "\n"));
                        }
                        throw ex;
                    }
                }
            }

            //无论是否为新建表，都采用这种方式进行数据表的添加
            

                //MySql.Data.MySqlClient.MySqlDataAdapter da = new MySql.Data.MySqlClient.MySqlDataAdapter("SELECT * FROM " + tName, conn);
                //MySql.Data.MySqlClient.MySqlCommandBuilder builder = new MySql.Data.MySqlClient.MySqlCommandBuilder(da);

                //DataSet ds = new DataSet();

                //da.Fill(ds, m_pTaskData.DataTableName);
                //int m = 0;

                //for ( int i = 0; i < m_pTaskData.PublishData.Rows.Count; i++)
                //{
                //    try
                //    {
                //        DataRow dr = ds.Tables[0].NewRow();
                //        for (int j = 0; j < m_pTaskData.PublishData.Columns.Count; j++)
                //        {
                //            dr[j] = m_pTaskData.PublishData.Rows[i][j].ToString();
                //        }
                //        ds.Tables[0].Rows.Add(dr);

                //        m = da.Update(ds.Tables[0]);
                //    }
                //    catch (System.Exception ex)
                //    {
                //        if (e_PublishLog != null)
                //        {
            //            e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "任务：" + this.TaskData.TaskName + "发布MySql失败，错误信息为：" + ex.Message + "\n"));
                //        }

                //        //throw ex ;
                //    }
                //}

            //无需建立新表，需要采用sql语句的方式进行，但需要替换sql语句中的内容
            MySqlCommand cm = new MySqlCommand();
            cm.Connection = conn;
            cm.CommandType = CommandType.Text;

            //开始拼sql语句
            string strInsertSql = m_pTaskData.InsertSql;

            //需要将双引号替换成单引号
            //strInsertSql = strInsertSql.Replace("\"", "'");


            string sql = "";

            

            for (int i = 0; i < m_pTaskData.PublishData.Rows.Count; i++)
            {
                sql = strInsertSql;

                for (int j = 0; j < m_pTaskData.PublishData.Columns.Count; j++)
                {
                    string strPara = "{" + m_pTaskData.PublishData.Columns[j].ColumnName + "}";
                    string strParaValue = m_pTaskData.PublishData.Rows[i][j].ToString().Replace ("\"","\"\"");
                    sql = sql.Replace(strPara, strParaValue);
                }

                try
                {
                    cm.CommandText = sql;
                    cm.ExecuteNonQuery();
                }
                catch (System.Exception ex)
                {
                    if (e_PublishLog != null)
                    {
                        WriteLog(ex.Message);
                        e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "任务：" + this.TaskData.TaskName + "发布MySql失败，Sql语句为：" + sql.ToString() + " 错误信息为：" + ex.Message + "\n"));
                    }

                }
            }
            
            conn.Close();
        }

        //web在线发布数据
        private void ExportWeb()
        {
            //开始循环发布数据
            //这是一个大循环
            for (int i = 0; i < m_pTaskData.PublishData.Rows.Count; i++)
            {
                string PostPara = "";
                string url = this.m_pTaskData.ExportUrl;

                CookieContainer CookieCon;

                HttpWebRequest wReq;


                CookieCon = new CookieContainer();

                if (Regex.IsMatch(url, @"<POST>.*</POST>", RegexOptions.IgnoreCase))
                {
                    wReq = (HttpWebRequest)WebRequest.Create(@url.Substring(0, url.IndexOf("<POST>")));
                }
                else
                {
                    wReq = (HttpWebRequest)WebRequest.Create(@url);
                }


                wReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.0; .NET CLR 1.1.4322; .NET CLR 2.0.50215;)";

                Match a = Regex.Match(url, @"(http://).[^/]*[?=/]", RegexOptions.IgnoreCase);
                string url1 = a.Groups[0].Value.ToString();
                wReq.Referer = url1;

                //判断是否有cookie
                string cookie = this.m_pTaskData.ExportCookie;
                if (cookie != "")
                {
                    CookieCollection cl = new CookieCollection();

                    foreach (string sc in cookie.Split(';'))
                    {
                        string ss = sc.Trim();
                        cl.Add(new Cookie(ss.Split('=')[0].Trim(), ss.Split('=')[1].Trim(), "/"));
                    }
                    CookieCon.Add(new Uri(url), cl);
                    wReq.CookieContainer = CookieCon;
                }

                //设置页面超时时间为8秒
                wReq.Timeout = 8000;


                //string ExportUrl=url;

                //替换Url地址中的参数
                for (int j = 0; j < m_pTaskData.PublishData.Columns.Count; j++)
                {
                    string strPara = "{" + m_pTaskData.PublishData.Columns[j].ColumnName + "}";
                    string strParaValue = m_pTaskData.PublishData.Rows[i][j].ToString();
                    url = url.Replace(strPara, strParaValue);
                }

                //判断是否需要进行Url编码
                if (int.Parse(this.m_pTaskData.ExportUrlCode) != (int)cGlobalParas.WebCode.NoCoding)
                {
                    url = cTool.UrlEncode(url, (cGlobalParas.WebCode)int.Parse(this.m_pTaskData.ExportUrlCode));
                }


                //开始发布数据，首先判断是否为POST方式进行数据发布
                //判断是否含有POST参数
                if (Regex.IsMatch(url, @"(?<=<POST>)[\S\s]*(?=</POST>)", RegexOptions.IgnoreCase))
                {

                    Match s = Regex.Match(url, @"(?<=<POST>).*(?=</POST>)", RegexOptions.IgnoreCase);
                    PostPara = s.Groups[0].Value.ToString();
                    byte[] pPara = Encoding.ASCII.GetBytes(PostPara);

                    wReq.ContentType = "application/x-www-form-urlencoded";
                    wReq.ContentLength = pPara.Length;

                    wReq.Method = "POST";

                    System.IO.Stream reqStream = wReq.GetRequestStream();
                    reqStream.Write(pPara, 0, pPara.Length);
                    reqStream.Close();

                }
                else
                {
                    wReq.Method = "GET";

                }

                HttpWebResponse wResp = (HttpWebResponse)wReq.GetResponse();
                System.IO.Stream respStream = wResp.GetResponseStream();

                System.IO.StreamReader reader;
                reader = new System.IO.StreamReader(respStream, Encoding.UTF8);
                string strWebData = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();

            }


        }

        #endregion

        //写错误日志
        private void WriteLog(string strMess)
        {

            //在此处理是否写入错误数据到日志的请求
            if (this.TaskData.IsErrorLog ==true)
            {
                cErrLog eLog = new cErrLog();
                eLog.WriteLog(this.TaskData.TaskName, cGlobalParas.LogType.PublishError, strMess);
                eLog = null;
            }
        }

        //处理触发器的执行事件
        private void TriggerRun()
        {
            //无论失败还是成功都要进行触发器的触发
            if (m_pTaskData.IsTrigger == true && m_pTaskData.TriggerType == ((int)cGlobalParas.TriggerType.PublishedRun).ToString())
            {
                cRunTask rt = new cRunTask();
                rt.RunSoukeyTaskEvent += this.onRunSoukeyTask;

                cTaskPlan p;

                for (int i = 0; i < m_pTaskData.TriggerTask.Count; i++)
                {
                    p = new cTaskPlan();

                    p.RunTaskType = m_pTaskData.TriggerTask[i].RunTaskType;
                    p.RunTaskName = m_pTaskData.TriggerTask[i].RunTaskName;
                    p.RunTaskPara = m_pTaskData.TriggerTask[i].RunTaskPara;

                    rt.AddTask(p);
                }

                rt.RunSoukeyTaskEvent -= this.onRunSoukeyTask;
                rt = null;

            }
        }

        private void onRunSoukeyTask(object sender, cRunTaskEventArgs e)
        {
            e_RunTask(this, new cRunTaskEventArgs(e.MessType, e.RunName, e.RunPara));
        }

      
    }
}
