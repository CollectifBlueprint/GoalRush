using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using LBE.Assets;
using Color = Microsoft.Xna.Framework.Color;

namespace LBE.Graphics.Effects
{
    public class SpriteEffect : EffectWrapper
    {
        new public static SpriteEffect Create(String path)
        {
            Asset<Effect> asset = Engine.AssetManager.GetAsset<Effect>(path);
            return new SpriteEffect(asset);
        }

        public SpriteEffect(Asset<Effect> effect)
            : base(effect)
        {
        }

        public Texture2D Texture
        {
            set { this["Texture"].SetValue(value); }
        }

        public Color Color
        {
            set { this["Color"].SetValue(value.ToVector4()); }
        }

        public float Alpha
        {
            set { this["Alpha"].SetValue(value); }
        }

        public Vector2 UVScale
        {
            set { this["UVScale"].SetValue(value); }
        }

        public Vector2 UVOffset
        {
            set { this["UVOffset"].SetValue(value); }
        }
    }
}
