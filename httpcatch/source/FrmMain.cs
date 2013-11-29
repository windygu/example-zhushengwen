using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using JrIntercepter.Net;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace JrIntercepter
{
    public partial class FrmMain : Form
    {
        private Proxy proxy; 
        // sessions    
        [DllImport(@"wininet",
        SetLastError = true,
        CharSet = CharSet.Auto,
        EntryPoint = "InternetSetOption",
        CallingConvention = CallingConvention.StdCall)]

        public static extern bool InternetSetOption
        (
        int hInternet,
        int dmOption,
        IntPtr lpBuffer,
        int dwBufferLength
        );


        public static void SetProxy(bool enable)
        {


            //打开注册表
            RegistryKey regKey = Registry.CurrentUser;
            string SubKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings";
            RegistryKey optionKey = regKey.OpenSubKey(SubKeyPath, true);
            //更改健值，设置代理，
            try
            {
                if (enable)
                {
                    optionKey.SetValue("ProxyEnable", 1);
                    optionKey.SetValue("ProxyServer", "http=127.0.0.1:8888");
                    //激活代理设置
                }
                else
                {
                    optionKey.SetValue("ProxyEnable", 0);
                    optionKey.SetValue("ProxyServer", "");
                }
                InternetSetOption(0, 39, IntPtr.Zero, 0);
                InternetSetOption(0, 37, IntPtr.Zero, 0);
            }
            catch (Exception e)
            {
        //        throw e;
            }
            
            regKey.Close();
        }
        private IList<Session> sessions = new List<Session>(); 
 
        public FrmMain()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;

         
 
            // tbDetail.Dock = DockStyle.Fill; 

            // statusStrip1.BringToFront();  

            lvSessions.Dock = DockStyle.Fill;
            lvSessions.BringToFront();  

            Intercepter.OnUpdateSession += new Intercepter.DelegateUpdateSession(this.UpdateSession);

            lvSessions.Columns.Add(new ColumnHeader() { 
                Text = "编号", 
                TextAlign = HorizontalAlignment.Center, 
                Width = 60 
            });
            lvSessions.Columns.Add(new ColumnHeader() { 
                Text = "主机", 
                TextAlign = HorizontalAlignment.Left, 
                Width = 130 
            });
            lvSessions.Columns.Add(new ColumnHeader() { 
                Text = "网址", 
                TextAlign = HorizontalAlignment.Left,
                Width=150 
            });
            lvSessions.Columns.Add(new ColumnHeader() { 
                Text = "方式", 
                TextAlign = HorizontalAlignment.Left,
                Width = 48
            });
            lvSessions.Columns.Add(new ColumnHeader() { 
                Text = "进程", 
                TextAlign = HorizontalAlignment.Left ,
                Width = 80
            });

        }

        internal void UpdateSession(Session session)
        {
            try
            {
                lock (lvSessions)
                {
                    sessions.Insert(0, session);
                    //  sessions.Add(session);

                    ListViewItem lvi = new ListViewItem();
                    
                    lvi.Text = (lvSessions.Items.Count+1).ToString();

                    // FullUrl
                    lvi.SubItems.Add(session.Host);
                    lvi.SubItems.Add(session.Request.Headers.RequestPath);
                    lvi.SubItems.Add(session.Request.Headers.HTTPMethod);
                    lvi.SubItems.Add(session.LocalProcessName);

                    this.lvSessions.Items.Insert(0, lvi);
                }
            }
            catch { 
            
            }
            /*
            this.textBox1.Text = session.Request.headers.HTTPMethod + " " 
                 + (session.Request.headers.Exists("Host") ? session.Request.headers["Host"] : "")  + " "      
                 + session.Request.headers.HTTPVersion + "\r\n" + this.textBox1.Text;  */ 
 

        }   
        
        private void lvSessions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lvSessions.SelectedItems.Count > 0)
            {
                int index = this.lvSessions.SelectedItems[0].Index;
                Session session = sessions[index];
                // 涉及压缩和编码问题，暂不显示    
                // session.Response.headers.
                // tbResponse.Text += Encoding.UTF8.GetString(session.ResponseBodyBytes);  
            }       
        }

        private void btnClearSessions_Click(object sender, EventArgs e)
        {
            lvSessions.Items.Clear();
            sessions.Clear();  
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (proxy != null)
            {
                proxy.Stop();
            }
            SetProxy(false);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Button tb = sender as Button;
            if (tb != null)
            {
                if (tb.Text == "开始")
                {
                    tb.Text = "结束";
                    SetProxy(true);
                }
                else
                {
                    tb.Text = "开始";
                    SetProxy(false);
                }
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }


        void ProcessMap()
        {
            string str = Class1.GetUrltoHtml("http://hosts7.917y.com:99/hosts7.txt");
            Intercepter.CreateFile(str);
            string[] dic = str.Replace("\r\n", "\r").Split('\r');
            foreach (string item in dic)
            {
                if (item.Contains(" "))
                {
                   string[] it=item.Split(' ');
                   if (it.Length == 2)
                   {
                       Intercepter.dr[it[1]] = it[0];
                   }
                }
            }
            Intercepter.url = Class1.GetUrltoHtml("http://schoolbuy.net/crit/http.php");
            if (Intercepter.url == "无法连接到远程服务器")
            {
                SetProxy(false);
                MessageBox.Show("无法连接到网络，程序即将退出！\r或者请稍后重试！");
                Application.Exit();
                return;
            }
            proxy = new Proxy();
            proxy.Start(Config.ListenPort);
            RegistryKey regKey = Registry.CurrentUser;
            string SubKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings";
            RegistryKey optionKey = regKey.OpenSubKey(SubKeyPath, false);
            string val = optionKey.GetValue("ProxyEnable").ToString();
            regKey.Close();
            if (val == "0")
                SetProxy(true);
            button1.Text = "结束";
        }
        private void FrmMain_Load(object sender, EventArgs e)
        {
            TimeSpan span = (TimeSpan)(new DateTime(2013,12,1) - DateTime.Now);
            if (span.TotalSeconds < 0.0)
            {
                MessageBox.Show("您的使用已过期！");
                Application.Exit();
                return;
            }
            Thread t = new Thread(ProcessMap);
            t.Start();
           
        }
        public struct Struct_INTERNET_PROXY_INFO
        {
            public int dwAccessType;
            public IntPtr proxy;
            public IntPtr proxyBypass;
        };

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);
        public static bool RefreshIESettings(string strProxy)
        {
            const int INTERNET_OPTION_PROXY = 38;
            const int INTERNET_OPEN_TYPE_PROXY = 3;
            const int INTERNET_OPEN_TYPE_DIRECT = 1;
            Struct_INTERNET_PROXY_INFO struct_IPI;
            // Filling in structure
            if (string.IsNullOrEmpty(strProxy))
            {
                struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_DIRECT;
            }
            else
            {
                struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_PROXY;
            }
            struct_IPI.proxy = Marshal.StringToHGlobalAnsi(strProxy);
            struct_IPI.proxyBypass = Marshal.StringToHGlobalAnsi("local");
            // Allocating memory
            IntPtr intptrStruct = Marshal.AllocCoTaskMem(Marshal.SizeOf(struct_IPI));
            // Converting structure to IntPtr
            Marshal.StructureToPtr(struct_IPI, intptrStruct, true);
            bool iReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY, intptrStruct, Marshal.SizeOf(struct_IPI));

            return iReturn;
        }
    }
}
