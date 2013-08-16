using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using StringTool;
using MySql.Data.MySqlClient;
using System.Xml;
using System.IO;
namespace ProcessNews
{

    //public delegate void TRFunc(IDataReader reader);

    public partial class Form1 : Form
    {
      
        public Form1()
        {
            InitializeComponent();
            MyClass.connectionString = connectionString;
        }
        static string path = "";
        static string connectionString="server= localhost;User Id= root;password=admin;Persist Security Info=True;port=3306;database=test;charset=gbk;";
        public static void InitialSetting()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path + "setting.xml");
            foreach (XmlNode item in xmlDoc.GetElementsByTagName("setting"))
            {
              /*  if (item["name"].InnerText == "TerminalName")
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
               * */
            }
        }
        void ProcessReader(IDataReader reader)
        {
            int i = int.Parse(count.Text);
            i++;
            count.Text = i.ToString();
            MessageToSend mts = new MessageToSend();
            mts.GetData(reader);
            label2.Text = reader["UserName"].ToString();
            label4.Text = mts.content;

            InterfaceClient ifc = new InterfaceClient();
            string ret;
            string send_ok = "0";
            string rets="";
            if (ifc.SendSms("LA04J254371",
                "Aa1234",
                reader["phone"].ToString(),
                mts.content,
                mts.phone,
                out ret))
            {
                System.Xml.XmlDocument xdoc = new System.Xml.XmlDocument();
                StringReader sr = new StringReader(ret);
                xdoc.Load(sr);
                XmlNode xnl = xdoc.SelectSingleNode("/Root/SMS/Return");
                if (xnl != null)
                {
                    rets = xnl.InnerText;
                    if (xnl.Attributes["State"].Value == "0")
                    {
                        send_ok = "1"; 
                        string sql = "UPDATE user SET SmsCount=SmsCount-1 WHERE  Id=" + mts.user_id;
                        MyClass.ExecuteNonQuery(sql);
                    }
                }
            }

            string retsql = "UPDATE sms SET send_is=1,return_data={0},send_yes={1} WHERE  Id={2}";
            retsql = string.Format(retsql, rets, send_ok, mts.id);
            MyClass.ExecuteNonQuery(retsql);
            progressBar1.PerformStep();
        }
        void ProcessTask()
        {
            while (true)
            {
                string comsql = @"SELECT {0} from (SELECT * from sms where  UNIX_TIMESTAMP(ding_time)-UNIX_TIMESTAMP(CURRENT_TIMESTAMP())<60) m
                LEFT JOIN (SELECT * from user where SmsCount>0) u ON U.id=m.user_id";
                string sql = string.Format(comsql,"count(*)");
                totle.Text = MyClass.ExecuteScalar(sql).ToString();
                progressBar1.Maximum = int.Parse(totle.Text);
                MyClass.TraverReader(string.Format(comsql, "*"), ProcessReader);
                label5.Text = "暂停一分钟！";
                progressBar1.Value = 0;
                Thread.Sleep(1000 * 60);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            label5.Text = "正在发送......";
            Thread t = new Thread(ProcessTask);
            Control.CheckForIllegalCrossThreadCalls = false;
            progressBar1.Value = 0;
            count.Text = "0";
            t.Start();
        }
    }
    public class MessageToSend
    {
        public string id;
        public string user_id;
        public string phone;
        public string aotu_is;
        public string content;
        public string ding_time;
        public string send_is;
        public string send_yes;
        public string num;
        public string return_data;
        public void GetData(IDataReader reader)
        {
            id = reader["id"].ToString();
            user_id = reader["user_id"].ToString();
            phone = reader["phone"].ToString();
            aotu_is = reader["aotu_is"].ToString();
            content = reader["content"].ToString();
            ding_time = reader["ding_time"].ToString();
            send_is = reader["send_is"].ToString();
            send_yes = reader["send_yes"].ToString();
            num = reader["num"].ToString();
            return_data = reader["return_data"].ToString();
        }
    }
    public class InterfaceClient
	{
		/// <summary>
		///服务器地址。
		/// </summary>
		private string ServerUrl = @"http://218.206.201.28:10657/SMS";
		/// <summary>
		/// 发送短信。
		/// </summary>
		/// <param name="UserName">用户名。</param>
		/// <param name="PWD">密码。</param>
		/// <param name="CorpCode">信箱号。</param>
		/// <param name="Message">要发送的信息。</param>
		/// <param name="Dest">发送的目标号码</param>
		/// <param name="OutString">服务器返回的信息。</param>
		/// <returns></returns>
		public bool SendSms(string UserName, string PWD, string CorpCode, string Message, string Dest, out string OutString)
		{
			bool ReturnValue = false;
			OutString = string.Empty;
			//生成发送的报文
			string PoketXml = this.EncryptBase64(this.CreateSendPuk(UserName, PWD, CorpCode, Message, Dest));
			//转换成字节数组
			byte[] Boday = System.Text.Encoding.UTF8.GetBytes(PoketXml);
			try
			{
				//连接服务器
				System.Net.HttpWebRequest gRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(this.ServerUrl);
				gRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
				gRequest.Method = "POST";
				gRequest.ContentLength = Boday.Length;
				System.IO.Stream reqStream = gRequest.GetRequestStream();
				//向服务器发送POST请求。
				reqStream.Write(Boday, 0, Boday.Length);
				System.Net.WebResponse webResponse = gRequest.GetResponse();
				System.IO.StreamReader reader = new System.IO.StreamReader(webResponse.GetResponseStream(), Encoding.UTF8);
				//读取服务器返回值。
				OutString = this.DecryptBase64(reader.ReadToEnd());
				webResponse.Close();
				ReturnValue = true;
			}
			catch
			{ }
			return ReturnValue;
		}
		/// <summary>
		/// 生成发送数据包
		/// </summary>
		/// <param name="UserName">用户名</param>
		/// <param name="PWD">密码</param>
		/// <param name="CorpCode">信箱号</param>
		/// <param name="Message">待发送的消息体</param>
		/// <param name="Dest">发送的目标号码</param>
		/// <returns>生成的发送数据包</returns>
		private string CreateSendPuk(string UserName, string PWD, string CorpCode, string Message, string Dest)
		{
			StringBuilder PokcetXML = new StringBuilder();
			System.Xml.XmlWriterSettings XmlSet = new System.Xml.XmlWriterSettings();
			XmlSet.Encoding = System.Text.Encoding.UTF8;
			System.Xml.XmlWriter gXmlWriter = System.Xml.XmlWriter.Create(PokcetXML, XmlSet);
			//写根节点
			gXmlWriter.WriteStartElement("Root");
			//协议版本
			gXmlWriter.WriteStartAttribute("version");
			gXmlWriter.WriteString("2.0");
			gXmlWriter.WriteEndAttribute();
			//用户名
			gXmlWriter.WriteStartAttribute("User");
			gXmlWriter.WriteString(UserName);
			gXmlWriter.WriteEndAttribute();
			//密码
			gXmlWriter.WriteStartAttribute("PWD");
			gXmlWriter.WriteString(PWD);
			gXmlWriter.WriteEndAttribute();
			//用户类型定死的是8
			gXmlWriter.WriteStartAttribute("UserType");
			gXmlWriter.WriteString("8");
			gXmlWriter.WriteEndAttribute();
			//信箱号码。
			gXmlWriter.WriteStartAttribute("CorpCode");
			gXmlWriter.WriteString(CorpCode);
			gXmlWriter.WriteEndAttribute();
			//生成SMS节点
			gXmlWriter.WriteStartElement("SMS");
			//生成M节点
			gXmlWriter.WriteStartElement("M");
			gXmlWriter.WriteString(this.EncryptBase64(Message));
			gXmlWriter.WriteEndElement();
			//生成H节点
			gXmlWriter.WriteStartElement("H");
			//写手机号M属性
			gXmlWriter.WriteStartAttribute("M");
			gXmlWriter.WriteString(Dest);
			gXmlWriter.WriteEndAttribute();

			gXmlWriter.WriteEndElement();

			gXmlWriter.WriteEndElement();

			gXmlWriter.WriteEndElement();
			gXmlWriter.Flush();
			gXmlWriter.Close();
			return PokcetXML.ToString();
		}
		/// <summary>
		/// 对数据进行Base64加密
		/// </summary>
		/// <param name="Data">待加密数据</param>
		/// <returns>加密后数据</returns>
		private string EncryptBase64(string Data)
		{
			return System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("UTF-8").GetBytes(Data));
		}
		/// <summary>
		/// 解密Base64数据。
		/// </summary>
		/// <param name="Data">待解密数据。</param>
		/// <returns>解密后的数据。</returns>
		private string DecryptBase64(string Data)
		{
			return System.Text.Encoding.GetEncoding("UTF-8").GetString(System.Convert.FromBase64String(Data));
		}
	}
}


