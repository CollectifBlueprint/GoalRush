using Ball.Gameplay.Arenas;
using FarseerPhysics.Factories;
using LBE.Physics;
using Microsoft.Xna.Framework;
using LBE.Gameplay;
using Microsoft.Xna.Framework.Graphics;
using LBE;
using LBE;
using LBE.Graphics.Sprites;
using FarseerPhysics.Common;
using System;
using LBE.Assets;
using Ball;
using Ball.Gameplay.Arenas.Objects;
using FarseerPhysics.Dynamics;
using Ball.Physics;

namespace Content.Arenas
{
    public class JungleArena2 : ArenaScript
    {
        Timer m_wallTimer;

        GameObject m_centerPieceObject;

        bool moving = false;

        LBE.Core.Transform[] m_launchersInitTransform;

        public override void OnInitGeometry()
        {
            var rbCmp = Arena.Owner.RigidBodyCmp;

            m_wallTimer = new Timer(Engine.GameTime.Source, 1500);
            m_wallTimer.OnTime += new TimerEvent(StartRotation);

            Asset<GameObjectDefinition> centerObjDef = Engine.AssetManager.GetAsset<GameObjectDefinition>("Arenas/JungleArena2/Center.lua::Center");
            m_centerPieceObject = new GameObject(centerObjDef);
            var centerPieceObjBodyCmp = m_centerPieceObject.FindComponent<RigidBodyComponent>();
            centerPieceObjBodyCmp.Body.CollidesWith = (Category)CollisionType.All | (Category)CollisionType.Ball;
            centerPieceObjBodyCmp.Body.CollisionCategories = (Category)CollisionType.Wall;

            Engine.World.EventManager.AddListener((int)EventId.FirstPeriod, OnFirstPeriod);
            Engine.World.EventManager.AddListener((int)EventId.HalfTime, OnHalfTime);
            Engine.World.EventManager.AddListener((int)EventId.HalfTimeTransition, OnHalfTimeTransition);
            Engine.World.EventManager.AddListener((int)EventId.SecondPeriod, OnSecondPeriod);
            Engine.World.EventManager.AddListener((int)EventId.MatchEnd, OnMatchEnd);

            m_launchersInitTransform = new LBE.Core.Transform[Arena.Launchers.Count];

            for (int i = 0; i < Arena.Launchers.Count; i++)
            {
                LauncherComponent launcher = (LauncherComponent)Arena.Launchers[i];
                m_launchersInitTransform[i] = new LBE.Core.Transform(launcher.Position, launcher.Owner.Orientation);
            }

            rbCmp.Body.CollidesWith = (Category)CollisionType.All | (Category)CollisionType.Ball;
            rbCmp.Body.CollisionCategories = (Category)CollisionType.Wall;

        }

        void StartRotation(Timer source)
        {
            moving = true;
        }

        public void OnFirstPeriod(object eventParamater)
        {
            m_wallTimer.Start();
            speed = 0;
        }

        public void OnHalfTime(object eventParamater)
        {
            moving = false;
        }

        public void OnHalfTimeTransition(object eventParamater)
        {
            angle = (float)Math.PI;
            moving = false;
            speed = 0;
        }

        public void OnSecondPeriod(object eventParamater)
        {
            m_wallTimer.Start();
        }

        public void OnMatchEnd(object eventParamater)
        {
            moving = false;
        }

        float angle = -(float)Math.PI;
        float maxSpeed = 0.00015f;
        float speed = 0;
        float accelTime = 1500;
        public override void OnUpdate()
        {
            Engine.Log.Debug("lp ", m_launchersInitTransform[0]);
            
            if (moving)
            {
                speed += Engine.GameTime.ElapsedMS / accelTime * maxSpeed;
                speed = LBE.MathHelper.Clamp(0, maxSpeed, speed);
                angle += speed * Engine.GameTime.ElapsedMS;
            }
            else if (speed > 0)
            {
                speed -= Engine.GameTime.ElapsedMS / accelTime * maxSpeed;
                speed = LBE.MathHelper.Clamp(0, maxSpeed, speed);
                angle += speed * Engine.GameTime.ElapsedMS;
            }

            m_centerPieceObject.Orientation = angle;

            float goalRadius = Engine.Debug.EditSingle("GoalPos", 120);
            Arena.LeftGoal.Owner.Orientation = angle;
            Arena.LeftGoal.Owner.Position = goalRadius * Vector2.UnitX.Rotate(angle);
            Arena.RightGoal.Owner.Orientation = angle;
            Arena.RightGoal.Owner.Position = -goalRadius * Vector2.UnitX.Rotate(angle);

            for (int i = 0; i < Arena.Launchers.Count; i++)
            {
                LauncherComponent launcher = (LauncherComponent) Arena.Launchers[i];

                launcher.Owner.Orientation = m_launchersInitTransform[i].Orientation + angle;
                launcher.Owner.Position = m_launchersInitTransform[i].Position.Rotate(angle);
            }
        }
    }
}
