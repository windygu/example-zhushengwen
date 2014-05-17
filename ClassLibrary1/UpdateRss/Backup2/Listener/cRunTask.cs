using System;
using System.Collections.Generic;
using System.Text;
using SoukeyNetget.Plan;
using System.Data;
using SoukeyNetget.Task;
using System.Data.OleDb;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace SoukeyNetget.Listener
{
    class cRunTask
    {
        
        Queue<cTaskPlan> m_runningTasks;

        public cRunTask()
        {
            m_runningTasks = new Queue<cTaskPlan>();
        }

        ~cRunTask()
        {
        }

        public void AddTask(cTaskPlan task)
        {
            m_runningTasks.Enqueue(task);
            
            RunTask();
        }

        //ִ������
        private void RunTask()
        {
            cPlanRunLog rLog = new cPlanRunLog();
            rLog.OpenLogFile();

            while (m_runningTasks.Count > 0)
            {
                cTaskPlan tp = m_runningTasks.Dequeue();

                switch (tp.RunTaskType)
                {
                    case (int)cGlobalParas.RunTaskType.SoukeyTask :
                        RunSoukeyTask(tp.RunTaskName);
                        break ;
                    case (int)cGlobalParas.RunTaskType.OtherTask :
                        RunOtherTask(tp.RunTaskName, tp.RunTaskPara);
                        break ;
                    case (int)cGlobalParas.RunTaskType.DataTask :
                        RunDataTask(tp.RunTaskName, tp.RunTaskPara);
                        break;
                }

                //д��־
                rLog.InsertLog(cGlobalParas.LogType.RunPlanTask,tp.PlanID ,tp.PlanName , (cGlobalParas.RunTaskType)tp.RunTaskType, tp.RunTaskName, tp.RunTaskPara);
               
            }

            rLog.CloseLogFile();
            rLog = null;

        }

        //�����Soukey��ժ��������Ҫ����Ϣ�����������棬������������
        //��ǰ֧����ʱ���������ڻ��ṩר������Soukey��������棬���Խ���
        //��̨����Soukey��ժ����

        private void RunDataTask(string DataType, string Para)
        {
            switch (int.Parse(DataType))
            {
                case (int)cGlobalParas.DatabaseType.Access :
                    ExecuteAccessQuery(Para);
                    break;
                case (int)cGlobalParas.DatabaseType.MSSqlServer:
                    ExecuteMSSqlQuery(Para);
                    break;
                case (int)cGlobalParas.DatabaseType.MySql:
                    ExecuteMySqlQuery(Para);
                    break;
                default :
                    break;
            }
        }

        private void ExecuteAccessQuery(string Para)
        {

            string strconn = Para.Substring(0, Para.IndexOf("Para"));
            string QueryName = Para.Substring(Para.IndexOf("Para=") + 5, Para.Length - Para.IndexOf("Para=") - 5);

            OleDbConnection conn = new OleDbConnection();
            conn.ConnectionString = strconn;

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            OleDbCommand com = new OleDbCommand();
            com.Connection = conn;
            com.CommandText = QueryName;
            com.CommandType = CommandType.StoredProcedure;
            try
            {
                int result = com.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            conn.Close();

           
        }

        private void ExecuteMSSqlQuery(string Para)
        {
            string strconn = Para.Substring(0, Para.IndexOf("Para"));
            string QueryName = Para.Substring(Para.IndexOf("Para=")+5, Para.Length - Para.IndexOf("Para=")-5);

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = strconn;

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            SqlCommand com = new SqlCommand();
            com.Connection = conn;
            com.CommandText = QueryName;
            com.CommandType = CommandType.StoredProcedure;
            try
            {
                int result = com.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            conn.Close();
        }

        private void ExecuteMySqlQuery(string Para)
        {

            string strconn = Para.Substring(0, Para.IndexOf("Para"));
            string QueryName = Para.Substring(Para.IndexOf("Para=") + 5, Para.Length - Para.IndexOf("Para=") - 5);

            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = strconn;

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                throw ex;
                
            }

            MySqlCommand com = new MySqlCommand();
            com.Connection = conn;
            com.CommandText = QueryName;
            com.CommandType = CommandType.StoredProcedure;
            try
            {
                int result = com.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            conn.Close();
          
        }

        private void RunSoukeyTask(string TaskName)
        {
            //��ȡTaskID
            //cTaskClass tClass = new cTaskClass();
            //string tClassPath=tClass.GetTaskClassPathByName();
            //tClass = null;

            //string tName = tClassPath + "\\" + TaskName.Substring(TaskName.IndexOf("\\"), TaskName.Length - TaskName.IndexOf("\\"));

            //cTask t = new cTask();
            //t.LoadTask(tName);
            //Int64 tID = t.TaskID;
            //t = null;

            //��������������������ִ�в���
            e_RunSoukeyTaskEvent(this, new cRunTaskEventArgs(cGlobalParas.MessageType.RunSoukeyTask, TaskName ,""));

        }

        //���������������ֱ������
        private void RunOtherTask(string FileName, string Para)
        {
             System.Diagnostics.Process.Start (FileName, Para);
        }

        private readonly Object m_eventLock = new Object();

        #region �¼�

        /// �ɼ���������¼�
        private event EventHandler<cRunTaskEventArgs> e_RunSoukeyTaskEvent;
        internal event EventHandler<cRunTaskEventArgs> RunSoukeyTaskEvent
        {
            add { lock (m_eventLock) { e_RunSoukeyTaskEvent += value; } }
            remove { lock (m_eventLock) { e_RunSoukeyTaskEvent -= value; } }
        }

        #endregion
    }
}
