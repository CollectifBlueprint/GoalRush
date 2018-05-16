using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBE.Gameplay
{
    //
    public class EventManager
    {

        //      
        Dictionary<int, List<EventAction>> m_eventListeners;
        
        //
        public delegate void EventAction(params object[] eventParameters);
        

        //
        public EventManager()
        {
            m_eventListeners = new Dictionary<int, List<EventAction>>();
        }
       

        //
        public void AddListener(int eventId, EventAction eventAction)
        {
            if (!m_eventListeners.ContainsKey(eventId))
                m_eventListeners[eventId] = new List<EventAction>();

            m_eventListeners[eventId].Add(eventAction);
        }

        public void RemoveListener(int eventId, EventAction eventAction)
        {
            if (m_eventListeners.ContainsKey(eventId))
                m_eventListeners[eventId].Remove(eventAction);
        }

        //
        public void ThrowEvent(int eventId, params object[] eventParameters)
        {
            if (!m_eventListeners.ContainsKey(eventId))
                return;

            foreach (EventAction eventIdAction in m_eventListeners[eventId])
            {
                eventIdAction(eventParameters);
            }
        }


        //
        public void RemoveAllListeners()
        {
            m_eventListeners.Clear();
        }
    }
}
