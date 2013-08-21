using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Excel;
using StringTool;
using System.Threading;
using System.Diagnostics;
using System.Xml;
namespace InputPhones
{

          
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private int InsertItem(NewsObject no)
        {
            string title = no.title;
            string type_id = no.type_id;
            string content = no.content;
            string source = no.source;
            string pubdate = no.pubdate;
            string misc = no.misc;
            if (content.Contains("?"))
                content = content.Replace("?", "");

            string sql = "INSERT INTO data_xp_news(title,type_id,content,source,misc,pubdate) VALUES('{0}',{1},'{2}','{3}','{4}','{5}')";
            sql = string.Format(sql, title.Replace("'", "''"), type_id, content.Replace("'", "''"), source.Replace("'", "''"), misc, pubdate);
            if (!MyClass.SqlExists("select * from data_xp_news where title='" + title + "'"))
            {
                MyClass.ExecuteNonQuery(sql);
                return 0;
            }
            else
            {
                sql = string.Format("update data_xp_news set content='{0}',source='{1}',misc='{2}',pubdate='{3}' where title='{4}'", content, source, misc, pubdate, title);
                MyClass.ExecuteNonQuery(sql);
                return 1;
            }

        }
        public delegate void SetTextDelegate(NewsObject no);
        public void SetText(NewsObject no)
        {
            label9.Text = no.type_name;
            label2.Text = no.title;
            label4.Text = no.pubdate;
            label6.Text = no.source;
            int i = int.Parse(count.Text);
            i++;
            count.Text = i.ToString();
            if (no.status == 0)
                label13.Text = "插入成功";
            else
                label13.Text = "成功更新";
        }
        NewsObject GetNewsObject(string title, string url,string type)
        {
            NewsObject no = new NewsObject();
            string allc = MyClass.GetUrltoHtml(url);

            if (type == "本地新闻")//本地新闻提取方法
            {
                string str1 = MyClass.ExtractStr(allc, "class=\"article\"", "<div", "div>", 1, true);
                if (str1 != "")
                    str1 = "<div" + str1 + "div>";

                string time = MyClass.GetNowTime();
                string resource = "";
                DateTime dt = DateTime.Now;
                try
                {
                    time = MyClass.ExtractStr(allc, "class=\"info\"", ">", " 来").Trim();
                    resource = MyClass.ExtractStr(allc, "class=\"info", "来源:", "</div>").Split('<')[2].Split('>')[1];
                    dt = DateTime.Parse(time);
                }
                catch (Exception)
                {
                }
                time = MyClass.FormateTime(dt);
                no.title = title;
                no.pubdate = time;
                no.source = resource;
                no.content = str1;
            }
            else
            {
                string str1 = MyClass.ExtractStr(allc, "id=\"contentText\"", "<div", "div>", 1, true);
                if (str1 != "")
                    str1 = "<div" + str1 + "div>";
                string time = MyClass.ExtractStr(allc, "class=\"l\"", ">", "<");
                string resource = MyClass.ExtractStr(allc, "id=\"media_span\"", ">", "<");
                if (resource == "")
                {
                    resource = MyClass.ExtractStr(allc, "itemprop=\"name\"", ">", "<");
                }
                if (time == "")
                {
                    time = MyClass.ExtractStr(allc, "class=\"time\"", ">", "<");
                }
                DateTime dt;
                try
                {
                    dt = DateTime.Parse(time);
                    time = MyClass.FormateTime(dt);
                }
                catch (Exception)
                {
                    time = MyClass.FormateTime(DateTime.Now);
                }
                no.title = title;
                no.pubdate = time;
                no.source = resource;
                no.content = str1;
            }
            return no;
        }
        void GetNews()
        {
            Dictionary<string, string> bg = GetBigTypes();
            progressBar1.Maximum = bg.Count;
            foreach (KeyValuePair<string,string> item in bg)
            {
                string typeid = (string)MyClass.ExecuteScalar(string.Format("SELECT id from data_xp_newstype where typename='{0}'", item.Key)).ToString();
                Dictionary<string,string> sm=GetTitles(item.Value);
                
                totle.Text = sm.Count.ToString();
                count.Text = "0";
                foreach (KeyValuePair<string, string> item1 in sm)
                {
                    NewsObject no = GetNewsObject(item1.Key, item1.Value, item.Key);
                    no.type_id = typeid;
                    no.type_name = item.Key;
                    if (no.content != "")
                    {
                       no.status = InsertItem(no);
                        label1.Invoke(new SetTextDelegate(SetText), no);
                    }
                }
                progressBar1.PerformStep();
            }
        }

        void ProcessTask()
        {
            while (true)
            {
                GetNews();
                label13.Text = "处理完毕，睡眠1小时";
                progressBar1.Value = 0;
                Thread.Sleep(60 * 60 * 1000);
            }
           
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(ProcessTask);
            Control.CheckForIllegalCrossThreadCalls = false;
            t.Start();
            //mulu
            //wenjian
            //chuli
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
        private string GetBigTypesContent()
        {
            return MyClass.GetUrltoHtml("http://rss.news.sohu.com/rss.shtml");
        }
        public Dictionary<string, string> GetTitles(string url1)
        {
            Dictionary<string, string> smt = new Dictionary<string, string>();
            XmlDocument doc = new XmlDocument();
            doc.Load(url1);
            //使用xPath选择需要的节点
            XmlNodeList nodes = doc.SelectNodes("/rss/channel//item");

            foreach (XmlNode item in nodes)
            {
                string title = item.SelectSingleNode("title").InnerText;
                string url2 = item.SelectSingleNode("link").InnerText;
                smt.Add(title,url2);
            }
            return smt;
        }
        private Dictionary<string,string> GetBigTypes()
        {
            Dictionary<string, string> bgt =new Dictionary<string, string>();
            bgt.Add("本地新闻", "http://rss.zynews.com/news_dujia.xml");
            string rt=GetBigTypesContent();
            string strs = MyClass.ExtractStr(rt, "·", " ", "<", 1000);
            string[] strss = strs.Split(',');

            foreach (string item in strss)
            {
                string url = MyClass.ExtractStr(rt, "· " + item, "href=\"", "\"");
                bgt.Add(item, url);
                string sql = string.Format("UPDATE data_xp_newstype SET url='{1}' WHERE typename='{0}'", item, url);
                string existsql = string.Format("SELECT * from data_xp_newstype where typename='{0}'", item);
                if (!MyClass.SqlExists(existsql))
                {
                    sql = string.Format("INSERT INTO data_xp_newstype(typename,url) VALUES('{0}','{1}');", item, url);
                }
                MyClass.ExecuteNonQuery(sql);
            }
            return bgt;
        }
    }
   public class NewsObject
    {
        public string title;
        public string type_id;
        public string content;
        public string source;
        public string pubdate;
        public string misc = "";
        public string type_name;
        public int status = 0;
    }
}
