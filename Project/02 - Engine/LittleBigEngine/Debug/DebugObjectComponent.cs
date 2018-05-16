using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Gameplay;

namespace LBE.Debug
{
    public class DebugObjectComponent : GameObjectComponent
    {
        Action<GameObject> m_action;

        public DebugObjectComponent(Action<GameObject> action)
        {
            m_action = action;
        }

        public DebugObjectComponent(Action<GameObject> action, float duration)
        {
            var timer = new Timer(Engine.GameTime.Source, duration);
            timer.OnTime += new TimerEvent(timer_OnTime);
            timer.Start();

            m_action = action;
        }

        void timer_OnTime(Timer source)
        {
            Owner.Kill();
        }

        public override void Update()
        {
            m_action(Owner);
        }
    }
}
