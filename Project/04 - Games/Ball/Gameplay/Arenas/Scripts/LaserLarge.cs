using Ball.Gameplay.Arenas;
using FarseerPhysics.Factories;
using LBE.Physics;
using Microsoft.Xna.Framework;
using LBE.Graphics.Sprites;
using LBE;
using System;
using LBE.Gameplay;
using FarseerPhysics.Dynamics;
using Ball.Gameplay;
using Ball;
using Ball.Physics;
using Ball.Gameplay.Arenas.Objects;

namespace Content.Arenas
{   
    public class LaserLarge : ArenaScript
    {
        int m_side;

        Timer m_laserTimer;
        TimerEvent m_laserTimerEvent;
        Timer m_laserTimer2;
        TimerEvent m_laserTimer2Event;
        Timer m_delayTimer;
        TimerEvent m_delayTimerEvent;

        Laser[] m_laser;

        int m_laserOnTimeMs = 12000;
        int m_delayTimeMs = 4000;

        public override void OnInitGeometry()
        {
            Engine.World.EventManager.AddListener((int)EventId.HalfTimeTransition, OnHalfTimeTransition);

            m_laser = new Laser[4];
            for (int i = 0; i < 4; i++)
            {
                m_laser[i] = new Laser(0.88f);
            }

            float laserPosX1 = 410;
            float laserPosX2 = 577;

            m_laser[0].LaserPosition = new Vector2(laserPosX1, -1);
            m_laser[1].LaserPosition = new Vector2(laserPosX2, -1);
            m_laser[2].LaserPosition = new Vector2(-laserPosX2, -1);
            m_laser[3].LaserPosition = new Vector2(-laserPosX1, -1);
                
            for (int i = 0; i < 4; i++)
            {
                GameObject obj = new GameObject("Laser");
                obj.Attach(m_laser[i]);
            }

            m_laserTimer = new Timer(Engine.GameTime.Source, m_laserOnTimeMs, TimerBehaviour.Restart);
            m_laserTimer2 = new Timer(Engine.GameTime.Source, m_laserOnTimeMs, TimerBehaviour.Restart);
            m_delayTimer = new Timer(Engine.GameTime.Source, m_delayTimeMs);
            m_laserTimerEvent = new TimerEvent(m_laserTimer_OnTime);
            m_laserTimer.OnTime += m_laserTimerEvent;
            m_laserTimer2Event = new TimerEvent(m_laserTimer2_OnTime);
            m_laserTimer2.OnTime += m_laserTimer2Event;
            m_delayTimerEvent = new TimerEvent(m_delayTimer_OnTime);
            m_delayTimer.OnTime += m_delayTimerEvent;
            m_laserTimer.Start();
            m_laser[1].StartLaser();
            m_laser[2].StartLaser();
            m_delayTimer.Start();

            var rbCmp = Arena.Owner.RigidBodyCmp;

            float width = ConvertUnits.ToSimUnits(Arena.ArenaHalfSize.X);
            float height = ConvertUnits.ToSimUnits(Arena.ArenaHalfSize.Y);
            float goalWidth = ConvertUnits.ToSimUnits(100);
            float goalHeight = ConvertUnits.ToSimUnits(80);

            rbCmp.Body.CollidesWith = (Category)CollisionType.All | (Category)CollisionType.Ball;
            rbCmp.Body.CollisionCategories = (Category)CollisionType.Wall;
        }



        void m_laserTimer_OnTime(Timer source)
        {

                m_laser[1].StartLaser();
                m_laser[2].StartLaser();

        }

        void m_laserTimer2_OnTime(Timer source)
        {

                m_laser[0].StartLaser();
                m_laser[3].StartLaser();

        }

        void m_delayTimer_OnTime(Timer source)
        {
            m_laser[0].StartLaser();
            m_laser[3].StartLaser();
            m_laserTimer2.Start();

        }


        public override void OnUpdate()
        {
        }

        public override void OnEnd()
        {
            m_laserTimer.Stop();
            m_laserTimer2.Stop();
            m_delayTimer.Stop();

            m_laserTimer.OnTime -= m_laserTimerEvent;
            m_laserTimer2.OnTime -= m_laserTimer2Event;
            m_delayTimer.OnTime -= m_delayTimerEvent;



            Engine.World.EventManager.RemoveListener((int)EventId.HalfTimeTransition, OnHalfTimeTransition);
        }

        public void OnHalfTimeTransition(object arg)
        {
            if (Arena.Overlay != null && Arena.Overlay != null)
            {
                Arena.Overlay.Visible = false;
                Arena.OverlayAlt.Visible = true;
            }
        }
    }
}
