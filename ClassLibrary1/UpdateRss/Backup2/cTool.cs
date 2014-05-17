using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.IO;
using System.Web;
using System.Windows.Forms;

///功能：常用小功能 静态
///完成时间：2009-3-2
///作者：一孑
///遗留问题：随着系统完善补充
///开发计划：待定
///说明：无
///版本：01.10.00
///修订：无
namespace SoukeyNetget
{
    static class cTool
    {
        private const int INTERNET_CONNECTION_MODEM =1;
        private const int INTERNET_CONNECTION_LAN= 2;
        private const int INTERNET_CONNECTION_PROXY=4;
        private const int INTERNET_CONNECTION_MODEM_BUSY = 8;
           
         //定义（引用）API函数  
        [DllImport("wininet.dll")]

        private static   extern   bool   InternetGetConnectedState   (out int lpdwFlags ,int dwReserved );  
           
        //判断当前是否连接Internet
        static public  bool IsLinkInternet ()
        {  
            int   lfag=0;
            bool IsInternet;

            if (InternetGetConnectedState(out lfag, 0))
                IsInternet = true;
            else
                IsInternet = false;

            return IsInternet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">指定的Url地址</param>
        /// <param name="Isbool">是否去除网页源码中的回车换行符，true 去除</param>
        /// <returns></returns>
        static  public string GetHtmlSource(string url,bool Isbool)
        {
            if (url == "")
                return "";

            string charSet = "";
            WebClient myWebClient = new WebClient();


            //获取或设置用于对向 Internet 资源的请求进行身份验证的网络凭据。   
            myWebClient.Credentials = CredentialCache.DefaultCredentials;


            byte[] myDataBuffer;
            string strWebData;

            try
            {
                //从资源下载数据并返回字节数组。（加@是因为网址中间有"/"符号）   
                myDataBuffer = myWebClient.DownloadData(@url);
                strWebData = Encoding.Default.GetString(myDataBuffer);
            }
            catch (System.Net.WebException  ) 
            {
                return "";
            }

            //获取此页面的编码格式
            Match charSetMatch = Regex.Match(strWebData, "<meta([^<]*)charset=([^<]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string webCharSet = charSetMatch.Groups[2].Value;
            if (charSet == null || charSet == "")
                charSet = webCharSet;

            if (charSet != null && charSet != "" && Encoding.GetEncoding(charSet) != Encoding.Default)
                strWebData = Encoding.GetEncoding(charSet).GetString(myDataBuffer);

            if (Isbool == true)
            {
                strWebData = Regex.Replace(strWebData, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strWebData.Replace(@"\r\n", "");
            }

            return strWebData;

        }

        //去除字符串的回车换行符号
        static public string ClearFlag(string str)
        {
            str = Regex.Replace(str, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            str.Replace(@"\r\n", "");

            return str;
        }

        //根据指定网址判断当前页面的编码
        static public string GetWebpageCode(string url,cGlobalParas.WebCode WebCode)
        {
            string charSet = "";

            WebClient myWebClient = new WebClient();    

            myWebClient.Credentials = CredentialCache.DefaultCredentials;

            //从资源下载数据并返回字节数组。（加@是因为网址中间有"/"符号） 
            byte[] myDataBuffer = myWebClient.DownloadData(url);
            string strWebData = Encoding.Default.GetString(myDataBuffer);

            //获取网页字符编码描述信息 
            Match charSetMatch = Regex.Match(strWebData, "<meta([^<]*)charset=([^<]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string webCharSet = charSetMatch.Groups[2].Value;
            if (charSet == null || charSet == "")
                charSet = webCharSet;
            
            return charSet;

        }

        static private int CountUrl(string WebUrl)
        {
            int Count = 0;
            Match Para = Regex.Match(WebUrl, "{.*}");
            
            return Count;
        }

        //将大写字母转成小写字母
        static public string LetterToLower(string str)
        {
            if (str == "" || str == null)
                return "";

            string lowerStr="";
            char c;

            for (int i=0;i<str.Length ;i++)
            {
                c =char .Parse ( str.Substring(i, 1));

                if (Char.IsUpper(c))
                {
                    c = Char.ToLower(c);
                }
                lowerStr += c;
            }

            return lowerStr;
           
        }

        //将字符串中的字符（转义的）进行替换
        //这个替换真的有点头晕，对正则理解还不到位，不知道是否可以一次按照分组那样的形式对应进行替换
        //请高手修改此类，这样写实在是不得已，呵呵
        static public string ReplaceTrans(string str)
        {
            if (str == "" || str==null )
                return "";

            string conStr="";
            if (Regex.IsMatch(str, "['\"<>&]"))
            {
                Regex re = new Regex("&", RegexOptions.IgnoreCase);
                str = re.Replace(str, "&amp;");
                re = null;

                re = new Regex("<", RegexOptions.IgnoreCase);
                str = re.Replace(str, "&lt;");
                re = null;

                re = new Regex(">", RegexOptions.IgnoreCase);
                str = re.Replace(str, "&gt;");
                re = null;

                re = new Regex("'", RegexOptions.IgnoreCase);
                str = re.Replace(str, "&apos;");
                re = null;

                re = new Regex("\"", RegexOptions.IgnoreCase);
                str = re.Replace(str, "&quot;");
                re = null;
                conStr = str;

            }
            else
            {
                conStr = str;
            }
            return conStr;
        }

        //正则表达式转义
        static public string RegexReplaceTrans(string str)
        {
            if (str == "" || str == null)
                return "";

            string conStr = "";
            if (Regex.IsMatch(str, @"[\$\*\[\]\?\\\(\)]"))
            {
                Regex re = new Regex(@"\\", RegexOptions.IgnoreCase);
                str = re.Replace(str, @"\\");
                re = null;

                re = new Regex(@"\$", RegexOptions.IgnoreCase);
                str = re.Replace(str, @"\$");
                re = null;

                //re = new Regex(@"\.", RegexOptions.IgnoreCase);
                //str = re.Replace(str, @"\.");
                //re = null;

                re = new Regex(@"\*", RegexOptions.IgnoreCase);
                str = re.Replace(str, @"\*");
                re = null;

                re = new Regex(@"\[", RegexOptions.IgnoreCase);
                str = re.Replace(str, @"\[");
                re = null;

                re = new Regex(@"\]", RegexOptions.IgnoreCase);
                str = re.Replace(str, @"\]");
                re = null;

                re = new Regex(@"\?", RegexOptions.IgnoreCase);
                str = re.Replace(str, @"\?");
                re = null;

                re = new Regex(@"\(", RegexOptions.IgnoreCase);
                str = re.Replace(str, @"\(");
                re = null;

                re = new Regex(@"\)", RegexOptions.IgnoreCase);
                str = re.Replace(str, @"\)");
                re = null;

                conStr = str;

            }
            else
            {
                conStr = str;
            }
            return conStr;
        }

        //用于将字符串转换成UTF-8编码
        static public string ToUtf8(string str)
        {
            if (str == null)
            {
                return string.Empty;
            }
            else
            {
                char[] hexDigits = {  '0', '1', '2', '3', '4','5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

                Encoding utf8 = Encoding.UTF8;
                StringBuilder result = new StringBuilder();

                for (int i = 0; i < str.Length; i++)
                {
                    string sub = str.Substring(i, 1);
                    byte[] bytes = utf8.GetBytes(sub);

                    for (int j = 0; j < bytes.Length; j++)
                    {
                        result.Append("%" + hexDigits[bytes[j] >> 4] + hexDigits[bytes[j] & 0XF]);
                    }
                }

                return result.ToString();
            }
        }

        //将UTF-8编码转换成字符串
        static public string FromUtf8(string str)
        {
            char[] hexDigits = {  '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
            List<byte> byteList = new List<byte>(str.Length / 3);

            if (str != null)
            {
                List<string> strList = new List<string>();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < str.Length; ++i)
                {
                    if (str[i] == '%')
                    {
                        strList.Add(str.Substring(i, 3));
                    }
                }

                foreach (string tempStr in strList)
                {
                    int num = 0;
                    int temp = 0;
                    for (int j = 0; j < hexDigits.Length; ++j)
                    {
                        if (hexDigits[j].Equals(tempStr[1]))
                        {
                            temp = j;
                            num = temp << 4;
                        }
                    }

                    for (int j = 0; j < hexDigits.Length; ++j)
                    {
                        if (hexDigits[j].Equals(tempStr[2]))
                        {
                            num += j;
                        }
                    }

                    byteList.Add((byte)num);
                }
            }

            return Encoding.UTF8.GetString(byteList.ToArray());
        }

        static public string UTF8ToGB2312(string str)
        {
            try
            {
                Encoding utf8 = Encoding.GetEncoding(65001);
                Encoding gb2312 = Encoding.GetEncoding("gb2312");//Encoding.Default ,936
                byte[] temp = utf8.GetBytes(str);
                byte[] temp1 = Encoding.Convert(utf8, gb2312, temp);
                string result = gb2312.GetString(temp1);
                return result;
            }
            catch (Exception)//(UnsupportedEncodingException ex)
            {
                return null;
            }
        }

        //返回相对于Soukey采摘的相对路径
        static public string GetRelativePath(string absPath)
        {
            string mainDir = Program.getPrjPath();

            //if (!mainDir.EndsWith("\\"))
            //{
            //    mainDir += "\\";
            //}

            int intIndex = -1, intPos = mainDir.IndexOf('\\');

            while (intPos >= 0)
            {
                intPos++;
                if (string.Compare(mainDir, 0, absPath, 0, intPos, true) != 0) break;
                intIndex = intPos;
                intPos = mainDir.IndexOf('\\', intPos);
            }

            if (intIndex >= 0)
            {
                absPath = absPath.Substring(intIndex);
                intPos = mainDir.IndexOf("\\", intIndex);
                while (intPos >= 0)
                {
                    absPath = "..\\" + absPath;
                    intPos = mainDir.IndexOf("\\", intPos + 1);
                }
            }

            return absPath;
        }

        //对Url中文部分进行编码，返回编码后的Url，
        //注意：只对中文进行编码
        static public string UrlEncode(string Url, cGlobalParas.WebCode uEncoding)
        {
            string DemoUrl = Url;

            Regex re = new Regex("[\\u4e00-\\u9fa5]", RegexOptions.None);
            MatchCollection mc = re.Matches(DemoUrl);

            switch (uEncoding)
            {
                case cGlobalParas.WebCode.utf8:
                    foreach (Match ma in mc)
                    {
                        DemoUrl = DemoUrl.Replace(ma.Value.ToString(), HttpUtility.UrlEncode(ma.Value.ToString(), Encoding.UTF8));
                    }
                    break;
                case cGlobalParas.WebCode.gb2312:
                    foreach (Match ma in mc)
                    {
                        DemoUrl = DemoUrl.Replace(ma.Value.ToString(), HttpUtility.UrlEncode(ma.Value.ToString(), Encoding.GetEncoding("gb2312")));
                    }
                    break;
                case cGlobalParas.WebCode.gbk:
                    foreach (Match ma in mc)
                    {
                        DemoUrl = DemoUrl.Replace(ma.Value.ToString(), HttpUtility.UrlEncode(ma.Value.ToString(), Encoding.GetEncoding("gbk")));
                    }
                    break;
                case cGlobalParas.WebCode.big5:
                    foreach (Match ma in mc)
                    {
                        DemoUrl = DemoUrl.Replace(ma.Value.ToString(), HttpUtility.UrlEncode(ma.Value.ToString(), Encoding.GetEncoding("big5")));
                    }
                    break;
                default:
                    foreach (Match ma in mc)
                    {
                        DemoUrl = DemoUrl.Replace(ma.Value.ToString(), HttpUtility.UrlEncode(ma.Value.ToString(), Encoding.UTF8));
                    }
                    break;
            }

            return  DemoUrl;
        }

        //判断指定的文件所在的目录是否存在，如果不存在则建立
        //传入的参数必须是文件，如果不是文件，则需以"\"结尾
        static public void CreateDirectory(string strDir)
        {

            //需要提取文件目录
            strDir = Path.GetDirectoryName(strDir);

            if (!Directory.Exists(strDir))
            {
                //创建此目录
                Directory.CreateDirectory(strDir);
            }


        }

        static public DialogResult MyMessageBox(string Mess, string Title, MessageBoxButtons but, MessageBoxIcon icon)
        {
            frmMessageBox fm = new frmMessageBox();
            fm.MessageBox(Mess, Title, but, icon);
            DialogResult dr= fm.ShowDialog();
            fm.Dispose();

            return dr;
        }
    }
}
