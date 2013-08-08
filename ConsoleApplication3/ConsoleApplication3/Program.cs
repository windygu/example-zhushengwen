using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Word;
using Word = Microsoft.Office.Interop.Word;
using System.Windows.Forms;

namespace ConsoleApplication3
{
    class Program
    {
        
        static void Main(string[] args)
        {
            WordOperate.KillWordProcess();
            WordOperate.CreateWordFile();
            WordOperate.KillWordProcess();
        }
        


    }
}
