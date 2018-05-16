using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LBE
{
    public static class MathHelper
    {
        /// <summary>
        /// Clamps the specified value to the specified minimum and maximum range.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="x">A value to clamp.</param>
        /// <returns>The clamped value for the x parameter.</returns>
        public static float Clamp(float min, float max, float x)
        {
            if (x < min)
                return min;
            if (x > max)
                return max;

            return x;
        }

        /// <summary>
        /// Clamps the specified value to the specified minimum and maximum range.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="x">A value to clamp.</param>
        /// <returns>The clamped value for the x parameter.</returns>
        public static int Clamp(int min, int max, int x)
        {
            if (x < min)
                return min;
            if (x > max)
                return max;

            return x;
        }

        /// <summary>
        /// Compute a weight function that is maximum a center and zero starting at maxDistance and over
        /// </summary>
        /// <param name="center"></param>
        /// <param name="maxDistance"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static float WeightDistance(Vector2 center, float maxDistance, Vector2 position)
        {
            return 1 - Clamp(0, 1, Vector2.DistanceSquared(center, position) / (maxDistance * maxDistance));
        }

        /// <summary>
        /// Performs a linear interpolation.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float Lerp(float a, float b, float x)
        {
            return a * (1 - x) + b * x;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Vector2 Lerp(Vector2 a, Vector2 b, float x)
        {
            return a * (1 - x) + b * x;
        }

        /// <summary>
        /// Returns a linear interpolation between 0 and 1, if x is in the range [min, max].
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="x"></param>
        /// <returns>Returns 0 if x is less than min; 1 if x is greater than max; otherwise, a value between 0 and 1 if x is in the range [min, max].</returns>
        public static float LinearStep(float min, float max, float x)
        {
            float k = (x - min) / (max - min);
            return Clamp(0, 1, k);
        }

        public static float AngleDistanceSigned(float angle1, float angle2)
        {
            return NormalizeAngle(angle2 - angle1);
        }        

        /// <summary>
        /// Return an angle between -PI and +PI
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static float NormalizeAngle(float angle)
        {
            angle = angle % (2 * (float)Math.PI);
            if (angle < -Math.PI)
                return angle + 2 * (float)Math.PI;
            if (angle > Math.PI)
                return angle - 2 * (float)Math.PI;
            return angle;
        }

        public static float CrossProductSign(Vector2 v1, Vector2 v2)
        {
            return (float)Math.Sign(CrossProductDet(v1, v2));
        }

        public static float CrossProductDet(Vector2 v1, Vector2 v2)
        {
            return v1.X * v2.Y - v1.Y * v2.X;
        }
        
        public static float Angle(Vector2 v1, Vector2 v2)
        {
            float dot = Vector2.Dot(v1, v2) / v2.Length() / v1.Length();
            return (float)Math.Acos(dot);
        }

        public static float Cos(float period, double time, float phase = 0)
        {            
            float angularSpeed = 2 * (float)Math.PI / period;
            return (float)Math.Cos(angularSpeed * time + phase);
        }
    }
}
