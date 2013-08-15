using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using StringTool;
using MySql.Data.MySqlClient;
namespace ProcessNews
{
    public delegate void TRFunc(IDataReader reader);

    public partial class Form1 : Form
    {
        public static void TraverReader(string queryString, TRFunc trfunc)
        {
            using (MySqlConnection connection = new MySqlConnection(MyClass.connectionString))
            {
                MySqlCommand command = new MySqlCommand(queryString, connection);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        trfunc(reader);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(queryString);
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    reader.Close();
                }
            }
        }
        public Form1()
        {
            InitializeComponent();
        }
        void ProcessReader(IDataReader reader)
        {
            int i = int.Parse(count.Text);
            i++;
            count.Text = i.ToString();

            string content = reader["content"].ToString();
            string id = reader["id"].ToString();
            if(content.Contains('?'))
            content = content.Replace('?', ' ');

            string hrefs=MyClass.ExtractStr(content, "href", "<", ">", 1000, true);
            foreach (string item in hrefs.Split(','))
            {
                if(item.StartsWith("a ") || item.StartsWith("A "))
                content=content.Replace("<"+item+">", "<a>");
            }
            //string srcs = MyClass.ExtractStr(content, "href", "<", ">", 1000, true);
            //foreach (string item in srcs.Split(','))
            //{
            //    if (item.StartsWith("<img ") || item.StartsWith("<IMG "))
            //    content.Replace(item, "<img>");
            //}
            string sql = "UPDATE data_xp_news SET content='{1}' WHERE  id={0}";
            content = content.Replace("'", "''");
            sql = string.Format(sql, id, content);
            MyClass.ExecuteNonQuery(sql);

            label2.Text = reader["title"].ToString();
            label4.Text=reader["type_id"].ToString();
            progressBar1.PerformStep();
        }
        void ProcessTask()
        {
            totle.Text = MyClass.ExecuteScalar("SELECT count(*) from data_xp_news").ToString();
            progressBar1.Maximum = int.Parse(totle.Text);
            TraverReader("SELECT * from data_xp_news", ProcessReader);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(ProcessTask);
            Control.CheckForIllegalCrossThreadCalls = false;
            progressBar1.Value = 0;
            count.Text = "0";
            t.Start();
        }
    }
}
