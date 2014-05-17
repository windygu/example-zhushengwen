using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

///���ܣ�������Ϣ��־�࣬��Ҫ��¼�ɼ��ͷ���ʱ�Ĵ������ݣ���־�洢λ�ù̶����ɸı�
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����һ���ֵ䲿��Ҫǿ��
///˵����
///�汾��01.60.00
///�޶�����
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
            //�ж�TaskName����־�ļ��Ƿ���ڣ��ļ�����Ϊ������������������ÿ������
            //ÿ��Ϊһ����־�ļ�
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
                Log =TaskName + "\t" + DateTime.Now .ToString () + "\t" + "�ɼ�����" + "\t" + strLog ;
                sw.WriteLine(Log);
            }
            else if (LogType ==cGlobalParas.LogType.PublishError )
            {
                Log =TaskName + "\t" + DateTime.Now .ToString () + "\t" + "��������" + "\t" + strLog ;
                sw.WriteLine(Log);
            }

            sw.Close();
            myStream.Close();

            
           
        }

    }
}
