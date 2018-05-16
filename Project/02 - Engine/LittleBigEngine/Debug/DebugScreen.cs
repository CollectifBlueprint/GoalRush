using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LBE.Graphics.Effects;

namespace LBE.Debug
{
    struct TextEntry
    {
        public String Text;
        public Color Color;
        public Vector2 Position;
        public bool Background;
    }

    public class DebugScreen : IRenderable, IDisposable
    {
        DebugBrush m_brush;
        public DebugBrush Brush
        {
            get { return m_brush; }
            set { m_brush = value; }
        }

        private DynamicMesh<VertexPositionColor> m_lineMesh;
        private DynamicMesh<VertexPositionColor> m_triangleMesh;

        private List<TextEntry> m_texts;

        private EffectWrapper m_effect;

        public DebugScreen()
        {
            int capacity = 1024 * 1024 * 4;
            m_lineMesh = new DynamicMesh<VertexPositionColor>(Engine.Renderer.Device, VertexPositionColor.VertexDeclaration, PrimitiveType.LineList, capacity);
            m_triangleMesh = new DynamicMesh<VertexPositionColor>(Engine.Renderer.Device, VertexPositionColor.VertexDeclaration, PrimitiveType.TriangleList, capacity);

            m_texts = new List<TextEntry>();

            ResetBrush();

            var basicEffect = new BasicEffect(Engine.Renderer.Device);
            basicEffect.LightingEnabled = false;
            basicEffect.VertexColorEnabled = true;
            basicEffect.FogEnabled = false;

            m_effect = new EffectWrapper(basicEffect);
        }

        public void ResetBrush()
        {
            m_brush = new DebugBrush();

            m_brush.DrawWireframe = true;
            m_brush.LineColor = Color.White;
            m_brush.LineAlpha = 1.0f;

            m_brush.DrawSurface = true;
            m_brush.SurfaceColor = Color.Red;
            m_brush.SurfaceAlpha = 0.4f;

            m_brush.TextColor = Color.White;
        }

        public void Reset()
        {
            m_lineMesh.Reset();
            m_triangleMesh.Reset();
            m_texts.Clear();
        }

        public void Dispose()
        {
            m_lineMesh.Dispose();
            m_triangleMesh.Dispose();
            m_effect.Effect.Dispose();
        }

        public void Draw()
        {
            if (!Engine.Debug.Flags.RenderDebug)
                return;

            float textBoxMargin = 1.0f;

            Engine.Renderer.CurrentCamera = Engine.Renderer.Cameras[0];

            //Build log debug string
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();
            foreach (var dbValue in Engine.Log.DebugValues)
            {
                sb.AppendLine(dbValue.Key + ": " + dbValue.Value);
            }
            String logDebugString = sb.ToString();
            Vector2 logDebugOriginScreen = new Vector2(5, 5);
            Vector2 logDebugSize = Engine.Renderer.SpriteFont.MeasureString(logDebugString);
            float logDebugMinWidth = 150;
            logDebugSize.X = Math.Max(logDebugMinWidth, logDebugSize.X);
            Vector2 logDebugCenterWorld = Engine.Renderer.CurrentCamera.ScreenToWorld(logDebugOriginScreen + logDebugSize / 2);

            //Prepare render paramters for text boxes
            var underlayColor = new Color(32, 32, 32);
            var lineColor = new Color(16, 16, 16);
            Brush.DrawWireframe = true;
            Brush.LineColor = lineColor;
            Brush.LineAlpha = 1.0f;
            Brush.DrawSurface = true;
            Brush.SurfaceColor = underlayColor;
            Brush.SurfaceAlpha = 0.75f;

            //Add debug strings text boxes
            foreach (var msg in m_texts)
            {
                if (msg.Background)
                {
                    Vector2 size = Engine.Renderer.SpriteFont.MeasureString(msg.Text);
                    AddRectangle(msg.Position, size / 2 + new Vector2(textBoxMargin, textBoxMargin));
                }
            }

            //Add log debug text box
            if (Engine.Log.DebugEnabled)
                AddRectangle(logDebugCenterWorld, logDebugSize / 2 + new Vector2(textBoxMargin, textBoxMargin));

            m_lineMesh.PrepareDraw();
            m_triangleMesh.PrepareDraw();

            Engine.Renderer.Device.BlendState = BlendState.AlphaBlend;
            Engine.Renderer.Device.RasterizerState = RasterizerState.CullNone;
            Engine.Renderer.Device.DepthStencilState = DepthStencilState.None;

            if (m_triangleMesh.PrimitiveCount > 0)
            {
                Engine.Renderer.DrawMesh(m_triangleMesh, m_effect);
            }

            if (m_lineMesh.PrimitiveCount > 0)
            {
                Engine.Renderer.DrawMesh(m_lineMesh, m_effect);
            }

            foreach (var msg in m_texts)
            {
                Vector2 size = Engine.Renderer.SpriteFont.MeasureString(msg.Text);
                Vector2 screenPos = Engine.Renderer.CurrentCamera.WorldToScreen(msg.Position) - size / 2;
                Engine.Renderer.DrawStringUnbatched(screenPos, msg.Text, Color.White);
            }

            if (Engine.Log.DebugEnabled)
                Engine.Renderer.DrawStringUnbatched(logDebugOriginScreen, logDebugString, Color.White);
        }

        public void AddString(String str, Vector2 pos, Color color, bool background = false)
        {
            var entry = new TextEntry();
            entry.Text = str;
            entry.Position = pos;
            entry.Color = color;
            entry.Background = background;
            m_texts.Add(entry);
        }

        public void AddString(String str, Vector2 pos, bool background = false)
        {
            AddString(str, pos, m_brush.TextColor, background);
        }

