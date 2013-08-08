using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using MySql.Data.MySqlClient;
using System.Net;
using System.IO;


namespace WindowsFormsApplication2
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

                ProcessCityAir.Process();
                Thread.Sleep(60 * 60 * 1000);
            }
        }
    }
    class UtilStr
    {
        public static string ExtractStr(string resource, string name, string stas, string ends)
        {
            string str = "";
            //首先定位到名称
            int bgn = resource.IndexOf(name);
            //如果未找到直接返回
            if (bgn != -1)
            {
                //再次定位到开始字符
                int sta = resource.IndexOf(stas, bgn + name.Length);
                if (sta != -1)
                {
                    //建立栈结构,开始字符和结束字符分别进行压栈出栈
                        int i = 1;
                        int index = sta + 1;
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
                            str = resource.Substring(sta + 1, index - sta - 1);
                        }
                }
            }
            return str;
        }
    }
    public class ProcessCityAir
    {
        public const string Url = "http://web.juhe.cn:8080/environment/air/cityair?city=zhengzhou&key=d7a52f34260366b287f9d2ce3275faec";
        public static string connectionString = "server= 120.194.12.194;User Id= root;password=bmtdb;Persist Security Info=True;port=3307;database=bmtdb;charset=gbk;";
        public static string GetNowTime()
        {
            return string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
        }
        public static void Process()
        {
            string s = ProcessCityAir.GetUrltoHtml();
            string resultcode = UtilStr.ExtractStr(s, "\"resultcode\"", "\"", "\"");
            if (resultcode == "200")
            {
                string citynow = UtilStr.ExtractStr(s, "\"lastMoniData\"", "{", "}");
                int index = 1;
                string qutstr = UtilStr.ExtractStr(citynow, "\"" + index + "\"", "{", "}");
                string air_time = UtilStr.ExtractStr(s, "\"date\"", "\"", "\"");

                while ("" != qutstr)
                {

                    string air_address = UtilStr.ExtractStr(qutstr, "\"city\"", "\"", "\"");
                    string air_so2 = "--";
                    string air_no2 = "--";
                    string air_pm25 = UtilStr.ExtractStr(qutstr, "\"PM2.5Hour\"", "\"", "\"");
                    string air_pm10 = UtilStr.ExtractStr(qutstr, "\"PM10Hour\"", "\"", "\""); ;
                    string air_index = UtilStr.ExtractStr(qutstr, "\"AQI\"", "\"", "\"");
                    string air_level = UtilStr.ExtractStr(qutstr, "\"quality\"", "\"", "\"");

                    InsertData(air_address, air_so2, air_no2, air_pm25, air_pm10, air_index, air_level, air_time);

                    qutstr = UtilStr.ExtractStr(citynow, "\"" + (++index) + "\"", "{", "}");
                }
            }
        }
        public static void InsertData(string air_address, string air_so2, string air_no2, string air_pm25, string air_pm10, string air_index, string air_level, string air_time)
        {
            string queryString =
        "INSERT INTO data_shxf_airqualityinfo(air_address,air_so2,air_no2,air_pm25,air_pm10,air_index,air_level,air_time) VALUES('" + air_address + "','" + air_so2 + "','" + air_no2 + "','" + air_pm25 + "','" + air_pm10 + "'," + air_index + ",'" + air_level + "','" + air_time + "')";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(queryString);
                command.Connection = connection;
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public static string GetUrltoHtml()
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
                //                StreamReader sReader = new StreamReader(responseStream, Encoding.GetEncoding("gb2312"));
                StreamReader sReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
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
            catch (Exception)
            {
                content = new StringBuilder("Runtime Error");
            }
            return content.ToString();
        }
    }
}
