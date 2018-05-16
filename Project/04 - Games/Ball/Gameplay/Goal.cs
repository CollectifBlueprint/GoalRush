 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Gameplay;
using LBE.Physics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using LBE;
using FarseerPhysics.Dynamics;
using Ball.Physics;
using LBE.Graphics.Sprites;
using LBE.Audio;

namespace Ball.Gameplay
{
    public class Goal : GameObjectComponent
    {
        TriggerComponent m_goalTrigger;

        Vector2 m_size;
        public Vector2 Size
        {
            get { return m_size; }
            set { m_size = value; }
        }

        int m_index;
        public int Index
        {
            get { return m_index; }
            set { m_index = value; }
        }

        int m_goalsCount;
        public int Score
        {
            set { m_goalsCount = value; }
            get { return m_goalsCount; }
        }

        int m_goalsCountDisplay;
        public int ScoreDisplay
        {
            set { m_goalsCountDisplay = value; }
            get { return m_goalsCountDisplay; }
        }

        Team m_team;
        public Team Team
        {
            set { m_team = value; }
            get { return m_team; }
        }

        Timer m_goalDisableTimer;

        bool m_ballIn;

        Timer m_goalTimer;

        SpriteComponent m_goalSprite;

        AudioComponent m_AudioCmpGoal;

        public Goal(Team team)
        {
            m_team = team;
        }

        public override void Start()
        {
            m_goalsCount = 0;
            m_goalsCountDisplay = 0;

            m_goalTrigger = new TriggerComponent();
            var triggerFixture = FixtureFactory.AttachRectangle(
                m_size.X, m_size.Y,
                0,
                Vector2.Zero,
                m_goalTrigger.Body);
            triggerFixture.IsSensor = true;
            m_goalTrigger.Body.CollidesWith = (Category)CollisionType.Ball;

            m_goalTrigger.OnTrigger += new Action<GameObject>(m_goalTrigger_OnTrigger);
            m_goalTrigger.OnLeave += new Action<GameObject>(m_goalTrigger_OnLeave);

            Owner.Attach(m_goalTrigger);

            RigidBodyComponent rbCmp = new RigidBodyComponent();
            FixtureFactory.AttachRectangle(
                 m_size.X, m_size.Y,
                 0,
                 Vector2.Zero,
                 rbCmp.Body);
            rbCmp.Body.BodyType = BodyType.Static;
            rbCmp.Body.CollidesWith = (Category)CollisionType.None;
            Owner.Attach(rbCmp);

            rbCmp.OnCollision += new CollisionEvent(RigidBodyCmp_OnCollision);

            Sprite sprite = Sprite.Create("Graphics/goalFx.lua::Sprite");
            sprite.Scale = new Vector2(0.7f, 0.55f);
            //sprite.Playing = true;
           
            m_goalSprite = new SpriteComponent(sprite, "ArenaOverlay8");
            m_goalSprite.Visible = false;
            Owner.Attach(m_goalSprite);
          


            m_AudioCmpGoal = new AudioComponent("Audio/Sounds.lua::Goal");
            Owner.Attach(m_AudioCmpGoal);

            float goalDisableTimeMS = 0.1f * 1000;
            m_goalDisableTimer = new Timer(Engine.GameTime.Source, goalDisableTimeMS, TimerBehaviour.Stop);

            Engine.World.EventManager.AddListener((int)EventId.MatchEnd, OnMatchEnd);

            m_goalTimer = new Timer(Engine.GameTime.Source, 1000, TimerBehaviour.Stop);
        }

        void m_goalTrigger_OnTrigger(GameObject obj)
        {
            Ball ball = obj.FindComponent<Ball>();
            Player player = obj.FindComponent<Player>();
            if (ball != null || (player != null && player.Ball != null))
            {
                BallEnter(ball);
            }
        }

        void m_goalTrigger_OnLeave(GameObject obj)
        {
            Ball ball = obj.FindComponent<Ball>();
            Player player = obj.FindComponent<Player>();
            if (ball != null || (player != null && player.Ball != null))
            {
                BallExit(ball);
            }
        }


        bool m_goalTimerWasActive = false;
        //
        public override void Update()
        {
            if (m_goalTrigger.Active)
            {
                Engine.Debug.Screen.AddCross(Owner.Position, 10);
            }

            //m_goalTextSpriteCmp.Visible = m_goalTimer.Active;
            if (m_goalTimerWasActive && !m_goalTimer.Active)
            {
                Engine.World.EventManager.ThrowEvent((int)EventId.KickOff, m_team);
            }


            m_goalTimerWasActive = m_goalTimer.Active;

        }

        public void BallEnter(Ball ball)
        {
            if (m_goalTimer.Active)
                return;

            m_goalSprite.Sprite.SetAnimation("Normal");
            m_goalSprite.Visible = true;
            m_goalSprite.Sprite.Playing = true;

            m_goalsCount++;

            if (ball != null)
                ball.SetSlowMode(true);

            Owner.RigidBodyCmp.Body.CollidesWith = (Category)CollisionType.None;

            m_ballIn = true;

			Engine.World.EventManager.ThrowEvent((int)EventId.Goal, m_team);
            m_AudioCmpGoal.Play();
            m_goalTimer.Start();
        }

        public void BallExit(Ball ball)
        {
            m_ballIn = false;

            m_goalTrigger.ActiveObjects.Clear();

            m_goalDisableTimer.Reset();
            m_goalDisableTimer.Start();
        }

        bool RigidBodyCmp_OnCollision(FarseerPhysics.Dynamics.Contacts.Contact contact, RigidBodyComponent self, RigidBodyComponent other)
        {
            return true;
        }

        //
        public void OnMatchEnd(object eventParamater)
        {
            m_goalDisableTimer = new Timer(Engine.GameTime.Source, float.PositiveInfinity);
        }
    }
}
