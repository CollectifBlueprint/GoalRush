using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Graphics;
using Microsoft.Xna.Framework.Graphics;
using LBE;
using LBE.Gameplay;
using Microsoft.Xna.Framework;
using LBE.Assets;
using LBE.Graphics.Effects;
using LBE.Graphics.Sprites;
using System.IO;
using LBE.Graphics.Camera;

namespace Ball.Stats
{
    public class HeatMapFx : IRenderable
    {
        float m_radius;
        public float Radius
        {
            get { return m_radius; }
            set { m_radius = value; }
        }

        Asset<Effect> m_drawEffect;
        Asset<Effect> m_drawSpotEffect;
        Asset<Texture2D> m_heatMapGradient;

        DynamicMesh<VertexPositionTexture> m_quadMesh;
        RenderTarget2D m_densityTexture;

        Sprite m_densitySprite;

        bool m_ready;

        public HeatMapFx()
        {
            Init();

            m_ready = false;
        }

        void Init()
        {
            m_densityTexture = new RenderTarget2D(Engine.Renderer.Device, 1280, 720, false, SurfaceFormat.Single, DepthFormat.None);
            m_densitySprite = Sprite.CreateFromTexture(m_densityTexture);

            m_drawEffect = Engine.AssetManager.GetAsset<Effect>("heatmap.fx");
            m_drawSpotEffect = Engine.AssetManager.GetAsset<Effect>("heatmapSpot.fx");

            m_heatMapGradient = Engine.AssetManager.GetAsset<Texture2D>("System/heatMapGradient.jpg");

            m_quadMesh = new DynamicMesh<VertexPositionTexture>(Engine.Renderer.Device, VertexPositionTexture.VertexDeclaration, PrimitiveType.TriangleStrip, 12);
            m_quadMesh.Vertex(new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 0)));
            m_quadMesh.Vertex(new VertexPositionTexture(new Vector3(+1, -1, 0), new Vector2(1, 0)));
            m_quadMesh.Vertex(new VertexPositionTexture(new Vector3(-1, +1, 0), new Vector2(0, 1)));
            m_quadMesh.Vertex(new VertexPositionTexture(new Vector3(+1, +1, 0), new Vector2(1, 1)));

            m_quadMesh.Index(0);
            m_quadMesh.Index(1);
            m_quadMesh.Index(2);
            m_quadMesh.Index(3);

            m_quadMesh.PrepareDraw();

            DrawToRT();
        }

        public void Start()
        {
            m_ready = false;

            Engine.Renderer.Device.SetRenderTarget(m_densityTexture);
            Engine.Renderer.Device.Clear(new Color(0, 0, 0, 0));
        }

        public void End()
        {
            Engine.Renderer.Device.SetRenderTarget(null);
            m_ready = true;
        }

        void DrawToRT()
        {
            for (int i = 0; i < 250; i++)
            {
                float radius = 160;
                float randRadius = Engine.Random.NextFloat(0, 1.0f);
                Vector2 pos = (float)Math.Sqrt(randRadius) * radius * Engine.Random.NextVector2();
                AddPoint(pos, 30);
            }
        }

        public void AddPoint(Vector2 pos, float radius)
        {
            Effect effect = m_drawSpotEffect.Content;
            Matrix world = Matrix.CreateScale(radius);
            world *= Matrix.CreateTranslation(new Vector3(pos.X / 2, -pos.Y / 2, 0));

            effect.Parameters["World"].SetValue(world);

            Engine.Renderer.Device.BlendState = BlendState.Additive;

            Engine.Renderer.Device.RasterizerState = RasterizerState.CullNone;
            Engine.Renderer.DrawMesh(m_quadMesh, new EffectWrapper(effect));
        }

        public void Draw()
        {
            if (!m_ready)
                return;

            Effect effect = m_drawEffect.Content;
            Matrix scale = Matrix.CreateScale(new Vector3(m_densityTexture.Width, m_densityTexture.Height, 0));

            try
            {
                effect.Parameters["World"].SetValue(scale);
                effect.Parameters["Heat"].SetValue(m_densityTexture);
                effect.Parameters["Gradient"].SetValue(m_heatMapGradient.Content);
            }
            catch { }

            Engine.Renderer.Device.DepthStencilState = DepthStencilState.None;
            Engine.Renderer.Device.BlendState = BlendState.AlphaBlend;
            Engine.Renderer.Device.RasterizerState = RasterizerState.CullNone;
            Engine.Renderer.DrawMesh(m_quadMesh, new EffectWrapper(effect));
        }
    }
}
