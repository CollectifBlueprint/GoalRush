using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LBE
{
    public struct Box2D
    {
        public Vector2 Min { get; set; }
        public Vector2 Max { get; set; }

        public Vector2 HalfSize
        {
            get { return (Max - Min) * 0.5f; }
        }

        public Box2D(Vector2 min, Vector2 max)
            : this()
        {
            Min = min;
            Max = max;
        }

        public Box2D(Vector2 center, float width, float height)
            : this()
        {
            Min = new Vector2(center.X - width / 2, center.Y - height / 2);
            Max = new Vector2(center.X + width / 2, center.Y + height / 2);
        }

        public Vector2 Center()
        {
            return 0.5f * (Min + Max);
        }

        public static bool Contains(Box2D a, Vector2 pos)
        {
            return
                (a.Min.X <= pos.X && a.Max.X >= pos.X &&
                a.Min.Y <= pos.Y && a.Max.Y >= pos.Y);
        }

        public static bool Intersect(Box2D a, Box2D b)
        {
            return 
                !(a.Min.X >= b.Max.X || a.Max.X <= b.Min.X ||
                a.Min.Y >= b.Max.Y || a.Max.Y <= b.Min.Y);
        }

        public static Vector2 IntersectionDepth(Box2D a, Box2D b)
        {
            float xDepth = DepthOnAxis(a.Min.X, a.Max.X, b.Min.X, b.Max.X);
            float yDepth = DepthOnAxis(a.Min.Y, a.Max.Y, b.Min.Y, b.Max.Y);

            return new Vector2(xDepth, yDepth);
        }

        static float DepthOnAxis(float aMin, float aMax, float bMin, float bMax)
        {
            //A is in B
            if (aMin >= bMin && aMax <= bMax)
            {
                return aMax - aMin;
            }
            //B is in A
            else if (bMin >= aMin && bMax <= aMax)
            {
                return bMax - bMin;
            }
            else
            {
                if(aMax >= bMin && aMax <= bMax)
                {
                    return aMax - bMin;
                }
                else if (aMin >= bMin && aMin <= bMax)
                {
                    return aMin - bMax;
                }
            }

            return 0;
        }
    }
}
