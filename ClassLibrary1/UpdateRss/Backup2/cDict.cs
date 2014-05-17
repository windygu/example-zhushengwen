using System;
using System.Collections.Generic;
using System.Text;
using System.Data ;

///功能：字典处理
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：下一版字典部分要强化
///说明：
///版本：01.10.00
///修订：无
namespace SoukeyNetget
{
    class cDict
    {
        cXmlIO xmlConfig;
        DataView DictClass;

        #region 构造 析构 函数
        public cDict()
        {
            try
            {
                xmlConfig = new cXmlIO(Program.getPrjPath () + "dict.xml");

                //获取TaskClass节点
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        ~cDict()
        {
            xmlConfig = null;
        }

        #endregion

        #region 返回字典类别的总数和字典类别的名称
        //必须首先调用GetDictClassCount
        public int GetDictClassCount()
        {
            DictClass  = new DataView();
            DictClass = xmlConfig.GetData("descendant::DictClasses");
            int tCount = DictClass.Count;
            return tCount;
        }

        //根据制定的index返回DictClassName
        public string GetDictClassName(int index)
        {
            string dClassName = DictClass[index].Row["Name"].ToString();
            return dClassName;
        }

        public void SetDictClassNull()
        {
            DictClass = null;
        }

        #endregion

        #region 返回字典及字典内容的DataView

        public DataView GetDictClass()
        {
            DataView dClass = new DataView();
            dClass = xmlConfig.GetData("descendant::DictClasses");
            return dClass;
        }

        //根据指定的字典名称返回字典内容
        public DataView GetDict(string DictName)
        {
            string strd = "Dict";
            strd += DictName;
            strd = "descendant::" + strd;

            DataView d = new DataView();
            d = xmlConfig.GetData(strd);
            return d;
        }

        #endregion

        #region 字典的增、删、改
        public void AddDictClass(string DictClassName)
        {
            //添加分类的节点
            string strDictClass;

            strDictClass = "<Name>" + cTool.ReplaceTrans( DictClassName) + "</Name>";
            xmlConfig.InsertElement("DictConfig/DictClasses", "DictClass", strDictClass);

            //同时还需要添加分类节点下的内容节点
            string DictPath;
            DictPath = "<Dict" + cTool.ReplaceTrans(DictClassName) + "></Dict" + cTool.ReplaceTrans(DictClassName) + ">";
            xmlConfig.InsertElement("DictConfig/Dict", "Dict" + cTool.ReplaceTrans(DictClassName), "");

            xmlConfig.Save();


        }

        public void AddDict(string DictClass,string DictName)
        {
            string strDictClass;
            string strDict;
            strDictClass = "Dict" + cTool.ReplaceTrans(DictClass);

            strDict = "<Name>" + cTool.ReplaceTrans(DictName) + "</Name>";
            xmlConfig.InsertElement("DictConfig/Dict/" + strDictClass, DictClass, strDict);
            xmlConfig.Save();

        }

        ////编辑字典分类的名称
        //public void EditDictClass(string OldName, string NewName)
        //{
        //    xmlConfig.EditNode("DictClass", OldName, NewName);
        //    xmlConfig.EditNodeName("DictConfig/Dict/Dict" + OldName, "Dict" + OldName, "Dict" + NewName);
        //    xmlConfig.Save();
        //}

        //编辑字典内容
        public void EditDict(string DictClass, string Old_Name,string DictName)
        {
            xmlConfig.EditNode(DictClass, Old_Name, DictName);
            xmlConfig.Save();
        }

        //删除分类节点
        public void DelDictClass(string DictClass)
        {

            xmlConfig.DeleteChildNodes("DictClasses" , "Name",DictClass);
            xmlConfig.DeleteNode("Dict" + DictClass);
            xmlConfig.Save();
        }


        //删除分类下字典的内容
        public void DelDict(string DictClass, string Dict)
        {
            xmlConfig.DeleteChildNodes("Dict" + DictClass,"Name", Dict);

            xmlConfig.Save();
        }
        #endregion 

    }
}
