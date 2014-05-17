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

///���ܣ����ݵ��� �ı� excel
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����


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
            //�����жϵ�ǰ��Ҫ����������
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

        //����Excel
        private void ExportExcel1()
        {
            // ����Ҫʹ�õ�Excel ����ӿ�
            // ����Application ����,�˶����ʾ����Excel ����

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
                //��ʼ�� Application ���� excelApp
                excelApp = new Microsoft.Office.Interop.Excel.Application();
                workBook = excelApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                ws = (Worksheet)workBook.Worksheets[1];

                // ���������������Ϊ "Task Management"
                ws.Name = "Soukey��������";


                // �������ݱ��е�������
                for ( i = 0; i < m_pData.Columns.Count; i++)
                {

                    ws.Cells[row, cell] = m_pData.Columns[i].ColumnName; 
                    r = (Range)ws.Cells[row, cell];
                    ws.get_Range(r, r).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

                    cell++;

                }

                Count = m_pData.Rows.Count;

                // ������,��������ͼ��¼�������Ӧ��Excel ��Ԫ��
                for ( i = 0; i < m_pData.Rows.Count; i++)
                {
                    for (int j = 0; j < m_pData.Columns.Count; j++)
                    {
                        ws.Cells[i + 2, j + 1] = m_pData.Rows[i][j];
                        Range rg = (Range)ws.get_Range(ws.Cells[i + 2, j + 1], ws.Cells[i + 2, j + 1]);
                        rg.EntireColumn.ColumnWidth = 20;
                        rg.NumberFormatLocal = "@";
                    }

                    //���½�������Ϣ
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
                
                //���½�����Ϊ��ɣ������ݲ�����ʾ�����������
                m_sender.BeginInvoke(m_senderDelegate, new object[] { Count , i, true });

            }

            return ;

        }

        //�����ı��ļ�
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
                //д���� 
                for ( i = 0; i < m_pData.Columns.Count; i++)
                {
                    str += "\t";
                    str += m_pData.Columns[i].ColumnName;
                }
              
                sw.WriteLine(str);

                Count = m_pData.Rows.Count;
                //д���� 
                for (i = 0; i < m_pData.Rows.Count; i++)
                {
                    for (int j = 0; j < m_pData.Columns.Count; j++)
                    {

                        tempStr += "\t";
                        tempStr += m_pData.Rows[i][j];
                    }
                    sw.WriteLine(tempStr);
                    tempStr = "";

                    //���½�������Ϣ
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

                //���½�����Ϊ��ɣ������ݲ�����ʾ�����������
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
                //д���� 
                for (i = 0; i < m_pData.Columns.Count; i++)
                {
                    str += "\t";
                    str += m_pData.Columns[i].ColumnName;
                }

                sw.WriteLine(str);

                Count = m_pData.Rows.Count;
                //д���� 
                for (i = 0; i < m_pData.Rows.Count; i++)
                {
                    for (int j = 0; j < m_pData.Columns.Count; j++)
                    {

                        tempStr += "\t";
                        tempStr += @m_pData.Rows[i][j].ToString();

                    }
                    sw.WriteLine(tempStr);
                    tempStr = "";

                    //���½�������Ϣ
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

                //���½�����Ϊ��ɣ������ݲ�����ʾ�����������
                m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i, true });

            }

            return;
        }
    }
}
