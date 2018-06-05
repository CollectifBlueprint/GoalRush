using LBE;
using LBE.Gameplay;
using LBE.Graphics;
using LBE.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ball.Gameplay.Fx
{
    class PlayerChargedPassFx : GameObjectComponent, IRenderable
    {

        DynamicMesh<VertexPositionColor> m_lineMesh;
        DynamicMesh<VertexPositionColor> m_triangleMesh;

        EffectWrapper m_effect;

        float m_angle;
        public float Angle
        {
            get { return m_angle; }
        }

        Timer m_chargeTimerMS;

        bool m_chargedMax;

        Player m_player;

        public PlayerChargedPassFx()
        {
           
           
        }

        public override void Start()
        {
            base.Start();

            var basicEffect = new BasicEffect(Engine.Renderer.Device);
            basicEffect.LightingEnabled = false;
            basicEffect.VertexColorEnabled = true;
            basicEffect.FogEnabled = false;

            m_effect = new EffectWrapper(basicEffect);

            int capacity = 512 * 512 * 4;
            m_lineMesh = new DynamicMesh<VertexPositionColor>(Engine.Renderer.Device, VertexPositionColor.VertexDeclaration, PrimitiveType.LineList, capacity);
            m_triangleMesh = new DynamicMesh<VertexPositionColor>(Engine.Renderer.Device, VertexPositionColor.VertexDeclaration, PrimitiveType.TriangleList, capacity);

            Engine.Renderer.RenderLayers["ArenaOverlay7"].Renderables.Add(this);

            m_player = Owner.FindComponent<Player>();

            m_chargeTimerMS = new Timer(Engine.GameTime.Source, m_player.Parameters.ChargedPass.ChargeTimeMS);
            m_chargeTimerMS.Start();
            m_chargeTimerMS.OnTime += m_chargeTimerMS_OnTime;

            m_chargedMax = false;
        }

        void m_chargeTimerMS_OnTime(Timer source)
        {
            m_chargedMax = true;
        }

        public override void Update()
        {
            base.Update();

            m_lineMesh.Reset();
            m_triangleMesh.Reset();

            int nbrSide = 32;
            int radius = 50;

            float chargeCompletion = LBE.MathHelper.LinearStep(0, m_chargeTimerMS.TargetTime, m_chargeTimerMS.TimeMS);
            if (m_chargedMax)
                chargeCompletion = 1;

            m_angle = 2 * (float)Math.PI * Math.Max(chargeCompletion, 0.05f * (float)Math.PI);
            for (int i = 0; i < nbrSide; i++)
            {
                float angleStep = m_angle/ nbrSide;

                Vector2 circlePoint1 = new Vector2((float)Math.Cos(angleStep * i), (float)Math.Sin(angleStep * i));
                circlePoint1 = circlePoint1.Rotate(m_player.BallAngle - 0.5f * m_angle);
                Vector2 p1 = Owner.Position + radius * circlePoint1;

                Vector2 circlePoint2 = new Vector2((float)Math.Cos(angleStep * (i + 1)), (float)Math.Sin(angleStep * (i + 1)));
                circlePoint2 = circlePoint2.Rotate(m_player.BallAngle - 0.5f * m_angle);
                Vector2 p2 = Owner.Position + radius * circlePoint2;

                // Circle
                var vertex = new VertexPositionColor();
                vertex.Color = m_player.Team.ColorScheme.Color1;

                vertex.Position = new Vector3(p1, 0);
                int idx1 = m_lineMesh.Vertex(vertex);

                vertex.Position = new Vector3(p2, 0);
                int idx2 = m_lineMesh.Vertex(vertex);

                m_lineMesh.Index(idx1);
                m_lineMesh.Index(idx2);

                // Disk
                var vertexT = new VertexPositionColor();
                vertexT.Color = new Color(m_player.Team.ColorScheme.Color1, 0.2f);

                vertexT.Position = new Vector3(Owner.Position, 0);
                int idxT1 = m_triangleMesh.Vertex(vertexT);

                vertexT.Position = new Vector3(p1, 0);
                int idxT2 = m_triangleMesh.Vertex(vertexT);

                vertexT.Position = new Vector3(p2, 0);
                int idxT3 = m_triangleMesh.Vertex(vertexT);

                m_triangleMesh.Index(idxT1);
                m_triangleMesh.Index(idxT2);
                m_triangleMesh.Index(idxT3);
            }
        }

        public void Draw()
        {
            m_lineMesh.PrepareDraw();
            m_triangleMesh.PrepareDraw();

            Engine.Renderer.Device.BlendState = BlendState.NonPremultiplied;
            Engine.Renderer.Device.RasterizerState = RasterizerState.CullNone;
            Engine.Renderer.Device.DepthStencilState = DepthStencilState.None;

            Engine.Renderer.DrawMesh(m_lineMesh, m_effect);
            Engine.Renderer.DrawMesh(m_triangleMesh, m_effect);
        }


        public void Stop()
        {
            Owner.Remove(this);
        }

        public override void End()
        {
            m_lineMesh.Reset();
            m_lineMesh.Dispose();
            m_triangleMesh.Reset();
            m_triangleMesh.Dispose();

            this.Enabled = false;

            m_chargeTimerMS.Stop();
            m_chargeTimerMS.OnTime -= m_chargeTimerMS_OnTime;
        }
    }
}
