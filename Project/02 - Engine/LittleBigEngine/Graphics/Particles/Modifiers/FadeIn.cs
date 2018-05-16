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
    public class FadeIn : IModifier
    {
        public float Time = 0.1f;

        public void Modify(Particle[] particles, int from, int to, float dt)
        {
            for (int i = from; i < to; i++)
            {
                float fade = LBE.MathHelper.LinearStep(0, Time * 1000, particles[i].Age);
                particles[i].OpacityModifier = fade;
            }
        }
    }

    public class FadeOut : IModifier
    {
        public float Time = 0.9f;

        public void Modify(Particle[] particles, int from, int to, float dt)
        {
            for (int i = from; i < to; i++)
            {
                float fade = LBE.MathHelper.LinearStep(Time * 1000, particles[i].LifetimeMS, particles[i].Age);
                particles[i].OpacityModifier = 1 - fade;
            }
        }
    }   
}
