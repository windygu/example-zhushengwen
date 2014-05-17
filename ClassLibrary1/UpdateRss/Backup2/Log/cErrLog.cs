using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

///功能：错误信息日志类，主要记录采集和发布时的错误数据，日志存储位置固定不可改变
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：下一版字典部分要强化
///说明：
///版本：01.60.00
///修订：无
namespace SoukeyNetget.Log
{
    class cErrLog
    {
        private string m_LogFileName;

        public cErrLog()
        {
        }

        public cErrLog(string TaskName)
        {
            //判断TaskName的日志文件是否存在，文件命名为：任务名和日期名，每个任务
            //每日为一个日志文件
            string FileName = Program.getPrjPath() + "Log\\" + TaskName + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + ".txt";

        }

        ~cErrLog()
        {
        }

        public void WriteLog(string TaskName,cGlobalParas.LogType LogType, string strLog)
        {
            string FileName = Program.getPrjPath() + "Log\\" + TaskName + DateTime.Now.Year;

            if (DateTime.Now.Month.ToString().Length == 1)
                FileName = FileName + "0" + DateTime.Now.Month.ToString();
            else
                FileName = FileName + DateTime.Now.Month.ToString();

            if (DateTime.Now.Day.ToString().Length == 1)
                FileName = FileName + "0" + DateTime.Now.Day.ToString();
            else
                FileName = FileName + DateTime.Now.Day.ToString();

            FileName = FileName + ".txt";
            
            string Log="";

            if (!Directory.Exists(Path.GetDirectoryName(FileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(FileName));

            FileStream myStream = File.Open(FileName, FileMode.Append, FileAccess.Write, FileShare.Write);
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gb2312"));

            if (LogType ==cGlobalParas.LogType.GatherError )
            {
                Log =TaskName + "\t" + DateTime.Now .ToString () + "\t" + "采集错误" + "\t" + strLog ;
                sw.WriteLine(Log);
            }
            else if (LogType ==cGlobalParas.LogType.PublishError )
            {
                Log =TaskName + "\t" + DateTime.Now .ToString () + "\t" + "发布错误" + "\t" + strLog ;
                sw.WriteLine(Log);
            }

            sw.Close();
            myStream.Close();

            
           
        }

    }
}
