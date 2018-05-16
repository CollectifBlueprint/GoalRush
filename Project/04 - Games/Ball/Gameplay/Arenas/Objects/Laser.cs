using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Graphics.Sprites;
using LBE.Gameplay;
using LBE;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using LBE.Physics;
using FarseerPhysics.Factories;
using Ball.Physics;
using LBE.Assets;
using Ball.Gameplay.BallEffects;

namespace Ball.Gameplay.Arenas.Objects
{
    public enum LaserState
    {
        Off,
        In,
        On,
        Out,
    }

    public class LaserParameters
    {
        public float PlayerBashImpulse;
        public float BallBashImpulse;
        public float IgnorePlayerTimerMS;
    }

    public class Laser : GameObjectComponent
    {
        SpriteComponent m_laserCmp;
        SpriteComponent m_botSparkCmp;
        SpriteComponent m_topSparkCmp;

        float m_ySpark = -83;
        float m_ySparkTop = 85;
        float m_width = 8;
        float m_height = 200;
        float m_scale = 1;

        LaserState m_state;

        Timer m_laserInTimer;
        Timer m_laserOutTimer;
        Timer m_laserOnTimer;

        Vector2 m_laserPosition;
        public Vector2 LaserPosition
        {
            get { return m_laserPosition; }
            set { m_laserPosition = value; }
        }

        float m_sparkFadeTimeMS = 250;

        Fixture m_laserFixture;

        Asset<LaserParameters> m_laserParameters;
        public LaserParameters LaserParameters
        {
            get { return m_laserParameters.Content; }
        }

        public Laser(float scale = 1)
        {
            m_ySpark *= scale;
            m_ySparkTop *= scale;
            m_height *= scale;
            m_scale = scale;
        }

        public override void Start()
        {
            m_laserParameters = Engine.AssetManager.GetAsset<LaserParameters>("Arenas/Common/Laser.lua::Laser");
            
            InitSprites();

            m_state = LaserState.Off;

            m_laserInTimer = new Timer(Engine.GameTime.Source, 1600);
            m_laserOnTimer = new Timer(Engine.GameTime.Source, 5000);
            m_laserOutTimer = new Timer(Engine.GameTime.Source, m_sparkFadeTimeMS);

            m_laserCmp.Visible = false;
            m_botSparkCmp.Visible = false;
            m_topSparkCmp.Visible = false;

            var bodyCmp = new RigidBodyComponent();
            bodyCmp.Body.CollisionCategories = (Category)CollisionType.Wall;

            m_laserFixture = FixtureFactory.AttachRectangle(
                ConvertUnits.ToSimUnits(m_width),
                ConvertUnits.ToSimUnits(m_height),
                0,
                ConvertUnits.ToSimUnits(m_laserPosition),
                bodyCmp.Body);

            Owner.Attach(bodyCmp);
            Owner.RigidBodyCmp.OnCollision += new CollisionEvent(RigidBodyCmp_OnCollision);
            Owner.RigidBodyCmp.Body.BodyType = BodyType.Static;
        }

        bool RigidBodyCmp_OnCollision(FarseerPhysics.Dynamics.Contacts.Contact contact, RigidBodyComponent self, RigidBodyComponent other)
        {
            Player player = other.Owner.FindComponent<Player>();
            if (contact.FixtureA == m_laserFixture && player != null && m_state == LaserState.On)
            {
                bool playerHasBall = player.Ball != null;

                float side = Math.Sign(player.Position.X - m_laserPosition.X);

                Vector2 bashDirection = -player.MoveDirection;

                if (playerHasBall)
                {
                    player.DettachBall(-bashDirection * m_laserParameters.Content.BallBashImpulse);
                }

                if (player.Properties.Bashed.Value == false)
                {
                    PlayerBashEffect bashEffect = new PlayerBashEffect();
                    bashEffect.SetDuration(80);
                    bashEffect.Direction = bashDirection;
                    bashEffect.Strength = m_laserParameters.Content.PlayerBashImpulse;
                    player.AddEffect(bashEffect);
                }     
            }

            return true;
        }

