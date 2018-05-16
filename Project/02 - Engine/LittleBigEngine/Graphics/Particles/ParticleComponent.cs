using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Gameplay;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using LBE.Graphics.Sprites;
using LBE.Graphics.Particles.Modifiers;
using LBE.Graphics.Particles.Utils;
using LBE.Input;

namespace LBE.Graphics.Particles
{
    public class ParticleComponent : GameObjectComponent, IRenderable
    {
        ParticleEmitter m_emitter;
        public ParticleEmitter Emitter
        {
            get { return m_emitter; }
            set { m_emitter = value; }
        }

        bool m_destroyOnEnd = false;
        public bool DestroyOnEnd
        {
            get { return m_destroyOnEnd; }
            set { m_destroyOnEnd = value; }
        }

        bool m_lockRotation = false;
        public bool LockRotation
        {
            get { return m_lockRotation; }
            set { m_lockRotation = value; }
        }

        SpriteBatch m_spriteBatch;

        public override void Start()
        {
            m_spriteBatch = new SpriteBatch(Engine.Renderer.Device);
            Engine.Renderer.RenderLayers["ArenaOverlay7"].Renderables.Add(this);
        }

        float m_timeBuffer = 0;
        public override void Update()
        {
            if (m_emitter == null)
                return;

            float stepMS = 3;
            float maxStep = 10;
            m_timeBuffer += Engine.GameTime.ElapsedMS;
            while (maxStep > 0 && m_timeBuffer > stepMS)
            {
                m_emitter.Position = Position;
                if(!m_lockRotation) m_emitter.Orientation = Owner.Orientation;
                m_emitter.Simulate(stepMS);

                m_timeBuffer -= stepMS;
                maxStep--;
            }

            if (m_emitter.Definition.Duration != 0 && m_emitter.TimeMS.Time >= m_emitter.Definition.Duration)
                m_emitter.Active = false;

            if (m_emitter.ParticleCount == 0 && m_emitter.Active == false && m_destroyOnEnd)
                Owner.Remove(this);
        }

        public override void End()
        {
            Engine.Renderer.RenderLayers["ArenaOverlay7"].Renderables.Remove(this);
        }

        public void Draw()
        {
            if (m_emitter == null)
                return;

            BlendState blendState = BlendState.Additive;
            if (m_emitter.Definition.BlendMode == ParticleBlendMode.Blend)
                blendState = BlendState.NonPremultiplied;
            if (m_emitter.Definition.BlendMode == ParticleBlendMode.Opaque)
                blendState = BlendState.AlphaBlend;
            if (m_emitter.Definition.BlendMode == ParticleBlendMode.Dual)
                blendState = BlendState.NonPremultiplied;

            m_spriteBatch.Begin(SpriteSortMode.Immediate, blendState, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone);
            {
                Render(m_emitter);
            }
            m_spriteBatch.End();

            if (m_emitter.Definition.BlendMode == ParticleBlendMode.Dual)
            {
                blendState = BlendState.Additive;
                m_spriteBatch.Begin(SpriteSortMode.Immediate, blendState, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone);
                {
                    Render(m_emitter, 0.4f);
                }
                m_spriteBatch.End();
            }
        }

        private void Render(ParticleEmitter e, float alpha = 1)
        {
            Sprite sprite = Sprite.CreateFromTexture(e.Texture);
            for (int i = 0; i < e.Particles.Length; i++)
            {
                if (e.Particles[i].Alive)
                {
                    Vector2 position = e.Particles[i].Position;
                    float orientation = e.Particles[i].Orientation;
                    if (e.Definition.Coordinates == CoordinatesMode.Local)
                    {
                        position += Owner.Position;
                        //if (!m_lockRotation) orientation += Owner.Orientation;
                    }
                    sprite.Scale = e.Particles[i].Scale * e.Particles[i].ScaleModifier * alpha;
                    sprite.Color = e.Particles[i].ColorModifier;
                    sprite.Alpha = e.Particles[i].Opacity * e.Particles[i].OpacityModifier * alpha;
                    Engine.Renderer.Draw(sprite, position, orientation, m_spriteBatch);
                }
            }
        }
    }
}
