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
namespace InputPhones
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        DirectoryInfo[] GetDirList(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            return di.GetDirectories();
        }
        FileInfo[] GetFileList(string path)
        {
            if (!Directory.Exists(path)) return null;
            DirectoryInfo di = new DirectoryInfo(path);
            return di.GetFiles();
        }
        bool InsertItem(string phone_number, string login_time, string online_time, string comm_flows)
        {
            string sql = "select * from data_users_flows where phone_number='" + phone_number + "' and longin_time='" + login_time + "'";
            if (!MyClass.SqlExists(sql))
            {
                sql = "INSERT INTO data_users_flows(phone_number ,longin_time ,online_time ,comm_flows ) VALUES ('{0}','{1}','{2}','{3}')"; 
            }
            else
            {
                sql = "update data_users_flows set online_time = '{2}',comm_flows = '{3}' where  phone_number='{0}' and longin_time='{1}'";

            }
            sql = string.Format(sql, phone_number, login_time, online_time, comm_flows);
            Console.WriteLine(sql);
            return MyClass.ExecuteNonQuery(sql);
        }
        public delegate void SetTextDelegate(string phone_number, string login_time, string online_time, string comm_flows);
        public void SetText(string phone_number, string login_time, string online_time, string comm_flows)
        {
            label2.Text = phone_number;
            label4.Text = login_time;
            label6.Text = online_time;
            label8.Text = comm_flows;
        }
        void InputInfoToDataBase(string file)
        {
            FileInfo fi = new FileInfo(file);
            if (!fi.Exists) return;
            string PhoneNum = fi.Directory.Name;
            Excel.Application excel;						//声明excel对象

			excel=new Excel.ApplicationClass();				//创建对象实例,这时在系统进程中会多出一个excel进程

				object missing=System.Reflection.Missing.Value;					//Missing 用于调用带默认参数的方法。		
				object readOnly=true;											
				excel.Visible=false;											//是否显示excel文档

				//Open Original Excel File
				excel.Application.Workbooks.Open(file,missing,readOnly,missing,missing,missing,missing,missing,missing,missing,missing,missing,missing,missing,missing);			
				
				Excel.Workbook myBook=excel.Workbooks[1];					   //Workbooks从1开始计数的
				Excel.Worksheet mySheet=(Excel.Worksheet)myBook.Worksheets[1]; //从1开始计数的
                int i = 1;
                int j = 1;
                string date;
                while (!((date=((Excel.Range)mySheet.Cells[i, 1]).Text.ToString())).Contains("-"))
                {
                    i++;
                }
                i++;
                string time=((Excel.Range)mySheet.Cells[i, 1]).Text.ToString();

                while (time.Contains("-") || time.Contains(":"))
                {
                    while(time.Contains(":"))
                    {
                        string date_time=date+" "+time;
                        //DateTime dt=DateTime.Parse(date_time);
                       // Console.WriteLine(file);
                       // label2.Text = PhoneNum;
                        //label4.Text = date_time;
                        string tl=((Excel.Range)mySheet.Cells[i, 4]).Text.ToString();
                        string ll = ((Excel.Range)mySheet.Cells[i, 5]).Text.ToString();
                       // label6.Text=tl;
                       // label8.Text=ll;
                        label2.Invoke(new SetTextDelegate(SetText), PhoneNum, date_time, tl, ll);

                        InsertItem(PhoneNum, date_time,tl ,ll );

                        i++;
                        time = ((Excel.Range)mySheet.Cells[i, 1]).Text.ToString();
                    }
                    if (time.Contains("-"))
                    {
                        date = time;
                    }
                    else
                    {
                        break;
                    }
                    i++;
                    time = ((Excel.Range)mySheet.Cells[i, 1]).Text.ToString();
                }
						 //设置该矩形框的文本格式
			

				//Save As  Original Excel File To CurrentPath
			

				//释放Excel对象,但在Asp.net Web程序中只有转向另一个页面的时候进程才结束
				//可以考虑使用KillExcelProcess()杀掉进程
				//ReleaseComObject 方法递减运行库可调用包装的引用计数。详细信息见MSDN
				System.Runtime.InteropServices.Marshal.ReleaseComObject(myBook);
				System.Runtime.InteropServices.Marshal.ReleaseComObject(mySheet);
				System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
				myBook.Close(null,null,null);
				excel.Workbooks.Close();
				mySheet=null;
				myBook=null;
				missing=null;
				readOnly=null;
				excel.Quit();
				excel=null;		
			

		}

        private void KillExcelProcess()
        {
            System.Diagnostics.Process[] myProcesses;
            DateTime startTime;
            myProcesses = System.Diagnostics.Process.GetProcessesByName("Excel");

            //得不到Excel进程ID，暂时只能判断进程启动时间
            foreach (System.Diagnostics.Process myProcess in myProcesses)
            {
                    myProcess.Kill();
            }
        }


        void ProcessTask()
        {
            string dir = userControl11.MyText;
            foreach (DirectoryInfo item in GetDirList(dir))
            {
                foreach (FileInfo item1 in GetFileList(item.FullName))
                {
                    InputInfoToDataBase(item1.FullName);
                }
            }
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(ProcessTask);
            t.Start();
            //mulu
            //wenjian
            //chuli
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
