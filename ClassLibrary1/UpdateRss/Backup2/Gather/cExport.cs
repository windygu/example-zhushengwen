using System;
using System.Collections.Generic;
using System.Text;
using System.Data ;
using Microsoft.Office;
using Microsoft.Office.Interop.Excel;
//using Interop.Excel;
using System.IO;
using System.Windows.Forms;
using System.Threading;

///功能：数据导出 文本 excel
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无


namespace SoukeyNetget.Gather
{
    class cExport
    {
        ContainerControl m_sender = null;
        int m_total = 0;
        Delegate m_senderDelegate = null;
        cGlobalParas.PublishType m_pType=cGlobalParas.PublishType.NoPublish ;
        string m_FileName = "";
        System.Data.DataTable m_pData;

        public cExport()
        {
        }

        ~cExport()
        {
        }

        public cExport(ContainerControl sender, Delegate senderDelegate, cGlobalParas.PublishType pType,string FileName,System.Data.DataTable pData )
        {
            m_sender = sender;
            m_senderDelegate = senderDelegate;
            m_pType =pType ;
            m_FileName = FileName;
            m_pData = pData;
        }

        public void RunProcess(object obj)
        {
            Thread.CurrentThread.IsBackground = true; //make them a daemon
            object[] objArray = (object[])obj;
            m_sender = (System.Windows.Forms.Form)objArray[0];
            m_senderDelegate = (System.Delegate)objArray[1];
            m_total = (int)objArray[2];

            LocalRunProcess();
        }

        /// <summary>
        /// Method for ThreadStart delegate
        /// </summary>
        public void RunProcess()
        {
            Thread.CurrentThread.IsBackground = true; //make them a daemon
            LocalRunProcess();
        }

        private void LocalRunProcess()
        {
            //首先判断当前需要发布的类型
            switch (m_pType)
            {
                case cGlobalParas.PublishType.PublishAccess :
                    
                    break;
                case cGlobalParas.PublishType.PublishExcel :
                    ExportExcel();
                    break;
                case cGlobalParas.PublishType.PublishTxt :
                    ExportTxt();
                    break;
                default :
                    break;
            }
        }

        //导出Excel
        private void ExportExcel1()
        {
            // 定义要使用的Excel 组件接口
            // 定义Application 对象,此对象表示整个Excel 程序

            Microsoft.Office.Interop.Excel.Application excelApp = null;
            Microsoft.Office.Interop.Excel.Workbook workBook = null;
            Microsoft.Office.Interop.Excel.Worksheet ws = null;
            Microsoft.Office.Interop.Excel.Range r;
            int row = 1; 
            int cell = 1;
            int i = 0;
            int Count = 0;

            try
            {
                //初始化 Application 对象 excelApp
                excelApp = new Microsoft.Office.Interop.Excel.Application();
                workBook = excelApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                ws = (Worksheet)workBook.Worksheets[1];

                // 命名工作表的名称为 "Task Management"
                ws.Name = "Soukey导出数据";


                // 遍历数据表中的所有列
                for ( i = 0; i < m_pData.Columns.Count; i++)
                {

                    ws.Cells[row, cell] = m_pData.Columns[i].ColumnName; 
                    r = (Range)ws.Cells[row, cell];
                    ws.get_Range(r, r).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

                    cell++;

                }

                Count = m_pData.Rows.Count;

                // 创建行,把数据视图记录输出到对应的Excel 单元格
                for ( i = 0; i < m_pData.Rows.Count; i++)
                {
                    for (int j = 0; j < m_pData.Columns.Count; j++)
                    {
                        ws.Cells[i + 2, j + 1] = m_pData.Rows[i][j];
                        Range rg = (Range)ws.get_Range(ws.Cells[i + 2, j + 1], ws.Cells[i + 2, j + 1]);
                        rg.EntireColumn.ColumnWidth = 20;
                        rg.NumberFormatLocal = "@";
                    }

                    //更新进度条信息
                    m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i, false });
                }

                workBook.SaveCopyAs(m_FileName);
                workBook.Saved = true;

            }
            catch (System.Exception )
            {
                return ;
            }
            finally
            {
                excelApp.UserControl = false;
                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ws);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workBook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                GC.Collect();
                
                //更新进度条为完成，并传递参数表示导出任务完成
                m_sender.BeginInvoke(m_senderDelegate, new object[] { Count , i, true });

            }

            return ;

        }

        //导出文本文件
        private  void ExportTxt()
        {
            FileStream  myStream = File.Open(m_FileName, FileMode.Create, FileAccess.Write, FileShare.Write);
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gb2312"));
            string str = "";
            string tempStr = "";
            int i = 0;
            int Count = 0;

            try
            {
                //写标题 
                for ( i = 0; i < m_pData.Columns.Count; i++)
                {
                    str += "\t";
                    str += m_pData.Columns[i].ColumnName;
                }
              
                sw.WriteLine(str);

                Count = m_pData.Rows.Count;
                //写内容 
                for (i = 0; i < m_pData.Rows.Count; i++)
                {
                    for (int j = 0; j < m_pData.Columns.Count; j++)
                    {

                        tempStr += "\t";
                        tempStr += m_pData.Rows[i][j];
                    }
                    sw.WriteLine(tempStr);
                    tempStr = "";

                    //更新进度条信息
                    m_sender.BeginInvoke(m_senderDelegate, new object[] { Count , i, false });
                }
               

                sw.Close();
                myStream.Close();

            }
            catch (Exception)
            {
                return ;
            }
            finally
            {
                sw.Close();
                myStream.Close();

                //更新进度条为完成，并传递参数表示导出任务完成
                m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i, true });

            }

            return ;
        }

        public void ExportExcel()
        {
            FileStream myStream = File.Open(m_FileName, FileMode.Create, FileAccess.Write, FileShare.Write);
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gb2312"));
            string str = "";
            string tempStr = "";
            int i = 0;
            int Count = 0;

            try
            {
                //写标题 
                for (i = 0; i < m_pData.Columns.Count; i++)
                {
                    str += "\t";
                    str += m_pData.Columns[i].ColumnName;
                }

                sw.WriteLine(str);

                Count = m_pData.Rows.Count;
                //写内容 
                for (i = 0; i < m_pData.Rows.Count; i++)
                {
                    for (int j = 0; j < m_pData.Columns.Count; j++)
                    {

                        tempStr += "\t";
                        tempStr += @m_pData.Rows[i][j].ToString();

                    }
                    sw.WriteLine(tempStr);
                    tempStr = "";

                    //更新进度条信息
                    m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i, false });
                }


                sw.Close();
                myStream.Close();

            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                sw.Close();
                myStream.Close();

                //更新进度条为完成，并传递参数表示导出任务完成
                m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i, true });

            }

            return;
        }
    }
}