        public void AddTriangle(Vector2 v1, Vector2 v2, Vector2 v3, ref DebugBrush debugRenderParams)
        {
            if (debugRenderParams.DrawSurface)
            {
                var vertex = new VertexPositionColor();

                vertex.Position = new Vector3(v1, 0);
                vertex.Color = debugRenderParams.SurfaceColor;
                int idx1 = m_triangleMesh.Vertex(vertex);

                vertex.Position = new Vector3(v2, 0);
                vertex.Color = debugRenderParams.SurfaceColor;
                int idx2 = m_triangleMesh.Vertex(vertex);

                vertex.Position = new Vector3(v3, 0);
                vertex.Color = debugRenderParams.SurfaceColor;
                int idx3 = m_triangleMesh.Vertex(vertex);

                m_triangleMesh.Index(idx1);
                m_triangleMesh.Index(idx2);
                m_triangleMesh.Index(idx3);
            }
        }

        public void AddTriangle(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            AddTriangle(v1, v2, v3, ref m_brush);
        }

        public void AddLine(Vector2 start, Vector2 end, ref DebugBrush debugRenderParams)
        {
            if (debugRenderParams.DrawWireframe)
            {
                var vertex = new VertexPositionColor();

                vertex.Position = new Vector3(start, 0);
                vertex.Color = debugRenderParams.LineColor;
                int idx1 = m_lineMesh.Vertex(vertex);

                vertex.Position = new Vector3(end, 0);
                vertex.Color = debugRenderParams.LineColor;
                int idx2 = m_lineMesh.Vertex(vertex);

                m_lineMesh.Index(idx1);
                m_lineMesh.Index(idx2);
            }
        }

        public void AddLine(Vector2 start, Vector2 end)
        {
            AddLine(start, end, ref m_brush);
        }

        public void AddArrow(Vector2 from, Vector2 to)
        {
            if (from != to)
            {
                AddLine(from, to);
                float arrowSize = 8;
                Vector2 dir = to - from; dir.Normalize();
                Vector2 orhtoDir = new Vector2(-dir.Y, dir.X);
                AddLine(to, to + (-dir + orhtoDir) * 0.5f * arrowSize);
                AddLine(to, to + (-dir - orhtoDir) * 0.5f * arrowSize);
            }

        }

        public void AddCross(Vector2 position, float halfSize)
        {
            AddLine(position - halfSize * Vector2.UnitX, position + halfSize * Vector2.UnitX);
            AddLine(position - halfSize * Vector2.UnitY, position + halfSize * Vector2.UnitY);
        }

        public void AddSquare(Vector2 position, float halfSize)
        {
            AddRectangle(position, Vector2.One * halfSize);
        }

        public void AddRectangle(Vector2 position, Vector2 halfSize)
        {
            Vector2 cornerTR = position + halfSize.X * Vector2.UnitX + halfSize.Y * Vector2.UnitY;
            Vector2 cornerTL = position - halfSize.X * Vector2.UnitX + halfSize.Y * Vector2.UnitY;
            Vector2 cornerBR = position + halfSize.X * Vector2.UnitX - halfSize.Y * Vector2.UnitY;
            Vector2 cornerBL = position - halfSize.X * Vector2.UnitX - halfSize.Y * Vector2.UnitY;

            AddLine(cornerTR, cornerTL);
            AddLine(cornerTL, cornerBL);
            AddLine(cornerBL, cornerBR);
            AddLine(cornerBR, cornerTR);

            AddTriangle(cornerTR, cornerTL, cornerBL);
            AddTriangle(cornerTR, cornerBR, cornerBL);
        }


        static readonly Vector2[] HexPointArray = 
        { 
            new Vector2((float)Math.Cos(Math.PI / 3 * 0), (float)Math.Sin(Math.PI / 3 * 0)),
            new Vector2((float)Math.Cos(Math.PI / 3 * 1), (float)Math.Sin(Math.PI / 3 * 1)),
            new Vector2((float)Math.Cos(Math.PI / 3 * 2), (float)Math.Sin(Math.PI / 3 * 2)),
            new Vector2((float)Math.Cos(Math.PI / 3 * 3), (float)Math.Sin(Math.PI / 3 * 3)),
            new Vector2((float)Math.Cos(Math.PI / 3 * 4), (float)Math.Sin(Math.PI / 3 * 4)),
            new Vector2((float)Math.Cos(Math.PI / 3 * 5), (float)Math.Sin(Math.PI / 3 * 5)),
            new Vector2((float)Math.Cos(Math.PI / 3 * 6), (float)Math.Sin(Math.PI / 3 * 6)),
        };

        public void AddHex(Vector2 position, float radius)
        {
            for (int i = 0; i < 6; i++)
            {
                Vector2 p1 = position + radius * HexPointArray[i];
                Vector2 p2 = position + radius * HexPointArray[i + 1];

                if (m_brush.DrawWireframe)
                    AddLine(p1, p2);
                if (m_brush.DrawSurface)
                    AddTriangle(position, p1, p2);
            }
        }

        public void AddCircle(Vector2 position, float radius, int nbrSide = 32)
        {
            for (int i = 0; i < nbrSide; i++)
            {
                float angleStep = 2 * (float)Math.PI / nbrSide;
                Vector2 p1 = position + radius * new Vector2((float)Math.Cos(angleStep * i), (float)Math.Sin(angleStep * i));
                Vector2 p2 = position + radius * new Vector2((float)Math.Cos(angleStep * (i + 1)), (float)Math.Sin(angleStep * (i + 1)));

                if (m_brush.DrawWireframe)
                    AddLine(p1, p2);
                if (m_brush.DrawSurface)
                    AddTriangle(position, p1, p2);
            }
        }
    }
}
