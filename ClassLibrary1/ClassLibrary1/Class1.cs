using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using MySql.Data.MySqlClient;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Web.Services.Description;
using Microsoft.CSharp;
using System.Xml;
using System.Data;
using System.Collections;
using System.Data.Common;
namespace StringTool
{
    public delegate void TRFunc(IDataReader reader, object inparam, ref object outparam);
    public class GetAssembly
    {

        /// <summary>  
        /// 根据代码获取一个Assembly  
        /// </summary>  
        /// <param name="Code">代码区域 包含USING</param>  
        /// <param name="UsingList">需要引用的DLL</param>         
        /// <returns>返回Assembly</returns>  
        public static Assembly GetCodeAssembly(string p_Code, IList<string> p_UsingList)
        {
            CodeDomProvider _CodeDom = new CSharpCodeProvider();
            CompilerParameters _CodeParamertes = new CompilerParameters();

            for (int i = 0; i != p_UsingList.Count; i++)
            {
                _CodeParamertes.ReferencedAssemblies.Add(p_UsingList[i].ToString());   //("System.dll");  
            }
            _CodeParamertes.GenerateExecutable = false;
            _CodeParamertes.GenerateInMemory = true;

            CompilerResults _CompilerResults = _CodeDom.CompileAssemblyFromSource(_CodeParamertes, p_Code);

            if (_CompilerResults.Errors.HasErrors)
            {
                string _ErrorText = "";
                foreach (CompilerError _Error in _CompilerResults.Errors)
                {
                    _ErrorText += _Error.ErrorText + "/r/n";
                }
                throw new Exception(_ErrorText);
            }
            else
            {
                return _CompilerResults.CompiledAssembly;
            }
        }

