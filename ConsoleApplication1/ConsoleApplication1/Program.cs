using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Xml.XmlDocument xx = new System.Xml.XmlDocument();
            xx.Load(@"C:\Documents and Settings\Administrator\My Documents\Visual Studio 2010\Projects\ConsoleApplication1\ConsoleApplication1\bin\Debug\123.xml");
            int count = xx.ChildNodes[1].ChildNodes.Count;
        }
    }
}
