using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;

///功能：任务升级操作类，当前支持的任务版本时1.3
///完成时间：2009-8-30
///作者：一孑
///遗留问题：无
///开发计划：下一版字典部分要强化
///说明：
///版本：01.60.00
///修订：无
namespace SoukeyNetget.Task
{
    class cUpgradeTask
    {
        public cUpgradeTask()
        {
        }

        ~cUpgradeTask()
        {

        }

        private Single m_SupportTaskVersion = Single.Parse("1.3");

        //此类别可处理的任务版本号，注意从1.3开始，任务处理类不再向前兼容
        public Single SupportTaskVersion
        {
            get { return m_SupportTaskVersion; }
        }

        //指定任务进行升级操作
        /// <summary>
        /// 升级任务操作
        /// </summary>
        /// <param name="FileName">待升级的任务文件</param>
        /// <param name="IsBackup">是否备份</param>
        /// <param name="IsTask">是否为系统任务，如果是系统任务则需要维护分类信息，如果不是就只保存文件</param>
        public void UpdradeTask(string FileName ,bool IsBackup,bool IsSystemTask)
        {
            //判断是否进行备份
            if (IsBackup == true)
            {
                if (File.Exists(FileName + ".bak"))
                    File.Delete(FileName + ".bak");

                File.Copy(FileName, FileName + ".bak");
            }


            //加载任务文件
            cXmlIO Old_Task = new cXmlIO(FileName);
            Single TaskVersion =Single.Parse ("0");

            cTask t = new cTask();

            //判断任务版本号
            try
            {
                TaskVersion = Single.Parse(Old_Task.GetNodeValue("Task/BaseInfo/Version"));
            }
            catch (System.Exception)
            {
                TaskVersion = Single.Parse("1.0");
            }

            if (TaskVersion >= this.SupportTaskVersion)
            {
                Old_Task = null;
                return;
            }

            //开始新建一个任务
            t.New();

            #region 此部分内容是任何版本都存在的
            t.TaskID = Int64.Parse(Old_Task.GetNodeValue("Task/BaseInfo/ID"));
            t.TaskName = Old_Task.GetNodeValue("Task/BaseInfo/Name");
            t.TaskVersion = this.SupportTaskVersion;



            t.TaskDemo = Old_Task.GetNodeValue("Task/BaseInfo/TaskDemo");
            t.TaskClass = Old_Task.GetNodeValue("Task/BaseInfo/Class");
            t.TaskType = Old_Task.GetNodeValue("Task/BaseInfo/Type");
            t.RunType = Old_Task.GetNodeValue("Task/BaseInfo/RunType");

            //因存的是相对路径，所以要加上系统路径
            t.SavePath = Program.getPrjPath() + Old_Task.GetNodeValue("Task/BaseInfo/SavePath");
            t.UrlCount = int.Parse(Old_Task.GetNodeValue("Task/BaseInfo/UrlCount").ToString());
            t.ThreadCount = int.Parse(Old_Task.GetNodeValue("Task/BaseInfo/ThreadCount"));
            t.Cookie = Old_Task.GetNodeValue("Task/BaseInfo/Cookie");
            t.DemoUrl = Old_Task.GetNodeValue("Task/BaseInfo/DemoUrl");
            t.StartPos = Old_Task.GetNodeValue("Task/BaseInfo/StartPos");
            t.EndPos = Old_Task.GetNodeValue("Task/BaseInfo/EndPos");
            t.WebCode = Old_Task.GetNodeValue("Task/BaseInfo/WebCode");
            t.IsLogin = (Old_Task.GetNodeValue("Task/BaseInfo/IsLogin") == "True" ? true : false);
            t.LoginUrl = Old_Task.GetNodeValue("Task/BaseInfo/LoginUrl");
            t.IsUrlEncode = (Old_Task.GetNodeValue("Task/BaseInfo/IsUrlEncode") == "True" ? true : false);
            t.UrlEncode = Old_Task.GetNodeValue("Task/BaseInfo/UrlEncode");
            #endregion



            //从下面的内容开始有可能就出现任务加载错误，
            //导出部分是1.2版本中增加的，高级设置、多层导航及触发器是1.3版本中出现的
            //
            if (Old_Task.GetNodeValue("Task/Result/ExportType") == "")
                t.ExportType = ((int)cGlobalParas.PublishType.PublishTxt).ToString();
            else
                t.ExportType = Old_Task.GetNodeValue("Task/Result/ExportType");

            if (Old_Task.GetNodeValue("Task/Result/ExportFileName") == "")
                t.ExportFile = Program.getPrjPath() + "data\\" + t.TaskName + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + ".txt";
            else
                t.ExportFile = Old_Task.GetNodeValue("Task/Result/ExportFileName");

            if (Old_Task.GetNodeValue("Task/Result/DataSource") == "")
                t.DataSource = "";
            else
                t.DataSource = Old_Task.GetNodeValue("Task/Result/DataSource");

            if (Old_Task.GetNodeValue("Task/Result/DataTableName") == "")
                t.DataTableName = "";
            else
                t.DataTableName = Old_Task.GetNodeValue("Task/Result/DataTableName");

            if (Old_Task.GetNodeValue("Task/Result/InsertSql") == "")
                t.InsertSql = "";
            else
                t.InsertSql = Old_Task.GetNodeValue("Task/Result/InsertSql");

            if (Old_Task.GetNodeValue("Task/Result/ExportUrl") == "")
                t.ExportUrl = "";
            else
                t.ExportUrl = Old_Task.GetNodeValue("Task/Result/ExportUrl");

            if (Old_Task.GetNodeValue("Task/Result/ExportUrlCode") == "")
                t.ExportUrlCode = "";
            else
                t.ExportUrlCode = Old_Task.GetNodeValue("Task/Result/ExportUrlCode");

            if (Old_Task.GetNodeValue("Task/Result/ExportCookie") == "")
                t.ExportCookie = "";
            else
                t.ExportCookie = Old_Task.GetNodeValue("Task/Result/ExportCookie");

            //此部分是1.3版本中存在的，需要升级
            //加载高级配置信息
            if (Old_Task.GetNodeValue("Task/Advance/GatherAgainNumber") == "")
                t.GatherAgainNumber = 3;
            else
                t.GatherAgainNumber = int.Parse(Old_Task.GetNodeValue("Task/Advance/GatherAgainNumber"));

            if (Old_Task.GetNodeValue("Task/Advance/IsIgnore404") == "")
                t.IsIgnore404 = true;
            else
                t.IsIgnore404 = (Old_Task.GetNodeValue("Task/Advance/IsIgnore404") == "True" ? true : false);

            if (Old_Task.GetNodeValue("Task/Advance/IsErrorLog") == "")
                t.IsErrorLog = false;
            else
                t.IsErrorLog = (Old_Task.GetNodeValue("Task/Advance/IsErrorLog") == "True" ? true : false);

            if (Old_Task.GetNodeValue("Task/Advance/IsDelRepeatRow") == "")
                t.IsDelRepRow = false;
            else
                t.IsDelRepRow = (Old_Task.GetNodeValue("Task/Advance/IsDelRepeatRow") == "True" ? true : false);

            if (Old_Task.GetNodeValue("Task/Advance/IsExportHeader") == "")
                t.IsExportHeader = true;
            else
                t.IsExportHeader = (Old_Task.GetNodeValue("Task/Advance/IsExportHeader") == "True" ? true : false);

            if (Old_Task.GetNodeValue("Task/Advance/IsTrigger") == "")
                t.IsTrigger = false;
            else
                t.IsTrigger = (Old_Task.GetNodeValue("Task/Advance/IsTrigger") == "True" ? true : false);

            if (Old_Task.GetNodeValue("Task/Advance/TriggerType")=="")
                t.TriggerType = ((int)cGlobalParas.TriggerType.GatheredRun).ToString();
            else
                t.TriggerType = Old_Task.GetNodeValue("Task/Advance/TriggerType");

            DataView dw = new DataView();
            int i;

            //此次升级中，不需要加载Trigger信息，以前任务版本中不存在，升级过程中可以不考虑
            //dw = Old_Task.GetData("descendant::Trigger");
            //cTriggerTask tt;

            //if (dw != null)
            //{
            //    for (i = 0; i < dw.Count; i++)
            //    {
            //        tt = new cTriggerTask();
            //        tt.RunTaskType = int.Parse(dw[i].Row["RunTaskType"].ToString());
            //        tt.RunTaskName = dw[i].Row["RunTaskName"].ToString();
            //        tt.RunTaskPara = dw[i].Row["RunTaskPara"].ToString();

            //        t.TriggerTask.Add(tt);
            //    }
            //}

            ////dw = null;
            ////dw = new DataView();

            dw = Old_Task.GetData("descendant::WebLinks");
            cWebLink w;

            DataView dn;

            if (dw != null)
            {
                for (i = 0; i < dw.Count; i++)
                {
                    w = new cWebLink();
                    w.id = i;
                    w.Weblink = dw[i].Row["Url"].ToString();
                    if (dw[i].Row["IsNag"].ToString() == "True")
                        w.IsNavigation = true;
                    else
                        w.IsNavigation = false;

                    if (dw[i].Row["IsNextPage"].ToString() == "True")
                        w.IsNextpage = true;
                    else
                        w.IsNextpage = false;

                    w.NextPageRule = dw[i].Row["NextPageRule"].ToString();
                    w.IsGathered = int.Parse((dw[i].Row["IsGathered"].ToString() == null || dw[i].Row["IsGathered"].ToString() == "") ? "2031" : dw[i].Row["IsGathered"].ToString());

                    if (dw[i].Row["IsNag"].ToString() == "True")
                    {
                        //在此处理导航的升级操作，如果以前版本中存在导航规则，则只是一层导航
                        cNavigRule nRule = new cNavigRule();
                        nRule.Url = dw[i].Row["Url"].ToString();
                        nRule.Level =1;
                        nRule.NavigRule = dw[i].Row["NagRule"].ToString();

                        w.NavigRules.Add(nRule);
                    }
                    else
                    {
                    }

                    t.WebpageLink.Add(w);
                    w = null;
                }
            }

            dw = null;
            dw = new DataView();
            dw = Old_Task.GetData("descendant::GatherRule");
            Task.cWebpageCutFlag c;
            if (dw != null)
            {
                for (i = 0; i < dw.Count; i++)
                {
                    c = new Task.cWebpageCutFlag();
                    c.Title = dw[i].Row["Title"].ToString();
                    c.DataType = int.Parse((dw[i].Row["DataType"].ToString() == null || dw[i].Row["DataType"].ToString() == "") ? "0" : dw[i].Row["DataType"].ToString());
                    c.StartPos = dw[i].Row["StartFlag"].ToString();
                    c.EndPos = dw[i].Row["EndFlag"].ToString();
                    c.LimitSign = int.Parse((dw[i].Row["LimitSign"].ToString() == null || dw[i].Row["LimitSign"].ToString() == "") ? "0" : dw[i].Row["LimitSign"].ToString());

                    //处理版本不同时造成的错误，捕获不处理
                    try
                    {
                        c.RegionExpression = dw[i].Row["RegionExpression"].ToString();
                        c.ExportLimit = int.Parse((dw[i].Row["ExportLimit"].ToString() == null || dw[i].Row["ExportLimit"].ToString() == "") ? "0" : dw[i].Row["ExportLimit"].ToString());
                        c.ExportExpression = dw[i].Row["ExportExpression"].ToString();
                    }
                    catch (System.Exception)
                    {
                    }

                    t.WebpageCutFlag.Add(c);
                    c = null;
                }
            }
            dw = null;

            Old_Task = null;


            //获取此文件的目录传入
            string FilePath = Path.GetDirectoryName(FileName);

            if (IsSystemTask == true)
            {
                t.DeleTask(FilePath, t.TaskName);

                t.Save(FilePath + "\\");
            }
            else
            {
                t.SaveTaskFile(FilePath);
            }
            t = null;

        }

    }
}
