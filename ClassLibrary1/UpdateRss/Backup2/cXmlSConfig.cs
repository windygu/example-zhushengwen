using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

///功能：处理系统所有的配置信息
///完成时间：2009-4-
///作者：一孑
///遗留问题：无
///开发计划：待定
///说明：与cSystem会有功能重复，下一步待定，所以保留
///版本：01.10.00
///修订：无
namespace SoukeyNetget
{
    class cXmlSConfig
    {
        cXmlIO xmlConfig;

        public cXmlSConfig()
        {
            //打开配置文件
            xmlConfig = new cXmlIO("SoukeyConfig.xml");
            m_IsInstantSave = true;
        }

        ~cXmlSConfig()
        {
            xmlConfig = null;
        }

        //是否即时保存，默认为true，即时保存，即调用方法后马上保存
        //false时，只修改，不保存文件，需要调用Save方法进行保存，主要用于
        //配置修改
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

        //退出选择：0-最小化 1-退出
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
