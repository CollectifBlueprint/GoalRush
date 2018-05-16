using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LBE.Graphics.Particles.Modifiers
{
    public class ScaleOverLifetime : IModifier
    {
        public void Modify(Particle[] particles, int from, int to, float dt)
        {
            for (int i = from; i < to; i++)
            {
                float scale = 1 - LBE.MathHelper.LinearStep(0, particles[i].LifetimeMS, particles[i].Age);
                particles[i].ScaleModifier *= new Vector2(scale, scale);
            }
        }
    }
}
