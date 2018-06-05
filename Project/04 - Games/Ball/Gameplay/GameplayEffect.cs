using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using LBE;

namespace Ball.Gameplay
{
    public class GameplayEffect
    {
        Timer m_timer;
        public Timer Timer
        {
            get { return m_timer; }
        }
        TimerEvent m_TimerEvent;

        bool m_active;
        public bool Active
        {
            get { return m_active; }
            set { m_active = true; }
        }

        public GameplayEffect()
        {
            m_timer = new Timer(Engine.GameTime.Source, 0);
            TimerEvent m_TimerEvent = new TimerEvent(m_timer_OnTime);
            m_timer.OnTime += m_TimerEvent;
        }

        public void SetDuration(float timeMs)
        {
            m_timer.TargetTime = timeMs;
            m_timer.Start();
        }

        void m_timer_OnTime(Timer source)
        {
            Cancel();
        }

        public void Cancel()
        {
            m_active = false;
        }

        public virtual void Start()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void End()
        {
            m_timer.OnTime -= m_TimerEvent;
        }
    }
}
