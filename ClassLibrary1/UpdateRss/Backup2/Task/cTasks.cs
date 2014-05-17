using System;
using System.Collections.Generic;
using System.Text;

///功能：任务集合管理
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
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

        //定义一个集合类
        public List<cTask> Task
        {
            get { return Task; }
            set { Task = value; }
        }

        //根据指定的任务分类返回一个任务集合
        //返回值为集合的数目
        public int GetTasksByClass(int StateID)
        {
            return 0;
        }

        //根据当前的状态返回一个任务集合
        //譬如正在执行的任务
        public int GetTaskByState(int StateID)
        {
            return 0;
        }
    }
}
