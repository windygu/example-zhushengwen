using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Reflection;
using System.Resources;

namespace SoukeyNetget
{
    public partial class frmUpdate : Form
    {
        private  string Old_Copy;
        private string New_Copy;
        private string SCode = "";
        private bool OnShow = false;

        private ResourceManager rm;

        //����һ�����������������°汾ʱ���Բ���ˢ��ҳ��
        //�ȴ���Ϣ���Ը����û��������������������ϵͳ����
        delegate void IsDownloadDelegate(bool Done);

        private bool IsDownloading = false;

        public frmUpdate()
        {
            InitializeComponent();
        }

        private void GetCopy()
        {
            Gather.cGatherWeb gData = new Gather.cGatherWeb();

            this.textBox1.Text =rm.GetString ("Info90");
            Application.DoEvents();
            Old_Copy = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.textBox1.Text += "\r\n" + rm.GetString("Info91") + Assembly.GetExecutingAssembly().GetName().Version;
            Application.DoEvents();
            this.textBox1.Text += "\r\n" + rm.GetString("Info92");
            Application.DoEvents();
            SCode = cTool.GetHtmlSource("http://www.yijie.net/user/soft/updatesoukey.html", true);
            if (SCode == "" || SCode == null)
            {
                this.textBox1.Text += "\r\n" + rm.GetString("Info93") + "\r\n" + rm.GetString("Info94");
                Application.DoEvents();
                return;
            }

            this.textBox1.Text += "\r\n" + rm.GetString("Info95") + "\r\n" + rm.GetString("Info96");
            Application.DoEvents();

            //���Ӳɼ��ı�־
            Task.cWebpageCutFlag c;

            c = new Task.cWebpageCutFlag();
            c.id =0;
            c.Title = "�汾";
            c.DataType =(int) cGlobalParas.GDataType.Txt;
            c.StartPos = "�汾��";
            c.EndPos = "</p>";
            c.LimitSign =(int) cGlobalParas.LimitSign.NoLimit;
            gData.CutFlag.Add(c);
            c = null;

            //���Ӱ汾˵���ı�־
            c = new Task.cWebpageCutFlag();
            c.id = 1;
            c.Title = "˵��";
            c.DataType = (int)cGlobalParas.GDataType.Txt;
            c.StartPos = "˵����";
            c.EndPos = "</p>";
            c.LimitSign = (int)cGlobalParas.LimitSign.NoLimit;
            gData.CutFlag.Add(c);
            c = null;


            DataTable dGather = gData.GetGatherData("http://www.yijie.net/user/soft/updatesoukey.html", cGlobalParas.WebCode.utf8, "", "", "", Program.getPrjPath(),false);

            New_Copy = dGather.Rows[0][0].ToString();
            this.textBox1.Text += "\r\n" + rm.GetString("Info97") + New_Copy;
            Application.DoEvents();

            ///�汾�űȽ���Ҫ�Ƚ��������00.00.00�����а汾�������մ˸�ʽ���������ִ���
            ///�Ƚ�˳��Ϊ�����汾->�Ͱ汾��ֻҪ��һ���°汾�Ŵ��ھɰ汾�ţ���ͽ�����������

            int Old_V;
            int New_V;

            for (int i = 0; i < 3; i++)
            {
                Old_V=int.Parse ( Old_Copy .Substring(0,Old_Copy .IndexOf (".")));
                Old_Copy =Old_Copy .Substring (Old_Copy .IndexOf (".")+1,Old_Copy .Length -Old_Copy .IndexOf (".")-1);

                New_V = int.Parse(New_Copy.Substring(0, New_Copy.IndexOf(".")));
                New_Copy = New_Copy.Substring(New_Copy.IndexOf(".")+1, New_Copy.Length - New_Copy.IndexOf(".")-1);

                if (New_V >Old_V )
                {
                    this.textBox1.Text += "\r\n" + rm.GetString("Info98");
                    Application.DoEvents();

                    this.textBox1.Text += "\r\n" + dGather.Rows [0][1].ToString ();
                    Application.DoEvents();

                    gData = null;

                    this.button2.Enabled = true;
                    this.button1.Enabled = true;
                    return;
                }
            }

            this.textBox1.Text += "\r\n" + rm.GetString("Info99");
            Application.DoEvents();

            this.button1.Enabled = true;

        }

        ContainerControl m_sender = null;
        Delegate m_senderDelegate = null;

        private void IsDownloadSoft( bool done)
        {

            if (done)
            {
                this.textBox1.Text += "\r\n" + rm.GetString("Info100");
                Application.DoEvents();

                this.button1.Enabled = true;

            }
        }

        private void DownloadSoft()
        {
            Gather.cGatherWeb gData = new Gather.cGatherWeb();

            //���Ӳɼ��ı�־
            Task.cWebpageCutFlag c;

            c = new Task.cWebpageCutFlag();
            c.id = 0;
            c.Title = "�汾";
            c.DataType = (int)cGlobalParas.GDataType.File;
            c.StartPos = "<a href=\"";
            c.EndPos = "\"";
            c.LimitSign = (int)cGlobalParas.LimitSign.NoLimit;
            gData.CutFlag.Add(c);
            c = null;


            DataTable dGather = gData.GetGatherData("http://www.yijie.net/user/soft/updatesoukey.html", cGlobalParas.WebCode.utf8, "", "", "", Program.getPrjPath(),false);

            dGather = null;
            gData = null;

            m_sender.BeginInvoke(m_senderDelegate, new object[] { true });

        }

        private void frmUpdate_Activated(object sender, EventArgs e)
        {
            if (OnShow == false)
            {
                OnShow = true;

                try
                {
                    GetCopy();
                }
                catch (System.Exception ex)
                {
                    this.textBox1.Text += "\r\n" + rm.GetString("Info101") + ex.Message;
                    Application.DoEvents();
                }

                if (IsDownloading == false)
                {
                    this.button1.Enabled = true;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.textBox1.Text += "\r\n" + rm.GetString("Info102");
            Application.DoEvents();

            this.button2.Enabled = false;
            this.button1.Enabled = false;


            //����һ����̨�߳����ڵ������ݲ���
            IsDownloadDelegate IsDownload = new IsDownloadDelegate(IsDownloadSoft);
            m_sender = this;
            m_senderDelegate = IsDownload;

            IsDownloading = true;

            Thread t = new Thread(new ThreadStart(DownloadSoft));
            t.IsBackground = true;
            t.Start();


            return;
        }

        private void frmUpdate_Load(object sender, EventArgs e)
        {
            rm = new ResourceManager("SoukeyNetget.Resources.globalUI", Assembly.GetExecutingAssembly());
        }

        private void frmUpdate_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }
    }
}