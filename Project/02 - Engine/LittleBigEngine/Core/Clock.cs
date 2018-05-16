using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBE
{
    public class Clock
    {
        TimeSource m_source;
        public TimeSource Source
        {
            get { return m_source; }
        }

        bool m_active;
        public bool Active
        {
            get { return m_active; }
            set { m_active = value; }
        }

        float m_elapsedMS;
        public float ElapsedMS
        {
            get { return m_elapsedMS; }
        }

        float m_time;
        public float TimeMS
        {
            get { return m_time; }
            set { m_time = value; }
        }

        float m_speed;
        public float Speed
        {
            get { return m_speed; }
            set { m_speed = value; }
        }

        public Clock(TimeSource source)
        {
            m_active = false;
            m_time = 0;
            m_speed = 1.0f;
            m_elapsedMS = 0;

            m_source = source;
        }

        public void Start()
        {
            if(!m_active)
            {
                m_active = true;
                m_source.TimeElapsedEvent += new TimeWatcher(Tick);
            }
        }
        
        public void Stop()
        {
            if (m_active)
            {
                m_active = false;
                m_source.TimeElapsedEvent -= Tick;
            }
            m_time = 0;
            m_elapsedMS = 0;
        }

        public void Pause()
        {
            if (m_active)
            {
                m_active = false;
                m_source.TimeElapsedEvent -= Tick;
            }
            m_elapsedMS = 0;
        }

        public void Reset()
        {
            m_time = 0;
            m_elapsedMS = 0;
        }

        public virtual void Tick(TimeSource source, float time)
        {
            if (m_active)
            {
                m_elapsedMS = m_speed * time;
                m_time += m_speed * time;
            }
            else
            {
                m_elapsedMS = 0;
            }
        }

    }
}
