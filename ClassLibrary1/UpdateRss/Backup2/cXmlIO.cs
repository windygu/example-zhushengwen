using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

///���ܣ�xml�ļ�����
///���ʱ�䣺2009��3��2
///���ߣ�һ��
///�������⣺��ǰ��xml�������Ǻܲ����㣬��xml�ļ���ʽ֧�ֵĲ�����
/// ������Ҫ����һ�׶����½����޸�
///�����ƻ�����
///˵�����μ�ע��
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget
{
    class cXmlIO
    {
        protected XmlDocument objXmlDoc;
        protected string strXmlFile;

        public cXmlIO()
        {
            objXmlDoc = new XmlDocument();
        }

        public cXmlIO(string XmlFile)
        {
            objXmlDoc = new XmlDocument();

            try
            {
                objXmlDoc.Load(XmlFile);
                
            }
            catch (System.Exception ex)
            {
                objXmlDoc = null;
                throw ex;
            }
            strXmlFile = XmlFile;
         }

        ~cXmlIO()
        {
            objXmlDoc = null;
        }

        public void NewXmlFile(string FileName,string strXml )
        {
            //��ȡ·��

            //string s = "\\b.*(?=\\\\)\\b";
            //Match m = Regex.Match(FileName, s);
            //string fPath = m.Groups[0].Value.ToString();

            string fPath = Path.GetDirectoryName(FileName);

            if (!System.IO.Directory.Exists(fPath))
            {
                //Ŀ¼�����ڣ�������Ҫ������Ŀ¼
                System.IO.Directory.CreateDirectory(fPath);
            }

            objXmlDoc = new XmlDocument();
            objXmlDoc.LoadXml(strXml);

            if (File.Exists(FileName))
            {
                File.SetAttributes(FileName, System.IO.FileAttributes.Normal);
            }

            objXmlDoc.Save(FileName);

            strXmlFile = FileName;

        }

        //����ָ����·����ȡһ��ֵ
        public string GetNodeValue(string nodPath)
        {
            XmlNode gNode = objXmlDoc.SelectSingleNode(nodPath);

            if (gNode == null)
                return "";
            else
                return  gNode.InnerText.ToString ();
        }

        //���ݽڵ㷵������,����Ϊdataview
        public DataView GetData(string XmlPathNode)
        {
            DataSet ds = new DataSet();
            StringReader read = new StringReader(objXmlDoc.SelectSingleNode(XmlPathNode).OuterXml);
            ds.ReadXml(read);
            if (ds.Tables.Count == 0)
            {
                return null;
            }
            else
            {
                return ds.Tables[0].DefaultView;
            }
        }

        //����ָ���ڵ�ı��,�����ؽڵ�����,���ܷ��ص���һ����¼,
        //��������ΪDataView���з���,��������Ŀ����Ϊ�˸��õķ�����
        public DataView GetData(string NodeCollection, string Node, string content)
        {
            XmlNodeList fathernode = objXmlDoc.GetElementsByTagName(NodeCollection);
            XmlNodeList nodes = fathernode[0].ChildNodes;

            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = 0; j < nodes[i].ChildNodes.Count; j++)
                {
                    //for (int m=0;
                    if (nodes[i].ChildNodes[j].Name == Node && nodes[i].ChildNodes[j].InnerText == content)
                    {
                        StringReader read = new StringReader(nodes[i].OuterXml);
                        DataSet ds = new DataSet();
                        ds.ReadXml(read);
                        if (ds.Tables.Count == 0)
                        {
                            return null;
                        }
                        else
                        {
                            return ds.Tables[0].DefaultView;
                        }

                    }
                }

            }
            return null;
           
        }

        //ɾ���ڵ�
        //����ָ���Ľڵ�ɾ���˽ڵ��Լ��˽ڵ�һ�µ�����
        public void DeleteNode(string Node)
        {
            XmlNodeList nodes = objXmlDoc.GetElementsByTagName(Node);
            XmlNode delNode = nodes[0];
            delNode.ParentNode.RemoveChild(delNode);
        }

        //����ָ���Ľڵ㣬ɾ���ӽڵ����content���ݵ��ӽڵ�
        //�˷����Ƚ����⣬�����soukey��ժ�����е�xml�����в���������֧�����е�xml�ļ�
        //��soukey��ժ�е�xml�ļ��У�ͨ��������1�Զ�Ĺ�ϵ�����ֹ�ϵͨ��һ����
        //�ظ��Ľڵ�����ʾ������ɾ����ʱ�򣬲���ָ������ڵ㣬����ָ������ڵ�ĸ��ڵ�
        //��ΪҪѭ�������е����ݣ����ݼ����е�һ���ڵ㣬���µ�����������ɾ��
        //����<tasks><task><id>1</id><name>soukey</name></task><task><id>2</id><name>��ժ</name></task></tasks>
        //ɾ���ӽڵ���ָɾ��task�ڵ㣬�����ݵ�������ָ����id����name����content�����ݣ�
        //���Ե��÷�����DeleteChildNodes("tasks","name","soukey")
        //���ú󣬽�ɾ��task��name��soukey��task�ڵ�,�����MainNode������һ�����ϣ�����������һ������ӽڵ㣬
        //�����´���
        public void DeleteChildNodes(string NodeCollection,string Node, string content)
        {
            XmlNodeList fathernode = objXmlDoc.GetElementsByTagName(NodeCollection);
            XmlNodeList nodes = fathernode[0].ChildNodes;

            for (int i=0;i< nodes.Count; i++)
            {
                for (int j = 0; j < nodes[i].ChildNodes.Count; j++)
                {
                    //for (int m=0;
                    if (nodes[i].ChildNodes[j].Name ==Node &&  nodes[i].ChildNodes[j].InnerText == content)
                    {
                        fathernode[0].RemoveChild(nodes[i]);
                        return;
                    }
                }

            }

        }

        //����һ���ڵ�ʹ˽ڵ��һ�ӽڵ�
        public void InsertNode(string MainNode,string ChildNode,string Element,string Content)
        {
           XmlNode objRootNode = objXmlDoc.SelectSingleNode(MainNode);
           XmlElement objChildNode = objXmlDoc.CreateElement(ChildNode);
           objRootNode.AppendChild(objChildNode);
           XmlElement objElement = objXmlDoc.CreateElement(Element);
           objElement.InnerText = Content;
           objChildNode.AppendChild(objElement);

        }

        //�޸�һ���ڵ��������Ϣ��Ϣ
        public void EditNode(string Element, string Old_Content,string Content)
        {

            XmlNodeList nodes = objXmlDoc.GetElementsByTagName(Element);

            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                if (nodes[i].ChildNodes[0].InnerText == Old_Content) 
                {
                    nodes[i].ChildNodes[0].InnerText = Content;
                }

            }
        }

        //�޸�һ���ڵ㱾���ֵ
        public void EditNodeName(string nodPath, string OldName, string NewName)
        {
            XmlNode Nod = objXmlDoc.SelectSingleNode(nodPath);
            string xml = Nod.InnerXml;

            DeleteNode(OldName);

            nodPath = nodPath.Substring(0, nodPath.LastIndexOf("/"));

            InsertElement(nodPath, NewName, xml);

        }

        //����ָ���Ľڵ��޸���ֵ
        public void EditNodeValue(string nodPath,string NewValue)
        {
            XmlNode Nod= objXmlDoc.SelectSingleNode(nodPath);
            Nod.InnerText = NewValue;
        }

        //����һ���ڵ㣬��һ������
        public void InsertElement(string MainNode,string Element,string Attrib,string AttribContent,string Content)
        {
           XmlNode objNode = objXmlDoc.SelectSingleNode(MainNode);
           XmlElement objElement = objXmlDoc.CreateElement(Element);
           objElement.SetAttribute(Attrib,AttribContent);
           objElement.InnerText = Content;
           objNode.AppendChild(objElement);
        }

        //����һ���ڵ�
        public void InsertElement(string MainNode,string Element,string Content)
        {
           XmlNode objNode = objXmlDoc.SelectSingleNode(MainNode);
           XmlElement objElement = objXmlDoc.CreateElement(Element);
           objElement.InnerXml  = Content;
           objNode.AppendChild(objElement);
        }

        private readonly Object m_fileLock = new Object();
        //����xml�ļ�
        public void Save()
        {
           try
           {
               if (File.Exists(strXmlFile))
               {
                   File.SetAttributes(strXmlFile, System.IO.FileAttributes.Normal);
               }
               objXmlDoc.Save(strXmlFile);

           }
           catch (System.Exception ex)
           {
                throw ex;
           }
           
        }

        //�˷���ר�����޸�taskrun.xml�вɼ�����ֵʹ�ã��������������ʹ��
        //�˷�����ͨ�ã���ǰ��xml��������������⣬������Ҫ�������
        //ͬʱ��Ҫ����Urlcount��Ҳ�����ֵ���Ǻ�׼ȷ�������ɼ���������ǣ������ˢ��
        public void EditTaskrunValue(string TaskID,cGlobalParas.TaskState tState, string GUrlCount,string GTrueUrlCount, string GErrUrlCount,string GTrueErrUrlCount, string TrueUrlCount)
        {
            XmlNodeList fathernode = objXmlDoc.GetElementsByTagName("Tasks");
            XmlNodeList nodes = fathernode[0].ChildNodes;

            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = 0; j < nodes[i].ChildNodes.Count; j++)
                {
                    //for (int m=0;
                    if (nodes[i].ChildNodes[j].Name == "TaskID" && nodes[i].ChildNodes[j].InnerText == TaskID)
                    {
                        XmlNode nod = nodes[i].SelectSingleNode("TaskState");
                        nod.InnerText = ((int)tState).ToString ();
                        nod = null;

                        nod = nodes[i].SelectSingleNode("TrueUrlCount");
                        nod.InnerText = TrueUrlCount;
                        nod=null;

                        nod = nodes[i].SelectSingleNode("GatheredUrlCount");
                        nod.InnerText = GUrlCount;
                        nod = null;

                        nod = nodes[i].SelectSingleNode("GatheredTrueUrlCount");
                        nod.InnerText = GTrueUrlCount;
                        nod=null;

                        nod = nodes[i].SelectSingleNode("ErrUrlCount");
                        nod.InnerText = GErrUrlCount;
                        nod =null;

                        nod = nodes[i].SelectSingleNode("TrueErrUrlCount");
                        nod.InnerText = GTrueErrUrlCount;
                        nod = null;

                        return;
                    }
                }

            }
        }


        public void EditNodeValue(string NodeCollection, string Node, string condition, string ValueName,string value)
        {
            XmlNodeList fathernode = objXmlDoc.GetElementsByTagName(NodeCollection);
            XmlNodeList nodes = fathernode[0].ChildNodes;

            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = 0; j < nodes[i].ChildNodes.Count; j++)
                {
                    //for (int m=0;
                    if (nodes[i].ChildNodes[j].Name == Node && nodes[i].ChildNodes[j].InnerText == condition)
                    {
                        XmlNode nod = nodes[i].SelectSingleNode(ValueName);
                        nod.InnerText = value;
                        return;
                    }
                }

            }

        }
   }

      
}
