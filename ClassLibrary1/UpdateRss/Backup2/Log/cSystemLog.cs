using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SoukeyNetget.Log
{
    class cSystemLog
    {
        public cSystemLog()
        {
        }

        ~cSystemLog()
        {
        }

        public void WriteLog(string strLog)
        {
            string FileName = Program.getPrjPath() + "Log\\" + DateTime.Now.Year;
            if (DateTime.Now.Month.ToString().Length == 1)
                FileName = FileName + "0" + DateTime.Now.Month.ToString();
            else
                FileName = FileName + DateTime.Now.Month.ToString();

            if (DateTime.Now.Day.ToString ().Length ==1)
                FileName = FileName + "0" + DateTime.Now.Day.ToString();
            else
                FileName = FileName + DateTime.Now.Day.ToString();

            FileName =FileName + ".txt";
            

            if (!Directory.Exists(Path.GetDirectoryName(FileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(FileName));

            FileStream myStream = File.Open(FileName, FileMode.Append, FileAccess.Write, FileShare.Write);
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gb2312"));

            sw.WriteLine(strLog);
            
            sw.Close();
            myStream.Close();



        }
    }
}
