using System;
using System.Collections.Generic;
using System.Text;

///���ܣ��ɼ����� URL�洢 ����
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget.Task
{
    public class cWebLink
    {

        #region ���� ����
        public cWebLink()
        {
            m_IsGathered =(int) cGlobalParas.UrlGatherResult.UnGather;
            m_NavigRules = new List<cNavigRule>();
        }

        ~cWebLink()
        {
        }

        #endregion

        #region ����
        private int m_id;
        public int id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        private string m_Weblink;
        public string Weblink
        {
            get { return m_Weblink; }
            set { m_Weblink = value; }
        }

        //�Ƿ�Ϊ����ҳ������ǵ���ҳ����Ҫ���ݵ�����������������Ҳ����ȡ
        private bool m_IsNavigation;
        public bool IsNavigation
        {
            get { return m_IsNavigation; }
            set { m_IsNavigation = value; }
        }

        #region �������ݱ�ע�ͣ�����Ϊ�汾������֧���˶�㵼����һ����㵼���ļ�������д洢
        ////�Ƿ�Ϊ���·��
        //private bool m_IsOppPath;
        //public bool IsOppPath
        //{
        //    get { return m_IsOppPath; }
        //    set { m_IsOppPath = value; }
        //}

        ////��������
        //private string m_NagRule;
        //public string NagRule
        //{
        //    get { return m_NagRule; }
        //    set { m_NagRule = value; }
        //}
        #endregion

        //��㵼��������һ��������
        private List<cNavigRule> m_NavigRules;
        public List<cNavigRule> NavigRules
        {
            get { return m_NavigRules; }
            set { m_NavigRules = value; }
        }

        //�Ƿ���ȡ��һҳ��ʶ
        private bool m_IsNextPage;
        public bool IsNextpage
        {
            get { return m_IsNextPage; }
            set { m_IsNextPage = value; }
        }

        //��һҳ��ʶ
        private string m_NextPageRule;
        public string NextPageRule
        {
            get { return m_NextPageRule; }
            set { m_NextPageRule = value; }
        }

        //��ʶ��ǰ��ҳ��ַ�Ƿ��Ѿ��ɼ�,Ĭ��cGlobalParas.UrlGatherResult.UnGather
        private int m_IsGathered;
        public int IsGathered
        {
            get { return m_IsGathered; }
            set { m_IsGathered = value; }
        }

        private string m_CurrentRunning;
        public string CurrentRunning
        {
            get { return m_CurrentRunning; }
            set { m_CurrentRunning = value; }
        }

        #endregion

    }
}
