using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using LBE;

namespace Ball.Gameplay
{
    public class ScreenFocus
    {
        static ScreenFocus m_instance;
        public static ScreenFocus Instance
        {
            get { return ScreenFocus.m_instance; }
        }

        float m_decay = 1;

        Vector2 m_position;
        public static Vector2 Position
        {
            get { return m_instance.m_position; }
        }

        float m_strength;
        public static float Strength
        {
            get { return m_instance.m_strength; }
        }

        public ScreenFocus()
        {
            m_instance = this;
        }

        public void Update()
        {
            m_decay = Engine.Debug.EditSingle("FocusDecay", 1);
            var deltaStrenght = 0.001f * m_decay * Engine.GameTime.ElapsedMS;
            m_strength = LBE.MathHelper.Clamp(0, 1, m_strength - deltaStrenght);

            var maxTimeCoef = 0.93f;
            Engine.TimeCoef = 1 - maxTimeCoef * m_strength;

            if (m_strength > 0)
            {
            }
        }

        public void Focus(Vector2 position, float strength)
        {
            m_position = position;
            m_strength = strength;
        }
    }
}
