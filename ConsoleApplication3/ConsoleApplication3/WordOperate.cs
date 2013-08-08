﻿using System;
using System.Collections.Generic;
using Microsoft.Office.Interop.Word;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace ConsoleApplication3
{

    public struct Point
    {
        public Point(int i, int j)
        {
            row = i; col = j;
        }
        public int row;
        public int col;
    }
    public enum TypeOfElement
    {
        CONST,
        STRING,
        ARRAY
    }
    public class CellElement
    {
        public TypeOfElement type = TypeOfElement.STRING;
        public object obj;
        public Point point;
    }
    public struct ColAContent
    {
        public ColAContent(int c, string con)
        {
            col = c;
            content = con;
        }
        public int col;
        public string content;

    }
    public class WordOperate
    {
        static string Trim(string str)
        {
            char[] tchr = new char[] { '\r', '\a', ' ' };
            return str.Trim(tchr);
        }
        static string path = AppDomain.CurrentDomain.BaseDirectory;
        static string docpath = Path.Combine(path, "a.doc");

        public static void FillElement(ref Table mt, ref Dictionary<string, CellElement> dic)
        {

            foreach (KeyValuePair<string, CellElement> item in dic)
            {

            }

        }
        #region 动态生成Word文档并填充数据
        /**/
        /// <summary>
        /// 动态生成Word文档并填充数据 
        /// </summary>
        /// <returns>返回自定义信息</returns>
        public static string CreateWordFile()
        {
            string message = "";
            //try
            //{
                Object oMissing = System.Reflection.Missing.Value;
                string dir = AppDomain.CurrentDomain.BaseDirectory;//System.Web.HttpContext.Current.Server.MapPath("");//首先在类库添加using System.web的引用
                if (!Directory.Exists(dir + "\\file"))
                {
                    Directory.CreateDirectory(dir + "\\file");  //创建文件所在目录
                }
                string name = DateTime.Now.ToLongDateString() + ".doc";
                object filename = dir + "\\file\\" + name;  //文件保存路径
                //创建Word文档
                Microsoft.Office.Interop.Word.Application WordApp = new Microsoft.Office.Interop.Word.ApplicationClass();
                //Microsoft.Office.Interop.Word.Document WordDoc = WordApp.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                Microsoft.Office.Interop.Word.Document WordDoc = WordApp.Documents.Open(@"C:\Documents and Settings\Administrator\桌面\work\1.doc");


                /**/
                ////添加页眉方法一：
                WordApp.ActiveWindow.View.Type = WdViewType.wdNormalView;
                #region 生成模板
                Dictionary<string, CellElement> Dic = new Dictionary<string, CellElement>();
                Tables tTs = WordDoc.Tables;
                {
                    Table tT = WordDoc.Tables[1];
                    int C = 1, A = 1;
                    int ARow = 0;
                    for (int i = 1; i <= tT.Rows.Count; i++)
                    {
                        int ci = 0;
                        WdColor wcc = tT.Cell(i, 1).Range.Shading.BackgroundPatternColor;
                        for (int k = 1; k <= tT.Rows[i].Cells.Count; k++)
                        {
                            WdColor wci = tT.Cell(i, k).Range.Shading.BackgroundPatternColor;
                            if (wci == wcc && wcc != WdColor.wdColorGray125) { ci++; } else break;
                        }
                        if (ci == tT.Rows[i].Cells.Count)
                        {
                            if (ARow == i - 1)
                            {
                                ARow = i;
                                continue;
                            }
                            else
                            {
                                ARow = i;
                            }
                        }
                        for (int j = 1; j <= tT.Rows[i].Cells.Count; j++)
                        {

                            if (ci == tT.Rows[i].Cells.Count)
                            {
                                CellElement ce = new CellElement();
                                ce.point = new Point(i, 0);
                                ce.type = TypeOfElement.ARRAY;
                                Dic.Add("{" + A + "}[]", ce);
                                A++;
                                break;
                            }
                            else
                            {
                                WdColor wc = tT.Cell(i, j).Range.Shading.BackgroundPatternColor;
                                CellElement ce = new CellElement();
                                ce.point = new Point(i, j);
                                ce.obj = Trim(tT.Cell(i, j).Range.Text);
                                if (wc == WdColor.wdColorGray125)
                                {
                                    ce.type = TypeOfElement.CONST;
                                    Dic.Add("C" + C, ce);
                                    C++;
                                }
                                else
                                {
                                    ce.type = TypeOfElement.STRING;
                                    Dic.Add("{" + A + "}", ce);
                                    A++;
                                }

                            }
                        }
                    }
                }
                #endregion

                Microsoft.Office.Interop.Word.Document WordDoc2 = WordApp.Documents.Open(@"C:\Documents and Settings\Administrator\桌面\work\3.doc");
                Tables mTs = WordDoc2.Tables;
                int TotleCount = WordDoc.Tables.Count;
                    object msta = 0;
                    object mend = mTs[1].Range.Start;
                    Range mr = WordDoc2.Range(msta, mend);
                    int index = 0;
                for (int x = 1; x <= TotleCount; x++)
                {

                    //进行条件筛选

                    if (!tTs[x].Range.Text.Contains("latitude")) continue;

                    Table t1 = WordDoc2.Tables[1];
                    Range wr1 = WordDoc2.Range(0, WordDoc2.Tables[1].Range.End);
                    wr1.Select();
                    wr1.Copy();
                    Range wr = WordDoc2.Range(WordDoc2.Content.End - 1);

                    /*
                    object count = 0;
                    object WdLine = Microsoft.Office.Interop.Word.WdUnits.wdLine;//换一行;
                    WordApp.Selection.MoveDown(ref WdLine, ref count, ref oMissing);//移动焦点
                    WordApp.Selection.TypeParagraph();//插入段落
                     */
                    wr.Paste();

                  

                    int Count = WordDoc2.Tables.Count;
                    Table mT = mTs[Count];

                    //确定标题常量
                    object sta = 0;
                    object end = tTs[x].Range.Start;
                    if (x != 1) sta = tTs[x - 1].Range.End;
                    Range wr2 = WordDoc.Range(sta, end);

                     sta = 1;
                     end = mTs[Count].Range.Start-1;
                     if (Count != 1) sta = mTs[Count - 1].Range.End+1;
                    Range wr3 = WordDoc2.Range(sta, end);
                    index++;
                    wr3.Text = wr3.Text.Replace("{C0}", Trim(wr2.Text)).Replace("{i}", index.ToString()+"  "); ;

                    Console.WriteLine(wr3.Text);

                    wr3.Font = mr.Font;
                    wr3.Font.Name = "微软雅黑";
                    {
                       
                        for (int i = 1; i <= mT.Rows.Count; i++)
                        {

                            #region 数组情况
                            if (mT.Cell(i, 1).Range.Text.Contains("}["))
                            {
                                int AL = 0;
                                int col = 0;
                                string content = Trim(mT.Cell(i, 1).Range.Text);
                                int i1 = content.IndexOf("{");
                                int i2 = content.IndexOf("}");

                                if (i1 == -1 || i2 == -1) continue;
                                string rowstr = content.Substring(i1 + 1, i2 - i1 - 1);
                                int Row = 0;
                                if (content.IndexOf("}[") != -1)
                                {
                                    Row = Dic["{" + rowstr + "}" + "[]"].point.row;
                                }
                                else
                                {
                                    Row = Dic["{" + rowstr + "}"].point.row;
                                }

                                //遍历母版表,获取正确附加行号 AL
                                Table tT = tTs[x];
                                int IndexRow = 1;//记录相对行号
                                for (int ia = 1; ia <= tT.Rows.Count && IndexRow < Row; ia++)
                                {
                                    int ci = 0;
                                    int ARowa = 0;
                                    WdColor wcc = tT.Cell(ia, 1).Range.Shading.BackgroundPatternColor;
                                    for (int k = 1; k <= tT.Rows[i].Cells.Count; k++)
                                    {
                                        WdColor wci = tT.Cell(ia, k).Range.Shading.BackgroundPatternColor;
                                        if (wci == wcc && wcc != WdColor.wdColorGray125) { ci++; } else break;
                                    }
                                    if (ci == tT.Rows[ia].Cells.Count)//该行为数组
                                    {
                                        if (ARowa == ia - 1)
                                        {
                                            AL++;
                                            ARowa = ia;
                                            continue;
                                        }
                                        else
                                        {
                                            ARowa = ia;
                                        }
                                    }
                                    IndexRow++;

                                }


                                //根据附加行号以及行号标识,获取数组行数 
                                int ARR = 0;
                                for (int ia = Row + AL; ia <= tT.Rows.Count; ia++)
                                {
                                    int ci = 0;
                                    WdColor wcc = tT.Cell(ia, 1).Range.Shading.BackgroundPatternColor;
                                    for (int k = 1; k <= tT.Rows[ia].Cells.Count; k++)
                                    {
                                        WdColor wci = tT.Cell(ia, k).Range.Shading.BackgroundPatternColor;
                                        if (wci == wcc && wcc != WdColor.wdColorGray125) { ci++; } else break;
                                    }
                                    if (ci == tT.Rows[ia].Cells.Count)//该行为数组
                                    {
                                        ARR++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }


                                string colstr = "";

                                //解析数组数字

                                ColAContent[] arri = new ColAContent[mT.Rows[i].Cells.Count];

                                for (int k = 1; k <= mT.Rows[i].Cells.Count; k++)
                                {
                                    string colcontent = Trim(mT.Cell(i, k).Range.Text);
                                    int i3 = colcontent.IndexOf("[");
                                    int i4 = colcontent.IndexOf("]");
                                    col = 1;
                                    //解析列值
                                    if (-1 != i3 && -1 != i4)
                                    {
                                        i3++;
                                        try
                                        {
                                            colstr = colcontent.Substring(i3, i4 - i3);
                                            col = int.Parse(colstr);
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                    arri[k - 1] = new ColAContent(col, colcontent);
                                }

                                for (int ar = 1; ar <= ARR; ar++)
                                {
                                    if (ar != 1)
                                    {
                                        object beforeRow = mT.Rows[i + ar -1];
                                        mT.Rows.Add(ref beforeRow);
                                     }
                                    for (int ai = 0; ai < arri.Length; ai++)
                                    {
                                        Cell ce=mT.Cell(i + ar - 1, ai + 1);
                                        if(ar!=1)
                                        {
                                            Cell cep=mT.Cell(i, ai + 1);
                                            ce.Range.Font = cep.Range.Font;
                                            ce.Shading.BackgroundPatternColor = cep.Shading.BackgroundPatternColor;
                                            ce.Width = cep.Width;
                                            ce.Range.ParagraphFormat.Alignment = cep.Range.ParagraphFormat.Alignment;
                                            ce.Range.Cells.Borders.OutsideLineStyle = ce.Range.Cells.Borders.OutsideLineStyle;
                                            ce.Range.Cells.Borders.InsideLineStyle = ce.Range.Cells.Borders.InsideLineStyle;
                                        }
                                        if (Dic.ContainsKey("{" + rowstr + "}[]"))
                                        {
                                            int a = Dic["{" + rowstr + "}[]"].point.row + AL;
                                            int b = arri[ai].col;
                                            string newContent = Trim(tTs[x].Cell(a, b).Range.Text);
                                            ce.Range.Text = arri[ai].content.Replace("{" + rowstr + "}[" + arri[ai].col + "]", newContent);
                                        }

                                    }
                                    AL++;
                                }
                            }
                            #endregion
                            else
                            {
                               
                                for (int j = 1; j <= mT.Rows[i].Cells.Count; j++)
                                {

                                    string content = Trim(mT.Cell(i, j).Range.Text);
                                    int i1 = content.IndexOf("{");
                                    int i2 = content.IndexOf("}");


                                    if (i1 == -1 || i2 == -1) continue;
                                    string rowstr = content.Substring(i1 + 1, i2 - i1 - 1);




                                    #region  常数情况
                                    if (rowstr.StartsWith("C"))
                                    {
                                        if (Dic.ContainsKey("{" + rowstr + "}"))
                                        {
                                            string Cstr = (string)Dic["{" + rowstr + "}"].obj;
                                            mT.Cell(i, j).Range.Text = content.Replace("{" + rowstr + "}", Cstr);
                                        }
                                        continue;
                                    };
                                    #endregion

                                    int AL = 0;
                                    


                                    int Row = 0;
                                    if (content.IndexOf("}[") != -1)
                                    {
                                        Row = Dic["{" + rowstr + "}" + "[]"].point.row;
                                    }
                                    else
                                    {
                                        Row = Dic["{" + rowstr + "}"].point.row;
                                    }

                                    //遍历母版表,获取正确附加行号 AL
                                    Table tT = tTs[x];
                                    int IndexRow = 1;//记录相对行号
                                    for (int ia = 1; ia <= tT.Rows.Count && IndexRow < Row; ia++)
                                    {
                                        int ci = 0;
                                        int ARowa = 0;
                                        WdColor wcc = tT.Cell(ia, 1).Range.Shading.BackgroundPatternColor;
                                        for (int k = 1; k <= tT.Rows[i].Cells.Count; k++)
                                        {
                                            WdColor wci = tT.Cell(ia, k).Range.Shading.BackgroundPatternColor;
                                            if (wci == wcc && wcc != WdColor.wdColorGray125) { ci++; } else break;
                                        }
                                        if (ci == tT.Rows[ia].Cells.Count)//该行为数组
                                        {
                                            if (ARowa == ia - 1)
                                            {
                                                AL++;
                                                ARowa = ia;
                                                continue;
                                            }
                                            else
                                            {
                                                ARowa = ia;
                                            }
                                        }
                                        IndexRow++;

                                    }
                                    #region  变量情况

                                    if (Dic.ContainsKey("{" + rowstr + "}"))
                                    {
                                        int a = Dic["{" + rowstr + "}"].point.row + AL;
                                        int b = Dic["{" + rowstr + "}"].point.col;
                                        string Cstr = Trim(tTs[x].Cell(a, b).Range.Text);
                                        mT.Cell(i, j).Range.Text = content.Replace("{" + rowstr + "}", Cstr);
                                    }


                                    #endregion
                                }
                            }
                        }
                    }
                    WordDoc2.Save();
                }

        

               
                //WordDoc.SaveAs(ref filename, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                WordDoc2.Close(ref oMissing, ref oMissing, ref oMissing);

                message = name + "文档生成成功";
            //}
            //catch
            //{
            //    message = "文件导出异常！";
            //}
            return message;
        }
        #endregion
        #region 创建并打开一个空的word文档进行编辑
        /**/
        /// <summary>
        /// 创建并打开一个空的word文档进行编辑
        /// </summary>
        public static void OpenNewWordFileToEdit()
        {
            object oMissing = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Word.Application WordApp;
            Microsoft.Office.Interop.Word.Document WordDoc;
            WordApp = new Microsoft.Office.Interop.Word.ApplicationClass();
            WordApp.Visible = true;
            WordDoc = WordApp.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);
        }
        #endregion
        #region 创建word文档
        /**/
        /// <summary>
        /// 创建word文档
        /// </summary>
        /// <returns></returns>
        public static string createWord()
        {
            Microsoft.Office.Interop.Word.Application WordApp = new Microsoft.Office.Interop.Word.ApplicationClass();
            Document WordDoc;
            string strContent = "";
            object strFileName = docpath;//System.Web.HttpContext.Current.Server.MapPath("test.doc");
            if (System.IO.File.Exists((string)strFileName))
                System.IO.File.Delete((string)strFileName);
            Object oMissing = System.Reflection.Missing.Value;
            WordDoc = WordApp.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            #region   将数据库中读取得数据写入到word文件中
            strContent = "你好\n\n\r ";
            WordDoc.Paragraphs.Last.Range.Text = strContent;
            strContent = "这是测试程序 ";
            WordDoc.Paragraphs.Last.Range.Text = strContent;
            #endregion
            //将WordDoc文档对象的内容保存为DOC文档   
            WordDoc.SaveAs(ref strFileName, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref   oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            //关闭WordDoc文档对象   
            WordDoc.Close(ref oMissing, ref oMissing, ref oMissing);
            //关闭WordApp组件对象   
            WordApp.Quit(ref oMissing, ref oMissing, ref oMissing);
            string message = strFileName + "\r\n " + "创建成功 ";
            return message;
        }
        #endregion
        #region 把Word文档装化为Html文件
        /**/
        /// <summary>
        /// 把Word文档装化为Html文件
        /// </summary>
        /// <param name="strFileName">要转换的Word文档</param>
        public static void WordToHtml(string strFileName)
        {
            string saveFileName = strFileName + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".html";
            WordToHtml(strFileName, saveFileName);
        }
        /**/
        /// <summary>
        /// 把Word文档装化为Html文件
        /// </summary>
        /// <param name="strFileName">要转换的Word文档</param>
        /// <param name="strSaveFileName">要生成的具体的Html页面</param>
        public static void WordToHtml(string strFileName, string strSaveFileName)
        {
            Microsoft.Office.Interop.Word.ApplicationClass WordApp;
            Microsoft.Office.Interop.Word.Document WordDoc;
            Object oMissing = System.Reflection.Missing.Value;
            WordApp = new Microsoft.Office.Interop.Word.ApplicationClass();
            object fileName = strFileName;

            WordDoc = WordApp.Documents.Open(ref fileName,
               ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
               ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
               ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            Type wordType = WordApp.GetType();
            // 打开文件
            Type docsType = WordApp.Documents.GetType();
            // 转换格式，另存为
            Type docType = WordDoc.GetType();
            object saveFileName = strSaveFileName;
            docType.InvokeMember("SaveAs", System.Reflection.BindingFlags.InvokeMethod, null, WordDoc, new object[] { saveFileName, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatHTML });
            #region 其它格式：
            /**/
            /**/
            /**/
            ///wdFormatHTML
            ///wdFormatDocument
            ///wdFormatDOSText
            ///wdFormatDOSTextLineBreaks
            ///wdFormatEncodedText
            ///wdFormatRTF
            ///wdFormatTemplate
            ///wdFormatText
            ///wdFormatTextLineBreaks
            ///wdFormatUnicodeText
            //-----------------------------------------------------------------------------------
            //            docType.InvokeMember( "SaveAs", System.Reflection.BindingFlags.InvokeMethod,
            //                null, WordDoc, new object[]{saveFileName, Word.WdSaveFormat.wdFormatHTML} );
            // 退出 Word
            //wordType.InvokeMember( "Quit", System.Reflection.BindingFlags.InvokeMethod,
            //    null, WordApp, null );
            #endregion
            WordDoc.Close(ref oMissing, ref oMissing, ref oMissing);
            WordApp.Quit(ref oMissing, ref oMissing, ref oMissing);
        }
        #endregion
        #region 导入模板
        /**/
        /// <summary>
        /// 导入模板
        /// </summary>
        /// <param name="filePath">模板文档路径</param>
        public static void ImportTemplate(string filePath)
        {
            object oMissing = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Word.Application WordApp;
            Microsoft.Office.Interop.Word.Document WordDoc;
            WordApp = new Microsoft.Office.Interop.Word.ApplicationClass();
            WordApp.Visible = true;
            object fileName = filePath;
            WordDoc = WordApp.Documents.Add(ref fileName, ref oMissing, ref oMissing, ref oMissing);
        }
        #endregion
        #region word中添加新表
        /**/
        /// <summary>
        /// word中添加新表
        /// </summary>
        public static void AddTable()
        {
            object oMissing = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Word.Application WordApp;
            Microsoft.Office.Interop.Word.Document WordDoc;
            WordApp = new Microsoft.Office.Interop.Word.ApplicationClass();
            WordApp.Visible = true;
            WordDoc = WordApp.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            object start = 0;
            object end = 0;
            Microsoft.Office.Interop.Word.Range tableLocation = WordDoc.Range(ref start, ref end);
            WordDoc.Tables.Add(tableLocation, 3, 4, ref oMissing, ref oMissing);//3行4列的表
        }
        #endregion
        #region 在表中插入新行
        /**/
        /// <summary>
        /// 在表中插入新的1行
        /// </summary>
        public static void AddRow()
        {
            object oMissing = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Word.Application WordApp;
            Microsoft.Office.Interop.Word.Document WordDoc;
            WordApp = new Microsoft.Office.Interop.Word.ApplicationClass();
            WordApp.Visible = true;
            WordDoc = WordApp.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            object start = 0;
            object end = 0;
            Microsoft.Office.Interop.Word.Range tableLocation = WordDoc.Range(ref start, ref end);
            WordDoc.Tables.Add(tableLocation, 3, 4, ref oMissing, ref oMissing);
            Microsoft.Office.Interop.Word.Table newTable = WordDoc.Tables[1];
            object beforeRow = newTable.Rows[1];
            newTable.Rows.Add(ref beforeRow);
        }
        #endregion
        #region 分离单元格
        /**/
        /// <summary>
        /// 合并单元格
        /// </summary>
        public static void CombinationCell()
        {
            object oMissing = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Word.Application WordApp;
            Microsoft.Office.Interop.Word.Document WordDoc;
            WordApp = new Microsoft.Office.Interop.Word.ApplicationClass();
            WordApp.Visible = true;
            WordDoc = WordApp.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            object start = 0;
            object end = 0;
            Microsoft.Office.Interop.Word.Range tableLocation = WordDoc.Range(ref start, ref end);
            WordDoc.Tables.Add(tableLocation, 3, 4, ref oMissing, ref oMissing);
            Microsoft.Office.Interop.Word.Table newTable = WordDoc.Tables[1];
            object beforeRow = newTable.Rows[1];
            newTable.Rows.Add(ref beforeRow);
            Microsoft.Office.Interop.Word.Cell cell = newTable.Cell(2, 1);//2行1列合并2行2列为一起
            cell.Merge(newTable.Cell(2, 2));
            //cell.Merge( newTable.Cell( 1, 3 ) );
        }
        #endregion
        #region 分离单元格
        /**/
        /// <summary>
        /// 分离单元格
        /// </summary>
        public static void SeparateCell()
        {
            object oMissing = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Word.Application WordApp;
            Microsoft.Office.Interop.Word.Document WordDoc;
            WordApp = new Microsoft.Office.Interop.Word.ApplicationClass();
            WordApp.Visible = true;
            WordDoc = WordApp.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            object start = 0;
            object end = 0;
            Microsoft.Office.Interop.Word.Range tableLocation = WordDoc.Range(ref start, ref end);
            WordDoc.Tables.Add(tableLocation, 3, 4, ref oMissing, ref oMissing);
            Microsoft.Office.Interop.Word.Table newTable = WordDoc.Tables[1];
            object beforeRow = newTable.Rows[1];
            newTable.Rows.Add(ref beforeRow);
            Microsoft.Office.Interop.Word.Cell cell = newTable.Cell(1, 1);
            cell.Merge(newTable.Cell(1, 2));
            object Rownum = 2;
            object Columnnum = 2;
            cell.Split(ref Rownum, ref  Columnnum);
        }
        #endregion

        #region 通过段落控制插入Insert a paragraph at the beginning of the document.
        /**/
        /// <summary>
        /// 通过段落控制插入Insert a paragraph at the beginning of the document.
        /// </summary>
        public static void Insert()
        {
            object oMissing = System.Reflection.Missing.Value;
            //object oEndOfDoc = "\\endofdoc"; /**//* \endofdoc is a predefined bookmark */
            //Start Word and create a new document.
            Microsoft.Office.Interop.Word.Application WordApp;
            Microsoft.Office.Interop.Word.Document WordDoc;
            WordApp = new Microsoft.Office.Interop.Word.ApplicationClass();
            WordApp.Visible = true;
            WordDoc = WordApp.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            //Insert a paragraph at the beginning of the document.
            Microsoft.Office.Interop.Word.Paragraph oPara1;
            oPara1 = WordDoc.Content.Paragraphs.Add(ref oMissing);
            oPara1.Range.Text = "Heading 1";
            oPara1.Range.Font.Bold = 1;
            oPara1.Format.SpaceAfter = 24;    //24 pt spacing after paragraph.
            oPara1.Range.InsertParagraphAfter();
        }
        #endregion

        #region word文档设置及获取光标位置
        /**/
        /// <summary>
        /// word文档设置及获取光标位置
        /// </summary>
        public static void WordSet()
        {
            object oMissing = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Word.Application WordApp;
            Microsoft.Office.Interop.Word.Document WordDoc;
            WordApp = new Microsoft.Office.Interop.Word.ApplicationClass();
            #region 文档格式设置
            WordApp.ActiveDocument.PageSetup.LineNumbering.Active = 0;//行编号
            WordApp.ActiveDocument.PageSetup.Orientation = Microsoft.Office.Interop.Word.WdOrientation.wdOrientPortrait;//页面方向
            WordApp.ActiveDocument.PageSetup.TopMargin = WordApp.CentimetersToPoints(float.Parse("2.54"));//上页边距
            WordApp.ActiveDocument.PageSetup.BottomMargin = WordApp.CentimetersToPoints(float.Parse("2.54"));//下页边距
            WordApp.ActiveDocument.PageSetup.LeftMargin = WordApp.CentimetersToPoints(float.Parse("3.17"));//左页边距
            WordApp.ActiveDocument.PageSetup.RightMargin = WordApp.CentimetersToPoints(float.Parse("3.17"));//右页边距
            WordApp.ActiveDocument.PageSetup.Gutter = WordApp.CentimetersToPoints(float.Parse("0"));//装订线位置
            WordApp.ActiveDocument.PageSetup.HeaderDistance = WordApp.CentimetersToPoints(float.Parse("1.5"));//页眉
            WordApp.ActiveDocument.PageSetup.FooterDistance = WordApp.CentimetersToPoints(float.Parse("1.75"));//页脚
            WordApp.ActiveDocument.PageSetup.PageWidth = WordApp.CentimetersToPoints(float.Parse("21"));//纸张宽度
            WordApp.ActiveDocument.PageSetup.PageHeight = WordApp.CentimetersToPoints(float.Parse("29.7"));//纸张高度
            WordApp.ActiveDocument.PageSetup.FirstPageTray = Microsoft.Office.Interop.Word.WdPaperTray.wdPrinterDefaultBin;//纸张来源
            WordApp.ActiveDocument.PageSetup.OtherPagesTray = Microsoft.Office.Interop.Word.WdPaperTray.wdPrinterDefaultBin;//纸张来源
            WordApp.ActiveDocument.PageSetup.SectionStart = Microsoft.Office.Interop.Word.WdSectionStart.wdSectionNewPage;//节的起始位置：新建页
            WordApp.ActiveDocument.PageSetup.OddAndEvenPagesHeaderFooter = 0;//页眉页脚-奇偶页不同
            WordApp.ActiveDocument.PageSetup.DifferentFirstPageHeaderFooter = 0;//页眉页脚-首页不同
            WordApp.ActiveDocument.PageSetup.VerticalAlignment = Microsoft.Office.Interop.Word.WdVerticalAlignment.wdAlignVerticalTop;//页面垂直对齐方式
            WordApp.ActiveDocument.PageSetup.SuppressEndnotes = 0;//不隐藏尾注
            WordApp.ActiveDocument.PageSetup.MirrorMargins = 0;//不设置首页的内外边距
            WordApp.ActiveDocument.PageSetup.TwoPagesOnOne = false;//不双面打印
            WordApp.ActiveDocument.PageSetup.BookFoldPrinting = false;//不设置手动双面正面打印
            WordApp.ActiveDocument.PageSetup.BookFoldRevPrinting = false;//不设置手动双面背面打印
            WordApp.ActiveDocument.PageSetup.BookFoldPrintingSheets = 1;//打印默认份数
            WordApp.ActiveDocument.PageSetup.GutterPos = Microsoft.Office.Interop.Word.WdGutterStyle.wdGutterPosLeft;//装订线位于左侧
            WordApp.ActiveDocument.PageSetup.LinesPage = 40;//默认页行数量
            WordApp.ActiveDocument.PageSetup.LayoutMode = Microsoft.Office.Interop.Word.WdLayoutMode.wdLayoutModeLineGrid;//版式模式为“只指定行网格”
            #endregion
            #region 段落格式设定
            WordApp.Selection.ParagraphFormat.LeftIndent = WordApp.CentimetersToPoints(float.Parse("0"));//左缩进
            WordApp.Selection.ParagraphFormat.RightIndent = WordApp.CentimetersToPoints(float.Parse("0"));//右缩进
            WordApp.Selection.ParagraphFormat.SpaceBefore = float.Parse("0");//段前间距
            WordApp.Selection.ParagraphFormat.SpaceBeforeAuto = 0;//
            WordApp.Selection.ParagraphFormat.SpaceAfter = float.Parse("0");//段后间距
            WordApp.Selection.ParagraphFormat.SpaceAfterAuto = 0;//
            WordApp.Selection.ParagraphFormat.LineSpacingRule = Microsoft.Office.Interop.Word.WdLineSpacing.wdLineSpaceSingle;//单倍行距
            WordApp.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphJustify;//段落2端对齐
            WordApp.Selection.ParagraphFormat.WidowControl = 0;//孤行控制
            WordApp.Selection.ParagraphFormat.KeepWithNext = 0;//与下段同页
            WordApp.Selection.ParagraphFormat.KeepTogether = 0;//段中不分页
            WordApp.Selection.ParagraphFormat.PageBreakBefore = 0;//段前分页
            WordApp.Selection.ParagraphFormat.NoLineNumber = 0;//取消行号
            WordApp.Selection.ParagraphFormat.Hyphenation = 1;//取消段字
            WordApp.Selection.ParagraphFormat.FirstLineIndent = WordApp.CentimetersToPoints(float.Parse("0"));//首行缩进
            WordApp.Selection.ParagraphFormat.OutlineLevel = Microsoft.Office.Interop.Word.WdOutlineLevel.wdOutlineLevelBodyText;
            WordApp.Selection.ParagraphFormat.CharacterUnitLeftIndent = float.Parse("0");
            WordApp.Selection.ParagraphFormat.CharacterUnitRightIndent = float.Parse("0");
            WordApp.Selection.ParagraphFormat.CharacterUnitFirstLineIndent = float.Parse("0");
            WordApp.Selection.ParagraphFormat.LineUnitBefore = float.Parse("0");
            WordApp.Selection.ParagraphFormat.LineUnitAfter = float.Parse("0");
            WordApp.Selection.ParagraphFormat.AutoAdjustRightIndent = 1;
            WordApp.Selection.ParagraphFormat.DisableLineHeightGrid = 0;
            WordApp.Selection.ParagraphFormat.FarEastLineBreakControl = 1;
            WordApp.Selection.ParagraphFormat.WordWrap = 1;
            WordApp.Selection.ParagraphFormat.HangingPunctuation = 1;
            WordApp.Selection.ParagraphFormat.HalfWidthPunctuationOnTopOfLine = 0;
            WordApp.Selection.ParagraphFormat.AddSpaceBetweenFarEastAndAlpha = 1;
            WordApp.Selection.ParagraphFormat.AddSpaceBetweenFarEastAndDigit = 1;
            WordApp.Selection.ParagraphFormat.BaseLineAlignment = Microsoft.Office.Interop.Word.WdBaselineAlignment.wdBaselineAlignAuto;
            #endregion
            #region 字体格式设定
            WordApp.Selection.Font.NameFarEast = "华文中宋";
            WordApp.Selection.Font.NameAscii = "Times New Roman";
            WordApp.Selection.Font.NameOther = "Times New Roman";
            WordApp.Selection.Font.Name = "宋体";
            WordApp.Selection.Font.Size = float.Parse("14");
            WordApp.Selection.Font.Bold = 0;
            WordApp.Selection.Font.Italic = 0;
            WordApp.Selection.Font.Underline = Microsoft.Office.Interop.Word.WdUnderline.wdUnderlineNone;
            WordApp.Selection.Font.UnderlineColor = Microsoft.Office.Interop.Word.WdColor.wdColorAutomatic;
            WordApp.Selection.Font.StrikeThrough = 0;//删除线
            WordApp.Selection.Font.DoubleStrikeThrough = 0;//双删除线
            WordApp.Selection.Font.Outline = 0;//空心
            WordApp.Selection.Font.Emboss = 0;//阳文
            WordApp.Selection.Font.Shadow = 0;//阴影
            WordApp.Selection.Font.Hidden = 0;//隐藏文字
            WordApp.Selection.Font.SmallCaps = 0;//小型大写字母
            WordApp.Selection.Font.AllCaps = 0;//全部大写字母
            WordApp.Selection.Font.Color = Microsoft.Office.Interop.Word.WdColor.wdColorAutomatic;
            WordApp.Selection.Font.Engrave = 0;//阴文
            WordApp.Selection.Font.Superscript = 0;//上标
            WordApp.Selection.Font.Subscript = 0;//下标
            WordApp.Selection.Font.Spacing = float.Parse("0");//字符间距
            WordApp.Selection.Font.Scaling = 100;//字符缩放
            WordApp.Selection.Font.Position = 0;//位置
            WordApp.Selection.Font.Kerning = float.Parse("1");//字体间距调整
            WordApp.Selection.Font.Animation = Microsoft.Office.Interop.Word.WdAnimation.wdAnimationNone;//文字效果
            WordApp.Selection.Font.DisableCharacterSpaceGrid = false;
            WordApp.Selection.Font.EmphasisMark = Microsoft.Office.Interop.Word.WdEmphasisMark.wdEmphasisMarkNone;
            #endregion
            #region 获取光标位置
            /**/
            ////get_Information
            WordApp.Selection.get_Information(WdInformation.wdActiveEndPageNumber);
            //关于行号-页号-列号-位置
            //information 属性 
            //返回有关指定的所选内容或区域的信息。variant 类型，只读。 
            //expression.information(type) 
            //expression 必需。该表达式返回一个 range 或 selection 对象。 
            //type long 类型，必需。需要返回的信息。可取下列 wdinformation 常量之一： 
            //wdactiveendadjustedpagenumber 返回页码，在该页中包含指定的所选内容或区域的活动结尾。如果设置了一个起始页码，并对页码进行了手工调整，则返回调整过的页码。 
            //wdactiveendpagenumber 返回页码，在该页中包含指定的所选内容或区域的活动结尾，页码从文档的开头开始计算而不考虑对页码的任何手工调整。 
            //wdactiveendsectionnumber 返回节号，在该节中包含了指定的所选内容或区域的活动结尾。 
            //wdatendofrowmarker 如果指定的所选内容或区域位于表格的行结尾标记处，则本参数返回 true。 
            //wdcapslock 如果大写字母锁定模式有效，则本参数返回 true。 
            //wdendofrangecolumnnumber 返回表格列号，在该表格列中包含了指定的所选内容或区域的活动结尾。 
            //wdendofrangerownumber 返回表格行号，在该表格行包含了指定的所选内容或区域的活动结尾。 
            //wdfirstcharactercolumnnumber 返回指定的所选内容或区域中第一个字符的位置。如果所选内容或区域是折叠的，则返回所选内容或区域右侧紧接着的字符编号。 
            //wdfirstcharacterlinenumber 返回所选内容中第一个字符的行号。如果 pagination 属性为 false，或 draft 属性为 true，则返回 - 1。 
            //wdframeisselected 如果所选内容或区域是一个完整的图文框文本框，则本参数返回 true。 
            //wdheaderfootertype 返回一个值，该值表明包含了指定的所选内容或区域的页眉或页脚的类型，如下表所示。 值 页眉或页脚的类型 
            //- 1 无 
            //0 偶数页页眉 
            //1 奇数页页眉 
            //2 偶数页页脚 
            //3 奇数页页脚 
            //4 第一个页眉 
            //5 第一个页脚 
            //wdhorizontalpositionrelativetopage 返回指定的所选内容或区域的水平位置。该位置是所选内容或区域的左边与页面的左边之间的距离，以磅为单位。如果所选内容或区域不可见，则返回 - 1。 
            //wdhorizontalpositionrelativetotextboundary 返回指定的所选内容或区域相对于周围最近的正文边界的左边的水平位置，以磅为单位。如果所选内容或区域没有显示在当前屏幕，则本参数返回 - 1。 
            //wdinclipboard 有关此常量的详细内容，请参阅 microsoft office 98 macintosh 版的语言参考帮助。 
            //wdincommentpane 如果指定的所选内容或区域位于批注窗格，则返回 true。 
            //wdinendnote 如果指定的所选内容或区域位于页面视图的尾注区内，或者位于普通视图的尾注窗格中，则本参数返回 true。 
            //wdinfootnote 如果指定的所选内容或区域位于页面视图的脚注区内，或者位于普通视图的脚注窗格中，则本参数返回 true。 
            //wdinfootnoteendnotepane 如果指定的所选内容或区域位于页面视图的脚注或尾注区内，或者位于普通视图的脚注或尾注窗格中，则本参数返回 true。详细内容，请参阅前面的 wdinfootnote 和 wdinendnote 的说明。 
            //wdinheaderfooter 如果指定的所选内容或区域位于页眉或页脚窗格中，或者位于页面视图的页眉或页脚中，则本参数返回 true。 
            //wdinmasterdocument 如果指定的所选内容或区域位于主控文档中，则本参数返回 true。 
            //wdinwordmail 返回一个值，该值表明了所选内容或区域的的位置，如下表所示。值 位置 
            //0 所选内容或区域不在一条电子邮件消息中。 
            //1 所选内容或区域位于正在发送的电子邮件中。 
            //2 所选内容或区域位于正在阅读的电子邮件中。 
            //wdmaximumnumberofcolumns 返回所选内容或区域中任何行的最大表格列数。 
            //wdmaximumnumberofrows 返回指定的所选内容或区域中表格的最大行数。 
            //wdnumberofpagesindocument 返回与所选内容或区域相关联的文档的页数。 
            //wdnumlock 如果 num lock 有效，则本参数返回 true。 
            //wdovertype 如果改写模式有效，则本参数返回 true。可用 overtype 属性改变改写模式的状态。 
            //wdreferenceoftype 返回一个值，该值表明所选内容相对于脚注、尾注或批注引用的位置，如下表所示。 值 描述 
            //— 1 所选内容或区域包含、但不只限定于脚注、尾注或批注引用中。 
            //0 所选内容或区域不在脚注、尾注或批注引用之前。 
            //1 所选内容或区域位于脚注引用之前。 
            //2 所选内容或区域位于尾注引用之前。 
            //3 所选内容或区域位于批注引用之前。 
            //wdrevisionmarking 如果修订功能处于活动状态，则本参数返回 true。 
            //wdselectionmode 返回一个值，该值表明当前的选定模式，如下表所示。 值 选定模式 
            //0 常规选定 
            //1 扩展选定 
            //2 列选定 
            //wdstartofrangecolumnnumber 返回所选内容或区域的起点所在的表格的列号。 
            //wdstartofrangerownumber 返回所选内容或区域的起点所在的表格的行号。 
            //wdverticalpositionrelativetopage 返回所选内容或区域的垂直位置，即所选内容的上边与页面的上边之间的距离，以磅为单位。如果所选内容或区域没有显示在屏幕上，则本参数返回 - 1。 
            //wdverticalpositionrelativetotextboundary 返回所选内容或区域相对于周围最近的正文边界的上边的垂直位置，以磅为单位。如果所选内容或区域没有显示在屏幕上，则本参数返回 - 1。 
            //wdwithintable 如果所选内容位于一个表格中，则本参数返回 true。 
            //wdzoompercentage 返回由 percentage 属性设置的当前的放大百分比。
            #endregion
            #region 光标移动
            //移动光标
            //光标下移3行 上移3行
            object unit = Microsoft.Office.Interop.Word.WdUnits.wdLine;
            object count = 3;
            WordApp.Selection.MoveEnd(ref unit, ref count);
            WordApp.Selection.MoveUp(ref unit, ref count, ref oMissing);
            //Microsoft.Office.Interop.Word.WdUnits说明
            //wdCell                  A cell. 
            //wdCharacter             A character. 
            //wdCharacterFormatting   Character formatting. 
            //wdColumn                A column. 
            //wdItem                  The selected item. 
            //wdLine                  A line. //行
            //wdParagraph             A paragraph. 
            //wdParagraphFormatting   Paragraph formatting. 
            //wdRow                   A row. 
            //wdScreen                The screen dimensions. 
            //wdSection               A section. 
            //wdSentence              A sentence. 
            //wdStory                 A story. 
            //wdTable                 A table. 
            //wdWindow                A window. 
            //wdWord                  A word.
            //录制的vb宏
            //     ,移动光标至当前行首
            //    Selection.HomeKey unit:=wdLine
            //    '移动光标至当前行尾
            //    Selection.EndKey unit:=wdLine
            //    '选择从光标至当前行首的内容
            //    Selection.HomeKey unit:=wdLine, Extend:=wdExtend
            //    '选择从光标至当前行尾的内容
            //    Selection.EndKey unit:=wdLine, Extend:=wdExtend
            //    '选择当前行
            //    Selection.HomeKey unit:=wdLine
            //    Selection.EndKey unit:=wdLine, Extend:=wdExtend
            //    '移动光标至文档开始
            //    Selection.HomeKey unit:=wdStory
            //    '移动光标至文档结尾
            //    Selection.EndKey unit:=wdStory
            //    '选择从光标至文档开始的内容
            //    Selection.HomeKey unit:=wdStory, Extend:=wdExtend
            //    '选择从光标至文档结尾的内容
            //    Selection.EndKey unit:=wdStory, Extend:=wdExtend
            //    '选择文档全部内容（从WholeStory可猜出Story应是当前文档的意思）
            //    Selection.WholeStory
            //    '移动光标至当前段落的开始
            //    Selection.MoveUp unit:=wdParagraph
            //    '移动光标至当前段落的结尾
            //    Selection.MoveDown unit:=wdParagraph
            //    '选择从光标至当前段落开始的内容
            //    Selection.MoveUp unit:=wdParagraph, Extend:=wdExtend
            //    '选择从光标至当前段落结尾的内容
            //    Selection.MoveDown unit:=wdParagraph, Extend:=wdExtend
            //    '选择光标所在段落的内容
            //    Selection.MoveUp unit:=wdParagraph
            //    Selection.MoveDown unit:=wdParagraph, Extend:=wdExtend
            //    '显示选择区的开始与结束的位置，注意：文档第1个字符的位置是0
            //    MsgBox ("第" & Selection.Start & "个字符至第" & Selection.End & "个字符")
            //    '删除当前行
            //    Selection.HomeKey unit:=wdLine
            //    Selection.EndKey unit:=wdLine, Extend:=wdExtend
            //    Selection.Delete
            //    '删除当前段落
            //    Selection.MoveUp unit:=wdParagraph
            //    Selection.MoveDown unit:=wdParagraph, Extend:=wdExtend
            //    Selection.Delete

            //表格的光标移动
            //光标到当前光标所在表格的地单元格
            WordApp.Selection.Tables[1].Cell(1, 1).Select();
            //unit对象定义
            object unith = Microsoft.Office.Interop.Word.WdUnits.wdRow;//表格行方式
            object extend = Microsoft.Office.Interop.Word.WdMovementType.wdExtend;/**//**//**////extend对光标移动区域进行扩展选择
            object unitu = Microsoft.Office.Interop.Word.WdUnits.wdLine;//文档行方式,可以看成表格一行.不过和wdRow有区别
            object unitp = Microsoft.Office.Interop.Word.WdUnits.wdParagraph;//段落方式,对于表格可以选择到表格行后的换车符,对于跨行合并的行选择,我能找到的最简单方式
            //object count = 1;//光标移动量
            #endregion
        }
        #endregion

        #region 读取Word表格中某个单元格的数据。其中的参数分别为文件名（包括路径），行号，列号。
        /**/
        /// <summary>
        /// 读取Word表格中某个单元格的数据。其中的参数分别为文件名（包括路径），行号，列号。
        /// </summary>
        /// <param name="fileName">word文档</param>
        /// <param name="rowIndex">行</param>
        /// <param name="colIndex">列</param>
        /// <returns>返回数据</returns>
        public static string ReadWord_tableContentByCell(string fileName, int rowIndex, int colIndex)
        {
            ApplicationClass cls = null;
            Document doc = null;
            Table table = null;
            object missing = Missing.Value;
            object path = fileName;
            cls = new ApplicationClass();
            try
            {
                doc = cls.Documents.Open
                  (ref path, ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing, ref missing);
                table = doc.Tables[1];
                string text = table.Cell(rowIndex, colIndex).Range.Text.ToString();
                text = text.Substring(0, text.Length - 2);　　//去除尾部的mark
                return text;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (doc != null)
                    doc.Close(ref missing, ref missing, ref missing);
                cls.Quit(ref missing, ref missing, ref missing);
            }
        }
        #endregion

        #region 修改word表格中指定单元格的数据
        /**/
        /// <summary>
        /// 修改word表格中指定单元格的数据
        /// </summary>
        /// <param name="fileName">word文档包括路径</param>
        /// <param name="rowIndex">行</param>
        /// <param name="colIndex">列</param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool UpdateWordTableByCell(string fileName, int rowIndex, int colIndex, string content)
        {
            ApplicationClass cls = null;
            Document doc = null;
            Table table = null;
            object missing = Missing.Value;
            object path = fileName;
            cls = new ApplicationClass();
            try
            {
                doc = cls.Documents.Open
                    (ref path, ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing, ref missing);
                table = doc.Tables[1];
                //doc.Range( ref 0, ref 0 ).InsertParagraphAfter();//插入回车
                table.Cell(rowIndex, colIndex).Range.InsertParagraphAfter();//.Text = content;
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close(ref missing, ref missing, ref missing);
                    cls.Quit(ref missing, ref missing, ref missing);
                }
            }
        }
        #endregion

        #region 清楚word进程
        /**/
        /// <summary>
        /// 清楚word进程
        /// </summary>
        public static void KillWordProcess()
        {
            System.Diagnostics.Process[] myPs;
            myPs = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process p in myPs)
            {
                if (p.Id != 0)
                {
                    string myS = "WINWORD.EXE" + p.ProcessName + "  ID:" + p.Id.ToString();
                    try
                    {
                        if (p.Modules != null)
                            if (p.Modules.Count > 0)
                            {
                                System.Diagnostics.ProcessModule pm = p.Modules[0];
                                myS += "\n Modules[0].FileName:" + pm.FileName;
                                myS += "\n Modules[0].ModuleName:" + pm.ModuleName;
                                myS += "\n Modules[0].FileVersionInfo:\n" + pm.FileVersionInfo.ToString();
                                if (pm.ModuleName.ToLower() == "winword.exe")
                                    p.Kill();
                            }
                    }
                    catch
                    { }
                    finally
                    {
                        ;
                    }
                }
            }
        }
        #endregion

        #region 清楚excel进程
        /**/
        /// <summary>
        /// 清楚excel进程
        /// </summary>
        public static void KillExcelProcess()
        {
            System.Diagnostics.Process[] myPs;
            myPs = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process p in myPs)
            {
                if (p.Id != 0)
                {
                    string myS = "excel.EXE" + p.ProcessName + "  ID:" + p.Id.ToString();
                    try
                    {
                        if (p.Modules != null)
                            if (p.Modules.Count > 0)
                            {
                                System.Diagnostics.ProcessModule pm = p.Modules[0];
                                myS += "\n Modules[0].FileName:" + pm.FileName;
                                myS += "\n Modules[0].ModuleName:" + pm.ModuleName;
                                myS += "\n Modules[0].FileVersionInfo:\n" + pm.FileVersionInfo.ToString();
                                if (pm.ModuleName.ToLower() == "excel.exe")
                                    p.Kill();
                            }
                    }
                    catch
                    { }
                    finally
                    {
                        ;
                    }
                }
            }
        }
        #endregion

        #region 网页内容或导入word或excel
        /**/
        /// <summary>
        /// 网页内容保存或导出为word或excel
        /// </summary>
        /// <param name="url">网页地址</param>
        /// <param name="num">0为导出word,1为导出excel</param>
        //public static void SaveOrOutData(string url, int num)//导出数据的函数0为word,1为Excel 
        //{
        //    WebRequest req = WebRequest.Create(url);
        //    WebResponse resp = req.GetResponse();
        //    StreamReader sr = new StreamReader(resp.GetResponseStream(), System.Text.Encoding.UTF8);
        //    string x = sr.ReadToEnd();
        //   System.Web.HttpContext.Current.Response.Clear();
        //    System.Web.HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
        //    string fName = DateTime.Now.ToString("yyyy-MM-dd-ss");
        //    if (num == 0)
        //    {
        //        fName = HttpUtility.UrlEncode(fName, System.Text.Encoding.GetEncoding("gb2312")) + ".doc";
        //        System.Web.HttpContext.Current.Response.ContentType = "application/ms-word";
        //    }
        //    else
        //    {
        //        fName = HttpUtility.UrlEncode(fName, System.Text.Encoding.GetEncoding("gb2312")) + ".xls";
        //        System.Web.HttpContext.Current.Response.ContentType = "application nd.xls";
        //    }
        //    System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + fName);
        //    System.Web.HttpContext.Current.Response.Write(getBodyContent(x));//获取table标签
        //    System.Web.HttpContext.Current.Response.Flush();
        //    System.Web.HttpContext.Current.Response.End();
        //}
        /**/
        /// <summary>
        /// 获取网页table标签的内容
        /// </summary>
        /// <param name="input">html代码</param>
        /// <returns></returns>
        private static string getBodyContent(string input)
        {
            string pattern = @"<table.*?</table>";
            Regex reg = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Match mc = reg.Match(input);
            string bodyContent = "";
            if (mc.Success)
            {
                bodyContent = mc.Value;
            }
            return bodyContent;
        }
        #endregion

        #region 判断系统是否装excel
        /**/
        /// <summary>
        /// 判断系统是否装excel
        /// </summary>
        /// <returns></returns>
        public static bool IsInstallExcel()
        {
            RegistryKey machineKey = Registry.LocalMachine;
            if (IsInstallExcelByVersion("12.0", machineKey))
            {
                return true;
            }
            if (IsInstallExcelByVersion("11.0", machineKey))
            {
                return true;
            }
            return false;
        }
        /**/
        /// <summary>
        /// 判断系统是否装某版本的excel
        /// </summary>
        /// <param name="strVersion">版本号</param>
        /// <param name="machineKey"></param>
        /// <returns></returns>
        private static bool IsInstallExcelByVersion(string strVersion, RegistryKey machineKey)
        {
            try
            {
                RegistryKey installKey = machineKey.OpenSubKey("Software").OpenSubKey("Microsoft").OpenSubKey("Office").OpenSubKey(strVersion).OpenSubKey("Excel").OpenSubKey("InstallRoot");
                if (installKey == null)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 判断系统是否装word
        /**/
        /// <summary>
        /// 判断系统是否装word
        /// </summary>
        /// <returns></returns>
        public static bool IsInstallWord()
        {
            RegistryKey machineKey = Registry.LocalMachine;
            if (IsInstallExcelByVersion("12.0", machineKey))
            {
                return true;
            }
            if (IsInstallExcelByVersion("11.0", machineKey))
            {
                return true;
            }
            return false;
        }
        /**/
        /// <summary>
        /// 判断系统是否装某版本的word
        /// </summary>
        /// <param name="strVersion">版本号</param>
        /// <param name="machineKey"></param>
        /// <returns></returns>
        private static bool IsInstallWordByVersion(string strVersion, RegistryKey machineKey)
        {
            try
            {
                RegistryKey installKey = machineKey.OpenSubKey("Software").OpenSubKey("Microsoft").OpenSubKey("Office").OpenSubKey(strVersion).OpenSubKey("Word").OpenSubKey("InstallRoot");
                if (installKey == null)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
