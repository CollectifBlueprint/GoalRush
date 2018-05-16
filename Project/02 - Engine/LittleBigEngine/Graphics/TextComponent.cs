using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Assets;
using Microsoft.Xna.Framework;
using LBE.Gameplay;
using Microsoft.Xna.Framework.Graphics;

namespace LBE.Graphics.Sprites
{
    public enum TextAlignementHorizontal
    {
        Left, 
        Center, 
        Right,
    }

    public enum TextAlignementVertical
    {
        Down,
        Center,
        Up,
    }

    public class TextStyle
    {
        public SpriteFont Font = null;
        public Color Color = Color.White;
        public float Scale = 1.0f;
    }

    public class TextDefinition
    {
        public String Text = "";

        public TextStyle Style = new TextStyle();
        public TextAlignementHorizontal Alignement = TextAlignementHorizontal.Left;
        public TextAlignementVertical AlignementVertical = TextAlignementVertical.Up;
    }

    public class TextComponent : GameObjectComponent, IRenderable
    {
        Asset<TextDefinition> m_textDefinitionAsset;
        public Asset<TextDefinition> TextDefinitionAsset
        {
            get { return m_textDefinitionAsset; }
            set { m_textDefinitionAsset = value; }
        }

        String m_text;
        public String Text
        {
            get { return m_text; }
            set { SetText(value); }
        }

        String m_wrappedText;

        TextStyle m_style;
        public TextStyle Style
        {
            get { return m_style; }
            set { m_style = value; }
        }

        TextAlignementHorizontal m_alignement;
        public TextAlignementHorizontal Alignement
        {
            get { return m_alignement; }
            set { m_alignement = value; }
        }

        TextAlignementVertical m_alignementVertical;
        public TextAlignementVertical AlignementVertical
        {
            get { return m_alignementVertical; }
            set { m_alignementVertical = value; }
        }

        bool m_wrap;
        public bool Wrap
        {
            get { return m_wrap; }
            set { m_wrap = value; if (m_wrap) SetText(m_text); }
        }

        float m_wrapWidth;
        public float WrapWidth
        {
            get { return m_wrapWidth; }
            set { m_wrapWidth = value; }
        }

        Vector2 m_position;
        public Vector2 Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        bool m_attachedToOwner;
        public bool AttachedToOwner
        {
            get { return m_attachedToOwner; }
            set { m_attachedToOwner = value; }
        }

        float m_scale = 1.0f;
        public float Scale
        {
            get { return m_scale; }
            set { m_scale = value; }
        }

        bool m_visible;
        public bool Visible
        {
            get { return m_visible; }
            set { m_visible = value; }
        }

        RenderLayer m_layer;

        public TextComponent(String renderLayer = "Default")
            : this(new TextDefinition() { Text = "" }, renderLayer)
        {
        }

        public TextComponent(String text, String renderLayer = "Default")
            : this(new TextDefinition() { Text = text }, renderLayer)
        {
        }

        public TextComponent(TextDefinition textDef, String renderLayer = "Default")
            : this(new Asset<TextDefinition>(textDef), renderLayer)
        {
        }

        public TextComponent(Asset<TextDefinition> textDefAsset, String renderLayer = "Default")
        {
            m_textDefinitionAsset = textDefAsset;
            m_textDefinitionAsset.OnAssetChanged += new OnChange(SetPropertiesFromAsset);

            SetPropertiesFromAsset();

            if (Engine.Renderer.RenderLayers.ContainsKey(renderLayer))
                m_layer = Engine.Renderer.RenderLayers[renderLayer];

            m_attachedToOwner = true;
            m_visible = true;
        }

        void SetPropertiesFromAsset()
        {
            m_text = m_textDefinitionAsset.Content.Text;
            m_alignement = m_textDefinitionAsset.Content.Alignement;
            m_alignementVertical = m_textDefinitionAsset.Content.AlignementVertical;
            m_style = m_textDefinitionAsset.Content.Style;
        }
        
        
        private void SetText(string value)
        {
            m_text = value;
            if (m_wrap)
                m_wrappedText = WrapText(m_text, m_wrapWidth);
        }

        String WrapText(String text, float width)
        {
            StringBuilder textBuilder = new StringBuilder();
            var words = text.Split(' ');
            float currentPos = 0;
            foreach (var word in words)
            {                
                float wordWidth = m_style.Font.MeasureString(word).X * m_style.Scale;
                float wordWidthWithSpace = m_style.Font.MeasureString(word + " ").X * m_style.Scale;
                if (currentPos == 0)
                {
                    currentPos += wordWidthWithSpace;
                    textBuilder.Append(word);
                    textBuilder.Append(' ');
                }
                else if (currentPos + wordWidth <= m_wrapWidth)
                {
                    currentPos += wordWidthWithSpace;
                    textBuilder.Append(word);
                    textBuilder.Append(' ');
                }
                else
                {
                    textBuilder.AppendLine();
                    currentPos = 0;
                    currentPos += wordWidthWithSpace;
                    textBuilder.Append(word);
                    textBuilder.Append(' ');
                }
            }

            return textBuilder.ToString();
        }

        public override void Start()
        {
            if(m_layer != null)
                m_layer.Renderables.Add(this);
        }

        public override void Update()
        {
        }

        public void Draw()
        {
            if (m_text != "")
            {
                Vector2 position = m_position;
                if (m_attachedToOwner) position += Owner.Position;

                String text = m_wrap ? m_wrappedText : m_text;

                if (m_visible)
                    Engine.Renderer.DrawString(text, position, m_scale, m_style, m_alignement, m_alignementVertical);
            }
        }

        public override void End()
        {
            if (m_layer != null)
                m_layer.Renderables.Remove(this);
        }
    }
}
