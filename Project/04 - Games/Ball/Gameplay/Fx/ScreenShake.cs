using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using LBE;

namespace Ball.Gameplay
{
    public class ShakeInstance
    {
        public Vector2 Direction;
        public float Strength;
        public float Time;
    }

    public class ScreenShake
    {
        static ScreenShake m_instance;

        List<ShakeInstance> m_shakeInstances = new List<ShakeInstance>();

        Vector2 m_shake;
        public static Vector2 Shake
        {
            get { return m_instance.m_shake; }
            set { m_instance.m_shake = value; }
        }

        public ScreenShake()
        {
            m_instance = this;
        }

        public void Update()
        {
            Random random = new Random();

            float totalAmount = 0;
            Vector2 shakeBias = new Vector2();
            float shakeScale = Engine.Debug.EditSingle("Shake", 3);
            foreach (var shake in m_shakeInstances.ToArray())
            {
                totalAmount += shake.Strength;
                shakeBias += shake.Direction * shake.Strength;

                shake.Time -= Engine.RealTime.ElapsedMS;
            }

            //Remove expired shake instances
            m_shakeInstances.RemoveAll(shake => shake.Time <= 0);

            m_shake = shakeBias + totalAmount * shakeScale * random.NextVector2();
        }

        public static void Add(float strength, Vector2 dir, float time = 30)
        {
            m_instance.m_shakeInstances.Add(new ShakeInstance() { Strength = strength, Direction = dir, Time = time });
        }

        public static void Add()
        {
            Add(1.0f, Vector2.Zero);
        }

        public static void Add(Vector2 dir)
        {
            Add(1.0f, dir);
        }

        public static void Add(float strength)
        {
            Add(strength, Vector2.Zero);
        }
    }
}
