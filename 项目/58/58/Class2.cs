using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

using System.Reflection;

using Microsoft.CSharp;
using System.Xml;
using System.Data;
using System.Collections;
namespace StringTool
{
    public delegate void TRFunc(IDataReader reader, object inparam, ref object outparam);

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




    }
}
