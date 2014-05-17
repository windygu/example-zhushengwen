using System;
using System.Collections.Generic;
using System.Text;
using System.Data ;

///���ܣ��ֵ䴦��
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����һ���ֵ䲿��Ҫǿ��
///˵����
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget
{
    class cDict
    {
        cXmlIO xmlConfig;
        DataView DictClass;

        #region ���� ���� ����
        public cDict()
        {
            try
            {
                xmlConfig = new cXmlIO(Program.getPrjPath () + "dict.xml");

                //��ȡTaskClass�ڵ�
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

        #region �����ֵ������������ֵ���������
        //�������ȵ���GetDictClassCount
        public int GetDictClassCount()
        {
            DictClass  = new DataView();
            DictClass = xmlConfig.GetData("descendant::DictClasses");
            int tCount = DictClass.Count;
            return tCount;
        }

        //�����ƶ���index����DictClassName
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

        #region �����ֵ估�ֵ����ݵ�DataView

        public DataView GetDictClass()
        {
            DataView dClass = new DataView();
            dClass = xmlConfig.GetData("descendant::DictClasses");
            return dClass;
        }

        //����ָ�����ֵ����Ʒ����ֵ�����
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

        #region �ֵ������ɾ����
        public void AddDictClass(string DictClassName)
        {
            //��ӷ���Ľڵ�
            string strDictClass;

            strDictClass = "<Name>" + cTool.ReplaceTrans( DictClassName) + "</Name>";
            xmlConfig.InsertElement("DictConfig/DictClasses", "DictClass", strDictClass);

            //ͬʱ����Ҫ��ӷ���ڵ��µ����ݽڵ�
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

        ////�༭�ֵ���������
        //public void EditDictClass(string OldName, string NewName)
        //{
        //    xmlConfig.EditNode("DictClass", OldName, NewName);
        //    xmlConfig.EditNodeName("DictConfig/Dict/Dict" + OldName, "Dict" + OldName, "Dict" + NewName);
        //    xmlConfig.Save();
        //}

        //�༭�ֵ�����
        public void EditDict(string DictClass, string Old_Name,string DictName)
        {
            xmlConfig.EditNode(DictClass, Old_Name, DictName);
            xmlConfig.Save();
        }

        //ɾ������ڵ�
        public void DelDictClass(string DictClass)
        {

            xmlConfig.DeleteChildNodes("DictClasses" , "Name",DictClass);
            xmlConfig.DeleteNode("Dict" + DictClass);
            xmlConfig.Save();
        }


        //ɾ���������ֵ������
        public void DelDict(string DictClass, string Dict)
        {
            xmlConfig.DeleteChildNodes("Dict" + DictClass,"Name", Dict);

            xmlConfig.Save();
        }
        #endregion 

    }
}
