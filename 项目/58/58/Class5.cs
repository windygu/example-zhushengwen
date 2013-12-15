using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Drawing;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.IO.Compression;
using StringTool;
using System.Data.OleDb;
using System.Data;
using System.Reflection;
namespace _58
{


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
        public static int Count(string where="",string col_name="")
        {
            string sql = "select count(*) from {0} {1}";
            if (col_name != "")
            {
                where = "where "+col_name +"='" + where +"'";
            }
            else if (where != "") where = "where " + where;
            sql = string.Format(sql, new T().GetType().Name, where);
             using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(sql);
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
        public static T[] GetAll(bool desc=false)
        {
            return GetSome("select * from " + new T().GetType().Name+(desc?(" order by id desc"):""));
        }
        public static T[] GetSome(string queryString)
        {
            List<T> lobs = new List<T>();
            object lob = lobs;
            TraverReader(queryString, MyTRFunc, null, ref lob);
            return lobs.ToArray();
        }
        public static T GetOne(Object o)
        {
            string val = GetObjId(o);
            if (val == "") return default(T);

            string sql = "select * from {0} where id={1}";
            
            if (val == "" || val == "0") return default(T);
            sql = string.Format(sql, new T().GetType().Name, val);
            T[] obs=GetSome(sql);
            if (obs.Length != 0)
                return obs[0];
            else return default(T);
        }
        public static T GetAnyOne()
        {
            string sql = "select * from {0} order by id desc";
            sql = string.Format(sql, new T().GetType().Name);
            T[] obs = GetSome(sql);
            if (obs.Length != 0)
                return obs[0];
            else return default(T);
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
                val = new T().GetType().GetProperty("id").GetValue(o, null).ToString();
            }
            return val;
        }
        public static T GetNext(Object o)
        {
            string val = GetObjId(o);
            
            if(val=="")  return default(T);
             
            string sql = "select * from {0} where id>{1} order by id";

            if (val == "" || val == "0") return default(T);
            sql = string.Format(sql, new T().GetType().Name,val);
            T[] obs = GetSome(sql);
            if (obs.Length != 0)
                return obs[0];
            else return default(T);
        }
        public static T GetFirst()
        {
            return GetAnyOne();
        }
        public static bool InsertObj(T o, string ExColNameList="")
        {
            FieldInfo[] fis = new T().GetType().GetFields();
            string ns = "";
            string vs = "";
            //iT.Name
            string sql = "insert into {0} ({1})values({2})";
            List<string> cls = new List<string>(ExColNameList.Split(','));
            foreach (FieldInfo item in fis)
            {
                if (item.Name == "id" || cls.Contains(item.Name)) continue;
                if(ns!="")ns+=",";
                ns += "[" + item.Name + "]";

                 if(vs!="")vs+=",";
                  vs += "'";
                 object val = new T().GetType().GetField(item.Name).GetValue(o);
                 if(val==null)val="";
                 vs += val.ToString().Replace("'","''");
                  vs += "'";
            }
            sql = string.Format(sql, new T().GetType().Name, ns, vs);
            return ExecuteNonQuery(sql);
        }
        public static bool UpdateObj(T o,string ColNameList="")
        {

            FieldInfo[] fis = new T().GetType().GetFields();
            string ss = "";
            //iT.Name
            string sql = "update {0} set {1} where id=";
            foreach (FieldInfo item in fis)
            {
                string val = "";
                object val1 = new T().GetType().GetField(item.Name).GetValue(o);
                if (val1 != null) val =  val1.ToString();
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
                    ss += "[" + item.Name + "]";
                    ss += "=";

                    ss += "'";
                    ss += val.Replace("'", "''");
                    ss += "'";
                }
            }
            sql = string.Format(sql, new T().GetType().Name, ss);
            return ExecuteNonQuery(sql);
        }
      
        public static bool DelObj(object o)
        {
            string val = "";

            if (o is int)
            {
                val =  o.ToString();
            }
            else if (o is string && o !=null)
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
                val = "=" + new T().GetType().GetProperty("id").GetValue(o, null).ToString();
            }
            string sql = "delete from {0} where id in ({1})";
            if (val == "" || val == "0") return false;
            sql = string.Format(sql, new T().GetType().Name, val);
            return ExecuteNonQuery(sql);

        } 

        public static bool ExecuteNonQuery(string queryString)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(queryString);
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
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(queryString, connection);
                connection.Open();
                OleDbDataReader reader = command.ExecuteReader();

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
        static string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "58.mdb"); //相对路径

    }
  
    public class Company
    {
        public string id="0";
        public string name = "百度";
        public string nick = "百度";
        public string profile = "百度（Nasdaq简称：BIDU）是全球最大的中文搜索引擎，2000年1月由李彦宏、徐勇两人创立于北京中关村，致力于向人们提供“简单，可依赖”的信息获取方式。“百度”二字源于中国宋朝词人辛弃疾的《青玉案·元夕》词句“众里寻他千百度”，象征着百度对中文信息检索技术的执著追求。";
        public string people = "李彦宏";
        public string phone = "13888888888";
        public string area = "北京 崇明";
        public string address = "北京祥和城天明路38号东100米";
    }
    public class Account
    {
        public string id = "0";
        public string user = "";
        public string pass = "";
        public string email = "";
    }
   public class Job
    {
        public string id="0";
        public string name = "网络兼职生";
        public string cat = "图片处理";
        public string num = "30";
        public string time = "ALL";
        public string tp = "ALL";
        public string salary = "100";
        public string jiesuan = "1";
        public string profile = "所以不能问PS能制作什么样子的图，而是问哪种类型的图不适合用PS来制作（比如图表、数据表格、数据走势图、各种平面结构或施工图纸……这类不适合用PS制作）";
        public string people = "李彦宏";
        public string phone = "13888888888";
        public string email = "shoujianren@126.com";
        public string area = "北京 崇明";
        public string address = "北京祥和城天明路38号东100米";

    }
   public class City
   {
       public string id="0";
       public string name = "郑州";
       public string tel = "0371-4864523";
       public string cityid = "0";
   }
   public class ProxyIP
   {
      public string id = "0";
      public string ip = "";
      public string port = "";
      public string user = "";
      public string pass = "";
   }
    /// <summary>
    /// 支持 Session 和 Cookie 的 WebClient。
    /// </summary>
    public class HttpClient : WebClient
    {
        // Cookie 容器
        private CookieContainer cookieContainer;

        /**/
        /// <summary>
        /// 创建一个新的 WebClient 实例。
        /// </summary>
        public HttpClient(ref CookieContainer c)
        {
            this.cookieContainer = c;
            
        }


        void ProCc(string url)
        {
            if (!string.IsNullOrEmpty(www58com))
            {
                cookieContainer.Add(new Uri(url), new Cookie("www58com", "\"" + www58com + "\"", "/"));
                cookieContainer.Add(new Uri(url), new Cookie("58cooper", "\"" + _58cooper + "\"", "/"));
                cookieContainer.Add(new Uri(url), new Cookie("PPU", "\"" + PPU + "\"", "/"));
            }
            
        }
  
        /**/
        /// <summary>
        /// Cookie 容器
        /// </summary>
        public CookieContainer Cookies
        {
            get {  return this.cookieContainer; }
            set { this.cookieContainer = value; }
        }
        void SetCookie(string resstr)
        {
            if (!string.IsNullOrEmpty(resstr))
            {
                string temp = MyClass.ExtractStr(resstr, "www58com", "\"", "\"");
                if(temp!="") www58com = temp;
                temp= MyClass.ExtractStr(resstr, "58cooper", "\"", "\"");
                if (temp != "") _58cooper = temp;
                temp = MyClass.ExtractStr(resstr, "PPU", "\"", "\"");
                if (temp != "") PPU = temp;
               
            }
        }
        /**/
        /// <summary>
        /// 返回带有 Cookie 的 HttpWebRequest。
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                HttpWebRequest httpRequest = request as HttpWebRequest;
                httpRequest.CookieContainer = Cookies;
            }

            return request;
        }
        public string PostStr(string url,string str)
        { 
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            ProCc(url);
            request.CookieContainer = Cookies;  

    request.Referer = url;  
    request.Accept = "Accept:text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";    request.Headers["Accept-Language"] = "zh-CN,zh;q=0.";  
    request.Headers["Accept-Charset"] = "GBK,utf-8;q=0.7,*;q=0.3";  
    request.UserAgent = "User-Agent:Mozilla/5.0 (Windows NT 5.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.202 Safari/535.1";

    request.KeepAlive = true;  
    //上面的http头看情况而定，但是下面俩必须加  
    request.ContentType = "application/x-www-form-urlencoded";  
    request.Method = "POST";  
      
    Encoding encoding = Encoding.UTF8;//根据网站的编码自定义  
    byte[] postData = encoding.GetBytes(str);//postDataStr即为发送的数据，格式还是和上次说的一样  
    request.ContentLength = postData.Length;  
    Stream requestStream = request.GetRequestStream();  
    requestStream.Write(postData, 0, postData.Length);
    if (www58com != "")
    {
     //   request.Headers["Cookie"] = string.Format("www58com=\"{0}\"; 58cooper=\"{1}\"; PPU=\"{2}\"", www58com, _58cooper, PPU);
    }
    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
    string resstr = response.Headers["Set-Cookie"];
    SetCookie(resstr);
    Stream responseStream = response.GetResponseStream();  
    //如果http头中接受gzip的话，这里就要判断是否为有压缩，有的话，直接解压缩即可  
    if (response.Headers["Content-Encoding"] != null && response.Headers["Content-Encoding"].ToLower().Contains("gzip"))  
    {  
        responseStream = new GZipStream(responseStream,CompressionMode.Decompress);  
    }  
      
    StreamReader streamReader = new StreamReader(responseStream, encoding);  
    string retString = streamReader.ReadToEnd();  
      
    streamReader.Close();  
    responseStream.Close();  
      
    return retString;  
  }
       static string www58com = "";
       static string PPU = "";
       static string _58cooper = "";
 
        /// <summary>
        /// 遍历CookieContainer
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public  List<Cookie> GetAllCookies()
        {
            List<Cookie> lstCookies = new List<Cookie>();

            Hashtable table = (Hashtable)cookieContainer.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cookieContainer, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) lstCookies.Add(c);
            }
            return lstCookies;
        }

        #region 封装了 GetStr
        /**/
        /// <summary>
        /// 获得指定 URL 的源文件
        /// </summary>
        /// <param name="uriString">页面 URL</param>
        /// <param name="dataEncoding">页面的 CharSet</param>
        /// <returns>页面的源文件</returns>
        public string GetStr(string uriString)
        {
            try
            {
                ProCc(uriString);
                // 返回页面的字节数组
                byte[] responseData = this.DownloadData(uriString);
                // 将返回的将字节数组转换成字符串(HTML);
                string srcString = Encoding.GetEncoding("utf-8").GetString(responseData);
                srcString = srcString.Replace("\t", "");
                srcString = srcString.Replace("\r", "");
                srcString = srcString.Replace("\n", "");

                return srcString;
            }
            catch (WebException we)
            {
                return string.Empty;
            }
        }
        public Image GetImg(string url)
        {
            this.Dispose();
            HttpWebRequest wr = (HttpWebRequest)GetWebRequest(new Uri(url));
            wr.Method = "GET";
            wr.ContentType = null;  
            wr.Timeout = 1000;
            try
            {
                HttpWebResponse wrs = (HttpWebResponse)wr.GetResponse();
                cookieContainer.Add(wrs.Cookies);
                System.IO.Stream s = wrs.GetResponseStream();
                Image img = Image.FromStream(s);
                s.Close();
                wrs.Close();
                wr.Abort();
                return img;
            }
            catch (Exception)
            {

                return null;
            }
           
            
        }

        /**/
        /// <summary>
        /// 从指定的 URL 下载文件到本地
        /// </summary>
        /// <param name="uriString">文件 URL</param>
        /// <param name="fileName">本地文件的完成路径</param>
        /// <returns></returns>
        public bool GetFile(string urlString, string fileName, out string msg)
        {
            try
            {
                this.DownloadFile(urlString, fileName);
                msg = string.Empty;
                return true;
            }
            catch (WebException we)
            {
                msg = we.Message;
                return false;
            }
        }
        #endregion

        public bool Register(string user,string pass)
        {
            //https://passport.58.com/douireg?
            string url = "https://passport.58.com/douireg";
            string data = "ts=1386663918249&callback=handleRegResult&domain=58.com&pptregcppassword={1}&pptregemail={0}%40126.com&pptregpassword={1}&pptregusername={0}&sysIndex=0";
            data = string.Format(data, user, pass);
            string rs= PostStr(url, data);
            return IsSuc(rs);

        }
        public bool Login(string user, string pass,string code="")
        {
            string url = "https://passport.58.com/douilogin";
            string data = "domain=58.com&callback=handleLoginResult&sysIndex=0&pptusername={0}&pptpassword={1}&pptvalidatecode={2}";
            data = string.Format(data, user, pass,code);
            string rs = PostStr(url, data);
            return IsSuc(rs);

        }
        public void FillCmp(Company cmp)
        {
            string url = "http://qy.58.com/addparttimerent";
            string data = "fc=&enterpriseName={0}&enterpriseAlias={1}&intro={2}&linkMan={3}&phone={4}&telPhone=&areaid=1&address={5}&pics=&jscode=53535250536349535353&jsmoverecord=276,37,1386765316952,txtCompIntro,blur,txt;190,140,1386765319244,txtCompIntro,focus,txt;177,431,1386765322818,txtCompIntro,blur,txt;233,705,1386765353084,save,click,button&validatecode=&checkTelphone=&checkBackCode=";
            data = string.Format(data, cmp.name, cmp.nick, cmp.profile, cmp.people, cmp.phone, cmp.address);
            data = System.Web.HttpUtility.UrlEncode(System.Text.Encoding.GetEncoding("utf-8").GetBytes(data));
            string rs = PostStr(url, data);
        }
        public string FillJob(Job job,string url)
        {
            string data = "useridentity=1&Title={0}&jobcateID=28927&yuzhong=&kemu=&grade=&fenzhiwei=&zhaopinrenshu=23&gongzuoshijian=1|2|3|4|5|6|7&jianzhishijian=1|2|3|4|5|6|7|8|9|10|11|12|13|14|15|16|17|18|19|20|21&longterm=1&xinzi=122&xinzidanwei=1&xinzijiesuan_radio=1&xinzijiesuan=1&Content={1}&goblianxiren={2}&lianxifangshi={3}&tel2=&Email={4}&address={5}&localArea=&localDiduan=&arealocal=&Phone={3}&tbx_backfill_smscode=234234&__hidden_iqasincode=1&tuiguang_option=1&cateapplyed=13948&localapplyed=342&Type=0&xingbieyaoqiu=3&yuzhong=&grade=&kemu=&fenzhiwei=&shifoufufeifabu=0&fufeijihuo=1&iqas_mcresult=0077084419583185963376110121&__post_gsxw=357,1210,1386669556077,fabu,click,button;376,1195,1386669564233,fabu,click,button;416,1251,1386669607181,fabu,click,button&HiddenForPara=&gobquzhi=xinzidanwei={6}元/小时&xinzijiesuan=日结&cateapplyed=13948&localapplyed=342&gobalsokey=122&fcookie=619a74a9-5e53-460c-bccd-fe468f22cc46&hiddenTextBoxJoinValue={0}:开始时间:结束时间:{2}:{3}:{4}:{5}&xiaobao_option=0";
            data = string.Format(data, job.name, job.profile, job.people, job.phone, job.email, job.address, job.salary);
            data = System.Web.HttpUtility.UrlEncode(System.Text.Encoding.GetEncoding("utf-8").GetBytes(data));
            return  PostStr(url, data);
        }
        public bool IsSuc(string rs)
        {
            if (rs.IndexOf("type=success") != -1)
            {
                return true;

            }
            else
            {
                return false;
            }
        }
    }
}
