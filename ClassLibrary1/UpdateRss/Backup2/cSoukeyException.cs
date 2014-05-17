using System;
using System.Collections.Generic;
using System.Text;


///功能：Soukey采摘自定义错误消息及错误处理方式
///完成时间：未完成
///作者：一孑
///遗留问题：无
///开发计划：下一版提供
///说明：当前系统还不提供此功能，但在系统中已经采用了此类，无任何用处，可以剔除
///版本：01.10.00
///修订：无
namespace SoukeyNetget
{
    class cSoukeyException:System.ApplicationException  
    {
        public cSoukeyException(): base("exception message")
        {
        }
    
        public cSoukeyException(string message): base(message)
        {
            
        }

        public cSoukeyException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