        /// <summary>  
        /// 根据WEBSERVICE地址获取一个 Assembly  
        /// </summary>  
        /// <param name="p_Url">地址</param>  
        /// <param name="p_NameSpace">命名空间</param>  
        /// <returns>返回Assembly</returns>  
        public static Assembly GetWebServiceAssembly(string p_Url, string p_NameSpace)
        {
            try
            {
                System.Net.WebClient _WebClient = new System.Net.WebClient();



                System.IO.Stream _WebStream = _WebClient.OpenRead(p_Url);

                ServiceDescription _ServiceDescription = ServiceDescription.Read(_WebStream);

                _WebStream.Close();
                _WebClient.Dispose();
                ServiceDescriptionImporter _ServiceDescroptImporter = new ServiceDescriptionImporter();
                _ServiceDescroptImporter.AddServiceDescription(_ServiceDescription, "", "");
                System.CodeDom.CodeNamespace _CodeNameSpace = new System.CodeDom.CodeNamespace(p_NameSpace);
                System.CodeDom.CodeCompileUnit _CodeCompileUnit = new System.CodeDom.CodeCompileUnit();
                _CodeCompileUnit.Namespaces.Add(_CodeNameSpace);
                _ServiceDescroptImporter.Import(_CodeNameSpace, _CodeCompileUnit);

                System.CodeDom.Compiler.CodeDomProvider _CodeDom = new Microsoft.CSharp.CSharpCodeProvider();
                System.CodeDom.Compiler.CompilerParameters _CodeParameters = new System.CodeDom.Compiler.CompilerParameters();
                _CodeParameters.GenerateExecutable = false;
                _CodeParameters.GenerateInMemory = true;
                _CodeParameters.ReferencedAssemblies.Add("System.dll");
                _CodeParameters.ReferencedAssemblies.Add("System.XML.dll");
                _CodeParameters.ReferencedAssemblies.Add("System.Web.Services.dll");
                _CodeParameters.ReferencedAssemblies.Add("System.Data.dll");

                System.CodeDom.Compiler.CompilerResults _CompilerResults = _CodeDom.CompileAssemblyFromDom(_CodeParameters, _CodeCompileUnit);

                if (_CompilerResults.Errors.HasErrors)
                {
                    string _ErrorText = "";
                    foreach (CompilerError _Error in _CompilerResults.Errors)
                    {
                        _ErrorText += _Error.ErrorText + "/r/n";
                    }
                    throw new Exception(_ErrorText);
                }

                return _CompilerResults.CompiledAssembly;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
    public class MyClass
    {
        public static string GetNowTime()
        {
            return string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
        }
        public static string FormateTime(DateTime dt)
        {
            return string.Format("{0:yyyy-MM-dd HH:mm:ss}", dt);
        }
        public static long GetTick()
        {
            DateTime start = new DateTime(1970, 1, 1);
            TimeSpan duration = DateTime.Now - start;
            long sec = duration.Seconds + duration.Minutes * 60 + duration.Hours * 3600 + duration.Days * 86400 - 8 * 60 * 60;
            return sec;
        }
        public static DateTime GetDateTimeFromTick(long sec)
        {
            return new DateTime(1970, 1, 1).AddSeconds(sec + 8 * 60 * 60);
        }
        public static string connectionString = "";
        public static string ExtractStr(string resource, string name, string stas, string ends, int ids = 1, bool restart = false, string separator = ",")
        {
            string str = "";
            int index = 0;
            //首先定位到名称
            while (ids != 0)
            {
                ids--;
                int bgn = resource.IndexOf(name, index);
                //如果未找到直接返回
                if (bgn != -1)
                {
                    //再次定位到开始字符
                    int sta = 0;
                    if (restart)
                    {
                        sta = resource.LastIndexOf(stas, bgn + name.Length);
                    }
                    else
                    {
                        sta = resource.IndexOf(stas, bgn + name.Length);
                    }
                    if (sta != -1)
                    {
                        //建立栈结构,开始字符和结束字符分别进行压栈出栈
                        int i = 1;
                        sta += stas.Length - 1;
                        index = sta + 1;
                        string tmps = "";
                        while (0 != i && index < resource.Length)
                        {

                            if (index + ends.Length > resource.Length) break;
                            tmps = resource.Substring(index, ends.Length);
                            if (tmps == ends)
                            {
                                i--;
                                if (0 == i) break;
                                index++;
                                continue;
                            }
                            if (index + stas.Length > resource.Length) break;
                            tmps = resource.Substring(index, stas.Length);
                            if (tmps == stas)
                            {
                                i++;
                            }
                            index++;
                        }
                        if (0 == i && index <= resource.Length)
                        {
                            if (str != "") str += separator;
                            str += resource.Substring(sta + 1, index - sta - 1);
                        }
                    }
                }
            }
            return str;
        }

        public static void CreateFile(string path = "temp")
        {
            if (!File.Exists(path))
            {
                FileInfo f = new FileInfo(path);
                FileStream wn = f.OpenWrite();
                wn.Close();
            }
        }
        public static void CreateFile(string content, bool isRecreate = false, string path = "temp")
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
        public static string ReadTextFile(string path)
        {
            string filetext = "";
            if (File.Exists(path))
            {
                FileInfo f = new FileInfo(path);
                StreamReader wn = f.OpenText();
                filetext = wn.ReadToEnd();
                wn.Close();
            }
            return filetext;
        }
        public static string GetUrltoHtml(string Url, string encode = "utf-8")
        {
            StringBuilder content = new StringBuilder();
            try
            {
                // 与指定URL创建HTTP请求
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";
                request.Method = "GET";
                request.Accept = "*/*";

                //不保持连接
                request.KeepAlive = true;
                // 获取对应HTTP请求的响应
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                // 获取响应流
                Stream responseStream = response.GetResponseStream();
                // 对接响应流(以"GBK"字符集)
                //StreamReader sReader = new StreamReader(responseStream, Encoding.GetEncoding("gb2312"));
                StreamReader sReader = new StreamReader(responseStream, Encoding.GetEncoding(encode));
                // 开始读取数据
                Char[] sReaderBuffer = new Char[256];
                int count = sReader.Read(sReaderBuffer, 0, 256);
                while (count > 0)
                {
                    String tempStr = new String(sReaderBuffer, 0, count);
                    content.Append(tempStr);
                    count = sReader.Read(sReaderBuffer, 0, 256);
                }
                // 读取结束
                sReader.Close();

            }
            catch (Exception e)
            {
                return e.Message;
            }
            return content.ToString();
        }
        public static string PostUrltoHtml(string url, string postString = "", string method = "POST")
        {
            string srcString = "";
            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                byte[] postData = Encoding.Default.GetBytes(postString);
                byte[] responseData = webClient.UploadData(url, method, postData);
                srcString = Encoding.Default.GetString(responseData);
            }
            catch (Exception e)
            {

                return e.Message;
            }
            return srcString;

        }
        public static bool ExecuteNonQuery(string queryString)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(queryString);
                command.Connection = connection;
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(queryString);
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
        public static int[] StrCount(string AllStr, string DivStr)
        {
            int i = AllStr.IndexOf(DivStr);
            List<int> iCount = new List<int>();
            while (i != -1)
            {
                iCount.Add(i);
                i = AllStr.IndexOf(DivStr, i + DivStr.Length);
            }
            return iCount.ToArray();
        }

        public static bool SqlExists(string queryString)
        {
            bool retval = false;
            MySqlDataReader reader = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    MySqlCommand command = new MySqlCommand(queryString, connection);
                    connection.Open();
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        retval = true;
                        break;
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
            return retval;
        }
        public static object ExecuteScalar(string queryString)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(queryString);
                command.Connection = connection;
                try
                {
                    connection.Open();
                    return command.ExecuteScalar(); ;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(queryString);
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }
        static void MyTRFunc(IDataReader reader, object inparam, ref object outparam)
        {
            List<object> Io = (List<object>)outparam;
            Type iT = (Type)inparam;
            Object instance = Activator.CreateInstance(iT);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                FieldInfo fiInstance = iT.GetField(reader.GetName(i));
                if (fiInstance != null) fiInstance.SetValue(instance, reader[i].ToString());

            }
            Io.Add(instance);
        }
        public static object[] GetObjects(string queryString, Type t)
        {
            List<object> lobs = new List<object>();
            object lob = lobs;
            TraverReader(queryString, MyTRFunc, t, ref lob);
            return lobs.ToArray();
        }
        public static void TraverReader(string queryString, TRFunc trfunc, object inparam, ref object outparam)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(queryString, connection);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        trfunc(reader, inparam, ref outparam);
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
        public static string[] SelectArray(string queryString)
        {
            List<string> rts = new List<string>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(queryString, connection);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        rts.Add(reader[0].ToString());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(queryString);
                    Console.WriteLine(ex.Message);
                    return null;
                }
                finally
                {
                    reader.Close();
                }
            }
            return rts.ToArray();
        }
        public static object DynamicInvokeWebService(string url, string method, object[] obja)
        {
            Assembly _WebServiceAssembly = GetAssembly.GetWebServiceAssembly(url + "?WSDL", "WebService");
            Type _Type = _WebServiceAssembly.GetTypes()[0];
            object _ObjectW = Activator.CreateInstance(_Type);
            MethodInfo _Method = _Type.GetMethod(method);
            try
            {
                return _Method.Invoke(_ObjectW, obja);

            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public static XmlNodeList ReadXml(string path, string xpath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            return xmlDoc.DocumentElement.SelectNodes(xpath);
        }
        public static string[] GetTableRecords(string table)
        {
            string queryString = "SHOW COLUMNS FROM " + table;
            List<string> rls = new List<string>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    MySqlCommand command = new MySqlCommand(queryString, connection);
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            rls.Add(reader[0].ToString());
                        }
                    }
                    finally
                    {
                        // always call Close when done reading.
                        reader.Close();
                        if (connection.State != ConnectionState.Closed)
                        {
                            connection.Close();
                        }

                    }
                }
            }
            catch (Exception e)
            {
            }
            return rls.ToArray();
        }
        public static string[] FormatSqlString(string table, string selectstr, string existcols, string intscols, string setscols, string delwhere = "")
        {
            string[] Sqls = new string[4];
            string[] records = GetTableRecords(table);

            string sels = "";
            if (selectstr == "*" || selectstr == "") sels = "*";
            else
            {
                string[] selcols = selectstr.Split(',');
                foreach (string item in selcols)
                {
                    if (sels != "") sels += ",";
                    sels += string.Format(item.Trim('\''), records);
                }
            }

            string where = "";
            if (existcols != "")
            {
                string[] exicols = existcols.Split(',');

                foreach (string item in exicols)
                {
                    if (where != "") where += " AND ";
                    where += string.Format(item.Trim('\''), records);
                    where += "=";
                    where += item;
                }
            }
            string sql = "SELECT " + sels + " FROM " + table;
            if (where != "") sql += " WHERE " + where;
            Sqls[0] = sql;

            sql = "";
            if (intscols != "")
            {
                string[] intcols = intscols.Split(',');
                string val = "";
                foreach (string item in intcols)
                {
                    if (sql != "")
                    {
                        sql += ",";
                        val += ",";
                    }
                    sql += string.Format(item.Trim('\''), records);
                    val += item;
                }

                sql = "INSERT INTO " + table + "(" + sql + ")";
                sql += "VALUES (" + val + ")";
            }
            Sqls[1] = sql;

            sql = "";
            if (setscols != "")
            {
                string[] setcols = setscols.Split(',');
                foreach (string item in setcols)
                {
                    if (sql != "")
                    {
                        sql += ",";
                    }
                    sql += string.Format(item.Trim('\''), records);
                    sql += "=";
                    sql += item;
                }
                sql = "UPDATE " + table + " SET " + sql;
                if (where != "") sql += " WHERE  " + where;
            }
            Sqls[2] = sql;

            where = "";
            if (delwhere != "")
            {
                string[] delwcols = delwhere.Split(',');

                foreach (string item in delwcols)
                {
                    if (where != "") where += " AND ";
                    where += string.Format(item.Trim('\''), records);
                    where += "=";
                    where += item;
                }
            }

            sql = "DELETE FROM " + table;
            if (where != "") sql += " WHERE  " + where;
            Sqls[3] = sql;

            return Sqls;
        }
        public string ReadMyDataBySql(string queryString)
        {
            string str = "`";
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    MySqlCommand command = new MySqlCommand(queryString, connection);
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

                    try
                    {
                        int seq = 1;
                        while (reader.Read())
                        {
                            str += "{";
                            str += "\"";
                            str += "NEW_SEQ";
                            str += "\":\"";
                            str += seq;
                            str += "\"";
                            seq++;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                //Type t = reader.GetFieldType(i);
                                //object o = Activator.CreateInstance(t);
                                //o = reader[i];
                                str += ",";
                                str += "\"";
                                str += reader.GetName(i);
                                str += "\":\"";
                                str += reader[i].ToString().Replace("\"", "\\\"");
                                str += "\"";
                                //[{"sss":"ddd",},{},{},]

                                //if (reader.FieldCount - 1 != i) += "\"";
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
                        if (connection.State != ConnectionState.Closed)
                        {
                            connection.Close();


                        }

                    }
                }
            }
            catch (Exception e)
            {

                str += "{";
                str += "\"Exception\":";
                str += "\"" + e.Message + "\"";
                str += "}";


            }
            str += "`";
            return str;
        }
        public string ReadMyData(string TableName, int PageNum = 0, int Count = 10, string where = "", bool whereIsAdded = false, string extraRecord = "", string orderBy = "", string seq = "", string colms = "*")
        {
            if (!whereIsAdded)
                if (where != "") where = "where " + where;
            where = " " + where;
            string selstr = "SELECT " + colms;
            if (extraRecord != "") selstr += "," + extraRecord;
            selstr += " FROM ";
            string queryString = selstr + TableName + where;
            if (orderBy != "")
            {
                queryString += " ORDER BY " + orderBy;
                if (seq != "")
                    queryString += " " + seq;
            }
            queryString += " LIMIT " + (PageNum * Count) + "," + Count;
            return ReadMyDataBySql(queryString);
        }

        public static bool Test(string regstr)
        {
            StringBuilder _Code = new StringBuilder();
            _Code.AppendLine("using System;");
            _Code.AppendLine("namespace TestName");
            _Code.AppendLine("{");
            _Code.AppendLine("public class TestClass");
            _Code.AppendLine("{");
            _Code.AppendLine("public bool Test()");
            _Code.AppendLine("{");
            _Code.AppendLine("return " + regstr + ";");
            _Code.AppendLine("}");
            _Code.AppendLine("}");
            _Code.AppendLine("}");

            IList<string> _List = new List<string>();
            _List.Add("System.dll");

            Assembly _Assembly = GetAssembly.GetCodeAssembly(_Code.ToString(), _List);
            Type _Class = _Assembly.GetType("TestName.TestClass");
            MethodInfo _Method = _Class.GetMethod("Test");

            object _Object = Activator.CreateInstance(_Class);
            object _Returun = _Method.Invoke(_Object, new object[] { });
            return (bool)_Returun;
        }
        public static T[] Select<T>(IEnumerable input, string property, string dest, string eq = "=")
        {
            List<T> lobs = new List<T>();
            if (input != null)
                foreach (T item in input)
                {
                    Type type = item.GetType();//获取类型
                    PropertyInfo propertyInfo = type.GetProperty(property);
                    string value = propertyInfo.GetValue(item, null).ToString().Trim();
                    switch (eq)
                    {
                        case "<":
                            if (Int32.Parse(value) < Int32.Parse(dest))
                            {
                                lobs.Add(item);
                            }
                            break;
                        case ">":
                            if (Int32.Parse(value) > Int32.Parse(dest))
                            {
                                lobs.Add(item);
                            }
                            break;
                        default:
                            if (value == dest)
                            {
                                lobs.Add(item);
                            }
                            break;
                    }
                }
            return lobs.ToArray();
        }
    }
    public class GetObj<T> where T : new()
    {
        static void MyTRFunc(IDataReader reader, object inparam, ref object outparam)
        {
            List<T> Io = (List<T>)outparam;

            T instance = new T();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                FieldInfo fiInstance = instance.GetType().GetField(reader.GetName(i));
                if (fiInstance != null) fiInstance.SetValue(instance, reader[i].ToString());

            }
            Io.Add(instance);
        }
        public static T[] GetObjects(string queryString)
        {
            List<T> lobs = new List<T>();
            object lob = lobs;
            MyClass.TraverReader(queryString, MyTRFunc, null, ref lob);
            return lobs.ToArray();
        }



    }
    public class ProcessBlock
    {
        public static int[][] Process(string str)
        {
            int I = 0;
            int L = str.Length;
            int LB = 1;
            bool F = true;
            bool FF = true;
            List<int[]> LIS = new List<int[]>();
            List<string> Comm = new List<string>();
            int[] ICAA = null;
            while (I + LB < L)
            {
                string B = str.Substring(I, LB);
                int[] ICA = MyClass.StrCount(str, B);
                if (ICA.Length > 1)
                {
                    if (F) F = false;
                    LB++;
                    ICAA = ICA;
                }
                else if (F) I++;
                else
                {
                    LIS.Add(ICAA);
                    Comm.Add(B.Substring(0, B.Length - 1));
                    if (FF) FF = false;
                    break;
                }
            }
            I = 1;
            LB = 1;
            while (true)
            {
                string B = str.Substring(LIS[0][0] + I, LB);
                while (I < LIS[0].Length)
                {
                    int index = str.IndexOf(B, LIS[0][I]);
                    if (index == -1) break;
                    if (I < LIS[0].Length - 1)
                        if (index > LIS[0][I + 1]) break;
                    I++;
                }
                if (I == LIS[0].Length - 1)
                {
                    //匹配成功
                }
                else
                {

                }

            }

            return null;
        }
    }

    public class Query<T> where T : new()
    {

        static void MyTRFunc(IDataReader reader, object inparam, ref object outparam)
        {
            List<T> Io = (List<T>)outparam;

            T instance = new T();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                FieldInfo fiInstance = instance.GetType().GetField(reader.GetName(i));
                if (fiInstance != null) fiInstance.SetValue(instance, reader[i].ToString());

            }
            Io.Add(instance);
        }
        public static int Count(string where = "", string col_name = "")
        {
            string sql = "select count(*) from {0} {1}";
            if (col_name != "")
            {
                where = "where " + col_name + "='" + where + "'";
            }
            else if (where != "") where = "where " + where;
            sql = string.Format(sql, new T().GetType().Name, where);
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sql);
                command.Connection = connection;
                try
                {
                    connection.Open();
                    return (int)command.ExecuteScalar(); ;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(sql);
                    Console.WriteLine(ex.Message);
                    return 0;
                }
            }
        }
        public static T[] GetAll(bool desc = false)
        {
            return GetSome("select * from " + new T().GetType().Name + (desc ? (" order by id desc") : ""));
        }
        public static T[] Select(string where = "")
        {
            return GetSome("select * from " + new T().GetType().Name + (where == "" ? "" : " where " + where));
        }
        public static T SelectOne(string where = "")
        {
            return FetchOne("select * from " + new T().GetType().Name + (where == "" ? "" : " where " + where));
        }
        public static T[] SelectObj(string name, string value)
        {
            string where = "`" + name + "`" + "='" + value.Replace("'", "''").Replace("\\", "\\\\") + "'";
            return Select(where);
        }
        public static T SelectObjOne(string name, string value)
        {
            string where = "`" + name + "`" + "='" + value.Replace("'", "''").Replace("\\", "\\\\") + "'";
            return SelectOne(where);
        }
        /// <summary>
        /// 根据字符串查询结果，GetSome的别名
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static T[] ExecuteQuery(string queryString)
        {
            return GetSome(queryString);
        }
        public static T[] GetSome(string queryString)
        {
            List<T> lobs = new List<T>();
            object lob = lobs;
            TraverReader(queryString, MyTRFunc, null, ref lob);
            return lobs.ToArray();
        }
        public static T FetchOne(string sql)
        {
            T[] obs = GetSome(sql);
            if (obs.Length != 0)
                return obs[0];
            else return default(T);
        }
        public static T GetOne(Object o)
        {
            string val = GetObjId(o);
            if (val == "") return default(T);

            string sql = "select * from {0} where id={1}";

            if (val == "" || val == "0") return default(T);
            sql = string.Format(sql, new T().GetType().Name, val);

            return FetchOne(sql);
        }
        public static T GetAnyOne()
        {
            string rid = (new Random(DateTime.Now.Millisecond).Next(Count()) + 1).ToString();

            string sql = "select  * from (select * from {0} limit 1) order by id desc limit 1";
            sql = string.Format(sql, new T().GetType().Name, rid);
            return FetchOne(sql);
        }
        public static string GetObjId(Object o)
        {
            string val = "";
            if (o is int)
            {
                val = o.ToString();
            }
            else if (o is string)
            {
                try
                {
                    int i = int.Parse(o.ToString());
                    val = i.ToString();
                }
                catch (Exception)
                {

                }
            }
            else if (o is T)
            {
                val = o.GetType().GetField("id").GetValue(o).ToString();
            }
            return val;
        }
        public static T GetNext(Object o, bool N2F = false, string where = "")
        {
            string val = GetObjId(o);

            if (val == "" || val == "0") return default(T);
            if (where != "") where = "and (" + where + ")";
            string sql = "select * from {0} where id>{1} {2} order by id limit 1";
            sql = string.Format(sql, new T().GetType().Name, val, where);
            T t = FetchOne(sql);
            if (t == null && N2F) t = GetFirst();
            return t;
        }
        public static T GetLast(string where = "")
        {
            if (where != "") where = "where " + where;
            string sql = "select  * from {0} {1} order by id desc limit 1";
            sql = string.Format(sql, new T().GetType().Name, where);
            return FetchOne(sql);
        }
        public static int Index(Object o)
        {
            string val = GetObjId(o);
            if (val == "" || val == "0") return 0;
            return Count("id <=" + val);

        }
        public static T GetFirst(string where = "")
        {
            if (where != "") where = "where " + where;
            string sql = "select  * from {0} {1} order by id limit 1";
            sql = string.Format(sql, new T().GetType().Name, where);
            return FetchOne(sql);
        }

        public static bool InsertObj(T o, string ExColNameList = "")
        {
            FieldInfo[] fis = new T().GetType().GetFields();
            string ns = "";
            string vs = "";
            //iT.Name
            string sql = "insert into {0} ({1})values({2})";
            List<string> cls = new List<string>(ExColNameList.Split(','));
            foreach (FieldInfo item in fis)
            {
                object val = new T().GetType().GetField(item.Name).GetValue(o);
                if (val == null) val = "";
                if (val.ToString() == "") continue;
                if (item.Name == "id" || cls.Contains(item.Name)) continue;
                if (ns != "") ns += ",";
                ns += "`" + item.Name + "`";
                if (vs != "") vs += ",";
                vs += "'";

                vs += val.ToString().Replace("'", "''").Replace("\\", "\\\\");
                vs += "'";
            }
            sql = string.Format(sql, new T().GetType().Name, ns, vs);
            return ExecuteNonQuery(sql);
        }
        public static bool Update(string id, string name, string value)
        {
            string sql = "update {0} set `{1}`='{2}' where id={3}";
            sql = string.Format(sql, new T().GetType().Name, name, value, id);
            return ExecuteNonQuery(sql);
        }
        public static bool UpdateObj(T o, string ColNameList = "")
        {

            FieldInfo[] fis = new T().GetType().GetFields();
            string ss = "";
            //iT.Name
            string sql = "update {0} set {1} where id=";
            foreach (FieldInfo item in fis)
            {
                string val = "";
                object val1 = new T().GetType().GetField(item.Name).GetValue(o);
                if (val1 != null) val = val1.ToString();

                if (item.Name == "id")
                {
                    if (val == "" || val == "0") return false;
                    sql += val;
                    continue;
                }
                List<string> cls = new List<string>(ColNameList.Split(','));
                if (ColNameList == "" || cls.Contains(item.Name))
                {
                    if (ss != "") ss += ",";
                    ss += "`" + item.Name + "`";
                    ss += "=";

                    ss += "'";
                    ss += val.Replace("'", "''").Replace("\\", "\\\\");
                    ss += "'";
                }
            }
            sql = string.Format(sql, new T().GetType().Name, ss);
            return ExecuteNonQuery(sql);
        }
        public static bool UpdateBatch(string sets, string where="")
        {
            if (where != "") where = "where "+where;
            string sql = "update {0} set {1} {2}";
            sql = string.Format(sql, new T().GetType().Name,sets, where);
            return ExecuteNonQuery(sql);
        }

        public static bool DelObj(object o)
        {
            string val = "";

            if (o is int)
            {
                val = o.ToString();
            }
            else if (o is string && o != null)
            {
                val = o.ToString();
                try
                {
                    foreach (string item in val.Split(','))
                    {
                        int i = int.Parse(item);
                    }


                }
                catch (Exception)
                {
                    return false;
                }
            }
            else if (o is T)
            {
                val = "=" + o.GetType().GetProperty("id").GetValue(o, null).ToString();
            }
            string sql = "delete from {0} where id in ({1})";
            if (val == "" || val == "0") return false;
            sql = string.Format(sql, new T().GetType().Name, val);
            return ExecuteNonQuery(sql);

        }

        public static bool ExecuteNonQuery(string queryString)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(queryString);
                command.Connection = connection;
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(queryString);
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
        public static void TraverReader(string queryString, TRFunc trfunc, object inparam, ref object outparam)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(queryString, connection);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        trfunc(reader, inparam, ref outparam);
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
        public static string connectionString
        {
            get
            {
                return MyClass.connectionString;
            }
        }

    }


}
