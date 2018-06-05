using Ball.Gameplay.Arenas;
using FarseerPhysics.Factories;
using LBE.Physics;
using Microsoft.Xna.Framework;
using LBE.Gameplay;
using Microsoft.Xna.Framework.Graphics;
using LBE;
using LBE.Graphics.Sprites;
using FarseerPhysics.Common;
using System;
using LBE.Assets;
using Ball;
using FarseerPhysics.Dynamics;
using Ball.Physics;

namespace Content.Arenas
{
    public class JungleBasic : ArenaScript
    {
        GameObject[] m_walls;
        Timer m_wallTimer;
        TimerEvent m_wallTimerEvent;

        bool m_rotating;
        float m_currentAngle = 0;
        float m_targetAngle = 0;

        public override void OnInitGeometry()
        {
            var rbCmp = Arena.Owner.RigidBodyCmp;

            Asset<GameObjectDefinition> wallDef = Engine.AssetManager.GetAsset<GameObjectDefinition>("Arenas/JungleBasic/Wall.lua::Wall");

            m_walls = new GameObject[3];
            m_walls[0] = new GameObject(wallDef);
            m_walls[1] = new GameObject(wallDef);
            m_walls[2] = new GameObject(wallDef);

            for (int i = 0; i < m_walls.Length; ++i)
            {
                var wallRbCmp = m_walls[i].FindComponent<RigidBodyComponent>();
                wallRbCmp.Body.CollidesWith = (Category)CollisionType.All | (Category)CollisionType.Ball;
                wallRbCmp.Body.CollisionCategories = (Category)CollisionType.Wall;
            }

            SetWallPosition();

            m_wallTimer = new Timer(Engine.GameTime.Source, 25000, TimerBehaviour.Restart);
            m_wallTimerEvent = new TimerEvent(m_wallTimer_OnTime);
            m_wallTimer.OnTime += m_wallTimerEvent;

            Engine.World.EventManager.AddListener((int)EventId.FirstPeriod, OnFirstPeriod);
            Engine.World.EventManager.AddListener((int)EventId.HalfTime, OnHalfTime);
            Engine.World.EventManager.AddListener((int)EventId.HalfTimeTransition, OnHalfTimeTransition);
            Engine.World.EventManager.AddListener((int)EventId.SecondPeriod, OnSecondPeriod);
            Engine.World.EventManager.AddListener((int)EventId.MatchEnd, OnMatchEnd);

            rbCmp.Body.CollidesWith = (Category)CollisionType.All | (Category)CollisionType.Ball;
            rbCmp.Body.CollisionCategories = (Category)CollisionType.Wall;
        }

        public void OnFirstPeriod(object eventParamater)
        {
            m_rotating = false;
            m_wallTimer.TimeMS = m_wallTimer.TargetTime / 2;
            m_wallTimer.Start();
        }

        public void OnHalfTime(object eventParamater)
        {
        }

        public void OnHalfTimeTransition(object eventParamater)
        {
            m_rotating = false;
            m_currentAngle = 0;
            if (Arena.Overlay != null && Arena.Overlay != null)
            {
                Arena.Overlay.Visible = false;
                Arena.OverlayAlt.Visible = true;
            }
        }

        public void OnSecondPeriod(object eventParamater)
        {
            m_wallTimer.TimeMS = m_wallTimer.TargetTime / 2;
            m_wallTimer.Start();
        }

        public void OnMatchEnd(object eventParamater)
        {
            m_wallTimer.Stop();
            m_wallTimer.OnTime -= m_wallTimerEvent;
        }

        public override void OnUpdate()
        {
            if (m_rotating)
            {
                float speed = 0.0001f;
                m_currentAngle += speed * Engine.GameTime.ElapsedMS;
                if (m_currentAngle > m_targetAngle)
                {
                    m_currentAngle = m_targetAngle;
                    m_rotating = false;
                }
            }

            float goalRadius = Engine.Debug.EditSingle("GoalPosBasic", 300);
            Arena.LeftGoal.Owner.Position = goalRadius * Vector2.UnitX;
            Arena.RightGoal.Owner.Position = -goalRadius * Vector2.UnitX;

            SetWallPosition();
        }

        private void SetWallPosition()
        {
            for (int i = 0; i < 3; i++)
            {
                float radius = 310;
                float wallAngle = m_currentAngle + 2 * i * (float)Math.PI / 3;
                m_walls[i].Position = radius * Vector2.UnitX.Rotate(wallAngle);
                m_walls[i].Orientation = wallAngle;
            }
        }

        void m_wallTimer_OnTime(Timer source)
        {
            m_targetAngle = m_currentAngle + (float)Math.PI / 3;
            m_rotating = true;
        }


    }
}
