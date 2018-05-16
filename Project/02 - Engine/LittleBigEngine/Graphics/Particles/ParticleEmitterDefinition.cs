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
using LBE.Graphics.Particles.Utils;
using LBE.Graphics.Particles.Modifiers;
using Microsoft.Xna.Framework.Graphics;

namespace LBE.Graphics.Particles
{
    public enum CoordinatesMode
    {
        World,
        Local,
    }

    public enum ParticleBlendMode
    {
        Opaque,
        Blend,
        Add,
        Dual,
    }

    public class ParticleEmitterDefinition
    {
        //Name
        public String Name = "Emitter";

        //Shape
        public IEmitterShape Shape = new CircleShape();
        public CoordinatesMode Coordinates = CoordinatesMode.World;

        //Emission
        public int EmitRate = 30;
        public float EmitDelay = 0;
        public int MaxParticle = 1000;
        public float Duration = 0;

        //Initialisation
        public ParticleParameterSingle Lifetime = 1.0f;
        public ParticleParameterVector2 Position = Vector2.Zero;
        public ParticleParameterVector2 Velocity = new Vector2(1, 0);
        public ParticleParameterSingle Orientation = 0;
        public ParticleParameterSingle AngularVelocity = 0;
        public ParticleParameterVector2 Scale = Vector2.One;
        public ParticleParameterSingle Opacity = 1;

        //Rendering
        public Texture2D Texture = null;
        public ParticleBlendMode BlendMode = ParticleBlendMode.Add;
        public Color Color = Color.White;

        //Modifiers
        public IModifier[] Modifiers = new IModifier[0];
    }
}
