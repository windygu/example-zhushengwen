using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StringTool;
namespace GongJiao
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            MyClass.connectionString = "server= localhost;User Id= root;password=admin;Persist Security Info=True;port=3306;database=test;charset=gbk;";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = MyClass.GetUrltoHtml(userControl11.MyText);
            if (userControl11.MyText != "") userControl11.AddItem();
        }
        string baseurl = "http://218.28.136.21:8081/";

        private void button2_Click(object sender, EventArgs e)
        {
            for (int index = 1; index < 15; index++)
            {
                userControl11.MyText = baseurl + "linelist.asp?topage=" + index.ToString();
                button1_Click(sender, e);
                string[] buslines = MyClass.ExtractStr(textBox1.Text, "<tr", "<tr", "tr>", 100, true, "~").Split('~');
                for (int i = 1; i < buslines.Length - 1; i++)
                {
                    string res = buslines[i];
                    string href = "<a" + MyClass.ExtractStr(res, "<a", "<a", ">", 1, true) + ">";
                    res = res.Replace(href, "");
                    string url = baseurl + MyClass.ExtractStr(href, "href", "\"", "\"");
                    string[] strs = MyClass.ExtractStr(res, "<td", "<td", "</td>", 100, true, "~").Split('~');
                    for (int j = 0; j < strs.Length - 1; j++)
                    {
                        int k = strs[j].LastIndexOf('>') + 1;
                        strs[j] = strs[j].Substring(k).Trim();
                    }
                    string[] dests = new string[strs.Length + 2];
                    Array.Copy(strs, dests, strs.Length);
                    string split = strs[strs.Length - 2];
                    dests[strs.Length - 2] = MyClass.ExtractStr(split, "票", "价", "元");
                    int st = split.IndexOf(' ');
                    dests[strs.Length - 1] = split.Substring(st + 1);
                   

                    string iurl = string.Format("http://218.28.136.21:8081/line.asp?xl={0}&ref=0", dests[0]);
                    string content = MyClass.GetUrltoHtml(iurl);
                   // textBox1.Text = content;
                    string[] posu = MyClass.ExtractStr(content, "上行线路", "：", "<br>").Replace("--", "-").Split('-');
                    for (int j = 0; j < posu.Length; j++)
                    {
                        posu[j] = MyClass.ExtractStr(posu[j], "ref", ">", "<");
                    }
                    string upl = posu.Aggregate((o, p) => o + "-" + p);
                    dests[strs.Length] = "-" + upl + "-";

                    string[] posd = MyClass.ExtractStr(content, "下行线路", "：", "<br>").Replace("--", "-").Split('-');
                    for (int j = 0; j < posd.Length; j++)
                    {
                        posd[j] = MyClass.ExtractStr(posd[j], "ref", ">", "<");

                    }
                    string dnl = posd.Aggregate((o, p) => o + "-" + p);
                    dests[strs.Length+1] = "-"+dnl+"-";
                    if (dests[5] == "")
                        dests[5] = "1";
                    string sql = "INSERT INTO buslines (line ,start_point ,end_point ,time_span ,company , price, misc,up_route,down_route ) VALUES ('{0}','{1}','{2}','{3}','{4}',{5},'{6}','{7}','{8}')";


                    if (!MyClass.SqlExists("select * from buslines where line = '" + dests[0] + "'"))
                    {
                        sql = string.Format(sql, dests);
                        if (!MyClass.ExecuteNonQuery(sql))
                            Console.WriteLine(dests[0]);
                        
                    }
                    else
                    {
                        sql = string.Format("update buslines set start_point = '{1}',end_point = '{2}',time_span = '{3}',company = '{4}',price={5},misc = '{6}',up_route='{7}',down_route='{8}' where line= '{0}'", dests);
                        if (!MyClass.ExecuteNonQuery(sql))
                            Console.WriteLine(dests[0]);
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int id = 1;
            object url;
            while ((url = MyClass.ExecuteScalar("select line from buslines where id=" + id)) != null)
            {
                string iurl = string.Format("http://218.28.136.21:8081/line.asp?xl={0}&ref=0", url);
                string content = MyClass.GetUrltoHtml(iurl);
                textBox1.Text = content;
                string[] pos = MyClass.ExtractStr(content, "上行线路", "：", "<br>").Replace("--", "-").Split('-');
                foreach (string item in pos)
                {
                    string station = MyClass.ExtractStr(item, "ref", ">", "<");
                    string getlines = string.Format("http://218.28.136.21:8081/station.asp?sta={0}&ref=0", station);
                    string linescontent = MyClass.GetUrltoHtml(getlines);
                    string s1 = MyClass.ExtractStr(linescontent, "<a", "<a", "a>", 100, true);
                    string lines = MyClass.ExtractStr(s1, "href", ">", "</", 100);
                    string sql = "select * from stations where name = '" + station + "'";
                    if (!MyClass.SqlExists(sql))
                    {
                        sql = "INSERT INTO stations (name ,buslines) VALUES ('" + station + "','" + lines + "')";
                    }
                    else
                    {
                        sql = string.Format("update stations set buslines = '{1}' where name = '{0}'", station, lines);
                    }
                    MyClass.ExecuteNonQuery(sql);
                }
                id++;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int id = 1;
            object url;
            while ((url = MyClass.ExecuteScalar("select line from buslines where id=" + id)) != null)
            {
                string iurl = string.Format("http://218.28.136.21:8081/line.asp?xl={0}&ref=0", url);
                string content = MyClass.GetUrltoHtml(iurl);
                textBox1.Text = content;
                string[] posu = MyClass.ExtractStr(content, "上行线路", "：", "<br>").Replace("--", "-").Split('-');
                for (int j=0;j<posu.Length;j++)
                {
                    posu[j] = MyClass.ExtractStr(posu[j], "ref", ">", "<");
                }
                string upl=posu.Aggregate((o, p) => o + "-" + p);
                string[] posd = MyClass.ExtractStr(content, "下行线路", "：", "<br>").Replace("--", "-").Split('-');
                for (int j = 0; j < posd.Length; j++)
                {
                    posd[j] = MyClass.ExtractStr(posd[j], "ref", ">", "<");

                }
                string dnl = posd.Aggregate((o, p) => o + "-" + p);
                if (dnl == "" && MyClass.ExecuteScalar("select down_route from buslines where line='" + url.ToString() + "'").ToString()=="")
                {
                    dnl = posu.Aggregate((o, p) => p + "-" + o);
                    string sql = string.Format("update buslines set up_route = '{1}',down_route = '{2}' where line= '{0}'", url.ToString(),upl,dnl);
                    MyClass.ExecuteNonQuery(sql);
                }

                id++;
            }
        }
    }
}
