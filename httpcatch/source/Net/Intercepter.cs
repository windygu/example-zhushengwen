using System;
using System.Collections.Generic;

using System.Text;
using System.IO;

namespace JrIntercepter.Net
{
    internal class Intercepter
    {
        internal delegate void DelegateUpdateSession(Session session);
        internal static event DelegateUpdateSession OnUpdateSession;
        internal static Dictionary<string, string> dr = new Dictionary<string, string>();
        internal static string  url="baidu.com";
        public static void CreateFile(string content, bool isRecreate = true, string path = "temp.txt")
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

        internal static void UpdateSession(Session session)
        {
            if (OnUpdateSession != null)
            {
                OnUpdateSession(session);  
            }
        }
    }
}
