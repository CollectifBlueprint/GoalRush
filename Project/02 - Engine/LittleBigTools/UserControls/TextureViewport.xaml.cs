using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Xna.Framework.Graphics;
using LBE;
using LBE.Graphics.Sprites;
using Microsoft.Xna.Framework;

using Color = Microsoft.Xna.Framework.Color;

namespace LBT.UserControls
{
    /// <summary>
    /// Interaction logic for TextureViewport.xaml
    /// </summary>
    public partial class TextureViewport : UserControl
    {
        public static readonly DependencyProperty TextureProperty =
            DependencyProperty.Register(
            "Texture", typeof(Texture2D), typeof(TextureViewport));

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
            "Color", typeof(Color), typeof(TextureViewport));

        public static readonly DependencyProperty RealTimeProperty =
            DependencyProperty.Register(
            "RealTime", typeof(bool), typeof(TextureViewport));

        public Texture2D Texture
        {
            get { return (Texture2D)GetValue(TextureProperty); }
            set { SetValue(TextureProperty, value); Viewport.Invalidate(); }
        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public bool RealTime
        {
            get { return (bool)GetValue(RealTimeProperty); }
            set { SetValue(RealTimeProperty, value); Viewport.RealTime = value; }
        }

        public TextureViewport()
        {
            Color = Color.Black;

            InitializeComponent();
            Viewport.OnRender += Render;
        }

        public void Render()
        {
            Engine.Renderer.Device.Clear(Color);
            if (Texture != null)
            {
                float xRatio = Texture.Width / (float)Viewport.ActualWidth;
                float yRatio = Texture.Height / (float)Viewport.ActualHeight;
                float textureRatio = Math.Max(xRatio, yRatio); textureRatio = Math.Max(textureRatio, 1);

                Engine.Renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
                Engine.Renderer.Draw(Texture, (int)(Texture.Width / textureRatio), (int)(Texture.Height / textureRatio));
                Engine.Renderer.SpriteBatch.End();

                int minDirtySize = 64;

                if (Texture.Width / textureRatio > minDirtySize && (int)(Texture.Width / textureRatio) > minDirtySize)
                {
                    Viewport.DirtyRectangle = new Int32Rect(
                        (int)(Viewport.RenderTarget.Width - Texture.Width / textureRatio) / 2,
                        (int)(Viewport.RenderTarget.Height - Texture.Height / textureRatio) / 2,
                        (int)(Texture.Width / textureRatio),
                        (int)(Texture.Height / textureRatio));
                }
                else
                    Viewport.DirtyRectangle = null;
            }
            else
            {
                Viewport.DirtyRectangle = new Int32Rect();
            }
        }
    }
}
