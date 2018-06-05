using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Gameplay;
using LBE;
using LBE.Graphics.Sprites;
using Microsoft.Xna.Framework;

namespace Ball.Menus
{
    public class MenuScaleFx : GameObjectComponent
    {
        GameObjectComponent m_targetCmp;
        Timer m_timer;
        TimerEvent m_timerEvent;

        public MenuScaleFx(TextComponent textCmp)
        {
            m_targetCmp = textCmp;
        }

        public MenuScaleFx(SpriteComponent sprtieComponent)
        {
            m_targetCmp = sprtieComponent;
        }

        public override void Start()
        {
            float time = Engine.Debug.EditSingle("MenuScaleTime", 200);
            m_timer = new Timer(Engine.GameTime, time);
            m_timerEvent = new TimerEvent(m_time_OnTime);
            m_timer.OnTime += m_timerEvent;
            m_timer.Start();
        }

        void m_time_OnTime(Timer source)
        {
            Owner.Remove(this);
        }

        public override void Update()
        {
            float progress = m_timer.TimeMS / m_timer.TargetTime;
            float scaleCoef = progress > 0.5f ? 2.0f * (1.0f - progress) : 2 * progress;

            float scale = LBE.MathHelper.Lerp(1.0f, Engine.Debug.EditSingle("MenuScale", 1.1f), scaleCoef);
            SetScale(scale);
        }

        public override void End()
        {
            m_timer.Stop();
            m_timer.OnTime -= m_timerEvent;
            SetScale(1);
        }

        private void SetScale(float scale)
        {
            if (m_targetCmp is TextComponent)
                ((TextComponent)m_targetCmp).Scale = scale;
            if (m_targetCmp is SpriteComponent)
                ((SpriteComponent)m_targetCmp).Scale = scale;
        }
    }
}
