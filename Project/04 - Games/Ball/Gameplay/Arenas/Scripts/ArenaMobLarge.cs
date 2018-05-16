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
using Ball.Physics;
using FarseerPhysics.Common;
using LBE.Assets;
using Ball.Graphics;
using Microsoft.Xna.Framework.Graphics;
using LBE.Utils;
using Ball;


namespace Content.Arenas
{
    public class ArenaMobLarge : ArenaScript
    {
        Timer m_rotateTimer;
        int m_timeFirstMS = 3000;
        int m_timeMS = 25000;

        GameObject[] m_mobileObjs;

        bool m_mobileObjMoving = false;

        float m_mobileObjTargetAngle = -(float)Math.PI;
        float m_mobileObjCurrentAngle = 0;
        float m_mobileObjMaxSpeed = 0.00016f;
        float m_mobileObjSpeed = 0;
        float m_mobileObjAccelTime = 1500;
        int m_clockwise = 1;

		int mobileObjsPosX = 483;
        
        public override void OnInitGeometry()
        {
            var rbCmp = Arena.Owner.RigidBodyCmp;

            float width = ConvertUnits.ToSimUnits(Arena.ArenaHalfSize.X);
            float height = ConvertUnits.ToSimUnits(Arena.ArenaHalfSize.Y);
            float goalWidth = ConvertUnits.ToSimUnits(100);
            float goalHeight = ConvertUnits.ToSimUnits(80);

            rbCmp.Body.CollidesWith = (Category)CollisionType.All | (Category)CollisionType.Ball;
            rbCmp.Body.CollisionCategories = (Category)CollisionType.Wall;

            m_mobileObjs = new GameObject[2];

            for (int i = 0; i < m_mobileObjs.Length; i++)
            {
                m_mobileObjs[i] = new GameObject("Arena Mobile Object");
                Sprite mobileObjAOSprite = Sprite.CreateFromTexture("Arenas/ArenaMobLarge/AOMob.png");
                mobileObjAOSprite.Scale = 0.5f * Vector2.One;
                ColorMaskedSprite mobileObjColorSprite  = new ColorMaskedSprite(mobileObjAOSprite, "ArenaOverlay3");


                var texAsset = Engine.AssetManager.GetAsset<Texture2D>("Arenas/ArenaMobLarge/materialMob" + i + ".png");
                mobileObjColorSprite.Mask = texAsset.Content;
                mobileObjColorSprite.Color1 = Arena.ColorScheme.Color2;
                mobileObjColorSprite.Color2 = Ball.Game.GameManager.Teams[i].ColorScheme.Color1.SetSaturation(0.5f); //mettre en public depuis Arena
                mobileObjColorSprite.Color3 = Ball.Game.GameManager.Teams[i].ColorScheme.Color1.SetSaturation(0.5f);
                
                m_mobileObjs[i].Attach(mobileObjColorSprite);

                Asset<Texture2D> textureAsset = Engine.AssetManager.GetAsset<Texture2D>("Arenas/ArenaMobLarge/collisionMob.png");
                CollisionDefinition collisionDefinition = CollisionDefinitionHelper.FromTexture(textureAsset.Content, 2.0f);
                for (int iCol = 0; iCol < collisionDefinition.Entries.Length; iCol++)
                    Engine.Log.Write(String.Format("Collision {0}: {1} vertices", iCol, collisionDefinition.Entries[iCol].Vertices.Length));
                
                RigidBodyComponentParameters parameters = new RigidBodyComponentParameters{ Collision = collisionDefinition };

                RigidBodyComponent rigidBodyCmp = new RigidBodyComponent(new Asset<RigidBodyComponentParameters>(parameters));
                m_mobileObjs[i].Attach(rigidBodyCmp);

                m_mobileObjs[i].RigidBodyCmp.Body.CollidesWith = (Category)CollisionType.Player | (Category)CollisionType.Ball;
                m_mobileObjs[i].RigidBodyCmp.Body.CollisionCategories = (Category)CollisionType.Wall;
            }

			m_mobileObjs[0].Position = new Vector2(-mobileObjsPosX, 0);
			m_mobileObjs[1].Position = new Vector2(mobileObjsPosX, 0);
            m_mobileObjs[0].Orientation = 0;
            m_mobileObjs[1].Orientation = 0;

            Engine.World.EventManager.AddListener((int)EventId.FirstPeriod, OnFirstPeriod);
            Engine.World.EventManager.AddListener((int)EventId.HalfTime, OnHalfTime);
            Engine.World.EventManager.AddListener((int)EventId.HalfTimeTransition, OnHalfTimeTransition);
            Engine.World.EventManager.AddListener((int)EventId.SecondPeriod, OnSecondPeriod);
            Engine.World.EventManager.AddListener((int)EventId.MatchEnd, OnMatchEnd);

            m_rotateTimer = new Timer(Engine.GameTime.Source, m_timeFirstMS, TimerBehaviour.Restart);
            m_rotateTimer.OnTime += new TimerEvent(StartRotation);
        }


