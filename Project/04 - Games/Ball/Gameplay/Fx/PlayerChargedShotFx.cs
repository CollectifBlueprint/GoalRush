using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Graphics.Sprites;
using LBE.Gameplay;
using Microsoft.Xna.Framework;
using LBE;
using LBE.Graphics.Particles;

namespace Ball.Gameplay
{
    public class PlayerChargedShotFx : GameObjectComponent
    {
        SpriteComponent m_spriteCmp;
        Timer m_fxTimer;

        Color m_color;
        public Color Color
        {
            get { return m_color; }
            set { m_color = value; }
        }

        ParticleComponent m_particleCmp;
        ParticleComponent m_particleCmpHL;

        public override void Start()
        {
            Sprite sprite = Sprite.Create("Graphics/ChargedShotFx.lua::Sprite");
            m_spriteCmp = new SpriteComponent(sprite, "ArenaOverlay5");
            Owner.Attach(m_spriteCmp);

            m_color = Color.White;
            m_spriteCmp.Sprite.Alpha = 0.0f;

            m_fxTimer = new Timer(Engine.GameTime.Source, 1500f);
            m_fxTimer.Start();

            var player = Owner.FindComponent<Player>();

            var emitterDef = Engine.AssetManager.Get<ParticleEmitterDefinition>("Graphics/Particles/ChargingPlayer.lua::Emitter");
            emitterDef.Color = player.PlayerColors[2];
            m_particleCmp = new ParticleComponent();
            m_particleCmp.Emitter = new ParticleEmitter(emitterDef);
            Owner.Attach(m_particleCmp);

            var emitterDefHL = Engine.AssetManager.Get<ParticleEmitterDefinition>("Graphics/Particles/ChargingPlayer.lua::EmitterHL");
            emitterDefHL.Color = player.PlayerColors[2];
            m_particleCmpHL = new ParticleComponent();
            m_particleCmpHL.Emitter = new ParticleEmitter(emitterDefHL);
            m_particleCmpHL.LockRotation = true;
            Owner.Attach(m_particleCmpHL);
        }

        public override void Update()
        {
            float relativeTime = 1;
            if (m_fxTimer.Active)
                relativeTime = m_fxTimer.TimeMS / m_fxTimer.TargetTime;

            float scaleAmount = 0.46f + 0.05f * relativeTime;
            m_spriteCmp.Sprite.Color = m_color;
            m_spriteCmp.Sprite.Scale = Vector2.One * scaleAmount;
            m_spriteCmp.Sprite.Alpha = relativeTime;
        }

        public void Stop()
        {
            m_particleCmp.Emitter.Active = false;
            m_particleCmp.DestroyOnEnd = true;
            m_particleCmpHL.Emitter.Active = false;
            m_particleCmpHL.DestroyOnEnd = true;

            Owner.Remove(this);
        }

        public override void End()
        {
            Owner.Remove(m_particleCmp);
            Owner.Remove(m_particleCmpHL);
            Owner.Remove(m_spriteCmp);
        }
    }
}
