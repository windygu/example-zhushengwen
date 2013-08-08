using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.IO;
using System.Xml;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Web.Services.Description;
using Microsoft.CSharp;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            while (true)
            {
                ClientAction ca = new ClientAction();
                List<string> scuss = new List<string>();
                foreach (string item in ca.GetAdList())
                {
                    string url = "http://" + Util.RemoteIP + ":" + Util.RemotePort + "/" + Util.Image_Dir + "/" + item;
                    if(Util.SaveImage(url))scuss.Add(item);
                }
                //ca.AdsUpdateRespond(scuss.ToArray());
                Thread.Sleep(60 * 1000);
            }
        }
    }
    public class ClientAction
    {
        public ClientAction()
        {
            Util.InitialSetting();
        }
        
        public string[] GetAdList()
        {
            string url = "http://" + Util.RemoteIP + ":" + Util.RemotePort + "/" + Util.Service_Dir + "/BMTService.asmx";
            string[] args = new string[] {Util.TerminalName};
            object result = WSHelper.InvokeWebService(url, "GetAdsList", args);
            string[] imgs = (string[])result;
            return imgs;
        }
        public void AdsUpdateRespond(string[] filelist)
        {
            string url = "http://" + Util.RemoteIP + ":" + Util.RemotePort + "/" + Util.Service_Dir + "/BMTService.asmx";
            object[] args = new object[] { Util.TerminalName,filelist };
            object result = WSHelper.InvokeWebService(url, "AdsUpdateRespond", args);
        }
        
    }
    public class Util
    {
        public static readonly string path = "D:\\Setting\\";

        public static string TerminalName = "";
        public static string RemoteIP = "";
        public static string RemotePort = "80";
        public static string Service_Dir = "BMTService";
        public static string Image_Dir = "ad_img";
        public static void InitialSetting()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path + "setting.xml");
            foreach (XmlNode item in xmlDoc.GetElementsByTagName("setting"))
            {
                if (item["name"].InnerText == "TerminalName")
                {
                    TerminalName = item["url"].InnerText;
                }
                if (item["name"].InnerText == "RemoteIP")
                {
                    RemoteIP = item["url"].InnerText;
                }
                if (item["name"].InnerText == "RemotePort")
                {
                    RemotePort = item["url"].InnerText;
                }
            }
        }
        public static void CheckDirectory()
        {
            DirectoryInfo di = new DirectoryInfo(path);
            if (!di.Exists)
            {
                Directory.CreateDirectory(path);
            }
            Console.Write(di.FullName);
        }
        public static bool SaveImage(string imgUrl)
        {
            CheckDirectory();
            if (path.Equals("")) return false;
            string imgName = imgUrl.ToString().Substring(imgUrl.ToString().LastIndexOf("/") + 1);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(imgUrl);
            request.UserAgent = "Mozilla/6.0 (MSIE 6.0; Windows NT 5.1; Natas.Robot)";
            request.Timeout = 3000;
            try
            {
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();

                if (response.ContentType.ToLower().StartsWith("image/"))
                {
                    byte[] arrayByte = new byte[1024];
                    int imgLong = (int)response.ContentLength;
                    int l = 0;

                    FileStream fso = new FileStream(path + imgName, FileMode.Create);
                    while (l < imgLong)
                    {
                        int i = stream.Read(arrayByte, 0, 1024);
                        fso.Write(arrayByte, 0, i);
                        l += i;
                    }

                    fso.Close();
                    stream.Close();
                    response.Close();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                
                return false;
            }
           
            
        }
    }
    public class WSHelper
    {
        /// < summary>
        /// 动态调用web服务
        /// < /summary>
        /// < param name="url">WSDL服务地址< /param>
        /// < param name="methodname">方法名< /param>
        /// < param name="args">参数< /param>
        /// < returns>< /returns>
        public static object InvokeWebService(string url, string methodname, object[] args)
        {
            return WSHelper.InvokeWebService(url, null, methodname, args);
        }
        /// < summary>
        /// 动态调用web服务
        /// < /summary>
        /// < param name="url">WSDL服务地址< /param>
        /// < param name="classname">类名< /param>
        /// < param name="methodname">方法名< /param>
        /// < param name="args">参数< /param>
        /// < returns>< /returns>
        public static object InvokeWebService(string url, string classname, string methodname, object[] args)
        {
            string @namespace = "EnterpriseServerBase.WebService.DynamicWebCalling";
            if ((classname == null) || (classname == ""))
            {
                classname = WSHelper.GetWsClassName(url);
            }
            try
            { //获取WSDL
                WebClient wc = new WebClient();
                Stream stream = wc.OpenRead(url + "?wsdl");
                ServiceDescription sd = ServiceDescription.Read(stream);
                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");
                CodeNamespace cn = new CodeNamespace(@namespace);
                //生成客户端代理类代码
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);
                CSharpCodeProvider icc = new CSharpCodeProvider();
                //设定编译参数
                CompilerParameters cplist = new CompilerParameters();
                cplist.GenerateExecutable = false;
                cplist.GenerateInMemory = true;
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");
                //编译代理类
                CompilerResults cr = icc.CompileAssemblyFromDom(cplist, ccu);
                if (true == cr.Errors.HasErrors)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(System.Environment.NewLine);
                    }
                    throw new Exception(sb.ToString());
                }
                //生成代理实例，并调用方法
                System.Reflection.Assembly assembly = cr.CompiledAssembly;
                Type t = assembly.GetType(@namespace + "." + classname, true, true);
                object obj = Activator.CreateInstance(t);
                System.Reflection.MethodInfo mi = t.GetMethod(methodname);
                return mi.Invoke(obj, args);
                // PropertyInfo propertyInfo = type.GetProperty(propertyname);
                //return propertyInfo.GetValue(obj, null);
            }
            catch (Exception ex)
            {
                return new List<string>(new string[] {  });
                //throw new Exception(ex.InnerException.Message, new Exception(ex.InnerException.StackTrace));
            }
        }
        private static string GetWsClassName(string wsUrl)
        {
            string[] parts = wsUrl.Split('/');
            string[] pps = parts[parts.Length - 1].Split('.');
            return pps[0];
        }
    }

}
