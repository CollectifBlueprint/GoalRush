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
    public class Gravity : IModifier
    {
        public const float G = 0.01f;
        public float Strength = 1.0f;

        public void Modify(Particle[] particles, int from, int to, float dt)
        {
            for (int i = from; i < to; i++)
            {
                float accel = G * Strength * dt;
                particles[i].Velocity -= new Vector2(0, accel);
            }
        }
    }
}
