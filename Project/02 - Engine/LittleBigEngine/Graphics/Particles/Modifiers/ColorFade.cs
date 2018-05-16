using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE;
using LBE.Assets;
using LBE.Core;
using LBE.Gameplay;
using LBE.Graphics;
using LBE.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LBE.Graphics.Particles.Modifiers
{
    public class ColorFade : IModifier
    {
        public Color Color;

        public void Modify(Particle[] particles, int from, int to, float dt)
        {
            for (int i = from; i < to; i++)
            {
                float relAge = LBE.MathHelper.Clamp(0, 1, particles[i].Age / particles[i].LifetimeMS);
                particles[i].ColorModifier = new Color(particles[i].Color.ToVector3() * (1 - relAge) + Color.ToVector3() * relAge);
            }
        }
    }

    public class ColorFadeOut : IModifier
    {
        public Color Color;
        public float Time;

        public void Modify(Particle[] particles, int from, int to, float dt)
        {
            for (int i = from; i < to; i++)
            {
                float relAge = LBE.MathHelper.Clamp(0, 1, (particles[i].Age - Time * 1000) / (particles[i].LifetimeMS - Time * 1000));
                particles[i].ColorModifier = new Color(particles[i].Color.ToVector3() * (1 - relAge) + Color.ToVector3() * relAge);
            }
        }
    }
}
