using System;
using System.Collections.Generic;
using System.Text;

namespace SoukeyNetget.Task
{
    public class cNavigRule
    {
        public cNavigRule()
        {
        }

        ~cNavigRule()
        {
        }

        //所对应的Url地址
        private string m_Url;
        public string Url
        {
            get { return m_Url; }
            set { m_Url = value; }
        }

        private int m_Level;
        public int Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        private string m_NavigRule;
        public string NavigRule
        {
            get { return m_NavigRule; }
            set { m_NavigRule = value; }
        }
    }
}
