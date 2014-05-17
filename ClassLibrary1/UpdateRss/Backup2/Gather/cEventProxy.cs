using System;
using System.Collections.Generic;
using System.Text;

///���ܣ��¼�����
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
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
        /// �����������д����¼�
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
                {   // ˫�ؼ��
                    for (int i = 0; i < events.Length; i++)
                    {
                        events[i]();
                    }
                }
            }
        }
        /// <summary>
        /// ����¼�����
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
