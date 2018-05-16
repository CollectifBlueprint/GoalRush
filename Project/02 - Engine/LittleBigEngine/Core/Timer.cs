using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBE
{
    public enum TimerBehaviour
    {
        Restart,
        Stop
    }

    public delegate void TimerEvent(Timer source);

    public class Timer : Clock
    {
        public event TimerEvent OnTime;

        TimerBehaviour m_behaviour;
        public TimerBehaviour Behaviour
        {
            get { return m_behaviour; }
            set { m_behaviour = value; }
        }

        float m_targetTimeMS;
        public float TargetTime
        {
            get { return m_targetTimeMS; }
            set { m_targetTimeMS = value; }
        }

        public float Completion
        {
            get { return TimeMS / m_targetTimeMS; }
        }

        public Timer(Clock clock, float targetTimeMS)
            : this(clock.Source, targetTimeMS, TimerBehaviour.Stop)
        {
        }

        public Timer(TimeSource source, float targetTimeMS)
            : this(source, targetTimeMS, TimerBehaviour.Stop)
        {
        }

        public Timer(TimeSource source, float targetTimeMS, TimerBehaviour behaviour)
            : base(source)
        {
            m_behaviour = behaviour;
            m_targetTimeMS = targetTimeMS;
        }

        public override void Tick(TimeSource source, float timeMS)
        {
            base.Tick(source, timeMS);

            if (Speed > 0 && TimeMS > m_targetTimeMS || Speed < 0 && TimeMS < m_targetTimeMS)
            {
                if (OnTime != null) OnTime(this);

                if (m_behaviour == TimerBehaviour.Restart)
                {
                    Reset();
                }
                else if (m_behaviour == TimerBehaviour.Stop)
                {
                    Stop();
                }
            }
        }
    }
}
