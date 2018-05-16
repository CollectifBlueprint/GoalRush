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
    public class TestArena : ArenaScript
    {
        int m_side;

        Timer m_laserTimer;
        Laser[] m_laser;

        public override void OnInitGeometry()
        {
            Engine.World.EventManager.AddListener((int)EventId.HalfTimeTransition, OnHalfTimeTransition);
            
            m_laserTimer = new Timer(Engine.GameTime.Source, 6000, TimerBehaviour.Restart);
            m_laserTimer.OnTime += new TimerEvent(m_laserTImer_OnTime);
            m_laserTimer.Start();

            m_laser = new Laser[4];
            for (int i = 0; i < 4; i++)
            {
                m_laser[i] = new Laser();
            }

            m_laser[0].LaserPosition = new Vector2(241, 219);
            m_laser[1].LaserPosition = new Vector2(241, -220);
            m_laser[2].LaserPosition = new Vector2(-239, 219);
            m_laser[3].LaserPosition = new Vector2(-239, -220);
                
            for (int i = 0; i < 4; i++)
            {
                GameObject obj = new GameObject("Laser");
                obj.Attach(m_laser[i]);
            }

            var rbCmp = Arena.Owner.RigidBodyCmp;

            float width = ConvertUnits.ToSimUnits(Arena.ArenaHalfSize.X);
            float height = ConvertUnits.ToSimUnits(Arena.ArenaHalfSize.Y);
            float goalWidth = ConvertUnits.ToSimUnits(100);
            float goalHeight = ConvertUnits.ToSimUnits(80);

            //Obstacle
            FixtureFactory.AttachRectangle(
                ConvertUnits.ToSimUnits(40),
                ConvertUnits.ToSimUnits(240),
                0,
                ConvertUnits.ToSimUnits(new Vector2(-240, 0)),
                rbCmp.Body);

            //Obstacle
            FixtureFactory.AttachRectangle(
                 ConvertUnits.ToSimUnits(40),
                 ConvertUnits.ToSimUnits(240),
                 0,
                 ConvertUnits.ToSimUnits(new Vector2(240, 0)),
                 rbCmp.Body);

            rbCmp.Body.CollidesWith = (Category)CollisionType.All | (Category)CollisionType.Ball;
            rbCmp.Body.CollisionCategories = (Category)CollisionType.Wall;
        }

        void m_laserTImer_OnTime(Timer source)
        {
            if (m_side % 2 == 0)
            {
                m_laser[0].StartLaser();
                m_laser[3].StartLaser();
            }
            else
            {
                m_laser[1].StartLaser();
                m_laser[2].StartLaser();
            }
            m_side++;
        }

        public override void OnUpdate()
        {
        }

        public override void OnEnd()
        {
            m_laserTimer.Stop();
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
