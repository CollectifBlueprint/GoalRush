using System;
using Microsoft.Xna.Framework;

namespace LBE
{
    public static class RandomHelper
    {
        /// <summary>
        /// Returns a random value in a enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="random"></param>
        /// <returns></returns>
        public static T NextEnum<T>(this Random random)
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            return values[random.Next(0, values.Length)];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        public static float NextSign(this Random random)
        {
            if (random.NextFloat() > 0.5f)
                return 1.0f;
            return -1.0f;
        }

        /// <summary>
        /// Returns a random normalized 2d vector
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        public static Vector2 NextVector2(this Random random)
        {
            float angle = random.NextFloat() * 2 * (float)Math.PI;
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        /// <summary>
        /// Returns a random float between 0 and 1.
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        public static float NextFloat(this Random random)
        {
            return (float)random.NextDouble();
        }

        /// <summary>
        /// Return a random float bewteen two values
        /// </summary>
        /// <param name="random"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float NextFloat(this Random random, float min, float max)
        {
            return min + (max - min) * (float)random.NextDouble();
        }
    }
}
