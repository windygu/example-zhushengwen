using System;
using System.Collections.Generic;
using System.Text;

///���ܣ����񼯺Ϲ���
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget.Task
{
    class cTasks
    {
        public cTasks()
        {
        }

        ~cTasks()
        {
        }

        //����һ��������
        public List<cTask> Task
        {
            get { return Task; }
            set { Task = value; }
        }

        //����ָ����������෵��һ�����񼯺�
        //����ֵΪ���ϵ���Ŀ
        public int GetTasksByClass(int StateID)
        {
            return 0;
        }

        //���ݵ�ǰ��״̬����һ�����񼯺�
        //Ʃ������ִ�е�����
        public int GetTaskByState(int StateID)
        {
            return 0;
        }
    }
}
