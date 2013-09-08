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
namespace StringTool
{
    public delegate void TRFunc(IDataReader reader,object inparam,ref object outparam);
    internal class GetAssembly
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
        public static string connectionString = "server= 120.194.12.194;User Id= root;password=bmtdb;Persist Security Info=True;port=3307;database=bmtdb;charset=gbk;";
        public static string ExtractStr(string resource, string name, string stas, string ends, int ids , bool restart , string separator)
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
        public static void CreateFile(string path )
        {
            if (!File.Exists(path))
            {
                FileInfo f = new FileInfo(path);
                FileStream wn = f.OpenWrite();
                wn.Close();
            }
        }
        public static void CreateFile(string content, bool isRecreate, string path )
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
        public static string GetUrltoHtml(string Url, string encode )
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
        public static string PostUrltoHtml(string url, string postString , string method )
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
        public static bool SqlExists(string queryString)
        {
            bool retval = false;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(queryString, connection);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                try
                {
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
        static void MyTRFunc(IDataReader reader,object inparam,ref object outparam)
        {
            List<object> Io=(List<object>)outparam;
            Type iT=(Type)inparam;
            Object instance= Activator.CreateInstance(iT);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                FieldInfo fiInstance = iT.GetField(reader.GetName(i));
                if(fiInstance!=null) fiInstance.SetValue(instance, reader[i].ToString());
                
           }
            Io.Add(instance);
        }
        public static object[] GetObjects(string queryString,Type t)
        {
            List<object> lobs=new List<object>();
            object lob = lobs;
            TraverReader(queryString,MyTRFunc,t,ref lob);
            return lobs.ToArray();
        }
        public static void TraverReader(string queryString,TRFunc trfunc,object inparam,ref object outparam)
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
            catch (Exception)
            {
            }
            return rls.ToArray();
        }
        public static string[] FormatSqlString(string table, string selectstr, string existcols, string intscols, string setscols,string delwhere)
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

    }
}
