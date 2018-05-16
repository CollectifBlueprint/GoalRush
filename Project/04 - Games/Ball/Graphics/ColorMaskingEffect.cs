using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Assets;
using Microsoft.Xna.Framework.Graphics;
using LBE.Graphics.Effects;
using Microsoft.Xna.Framework;
using LBE;

namespace Ball.Graphics
{
    public class ColorMaskingEffect : EffectWrapper
    {
        new public static ColorMaskingEffect Create(String path)
        {
            Asset<Effect> asset = Engine.AssetManager.GetAsset<Effect>(path);
            return new ColorMaskingEffect(asset);
        }

        public ColorMaskingEffect(Asset<Effect> effect)
            : base(effect)
        {
        }

        public Texture2D Texture
        {
            set { try { m_effect.Content.Parameters["Texture"].SetValue(value); } catch { } }
        }

        public Texture2D Mask
        {
            set { try { m_effect.Content.Parameters["Mask"].SetValue(value); } catch { }}
        }

        public Color Color1
        {
            set { try { m_effect.Content.Parameters["Color1"].SetValue(value.ToVector3());} catch { } }
        }

        public Color Color2
        {
            set {try {  m_effect.Content.Parameters["Color2"].SetValue(value.ToVector3()); } catch { }}
        }

        public Color Color3
        {
            set {try {  m_effect.Content.Parameters["Color3"].SetValue(value.ToVector3()); } catch { }}
        }

        public Color Color4
        {
            set { try { m_effect.Content.Parameters["Color4"].SetValue(value.ToVector3()); } catch { } }
        }

        public float Alpha
        {
            set { try { m_effect.Content.Parameters["Alpha"].SetValue(value); } catch { } }
        }
    }
}
