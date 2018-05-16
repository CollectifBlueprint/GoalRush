using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Gameplay;
using LBE;
using Microsoft.Xna.Framework;
using LBE.Assets;
using LBE.Core;
using Ball.Gameplay.BallEffects;
using LBE.Audio;

namespace Ball.Gameplay.Arenas.Objects
{
    public class AssistBallLauncherComponent : LauncherComponent
    {
        Timer m_beginLauncherTimerMS;

        Timer m_scanPlayerTimerMS;
        float m_scanRadius = 350f;

        Timer m_launchBallTimerMS;

        Team m_team;
        public Team Team
        {
            get { return m_team; }
            set { m_team = value; }
        }

        AudioComponent m_audioCmpBallLaunch;
        public AudioComponent AudioCmpBallLaunch
        {
            get { return m_audioCmpBallLaunch; }
        }

        Player m_playerToAim;
        public Player PlayerToAim
        {
            get { return m_playerToAim; }
        }

        public AssistBallLauncherComponent()
        {

        }

        public override void Start()
        {
            base.Start();
            
            m_beginLauncherTimerMS = new Timer(Engine.GameTime.Source, 300);
            m_beginLauncherTimerMS.OnTime += new TimerEvent(m_beginLauncherTimerMS_OnTime);

            m_audioCmpBallLaunch = new AudioComponent("Audio/Sounds.lua::BallShot");
            Owner.Attach(m_audioCmpBallLaunch);

            m_ballSpawnOffset = 20 * Vector2.UnitX;

        }

        public override void Update()
        {
            base.Update();

            Transform parent = new Transform(Owner.Position, Owner.Orientation);
            Transform world = parent.Compose(m_transform);

            Engine.Debug.Screen.ResetBrush();

            Engine.Debug.Screen.Brush.DrawSurface = false;
            if (m_team != null)
                Engine.Debug.Screen.Brush.LineColor = Team.ColorScheme.Color1;
            else
                Engine.Debug.Screen.Brush.LineColor = Color.White;

            Engine.Debug.Screen.AddCircle(world.Position, m_scanRadius);
        }

        public override void End()
        {
            base.End();
        }

        public override GameObjectComponent Clone()
        {
            var launcher = new AssistBallLauncherComponent();
            launcher.Transform = m_transform;
            return launcher;
        }

        public override void Select()
        {
            base.Select();
        }

        public override void Unselect()
        {
            base.Unselect();
        }


        public override void Launch()
        {
            if (!Enabled)
                return;

            m_beginLauncherTimerMS.Start();

        }


        public override void Scan()
        {
            List<Player> launcherTeam = new List<Player>();

            foreach (var player in Game.GameManager.Players)
            {
                if (player.Team == m_team)
                    launcherTeam.Add(player);
            }

            Transform parent = new Transform(Owner.Position, Owner.Orientation);
            Transform world = parent.Compose(m_transform);

            m_playerToAim = null;
            float shortestPlayerDist = float.MaxValue;
            foreach (var player in launcherTeam)
            {
                if (player.Team == m_team)
                {
                    float dist = Vector2.Distance(player.Position, world.Position);

                    //test si player ds les but
                    if (dist < shortestPlayerDist)
                    {
                        shortestPlayerDist = dist;
                        m_playerToAim = player;
                    }
                }
            }
        }


        void PlaceBall()
        {
            Ball ball = Game.GameManager.Ball;

            Transform world = GetWorldTransform();

            ball.BodyCmp.SetPosition(world.Position + m_ballSpawnOffset.Rotate(world.Orientation));

        }

        void PassBallToTeam()
        {
            var ball = Game.GameManager.Ball;

            ball.AddEffect(new PassAssistEffect(m_playerToAim, null));

            Vector2 impulseDirection = m_playerToAim.Position - GetWorldTransform().Position;
            impulseDirection.Normalize();

            float launchImpulse = 280;
            ball.BodyCmp.Body.ApplyLinearImpulse(impulseDirection * launchImpulse);

            m_audioCmpBallLaunch.Play();

            Engine.World.EventManager.ThrowEvent((int)EventId.LauncherShot, this);
        }

        void ShootBall()
        {
            var ball = Game.GameManager.Ball;
            
            Transform world = GetWorldTransform();

            Vector2 impulseDirection = m_direction.Rotate(world.Orientation);

            float impulseDirMod = Engine.Random.NextFloat(-0.05f, 0.05f);
            impulseDirection.Rotate(impulseDirMod);

            impulseDirection.Normalize();
            float impulseForce = 280;
            
            ball.BodyCmp.SetPosition(world.Position + m_ballSpawnOffset.Rotate(world.Orientation));
            ball.BodyCmp.Body.ApplyLinearImpulse(impulseDirection * impulseForce);

            m_audioCmpBallLaunch.Play();

            Engine.World.EventManager.ThrowEvent((int)EventId.LauncherShot, this);

        }

        void m_beginLauncherTimerMS_OnTime(Timer source)
        {
            if (!Enabled)
                return;

            PlaceBall();

            Game.GameManager.Ball.BodyCmp.Body.Enabled = true;
            Game.GameManager.Ball.BallSprite.Alpha = 255;

            if (m_playerToAim != null
               && Vector2.Distance(m_playerToAim.Position, GetWorldTransform().Position) < m_scanRadius)
                PassBallToTeam();
            else
                ShootBall();
        }
    }

}
