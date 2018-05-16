using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE;
using LBE.Gameplay;
using LBE.Graphics;
using Microsoft.Xna.Framework;
using LBE.Graphics.Effects;
using Microsoft.Xna.Framework.Graphics;

namespace Ball.Gameplay
{
    public class BallTrailFx : GameObjectComponent , IRenderable
    {
        Ball m_ball;

        List<Vector2> m_pastPositions;

        DynamicMesh<VertexPositionColor> m_mesh;
        EffectWrapper m_effect;

        bool m_active = false;
        public bool Active
        {
            get { return m_active; }
        }

        Timer m_fadeOutTimer;

        Color m_color;

        public BallTrailFx(Ball ball)
        {
            m_ball = ball;
            m_pastPositions = new List<Vector2>();

            m_color = Color.White;
        }

        public override void Start()
        {
            var basicEffect = new BasicEffect(Engine.Renderer.Device);
            basicEffect.LightingEnabled = false;
            basicEffect.VertexColorEnabled = true;
            basicEffect.FogEnabled = false;

            m_fadeOutTimer = new Timer(Engine.GameTime.Source, 300, TimerBehaviour.Stop);
            m_fadeOutTimer.OnTime += new TimerEvent(m_fadeOutTimer_OnTime);

            m_effect = new EffectWrapper(basicEffect);

            m_mesh = new DynamicMesh<VertexPositionColor>(Engine.Renderer.Device, VertexPositionColor.VertexDeclaration, Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 2048);

            Engine.Renderer.RenderLayers["ArenaOverlay7"].Renderables.Add(this);
        }

        public void Activate(Color color)
        {
            m_active = true;
            m_color = color;
            m_pastPositions.Clear();
        }

        public void Desactivate(bool fadeOut)
        {
            if (fadeOut)
            {
                m_fadeOutTimer.Start();
            }
            else
            {
                m_active = false;
            }
        }

        void m_fadeOutTimer_OnTime(Timer source)
        {
            m_active = false;
            m_fadeOutTimer.TimeMS = 0;
            m_pastPositions.Clear();
        }

        public override void Update()
        {
            if (!m_active)
                return;

            m_pastPositions.Add(Position);

            m_mesh.Reset();

            float width = 3;
            int nbrPoint = 24;
            int iStart = m_pastPositions.Count > nbrPoint ? m_pastPositions.Count - nbrPoint : 0;
            for (int i = iStart; i < m_pastPositions.Count-1; i++)
            {
                float alphaCoef = (m_fadeOutTimer.TargetTime - m_fadeOutTimer.TimeMS) / m_fadeOutTimer.TargetTime;
                float alpha = 1.0f / (float)nbrPoint * (i - iStart) * alphaCoef;
                Color colCenter = new Color(m_color, alpha);
                Color colSide = new Color(m_color, alpha * 0.25f);

                Vector2 dir = m_pastPositions[i+1] - m_pastPositions[i]; dir.Normalize();
                Vector2 orthoDir = dir.Rotate((float)Math.PI * 0.5f); orthoDir.Normalize();
                int i1 = m_mesh.Vertex(new VertexPositionColor(new Vector3(m_pastPositions[i] + orthoDir * width * 2, 0), colSide));
                int i2 = m_mesh.Vertex(new VertexPositionColor(new Vector3(m_pastPositions[i] + orthoDir * width, 0), colCenter));
                int i3 = m_mesh.Vertex(new VertexPositionColor(new Vector3(m_pastPositions[i] - orthoDir * width, 0), colCenter));
                int i4 = m_mesh.Vertex(new VertexPositionColor(new Vector3(m_pastPositions[i] - orthoDir * width * 2, 0), colSide));

                int i5 = m_mesh.Vertex(new VertexPositionColor(new Vector3(m_pastPositions[i + 1] + orthoDir * width * 2, 0), colSide));
                int i6 = m_mesh.Vertex(new VertexPositionColor(new Vector3(m_pastPositions[i + 1] + orthoDir * width, 0), colCenter));
                int i7 = m_mesh.Vertex(new VertexPositionColor(new Vector3(m_pastPositions[i + 1] - orthoDir * width, 0), colCenter));
                int i8 = m_mesh.Vertex(new VertexPositionColor(new Vector3(m_pastPositions[i + 1] - orthoDir * width * 2, 0), colSide));

                m_mesh.Index(i1); m_mesh.Index(i2); m_mesh.Index(i5); 
                m_mesh.Index(i2); m_mesh.Index(i5); m_mesh.Index(i6);

                m_mesh.Index(i2); m_mesh.Index(i3); m_mesh.Index(i6);
                m_mesh.Index(i3); m_mesh.Index(i6); m_mesh.Index(i7);

                m_mesh.Index(i3); m_mesh.Index(i4); m_mesh.Index(i7); 
                m_mesh.Index(i4); m_mesh.Index(i7); m_mesh.Index(i8);
            }
        }

        public void Draw()
        {
            if (!m_active)
                return;

            m_mesh.PrepareDraw();

            Engine.Renderer.Device.BlendState = BlendState.NonPremultiplied;
            Engine.Renderer.Device.RasterizerState = RasterizerState.CullNone;
            Engine.Renderer.Device.DepthStencilState = DepthStencilState.None;
            Engine.Renderer.DrawMesh(m_mesh, m_effect);
        }
    }
}
