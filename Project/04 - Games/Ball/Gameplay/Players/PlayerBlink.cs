using LBE;
using LBE.Gameplay;
using LBE.Graphics.Sprites;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Graphics.Particles.Modifiers;
using LBE.Graphics.Particles;

namespace Ball.Gameplay.Players
{
    public enum BlinkState
    {
        None,
        BlinkOut,
        Blink,
        BlinkIn,
    }

    public class PlayerBlink : GameObjectComponent
    {


        Player m_player;
        Timer m_blinkTimer;
        BlinkState m_state;

        GameObject m_particleInObject;
        ParticleComponent m_particleInCmp;

        GameObject m_particleOutObject;
        ParticleComponent m_particleOutCmp;

        SpriteComponent m_holeInCmp;
        SpriteComponent m_holeOutCmp;
        float m_holeScale = 1.2f;
        float m_holeAngularSpeed = 20.0f;

        Vector2 m_tpPosition;
        Vector2 m_targetPosition;

        public PlayerBlink(Player player)
        {
            m_player = player;
        }

        public override void Start()
        {
            

            m_blinkTimer = new Timer(Engine.GameTime.Source, 200);
            m_state = BlinkState.None;

            m_particleInObject = new GameObject("Player Teleport In FX");
            m_particleOutObject = new GameObject("Player Teleport Out FX");

            var holeColor = Color.Lerp(m_player.PlayerColors[2], Color.White, 0.4f);

            m_holeInCmp = new SpriteComponent(Sprite.CreateFromTexture("Graphics/ball48.png"), "ArenaOverlay3");
            m_holeInCmp.AttachedToOwner = false;
            m_holeInCmp.Visible = false;
            m_holeInCmp.Sprite.Color = holeColor;
            Owner.Attach(m_holeInCmp);

            m_holeOutCmp = new SpriteComponent(Sprite.CreateFromTexture("Graphics/ball48.png"), "ArenaOverlay3");
            m_holeOutCmp.AttachedToOwner = false;
            m_holeOutCmp.Visible = false;
            m_holeOutCmp.Sprite.Color = holeColor;
            Owner.Attach(m_holeOutCmp);
        }

        public void Do()
        {
            if (m_player.Properties.Blink)
                return;

            m_player.Properties.Blink.Set();
            m_player.Properties.MagnetDisabled.Set();
            m_player.Properties.ControlDisabled.Set();
            m_player.Properties.Invincible.Set();

            m_blinkTimer.Start();
            m_state = BlinkState.BlinkOut;
            m_tpPosition = m_player.Position;

            var partColor = Color.Lerp(m_player.PlayerColors[2], Color.White, 0.7f);
            var emitterDefAsset = Engine.AssetManager.GetAsset<ParticleEmitterDefinition>("Graphics/Particles/Teleport.lua::Emitter");
            emitterDefAsset.Content.Color = partColor;

            m_particleInCmp = new ParticleComponent();
            m_particleInCmp.Emitter = new ParticleEmitter(emitterDefAsset);
            m_particleInObject.Attach(m_particleInCmp);

            m_particleOutCmp = new ParticleComponent();
            m_particleOutCmp.Emitter = new ParticleEmitter(emitterDefAsset);
            m_particleOutObject.Attach(m_particleOutCmp);

            if (Game.Arena.LeftGoal.Team == m_player.Team)
                m_targetPosition = Game.Arena.LeftGoal.Position + Vector2.UnitX * 80;
            else
                m_targetPosition = Game.Arena.RightGoal.Position - Vector2.UnitX * 80;

            m_holeInCmp.Position = m_player.Position;
            m_holeOutCmp.Position = m_targetPosition;

            m_holeInCmp.Visible = true;
            m_holeInCmp.Scale = 0;
            m_holeOutCmp.Visible = true;
            m_holeOutCmp.Scale = 0;
        }

        public override void Update()
        {
            Engine.Log.Debug("State", m_state);
            if (m_state == BlinkState.None)
                return;

            var angle = m_holeAngularSpeed * 0.001f * Engine.GameTime.TimeMS;
            m_holeInCmp.Orientation = angle;
            m_holeOutCmp.Orientation = 1 - angle; //Rotate in ohter direction, and add offset

            m_particleInObject.Orientation = angle;
            m_particleOutObject.Orientation = 1 - angle; //Rotate in ohter direction, and add offset

            if (m_state == BlinkState.BlinkOut)
            {
                m_particleInObject.Position = m_player.Position;
                m_particleOutObject.Position = m_targetPosition;

                m_holeInCmp.Position = m_player.Position;
                m_holeOutCmp.Position = m_targetPosition;

                m_player.SpritePlayerCmp.Sprite.Alpha = 1 - m_blinkTimer.Completion;
                m_player.SpritePlayerCmp.Scale = 1 - m_blinkTimer.Completion;

                if (m_blinkTimer.Active)
                {
                    var holeSize = m_holeScale * LBE.MathHelper.Clamp(0, 1, 1.2f * m_blinkTimer.Completion);
                    m_holeInCmp.Scale = holeSize;
                    m_holeOutCmp.Scale = holeSize;
                }

                if (!m_blinkTimer.Active)
                {
                    m_player.SpritePlayerCmp.Sprite.Alpha = 0;
                    m_player.SpritePlayerCmp.Scale = 0;
                    m_state = BlinkState.Blink;
                }
            }
            else if (m_state == BlinkState.Blink)
            {
                m_tpPosition = m_targetPosition;
                m_state = BlinkState.BlinkIn;
                m_blinkTimer.Start();

                m_player.Owner.Position = m_targetPosition;
            }
            else if (m_state == BlinkState.BlinkIn)
            {
                m_player.SpritePlayerCmp.Sprite.Alpha = m_blinkTimer.Completion;
                m_player.SpritePlayerCmp.Scale = m_blinkTimer.Completion;

                m_particleOutObject.Position = m_player.Position;

                if (m_blinkTimer.Active)
                {
                    var holeSize = LBE.MathHelper.Clamp(0, 1, 1.2f - 1.2f * m_blinkTimer.Completion);
                    m_holeInCmp.Scale = m_holeScale * holeSize;
                    m_holeOutCmp.Scale = m_holeScale * holeSize;
                }

                if (!m_blinkTimer.Active)
                {
                    m_player.SpritePlayerCmp.Sprite.Alpha = 1;
                    m_player.SpritePlayerCmp.Scale = 1;
                    m_state = BlinkState.None;

                    m_player.Properties.Blink.Unset();
                    m_player.Properties.MagnetDisabled.Unset();
                    m_player.Properties.ControlDisabled.Unset();
                    m_player.Properties.Invincible.Unset();

                    m_particleInCmp.Emitter.Active = false;
                    m_particleInCmp.DestroyOnEnd = true;
                    m_particleOutCmp.Emitter.Active = false;
                    m_particleOutCmp.DestroyOnEnd = true;

                    m_holeInCmp.Visible = false;
                    m_holeOutCmp.Visible = false;
                }
            }
        }

        public override void End()
        {
        }
    }
}
