using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBE
{
    public delegate void TimeWatcher(TimeSource source, float time);

    public class TimeSource
    {
        public event TimeWatcher TimeElapsedEvent;

        float m_currentTime;
        public float Time
        {
            get { return m_currentTime; }
        }

        float m_elapsedTime;
        public float ElapsedTime
        {
            get { return m_elapsedTime; }
        }

        int m_ticks;
        public int Ticks
        {
            get { return m_ticks; }
            set { m_ticks = value; }
        }

        bool m_paused;
        public bool Paused
        {
            get { return m_paused; }
            set { m_paused = value; }
        }

        public TimeSource()
        {
            m_currentTime = 0;
            m_ticks = 0;
        }

        public void TickMS(float time)
        {
            if (m_paused)
                return;

            m_ticks++;
            m_currentTime += time;
            m_elapsedTime = time;

            if (TimeElapsedEvent != null) TimeElapsedEvent(this, time);
        }
    }
}
