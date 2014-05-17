using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;

///���ܣ���������
///���ʱ�䣺����
///���ߣ�һ��
///�������⣺��
///�����ƻ�������
///˵�����������ϣ����뱣��
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget
{
    class cWebPage
    {
        public cWebPage()
        {
        }

        ~cWebPage()
        {
        }

        private string GetHtml(string url, Encoding encoding)
        {

            byte[] buf = new WebClient().DownloadData(url);

            if (encoding != null) return encoding.GetString(buf);

            string html = Encoding.UTF8.GetString(buf);

            encoding = GetEncoding(html);

            if (encoding == null || encoding == Encoding.UTF8) return html;

            return encoding.GetString(buf);

        }

        private string GetFixTagContent(string inputstring, string tagName)
        {
            string leftTag = "<" + tagName + ">";
            string rightTag = "</" + tagName + ">";

            Regex reg = new Regex("<" + tagName + "[^<>]*>(.|\n)*" + rightTag, RegexOptions.IgnoreCase);
            return reg.Match(inputstring).Value.Replace(leftTag, "").Replace(rightTag, "");
        }

        // ������ҳ��HTML������ȡ��ҳ��Encoding

        private Encoding GetEncoding(string html)
        {

            string pattern = @"(?i)\bcharset=(?<charset>[-a-zA-Z_0-9]+)";

            string charset = Regex.Match(html, pattern).Groups["charset"].Value;

            try { return Encoding.GetEncoding(charset); }

            catch (ArgumentException) { return null; }
        }

    }
}
