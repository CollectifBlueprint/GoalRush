using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Gameplay;
using LBE;
using Microsoft.Xna.Framework;
using LBE.Assets;
using LBE.Core;
using LBE.Graphics.Sprites;
using LBE.Audio;

namespace Ball.Gameplay.Arenas.Objects
{
    public class SimpleBallLauncherComponent : LauncherComponent
    {

        AudioComponent m_audioCmpBallLaunch;
        public AudioComponent AudioCmpBallLaunch
        {
            get { return m_audioCmpBallLaunch; }
        }
        
        public SimpleBallLauncherComponent()
        {
        }

        public override void Start()
        {
            base.Start();

            m_audioCmpBallLaunch = new AudioComponent("Audio/Sounds.lua::BallShot");
            Owner.Attach(m_audioCmpBallLaunch);

            m_ballSpawnOffset = 20 * Vector2.UnitX;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void End()
        {
            base.End();
        }

        public override GameObjectComponent Clone()
        {
            var launcher = new SimpleBallLauncherComponent();
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

            Game.GameManager.Ball.BodyCmp.Body.Enabled = true;
            Game.GameManager.Ball.BallSprite.Alpha = 255;

            Transform parent = new Transform(Owner.Position, Owner.Orientation);
            Transform world = parent.Compose(m_transform); 

            float launchImpulse = 200;
            Ball ball = Game.GameManager.Ball;
            ball.BodyCmp.SetPosition(world.Position + m_ballSpawnOffset.Rotate(world.Orientation));
            ball.BodyCmp.Body.ApplyLinearImpulse(m_direction.Rotate(world.Orientation) * launchImpulse);

            m_audioCmpBallLaunch.Play();

            Engine.World.EventManager.ThrowEvent((int)EventId.LauncherShot, this);
        }

        public override void Scan()
        {
        }
    }
}
