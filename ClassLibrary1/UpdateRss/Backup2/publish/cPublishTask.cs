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

///���ܣ���������
///���ʱ�䣺2009-7-21
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.01.00
///�޶��������˷������֧�����ݿ⡢web���ļ�����
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

            //��ʼ����������
            LoadTaskInfo(TaskID, dData);

        }

        ~cPublishTask()
        {

        }

        #region ����

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

        //�����ṩ��taskid����������Ϣ
        //���ݲ�Ӧ���Ǵ�����,�Ƕ�ȡ�ļ���,�����ڲ�֧��������,���Դ�����
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
            //��Ҫ����Ļ��ߵ��������ݻ��Ǵ��룬��Ϊ��Ҫ��ʱ���ݵı���
            //��һ����Ҫ����ʱ���ݱ���ͷ������ݽ��з���
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

        #region �¼�����
        private Thread m_Thread;
        private readonly Object m_eventLock = new Object();

        /// �ɼ���������¼�
        private event EventHandler<PublishCompletedEventArgs> e_PublishCompleted;
        internal event EventHandler<PublishCompletedEventArgs> PublishCompleted
        {
            add { lock (m_eventLock) { e_PublishCompleted += value; } }
            remove { lock (m_eventLock) { e_PublishCompleted -= value; } }
        }

        /// �ɼ�����ɼ�ʧ���¼�
        private event EventHandler<PublishFailedEventArgs> e_PublishFailed;
        internal event EventHandler<PublishFailedEventArgs> PublishFailed
        {
            add { lock (m_eventLock) { e_PublishFailed += value; } }
            remove { lock (m_eventLock) { e_PublishFailed -= value; } }
        }

        /// �ɼ�����ʼ�¼�
        private event EventHandler<PublishStartedEventArgs> e_PublishStarted;
        internal event EventHandler<PublishStartedEventArgs> PublishStarted
        {
            add { lock (m_eventLock) { e_PublishStarted += value; } }
            remove { lock (m_eventLock) { e_PublishStarted -= value; } }
        }

        /// �ɼ���������¼�
        private event EventHandler<PublishErrorEventArgs> e_PublishError;
        internal event EventHandler<PublishErrorEventArgs> PublishError
        {
            add { lock (m_eventLock) { e_PublishError += value; } }
            remove { lock (m_eventLock) { e_PublishError -= value; } }
        }

        /// ��ʱ���ݷ������ʱ��
        private event EventHandler<PublishTempDataCompletedEventArgs> e_PublishTempDataCompleted;
        internal event EventHandler<PublishTempDataCompletedEventArgs> PublishTempDataCompleted
        {
            add { lock (m_eventLock) { e_PublishTempDataCompleted += value; } }
            remove { lock (m_eventLock) { e_PublishTempDataCompleted -= value; } }
        }

        //������־�¼�
        private event EventHandler<PublishLogEventArgs> e_PublishLog;
        internal event EventHandler<PublishLogEventArgs> PublishLog
        {
            add { lock (m_eventLock) { e_PublishLog += value; } }
            remove { lock (m_eventLock) { e_PublishLog -= value; } }
        }

        /// <summary>
        /// ����һ��ִ��Soukey��ժ������¼���������Ӧ������ִ��Soukey��ժ����ʱ
        /// �Ĵ���
        /// </summary>
        private event EventHandler<cRunTaskEventArgs> e_RunTask;
        internal event EventHandler<cRunTaskEventArgs> RunTask
        {
            add { lock (m_eventLock) { e_RunTask += value; } }
            remove { lock (m_eventLock) { e_RunTask -= value; } }
        }
        #endregion

        private readonly Object m_threadLock = new Object();

        #region ������ʱ�洢��������
        //�˷���������ʱ��������ʹ�ã�������û��ն��˲���
        //����ô˷������Ѿ��ɼ������ݽ�����ʱ��������ǰ
        //����ʱ���б���
        public void startSaveTempData()
        {
            lock (m_threadLock)
            {
                m_Thread = new Thread(this.SaveTempData);

                //�����߳�����,���ڵ���ʹ��
                m_Thread.Name = FileName;

                m_Thread.Start();
            }
        }

        private readonly Object m_fileLock = new Object();

        private void SaveTempData()
        {
            //���������Ƿ񷢲�������Ҫ����ɼ�����������
            //���浽���ش���
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

                //������ʱ���ݷ����ɹ��¼�
                e_PublishTempDataCompleted(this, new PublishTempDataCompletedEventArgs(this.TaskData.TaskID, this.TaskData.TaskName));
            }
            catch (System.Exception ex)
            {
                //�洢��ʱ����ʱ���п��ܵ��¶���̷߳��ʵ�ʧ�ܲ�������ǰ��û��
                //�������ƣ�������������һ���ṩ
                if (e_PublishLog != null)
                {
                    WriteLog(ex.Message);
                    e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "����" + this.TaskData.TaskName + "��ʱ�洢ʧ�ܣ�������ϢΪ��" + ex.Message + "\n"));

                }
                
                if (e_PublishError != null)
                {
                    e_PublishError(this, new PublishErrorEventArgs(this.TaskData.TaskID, this.TaskData.TaskName, ex));
                }

                
            }
        }
        #endregion

        //�˷������ڲɼ�������ɺ����ݵķ�������
        public void startPublic()
        {

            lock (m_threadLock)
            {
                m_Thread = new Thread(this.ThreadWorkInit);

                //�����߳�����,���ڵ���ʹ��
                m_Thread.Name = FileName;

                m_Thread.Start();
            }
        }

        private void ThreadWorkInit()
        {
            //�жϵ�ǰ�������ļ��Ƿ����

            //��ʼ����
            ThreadWork();
           
        }

        private void ThreadWork()
        {
            //���������Ƿ񷢲�������Ҫ����ɼ�����������
            //���浽���ش��̣��������SaveTempData�������
            //��������Ŀ���ǲ�������Ϣ�¼����ŵ���ִ̨��
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

        #region �������� ֧���ı� Excel Access
        private bool ExportData()
        {
            //�������������¼�
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

            //�����mysql���ݿ⣬��Ҫ�������Ӵ����ַ����������ݱ�Ľ���
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

            //�ж��Ƿ�Ϊ�½���
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
                    e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "����" + this.TaskData.TaskName + "����Accessʧ�ܣ�������ϢΪ��" + ex.Message + "\n"));
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
                //��Ҫ�����±������±��ʱ�����ado.net�½��еķ�ʽ������������
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
                            e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "����" + this.TaskData.TaskName + "����MySqlʧ�ܣ�������ϢΪ��" + ex.Message + "\n"));
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
                        e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "����" + this.TaskData.TaskName + "����Accessʧ�ܣ�������ϢΪ��" + ex.Message + "\n"));
                    }

                    return;
                }

            }
            else
            {
                try
                {
                    //���轨���±���Ҫ����sql���ķ�ʽ���У�����Ҫ�滻sql����е�����
                    System.Data.OleDb.OleDbCommand cm = new System.Data.OleDb.OleDbCommand();
                    cm.Connection = conn;
                    cm.CommandType = CommandType.Text;

                    //��ʼƴsql���
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
                        e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "����" + this.TaskData.TaskName + "����Accessʧ�ܣ�������ϢΪ��" + ex.Message + "\n"));
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

            //�ж�Ŀ¼���ݽ��������
            cTool.CreateDirectory(FileName);
            
            // ����Ҫʹ�õ�Excel ����ӿ�
            // ����Application ����,�˶����ʾ����Excel ����

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
                    //д���� 
                    for (i = 0; i < gData.Columns.Count; i++)
                    {
                        str += "\t";
                        str += gData.Columns[i].ColumnName;
                    }

                    sw.WriteLine(str);
                }

                Count = gData.Rows.Count;
                //д���� 
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
                    e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "����" + this.TaskData.TaskName + "����" + FileName + "ʧ�ܣ�������ϢΪ��" + ex.Message + "\n"));
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

            //�ж�Ŀ¼���ݽ��������
            cTool.CreateDirectory(FileName);


            FileStream myStream = File.Open(FileName, FileMode.Create, FileAccess.Write, FileShare.Write);
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gb2312"));
            string str = "";
            string tempStr = "";

            try
            {
                if (this.m_pTaskData.IsExportHeader == true)
                {
                    //д���� 
                    for (int i = 0; i < gData.Columns.Count; i++)
                    {
                        str += "\t";
                        str += gData.Columns[i].ColumnName;
                    }

                    sw.WriteLine(str);
                }

                //д���� 
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
                    e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "����" + this.TaskData.TaskName + "����" + FileName + "ʧ�ܣ�������ϢΪ��" + ex.Message + "\n"));
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

            //�ж��Ƿ�Ϊ�½���
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
                    e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "����" + this.TaskData.TaskName + "����MSSqlʧ�ܣ�������ϢΪ��" + ex.Message + "\n"));
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
                //��Ҫ�����±������±��ʱ�����ado.net�½��еķ�ʽ������������
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
                            e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "����" + this.TaskData.TaskName + "����MSSqlʧ�ܣ�������ϢΪ��" + ex.Message + "\n"));
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
            //            e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "����" + this.TaskData.TaskName + "����MSSqlʧ�ܣ�������ϢΪ��" + ex.Message + "\n"));
            //        }

            //        return;
            //    }

            }
            //else
            //{
                //���轨���±���Ҫ����sql���ķ�ʽ���У�����Ҫ�滻sql����е�����
                SqlCommand cm = new SqlCommand();
                cm.Connection = conn;
                cm.CommandType = CommandType.Text;

                //��ʼƴsql���
                string strInsertSql = m_pTaskData.InsertSql;

                //��Ҫ��˫�����滻�ɵ�����
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
                            e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "����" + this.TaskData.TaskName + "����MSSqlʧ�ܣ�Sql���Ϊ��" + sql.ToString() + " ������ϢΪ��" + ex.Message + "\n"));
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

            //�ж��Ƿ�Ϊ�½���
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
                    e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "����" + this.TaskData.TaskName + "����MySqlʧ�ܣ�������ϢΪ��" + ex.Message + "\n"));
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
                //ͨ�������ַ����ѱ����ȡ���������ݱ���������ݱ�Ľ���
                string strMatch = "(?<=character set=)[^\\s]*(?=[\\s;])";
                Match s = Regex.Match(connectionstring, strMatch, RegexOptions.IgnoreCase);
                string Encoding = s.Groups[0].Value;

                //��Ҫ�����±������±��ʱ�����ado.net�½��еķ�ʽ������������
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
                            e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "����" + this.TaskData.TaskName + "����MySqlʧ�ܣ�������ϢΪ��" + ex.Message + "\n"));
                        }
                        throw ex;
                    }
                }
            }

            //�����Ƿ�Ϊ�½������������ַ�ʽ�������ݱ�����
            

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
            //            e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "����" + this.TaskData.TaskName + "����MySqlʧ�ܣ�������ϢΪ��" + ex.Message + "\n"));
                //        }

                //        //throw ex ;
                //    }
                //}

            //���轨���±���Ҫ����sql���ķ�ʽ���У�����Ҫ�滻sql����е�����
            MySqlCommand cm = new MySqlCommand();
            cm.Connection = conn;
            cm.CommandType = CommandType.Text;

            //��ʼƴsql���
            string strInsertSql = m_pTaskData.InsertSql;

            //��Ҫ��˫�����滻�ɵ�����
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
                        e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, ((int)cGlobalParas.LogType.Error).ToString() + "����" + this.TaskData.TaskName + "����MySqlʧ�ܣ�Sql���Ϊ��" + sql.ToString() + " ������ϢΪ��" + ex.Message + "\n"));
                    }

                }
            }
            
            conn.Close();
        }

        //web���߷�������
        private void ExportWeb()
        {
            //��ʼѭ����������
            //����һ����ѭ��
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

                //�ж��Ƿ���cookie
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

                //����ҳ�泬ʱʱ��Ϊ8��
                wReq.Timeout = 8000;


                //string ExportUrl=url;

                //�滻Url��ַ�еĲ���
                for (int j = 0; j < m_pTaskData.PublishData.Columns.Count; j++)
                {
                    string strPara = "{" + m_pTaskData.PublishData.Columns[j].ColumnName + "}";
                    string strParaValue = m_pTaskData.PublishData.Rows[i][j].ToString();
                    url = url.Replace(strPara, strParaValue);
                }

                //�ж��Ƿ���Ҫ����Url����
                if (int.Parse(this.m_pTaskData.ExportUrlCode) != (int)cGlobalParas.WebCode.NoCoding)
                {
                    url = cTool.UrlEncode(url, (cGlobalParas.WebCode)int.Parse(this.m_pTaskData.ExportUrlCode));
                }


                //��ʼ�������ݣ������ж��Ƿ�ΪPOST��ʽ�������ݷ���
                //�ж��Ƿ���POST����
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

        //д������־
        private void WriteLog(string strMess)
        {

            //�ڴ˴����Ƿ�д��������ݵ���־������
            if (this.TaskData.IsErrorLog ==true)
            {
                cErrLog eLog = new cErrLog();
                eLog.WriteLog(this.TaskData.TaskName, cGlobalParas.LogType.PublishError, strMess);
                eLog = null;
            }
        }

        //����������ִ���¼�
        private void TriggerRun()
        {
            //����ʧ�ܻ��ǳɹ���Ҫ���д������Ĵ���
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
