using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE;
using Microsoft.Xna.Framework;
using Ball.Gameplay.Players;

namespace Ball.Gameplay
{
    public class StunParamters
    {
        public float Duration = 1.0f;
        public float SpeedModifier;
        public float BallImpulse = 1.0f;
    }

    public class PlayerStunEffect : PlayerEffect
    {
        StunParamters m_parameters;
        public StunParamters Parameters
        {
            get { return m_parameters; }
            set { m_parameters = value; }
        }

        public override void Start()
        {
            if (!Player.Properties.Stunned)
            {
                BeginStun();
            }

            //Audio
            Player.AudioCmpStun.Play();

            //Set properties
            Player.Properties.ControlDisabled.Set();
            Player.Properties.MagnetDisabled.Set();
            Player.Properties.Stunned.Set();

            ScreenShake.Add(2.0f, Vector2.Zero, 60);
        }

        public override void Update()
        {
            Player.SparksCmp.Orientation += 0.0005f * Engine.GameTime.ElapsedMS;
        }

        public override void End()
        {
            //Unset properties
            if (Player.Properties.ControlDisabled.Value == true)
                Player.Properties.ControlDisabled.Unset();

            if (Player.Properties.MagnetDisabled.Value == true)
                Player.Properties.MagnetDisabled.Unset();

            if (Player.Properties.Stunned.Value == true)
                Player.Properties.Stunned.Unset();

            if (!Player.Properties.Stunned)
            {
                EndStun();

                PlayerInvincibleEffect invincibleEffect = new PlayerInvincibleEffect();
                InvincibleParameters invincibleParemeters = new InvincibleParameters();
                invincibleEffect.SetDuration(invincibleParemeters.InvincibleTimeMS);
                Player.AddEffect(invincibleEffect);
            }
        }

        private void BeginStun()
        {
            //Effect on ball
            DetachBall();

            //GFX
            Player.SparksCmp.Visible = true;
            Player.SpritePlayerCmp.Color1 = Player.SpritePlayerCmp.Color1 * 0.5f;
            Player.SpritePlayerCmp.Color2 = Player.SpritePlayerCmp.Color2 * 0.5f;
            Player.SpritePlayerCmp.Color3 = Player.SpritePlayerCmp.Color3 * 0.5f;

        }

        private void EndStun()
        {
            Player.SparksCmp.Visible = false;
            Player.ResetColors();
        }

        void DetachBall()
        {
            if (Player.Ball == null)
                return;

            Player.DettachBall(m_parameters.BallImpulse);

            Engine.World.EventManager.ThrowEvent((int)EventId.PlayerLoseBall, Player);
        }
    }
}
