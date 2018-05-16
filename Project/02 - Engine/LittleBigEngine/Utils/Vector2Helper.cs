using Microsoft.Xna.Framework;
using System;

namespace LBE
{
    public static class Vector2Helper
    {
        public static float Angle(this Vector2 v)
        {
            return (float)Math.Atan2(v.Y, v.X);
        }

        public static Vector2 Orthogonal(this Vector2 v)
        {
            return new Vector2(-v.Y, v.X);
        }

        public static Vector2 ProjectX(this Vector2 v)
        {
            return new Vector2(v.X, 0);
        }

        public static Vector2 ProjectY(this Vector2 v)
        {
            return new Vector2(0, v.Y);
        }

        public static Vector2 Rotate(this Vector2 v, float angle)
        {
            return new Vector2(
                (float)Math.Cos(angle) * v.X - (float)Math.Sin(angle) * v.Y,
                (float)Math.Sin(angle) * v.X + (float)Math.Cos(angle) * v.Y);
        }
    }
}
