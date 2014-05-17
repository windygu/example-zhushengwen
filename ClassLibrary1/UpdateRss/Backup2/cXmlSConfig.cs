using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

///���ܣ�����ϵͳ���е�������Ϣ
///���ʱ�䣺2009-4-
///���ߣ�һ��
///�������⣺��
///�����ƻ�������
///˵������cSystem���й����ظ�����һ�����������Ա���
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget
{
    class cXmlSConfig
    {
        cXmlIO xmlConfig;

        public cXmlSConfig()
        {
            //�������ļ�
            xmlConfig = new cXmlIO("SoukeyConfig.xml");
            m_IsInstantSave = true;
        }

        ~cXmlSConfig()
        {
            xmlConfig = null;
        }

        //�Ƿ�ʱ���棬Ĭ��Ϊtrue����ʱ���棬�����÷��������ϱ���
        //falseʱ��ֻ�޸ģ��������ļ�����Ҫ����Save�������б��棬��Ҫ����
        //�����޸�
        private bool m_IsInstantSave;
        public bool IsInstantSave
        {
            get { return m_IsInstantSave; }
            set { m_IsInstantSave = value; }
        }

        public cGlobalParas.CurLanguage CurrentLanguage
        {
            get
            {
                return (cGlobalParas.CurLanguage)int.Parse (xmlConfig.GetNodeValue("Config/System/UILanguage"));
            }
            set
            {

                cGlobalParas.CurLanguage cl = value;

                xmlConfig.EditNodeValue("Config/System/UILanguage", ((int)cl).ToString ());

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public bool IsFirstRun
        {
            get
            {
                if (xmlConfig.GetNodeValue("Config/Start/First") == "True")
                    return true;
                else
                    return false;
            }
            set
            {
                bool isFirst = value;
                if (isFirst==true )
                    xmlConfig.EditNodeValue("Config/Start/First", "True");
                else
                    xmlConfig.EditNodeValue("Config/Start/First", "False");

                if (m_IsInstantSave==true )
                    xmlConfig.Save();
            }
        }

        public bool ExitIsShow
        {
            get 
            {
                if (xmlConfig.GetNodeValue("Config/Exit/IsShow") == "0")
                    return false;
                else
                    return true;
            }
            set 
            {
                string s = "0";
                if (value == true)
                    s = "1";
                else
                    s = "0";
            
                xmlConfig.EditNodeValue("Config/Exit/IsShow", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        //�˳�ѡ��0-��С�� 1-�˳�
        public int ExitSelected
        {
            get {return int.Parse (xmlConfig.GetNodeValue("Config/Exit/Selected")); }
            set 
            {
                int i = value;
                xmlConfig.EditNodeValue("Config/Exit/Selected", i.ToString ());

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public bool AutoSaveLog
        {
            get 
            {
                if (xmlConfig.GetNodeValue("Config/System/AutoSaveLog") == "0")
                    return false;
                else
                    return true;
            }
            set
            {
                string s = "0";
                if (value == true)
                    s = "1";
                else
                    s = "0";

                xmlConfig.EditNodeValue("Config/System/AutoSaveLog", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public void Save()
        {
            xmlConfig.Save();
        }

    }
}
