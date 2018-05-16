using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using LBE.Graphics.Sprites;
using Microsoft.Xna.Framework.Graphics;

namespace LBE.Graphics.Particles
{
    public class Particle
    {
        //Particle System
        public ParticleEmitter Owner;

        //State
        public float Age;
        public float LifetimeMS;
        public bool Alive;

        //Transform
        public Vector2 Position;
        public Vector2 Velocity;
        public float Orientation;
        public float AngularVelocity;
        public Vector2 Scale;

        //Rendering
        public Color Color;
        public float Opacity;

        //Modifiers
        public Vector2 ScaleModifier;
        public Color ColorModifier;
        public float OpacityModifier;

        public void Reset()
        {
            Alive = false;
            Age = 0;
            LifetimeMS = 0;
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
            Orientation = 0;
            AngularVelocity = 0;
            Scale = Vector2.One;
            ScaleModifier = Vector2.One;
            Color = Color.White;
            ColorModifier = Color.White;
            Opacity = 1.0f;
            OpacityModifier = 1.0f;
        }
    }
}
