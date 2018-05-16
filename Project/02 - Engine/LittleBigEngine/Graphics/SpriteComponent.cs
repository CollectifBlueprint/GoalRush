using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Assets;
using Microsoft.Xna.Framework;
using LBE.Gameplay;
using LBE.Core;

namespace LBE.Graphics.Sprites
{
    public class SpriteComponentParameters
    {
        public String RenderLayer = "Default";
        public SpriteDefinition Sprite;
        public Transform Transform = new Transform();
    }

    public class SpriteComponent : GameObjectComponent, IRenderable
    {
        Asset<SpriteComponentParameters> m_params;
        public Asset<SpriteComponentParameters> ParametersAsset
        {
            get { return m_params; }
            set { SetParameters(value); }
        }

        public SpriteComponentParameters Parameters
        {
            get { return m_params.Content; }
            set { SetParameters(value); }
        }

        Sprite m_sprite;
        public Sprite Sprite
        {
            get { return m_sprite; }
            set { m_sprite = value; }
        }

        Vector2 m_position;
        public Vector2 Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        float m_orientation;
        public float Orientation
        {
            get { return m_orientation; }
            set { m_orientation = value; }
        }

        float m_scale = 1;
        public float Scale
        {
            get { return m_scale; }
            set { m_scale = value; }
        }

        bool m_attachedToOwner;
        public bool AttachedToOwner
        {
            get { return m_attachedToOwner; }
            set { m_attachedToOwner = value; }
        }

        bool m_visible;
        public bool Visible
        {
            get { return m_visible; }
            set { m_visible = value; }
        }

        RenderLayer m_layer;

        public SpriteComponent()
            : this((Sprite)null, "Default")
        {
        }

        public SpriteComponent(String renderLayer)
            : this((Sprite)null, renderLayer)
        {
        }

        public SpriteComponent(Sprite sprite)
            : this(sprite, "Default")
        {
        }

        public SpriteComponent(Sprite sprite, String renderLayer)
        {
            m_sprite = sprite; 
            m_attachedToOwner = true;
            m_visible = true;

            if (Engine.Renderer.RenderLayers.ContainsKey(renderLayer))
                m_layer = Engine.Renderer.RenderLayers[renderLayer];
        }

        public SpriteComponent(SpriteComponentParameters parameters)
            : this()
        {
            SetParameters(parameters);
        }

        public SpriteComponent(Asset<SpriteComponentParameters> parameters)
            : this()
        {
            SetParameters(parameters);
        }

        private void SetParameters(SpriteComponentParameters value)
        {
            m_params = new Asset<SpriteComponentParameters>(value);
            Reset();
        }

        private void SetParameters(Asset<SpriteComponentParameters> value)
        {
            m_params = value;
            m_params.OnAssetChanged += new OnChange(Reset);
            Reset();
        }

        void Reset()
        {
            if (m_params == null)
                return;

            m_sprite = Sprite.Create(Parameters.Sprite);
            m_position = Parameters.Transform.Position;
            m_orientation = Parameters.Transform.Orientation;
            m_attachedToOwner = !Parameters.Transform.Absolute;        

            RenderLayer newLayer = null;
            if (Engine.Renderer.RenderLayers.ContainsKey(Parameters.RenderLayer))
                newLayer = Engine.Renderer.RenderLayers[Parameters.RenderLayer];

            if (Started)
            {
                if (newLayer != m_layer)
                {
                    if (m_layer != null) m_layer.Renderables.Remove(this);
                    m_layer = newLayer;
                    m_layer.Renderables.Add(this);
                }
            }
            else
            {
                m_layer = newLayer;
            }
        }

        public override void Start()
        {
            if (m_layer != null)
                m_layer.Renderables.Add(this);
        }

        public override void Update()
        {
            if (m_sprite != null)
                m_sprite.Update(Engine.GameTime.ElapsedMS);
        }

        public void Draw()
        {
            if (m_sprite != null && m_visible && Engine.Debug.Flags.RenderSprites)
            {
                Transform local = new Transform(m_position, m_orientation + m_sprite.Orientation, !m_attachedToOwner);
                Transform parent = new Transform(Owner.Position, Owner.Orientation);
                Transform world = parent.Compose(local);

                var baseScale = m_sprite.Scale;
                m_sprite.Scale *= m_scale;
                DoDraw(m_sprite, world);
                m_sprite.Scale = baseScale;
            }
        }

        protected virtual void DoDraw(Sprite sprite, Transform transform)
        {
            Engine.Renderer.Draw(m_sprite, transform.Position, transform.Orientation);
        }

        public override void End()
        {
            if (m_layer != null)
                m_layer.Renderables.Remove(this);
        }

        public override GameObjectComponent Clone()
        {
            return new SpriteComponent(m_params);
        }
    }
}
