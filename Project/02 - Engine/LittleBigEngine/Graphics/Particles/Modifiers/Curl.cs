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
    public class Curl : IModifier
    {
        public float Strength = 1.8f;
        public float Period = 1.0f;

        public void Modify(Particle[] particles, int from, int to, float dt)
        {
            Random r = new Random(from);
            for (int i = from; i < to; i++)
            {
                float relAge = LBE.MathHelper.Clamp(0, 1, particles[i].Age / particles[i].LifetimeMS);
                float phase = 2 * r.NextFloat(0, 1) * (float)Math.PI + 2 * particles[i].Owner.TimeMS.Time / (Period * 1000) * (float)Math.PI;
                float angle = 0.003f * Strength * dt * (float)Math.Cos(phase) * particles[i].Velocity.Length() * relAge;
                particles[i].Velocity = particles[i].Velocity.Rotate(angle);
                particles[i].Velocity *= 1.005f;
            }
        }
    }
}
