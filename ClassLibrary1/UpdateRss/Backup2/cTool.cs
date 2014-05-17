using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.IO;
using System.Web;
using System.Windows.Forms;

///���ܣ�����С���� ��̬
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺����ϵͳ���Ʋ���
///�����ƻ�������
///˵������
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget
{
    static class cTool
    {
        private const int INTERNET_CONNECTION_MODEM =1;
        private const int INTERNET_CONNECTION_LAN= 2;
        private const int INTERNET_CONNECTION_PROXY=4;
        private const int INTERNET_CONNECTION_MODEM_BUSY = 8;
           
         //���壨���ã�API����  
        [DllImport("wininet.dll")]

        private static   extern   bool   InternetGetConnectedState   (out int lpdwFlags ,int dwReserved );  
           
        //�жϵ�ǰ�Ƿ�����Internet
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
        /// <param name="url">ָ����Url��ַ</param>
        /// <param name="Isbool">�Ƿ�ȥ����ҳԴ���еĻس����з���true ȥ��</param>
        /// <returns></returns>
        static  public string GetHtmlSource(string url,bool Isbool)
        {
            if (url == "")
                return "";

            string charSet = "";
            WebClient myWebClient = new WebClient();


            //��ȡ���������ڶ��� Internet ��Դ��������������֤������ƾ�ݡ�   
            myWebClient.Credentials = CredentialCache.DefaultCredentials;


            byte[] myDataBuffer;
            string strWebData;

            try
            {
                //����Դ�������ݲ������ֽ����顣����@����Ϊ��ַ�м���"/"���ţ�   
                myDataBuffer = myWebClient.DownloadData(@url);
                strWebData = Encoding.Default.GetString(myDataBuffer);
            }
            catch (System.Net.WebException  ) 
            {
                return "";
            }

            //��ȡ��ҳ��ı����ʽ
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

        //ȥ���ַ����Ļس����з���
        static public string ClearFlag(string str)
        {
            str = Regex.Replace(str, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            str.Replace(@"\r\n", "");

            return str;
        }

        //����ָ����ַ�жϵ�ǰҳ��ı���
        static public string GetWebpageCode(string url,cGlobalParas.WebCode WebCode)
        {
            string charSet = "";

            WebClient myWebClient = new WebClient();    

            myWebClient.Credentials = CredentialCache.DefaultCredentials;

            //����Դ�������ݲ������ֽ����顣����@����Ϊ��ַ�м���"/"���ţ� 
            byte[] myDataBuffer = myWebClient.DownloadData(url);
            string strWebData = Encoding.Default.GetString(myDataBuffer);

            //��ȡ��ҳ�ַ�����������Ϣ 
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

        //����д��ĸת��Сд��ĸ
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

        //���ַ����е��ַ���ת��ģ������滻
        //����滻����е�ͷ�Σ���������⻹����λ����֪���Ƿ����һ�ΰ��շ�����������ʽ��Ӧ�����滻
        //������޸Ĵ��࣬����дʵ���ǲ����ѣ��Ǻ�
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

        //������ʽת��
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

        //���ڽ��ַ���ת����UTF-8����
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

        //��UTF-8����ת�����ַ���
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

        //���������Soukey��ժ�����·��
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

        //��Url���Ĳ��ֽ��б��룬���ر�����Url��
        //ע�⣺ֻ�����Ľ��б���
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

        //�ж�ָ�����ļ����ڵ�Ŀ¼�Ƿ���ڣ��������������
        //����Ĳ����������ļ�����������ļ���������"\"��β
        static public void CreateDirectory(string strDir)
        {

            //��Ҫ��ȡ�ļ�Ŀ¼
            strDir = Path.GetDirectoryName(strDir);

            if (!Directory.Exists(strDir))
            {
                //������Ŀ¼
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
