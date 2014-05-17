using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using System.Resources;
using System.Reflection;

//程序入口类
namespace SoukeyNetget
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        /// 
        private static ApplicationContext context;

        [STAThread]
        static void Main()
        {
            //检测是否指定了界面语言
            CultureInfo cLanguage=null ;

            try
            {
                cXmlSConfig Config = new cXmlSConfig();
                cGlobalParas.CurLanguage cl=Config.CurrentLanguage ;
                switch (cl)
                {
                    case cGlobalParas.CurLanguage .Auto :
                        break ;
                    case cGlobalParas.CurLanguage .enUS :
                       cLanguage =new CultureInfo ("en-US");
                        break ;
                    case cGlobalParas.CurLanguage .zhCN :
                        cLanguage =new CultureInfo ("zh-CN");
                        break ;
                    default :
                        break ;
                }
                Config = null;
            }
            catch (System.Exception)
            {
                
            }

            if (cLanguage != null)
            {
                Thread.CurrentThread.CurrentUICulture = cLanguage;
                Thread.CurrentThread.CurrentCulture = cLanguage;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            frmStart sf = new frmStart();
            sf.Show();
            context = new ApplicationContext();
            context.Tag = sf;
            Application.Idle += new EventHandler(Application_Idle); //注册程序运行空闲去执行主程序窗体相应初始化代码
            Application.Run(context);
        }

        private static void Application_Idle(object sender, EventArgs e)
        {
            Application.Idle -= new EventHandler(Application_Idle);
            if (context.MainForm == null)
            {
                ResourceManager rm =new ResourceManager("SoukeyNetget.Resources.globalUI", Assembly.GetExecutingAssembly());

                frmMain mf = new frmMain ();
                context.MainForm = mf;
                frmStart sf = (frmStart)context.Tag;

                //初始化界面信息
                sf.label3.Text = rm.GetString ("Info69");
                Application.DoEvents();
                mf.IniForm();

                //初始化对象并开始启动运行区的任务
                sf.label3.Text =  rm.GetString ("Info70");
                Application.DoEvents();
                mf.UserIni();

                sf.label3.Text = rm.GetString ("Info71");
                Application.DoEvents();
                mf.StartListen();

                rm = null;
                //mf.IniForm();

                sf.Close();                                 //关闭启动窗体
                sf.Dispose();

                
                mf.Show();                                  //启动主程序窗体

            }
        }

        public static string getPrjPath()
        {
            return Application.StartupPath + "\\";

        }
    }
}