using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using LBE.Assets;


namespace LBE.Graphics.Effects
{
    public class EffectWrapper
    {
        public EffectParameter this[String name]
        {
            get { return m_effect.Content.Parameters[name]; }
        }

        public static EffectWrapper Create(String path)
        {
            Asset<Effect> asset = Engine.AssetManager.GetAsset<Effect>(path);
            return new EffectWrapper(asset);
        }

        protected Asset<Effect> m_effect;
        public Effect Effect
        {
            get { return m_effect.Content; }
        }

        public EffectWrapper(Effect effect)
        {
            m_effect = new Asset<Effect>(effect);
        }

        public EffectWrapper(Asset<Effect> effect)
        {
            m_effect = effect;
        }

        //public void Set<T>(String name, T value)
        //{
        //    m_effect.Content.Parameters[name].SetValue(value);
        //}

        public Matrix World
        {
            set { m_effect.Content.Parameters["World"].SetValue(value); }
        }

        public Matrix View
        {
            set { m_effect.Content.Parameters["View"].SetValue(value); }
        }

        public Matrix Projection
        {
            set { m_effect.Content.Parameters["Projection"].SetValue(value); }
        }
    }
}
