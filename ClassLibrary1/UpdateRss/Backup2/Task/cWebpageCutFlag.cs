using System;
using System.Collections.Generic;
using System.Text;

///���ܣ��ɼ����� �ɼ���־ �洢����
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget.Task
{
    public class cWebpageCutFlag
    {
        private int m_id;
        public int id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        private string m_Title;
        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        private string m_StartPos;
        public string StartPos
        {
            get { return m_StartPos; }
            set { m_StartPos = value; }
        }

        private string m_EndPos;
        public string EndPos
        {
            get { return m_EndPos; }
            set { m_EndPos = value; }
        }

        private bool m_loopFlag;
        public bool loopFlag
        {
            get { return m_loopFlag; }
            set { m_loopFlag = value; }
        }

        private string m_Content;
        public string Content
        {
            get { return m_Content; }
            set { m_Content = value; }
        }

        //��ҳ�ɼ����ݵ��޶���ʶ
        private int m_LimitSign;
        public int LimitSign
        {
            get { return m_LimitSign; }
            set { m_LimitSign = value; }
        }

        private string m_RegionExpression;
        public string RegionExpression
        {
            get { return m_RegionExpression; }
            set { m_RegionExpression = value; }
        }

        private int m_ExportLimit;
        public int ExportLimit
        {
            get { return m_ExportLimit; }
            set { m_ExportLimit = value; }
        }

        private string m_ExportExpression;
        public string ExportExpression
        {
            get { return m_ExportExpression; }
            set { m_ExportExpression = value; }
        }

        //�ɼ����ݵ�����
        private int m_DataType;
        public int DataType
        {
            get { return m_DataType; }
            set { m_DataType = value; }
        }
    }
}
