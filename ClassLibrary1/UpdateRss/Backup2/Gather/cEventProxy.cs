using System;
using System.Collections.Generic;
using System.Text;

///功能：事件代理
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace SoukeyNetget.Gather
{
    internal class cEventProxy
    {
        internal cEventProxy()
        {
            m_EventList = new List<EventInvoke>();
        }

        internal delegate void EventInvoke();
        private List<EventInvoke> m_EventList;
        private readonly Object m_eventLock = new Object();

        /// <summary>
        /// 立即处理所有代理事件
        /// </summary>
        internal void DoEvents()
        {
            if (m_EventList.Count > 0)
            {
                EventInvoke[] events;
                lock (m_eventLock)
                {
                    events = m_EventList.ToArray();
                    m_EventList = new List<EventInvoke>();
                }
                if (events.Length > 0)
                {   // 双重检查
                    for (int i = 0; i < events.Length; i++)
                    {
                        events[i]();
                    }
                }
            }
        }
        /// <summary>
        /// 添加事件代理
        /// </summary>
        /// <param name="evt"></param>
        internal void AddEvent(EventInvoke evt)
        {
            lock (m_eventLock)
            {
                m_EventList.Add(evt);
            }
        }
    }
}
