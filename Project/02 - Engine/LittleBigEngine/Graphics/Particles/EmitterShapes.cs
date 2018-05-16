using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LBE.Graphics.Particles
{
    public interface IEmitterShape
    {
        Vector2 Get(float t);
    }

    public class CircleShape : IEmitterShape
    {
        public Vector2 Get(float t)
        {
            return Vector2.UnitX.Rotate(2 * (float)Math.PI * t);
        }
    }

    public class ConeShape : IEmitterShape
    {
        public float Orientation = 0;
        public float Angle = (float)Math.PI * 0.5f;

        public Vector2 Get(float t)
        {
            t = 0.5f - t;
            return Vector2.UnitX.Rotate(Orientation + Angle * t);
        }
    }

    public class LineShape : IEmitterShape
    {
        public float Orientation = 0;

        public Vector2 Get(float t)
        {
            return Vector2.UnitX.Rotate(Orientation);
        }
    }
}