        public override void Update()
        {
            m_laserCmp.Position = m_laserPosition;
            m_botSparkCmp.Position = m_laserPosition + new Vector2(0, m_ySpark);
            m_topSparkCmp.Position = m_laserPosition + new Vector2(0, m_ySparkTop);

            if (m_state == LaserState.In)
            {
                m_laserCmp.Visible = false;
                m_botSparkCmp.Visible = true;
                m_topSparkCmp.Visible = true;

                m_botSparkCmp.Sprite.Alpha = (float)Math.Min(m_laserInTimer.TimeMS / m_sparkFadeTimeMS, 1);
                m_topSparkCmp.Sprite.Alpha = (float)Math.Min(m_laserInTimer.TimeMS / m_sparkFadeTimeMS, 1);

                if (!m_laserInTimer.Active)
                {
                    m_state = LaserState.On;
                    m_laserOnTimer.Start();
                    m_botSparkCmp.Sprite.SetAnimation("Small");
                    m_topSparkCmp.Sprite.SetAnimation("Small");
                }
            }

            if (m_state == LaserState.On)
            {
                m_laserCmp.Visible = true;
                m_botSparkCmp.Visible = true;
                m_topSparkCmp.Visible = true;

                m_botSparkCmp.Sprite.Alpha = 1;
                m_topSparkCmp.Sprite.Alpha = 1;


                if (!m_laserOnTimer.Active)
                {
                    m_state = LaserState.Out;
                    m_laserOutTimer.Start();
                    m_botSparkCmp.Sprite.SetAnimation("Normal");
                    m_topSparkCmp.Sprite.SetAnimation("Normal");
                }
            }

            if (m_state == LaserState.Out)
            {
                m_laserCmp.Visible = false;
                m_botSparkCmp.Visible = true;
                m_topSparkCmp.Visible = true;

                m_laserCmp.Sprite.Alpha = (float)Math.Max(1 - m_laserOutTimer.TimeMS / m_sparkFadeTimeMS, 0);
                m_botSparkCmp.Sprite.Alpha = (float)Math.Max(1 - m_laserOutTimer.TimeMS / m_sparkFadeTimeMS, 0);
                m_topSparkCmp.Sprite.Alpha = (float)Math.Max(1 - m_laserOutTimer.TimeMS / m_sparkFadeTimeMS, 0);

                if (!m_laserOutTimer.Active)
                {
                    m_state = LaserState.Off;
                }
            }

            if (m_state == LaserState.Off)
            {
                m_laserCmp.Visible = false;
                m_botSparkCmp.Visible = false;
                m_topSparkCmp.Visible = false;
            }

            if (m_state == LaserState.On)
                Owner.RigidBodyCmp.Body.Enabled = true;
            else
                Owner.RigidBodyCmp.Body.Enabled = false;

        }

        public void StartLaser()
        {
            m_state = LaserState.In;
            m_laserInTimer.Start();
            m_botSparkCmp.Sprite.SetAnimation("Normal");
            m_topSparkCmp.Sprite.SetAnimation("Normal");
        }

        private void InitSprites()
        {
            Sprite laserSprite = Sprite.CreateFromTexture("Graphics/Laser.png");
            m_laserCmp = new SpriteComponent(laserSprite, "ArenaOverlay3");
            m_laserCmp.Sprite.Scale = new Vector2(1, m_scale);

            Sprite sparksSprite = Sprite.Create("Graphics/sparks.lua::Sprite");
            m_botSparkCmp = new SpriteComponent(sparksSprite, "ArenaOverlay3");
            Sprite sparksSpriteTop = Sprite.Create("Graphics/sparks.lua::Sprite");
            m_topSparkCmp = new SpriteComponent(sparksSpriteTop, "ArenaOverlay3");
            m_topSparkCmp.Orientation = (float)Math.PI;

            sparksSprite.Playing = true;
            sparksSpriteTop.Playing = true;

            Owner.Attach(m_laserCmp);
            Owner.Attach(m_botSparkCmp);
            Owner.Attach(m_topSparkCmp);
        }

        public override void End()
        {
            base.End();
        }
    }
}