        void StartRotation(Timer source)
        {
            m_mobileObjMoving = true;
            m_mobileObjTargetAngle = m_mobileObjCurrentAngle + (float)Math.PI;
            m_rotateTimer.TargetTime = m_timeMS;
            m_clockwise = -m_clockwise;
        }

        public void OnFirstPeriod(object eventParamater)
        {
            if (m_rotateTimer != null)
                m_rotateTimer.Stop();

            m_rotateTimer = new Timer(Engine.GameTime.Source, m_timeFirstMS, TimerBehaviour.Restart);
            m_rotateTimer.OnTime += new TimerEvent(StartRotation);
            m_mobileObjs[0].Orientation = 0;
            m_mobileObjs[1].Orientation = 0;
            m_mobileObjCurrentAngle = 0;

            m_clockwise = 1;

            m_rotateTimer.Start();
            m_mobileObjSpeed = 0;
        }

        public void OnHalfTime(object eventParamater)
        {
            m_mobileObjMoving = false;
        }

        public void OnHalfTimeTransition(object eventParamater)
        {
            m_mobileObjCurrentAngle = (float)Math.PI;
            m_mobileObjMoving = false;
            m_mobileObjSpeed = 0;

			m_mobileObjs[0].Position = new Vector2(mobileObjsPosX, 0);
			m_mobileObjs[1].Position = new Vector2(-mobileObjsPosX, 0);
        }

        public void OnSecondPeriod(object eventParamater)
        {
            if (m_rotateTimer != null)
                m_rotateTimer.Stop();

            m_rotateTimer = new Timer(Engine.GameTime.Source, m_timeFirstMS, TimerBehaviour.Restart);
            m_rotateTimer.OnTime += new TimerEvent(StartRotation);
            m_mobileObjs[0].Orientation = 0;
            m_mobileObjs[1].Orientation = 0;
            m_mobileObjCurrentAngle = 0;

            m_clockwise = 1;

            m_rotateTimer.Start();
        }

        public void OnMatchEnd(object eventParamater)
        {
            m_mobileObjMoving = false;
            m_rotateTimer.Stop(); 
        }

        public override void OnUpdate()
        {
            if (m_mobileObjMoving)
            {
                m_mobileObjSpeed += Engine.GameTime.ElapsedMS / m_mobileObjAccelTime * m_mobileObjMaxSpeed;
                m_mobileObjSpeed = LBE.MathHelper.Clamp(0, m_mobileObjMaxSpeed, m_mobileObjSpeed);
                m_mobileObjCurrentAngle += m_mobileObjSpeed * Engine.GameTime.ElapsedMS;
            }
            else if (m_mobileObjSpeed > 0)
            {
                m_mobileObjSpeed -= Engine.GameTime.ElapsedMS / m_mobileObjAccelTime * m_mobileObjMaxSpeed;
                m_mobileObjSpeed = LBE.MathHelper.Clamp(0, m_mobileObjMaxSpeed, m_mobileObjSpeed);
                m_mobileObjCurrentAngle += m_mobileObjSpeed * Engine.GameTime.ElapsedMS;
            }

            m_mobileObjs[0].Orientation = m_mobileObjCurrentAngle * m_clockwise;
            m_mobileObjs[1].Orientation = -m_mobileObjCurrentAngle * m_clockwise;
            
            if (m_mobileObjCurrentAngle > m_mobileObjTargetAngle)
            {
                m_mobileObjCurrentAngle = m_mobileObjTargetAngle;
                m_mobileObjMoving = false;
            }
        }

        public override void OnEnd()
        {
            OnMatchEnd(null);

			Engine.World.EventManager.RemoveListener((int)EventId.FirstPeriod, OnFirstPeriod);
			Engine.World.EventManager.RemoveListener((int)EventId.HalfTime, OnHalfTime);
			Engine.World.EventManager.RemoveListener((int)EventId.HalfTimeTransition, OnHalfTimeTransition);
			Engine.World.EventManager.RemoveListener((int)EventId.SecondPeriod, OnSecondPeriod);
			Engine.World.EventManager.RemoveListener((int)EventId.MatchEnd, OnMatchEnd);
        }
    }
}
