using System;
using System.Collections.Generic;
using System.Text;


///���ܣ�Soukey��ժ�Զ��������Ϣ��������ʽ
///���ʱ�䣺δ���
///���ߣ�һ��
///�������⣺��
///�����ƻ�����һ���ṩ
///˵������ǰϵͳ�����ṩ�˹��ܣ�����ϵͳ���Ѿ������˴��࣬���κ��ô��������޳�
///�汾��01.10.00
///�޶�����
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
