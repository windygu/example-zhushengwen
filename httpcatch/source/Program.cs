using System;
using System.Collections.Generic;

using System.Reflection;
using System.Windows.Forms;

namespace JrIntercepter
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool flag = false;
            System.Threading.Mutex mutex = new System.Threading.Mutex(
                true,
                Assembly.GetExecutingAssembly().FullName,
                out flag
            );
            if (!flag)
            {
                MessageBox.Show("本程序已经有一个实例在运行了!", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.Exit(1);
            }  
            Application.EnableVisualStyles();
            // Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());  
        }
    }
}
