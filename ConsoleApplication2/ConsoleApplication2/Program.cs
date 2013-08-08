using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.IO;
using Microsoft.Office.Interop.Word;
using Word = Microsoft.Office.Interop.Word; 
namespace ConsoleApplication2
{

      class Program
    {
        static void Main(string[] args)
        {
            MyClass.WriteFile("\r\n", true);
            Test();
            
      /*      MyClass.WriteFile("\r\n",true);
            foreach (string item in ReadMyData(""))
            {

               // string str = " [WebMethod] \r\n  public string BM_" + item + "(string name,int PageNum = 0, int Count = 10)   \r\n { \r\n string where = \"name LIKE '%\"+name+\"%'\";\r\n return ReadMyData(\"" + item + "\",  PageNum,  Count ,where ); \r\n}\r\n";
               // MyClass.WriteFile(str);
            }
        */
        }

        public static string Test()
        {
            string str = "";
            foreach (string item in ReadMyData(""))
            {
                 str += " [WebMethod] \n  public string Read_" + item + "_Data(int PageNum = 0, int Count = 10)   \n { return ReadMyData(\"" + item + "\",  PageNum,  Count ); }\n";
            }
            return str;
        }

        public static string connectionString = "server= 120.194.12.194;User Id= root;password=bmtdb;Persist Security Info=True;port=3307;database=bmtdb;charset=gbk;";

        public static object[] ReadMyData(string TableName, int PageNum = 0, int Count = 10)
        {
            string queryString = "show tables";//"SELECT * FROM " + TableName + " LIMIT " + (PageNum * Count) + "," + Count;
            List<object> imgs = new List<object>();
            string str = "[";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(queryString, connection);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                try
                {
                    List<string> ls = new List<string>();
                    while (reader.Read())
                    {
                        ls = new List<string>(ReadTabCols(reader[0].ToString()));
                        str += "{";
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            //Type t = reader.GetFieldType(i);
                            //object o = Activator.CreateInstance(t);
                            //o = reader[i];
                            
                            str += "\"";
                            str += reader.GetName(i);
                            str += "\":\"";
                            str += reader[i];
                            str += "\",";
                            //[{"sss":"ddd",},{},{},]
                            imgs.Add(reader[i]);
                          //  if (reader[i].ToString() == "data_gasstation")
                            if (ls.Contains("city"))
                            {


                                string str1 = ConsoleApplication2.Properties.Resources.String1;
                                //str1 = string.Format(str1, reader[i], reader[i]);
                                str1=str1.Replace("{0}", reader[i].ToString());
                                MyClass.WriteFile(str1);
                            }
                            
                            //Console.WriteLine(reader[i]);
                        }
                        str += "},";
                    }
                   str=str.TrimEnd(',');

                }
                finally
                {
                    // always call Close when done reading.
                    reader.Close();

                }
            }
            str += "]";
          //  Console.WriteLine(str);
            return imgs.ToArray();
        }
        public static string[] ReadTabCols(string TableName)
        {
            string queryString = @"SELECT  COLUMN_NAME
FROM  `information_schema`.`COLUMNS`
where `TABLE_SCHEMA`='bmtdb'  and  `TABLE_NAME`='"+TableName+@"' 
order by COLUMN_NAME;";
    
    //"SELECT * FROM " + TableName + " LIMIT " + (PageNum * Count) + "," + Count;
            List<string> imgs = new List<string>();
            string str = "[";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(queryString, connection);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        str += "{";
                        for (int i = 0; i < reader.FieldCount; i++)
                        {

                            str += "\"";
                            str += reader.GetName(i);
                            str += "\":\"";
                            str += reader[i];
                            str += "\",";
                            //[{"sss":"ddd",},{},{},]
                            imgs.Add(reader[i].ToString());
                            //Console.WriteLine(reader[i]);
                        }
                        str += "},";
                    }
                    str = str.TrimEnd(',');

                }
                finally
                {
                    // always call Close when done reading.
                    reader.Close();

                }
            }
            str += "]";
            //  Console.WriteLine(str);
            return imgs.ToArray();
        }
        public static string ReadMyData1(string TableName, int PageNum = 0, int Count = 10, string where = "")
        {
            if (where != "") where = " where " + where;
            string queryString = "SELECT * FROM " + TableName + where + " LIMIT " + (PageNum * Count) + "," + Count;
            string str = "[";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(queryString, connection);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        str += "{";
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            //Type t = reader.GetFieldType(i);
                            //object o = Activator.CreateInstance(t);
                            //o = reader[i];

                            str += "\"";
                            str += reader.GetName(i);
                            str += "\":\"";
                            str += reader[i];
                            str += "\"";
                            //[{"sss":"ddd",},{},{},]

                            if (reader.FieldCount - 1 != i) str += ",";
                            //Console.WriteLine(reader[i]);
                        }
                        str += "},";
                    }
                    str = str.TrimEnd(',');

                }
                finally
                {
                    // always call Close when done reading.
                    reader.Close();

                }
            }
            str += "]";

            return str;
        }

    }


    public class MyClass
    {
        public static string path = AppDomain.CurrentDomain.BaseDirectory+"ex.txt";
        public static string GetNowTime()
        {
            return string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
        }
        public static void CreateFile()
        {
            if (!File.Exists(path))
            {
                FileInfo f = new FileInfo(path);
                FileStream wn = f.OpenWrite();
                wn.Close();
            }
        }
        public static void CreateFile(string content, bool isRecreate)
        {
            if (File.Exists(path))
            {
                if (!isRecreate)
                {
                    FileInfo f = new FileInfo(path);
                    try
                    {
                        StreamWriter wn = f.AppendText();
                        wn.WriteLine(content);
                        wn.Close();

                    }
                    catch (Exception)
                    {
                    }
                    return;
                }
                else
                {
                    try
                    {
                        File.Delete(path);
                    }
                    catch (Exception)
                    {

                        return;
                    }
                }
            }
            
            FileInfo f1 = new FileInfo(path);
            FileStream wn1 = f1.Create();
            byte[] b = System.Text.Encoding.Default.GetBytes(content);
            wn1.Write(b, 0, b.Length);
            wn1.Close();

        }

        public static void WriteFile(string content,bool isRe=false)//string path,
        {
            CreateFile(content,isRe);
        }
    }
}