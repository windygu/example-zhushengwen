using System;
using System.Collections.Generic;
using System.Text;

///���ܣ��������򼯺��࣬��Ҫ������ʱ�洢��������
/// ����������Ϣά������
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget.Task
{
    public class cNavigRules
    {
        public cNavigRules()
        {
            m_NavigRule = new List<cNavigRule>();
        }

        ~cNavigRules()
        {
        }

        private string m_Url;
        public string Url
        {
            get { return m_Url; }
            set { m_Url = value; }
        }

        private List<cNavigRule> m_NavigRule;
        public List<cNavigRule> NavigRule
        {
            get { return m_NavigRule; }
            set { m_NavigRule = value; }
        }
    }
}
