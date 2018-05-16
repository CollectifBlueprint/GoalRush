using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Graphics.Sprites;
using LBE;
using LBE.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ball.Graphics
{
    public class ColorMaskedSprite : SpriteComponent
    {
        DynamicMesh<VertexPositionTexture> m_quadMesh;
        ColorMaskingEffect m_effect;

        Texture2D m_mask;
        public Texture2D Mask
        {
            get { return m_mask; }
            set { m_mask = value; }
        }

        Color m_color1;
        public Color Color1
        {
            get { return m_color1; }
            set { m_color1 = value; }
        }

        Color m_color2;
        public Color Color2
        {
            get { return m_color2; }
            set { m_color2 = value; }
        }

        Color m_color3;
        public Color Color3
        {
            get { return m_color3; }
            set { m_color3 = value; }
        }

        Color m_color4;
        public Color Color4
        {
            get { return m_color4; }
            set { m_color4 = value; }
        }

        public ColorMaskedSprite(Sprite sprite, String renderLayer)
            : base(sprite, renderLayer)
        {
        }

        public override void Start()
        {
            //Initialise SpriteComponent
            base.Start();

            m_effect = ColorMaskingEffect.Create("Graphics/Shaders/ColorMasking.fx");

            m_quadMesh = new DynamicMesh<VertexPositionTexture>(Engine.Renderer.Device, VertexPositionTexture.VertexDeclaration, PrimitiveType.TriangleStrip, 12);
            m_quadMesh.Vertex(new VertexPositionTexture(new Vector3(-1, +1, 0), new Vector2(0, 0)));
            m_quadMesh.Vertex(new VertexPositionTexture(new Vector3(+1, +1, 0), new Vector2(1, 0)));
            m_quadMesh.Vertex(new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)));
            m_quadMesh.Vertex(new VertexPositionTexture(new Vector3(+1, -1, 0), new Vector2(1, 1)));

            m_quadMesh.Index(0);
            m_quadMesh.Index(1);
            m_quadMesh.Index(2);
            m_quadMesh.Index(3);

            m_quadMesh.PrepareDraw();
        }

        protected override void DoDraw(Sprite sprite, LBE.Core.Transform transform)
        {
            Matrix world = Matrix.Identity;
            world *= Matrix.CreateScale(sprite.Size.X * 0.5f * sprite.Scale.X, sprite.Size.Y * 0.5f * Sprite.Scale.Y, 1);
            world *= Matrix.CreateRotationZ(transform.Orientation);
            world *= Matrix.CreateTranslation(new Vector3(transform.Position, 0));

            m_effect.World = world;
            m_effect.Texture = sprite.Texture;
            m_effect.Mask = m_mask;
            m_effect.Color1 = m_color1;
            m_effect.Color2 = m_color2;
            m_effect.Color3 = m_color3;
            m_effect.Color4 = m_color4;
            m_effect.Alpha = sprite.Alpha;

            Engine.Renderer.DrawMesh(m_quadMesh, m_effect);
        } 
    }
}
