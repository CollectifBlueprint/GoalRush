using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LBE.Utils
{
    /// <summary>
    /// Source: https://www.codeproject.com/Articles/18936/A-Csharp-Implementation-of-Douglas-Peucker-Line-Ap
    /// Author: Craig Selbert
    /// Licensed under the MIT license (http://www.opensource.org/licenses/mit-license.php)
    /// </summary>
    public class DouglasPeucker
    {
        /// <summary>
        /// Uses the Douglas Peucker algorithm to reduce the number of Vector2s.
        /// </summary>
        /// <param name="points">The Vector2s.</param>
        /// <param name="Tolerance">The tolerance.</param>
        /// <returns></returns>
        public static List<Vector2> DouglasPeuckerReduction
            (List<Vector2> points, Double Tolerance)
        {
            if (points == null || points.Count < 3)
                return points;

            Int32 firstVector2 = 0;
            Int32 lastVector2 = points.Count - 1;
            List<Int32> Vector2IndexsToKeep = new List<Int32>();

            //Add the first and last index to the keepers
            Vector2IndexsToKeep.Add(firstVector2);
            Vector2IndexsToKeep.Add(lastVector2);

            //The first and the last Vector2 cannot be the same
            while (points[firstVector2].Equals(points[lastVector2]))
            {
                lastVector2--;
            }

            DouglasPeuckerReduction(points, firstVector2, lastVector2,
            Tolerance, ref Vector2IndexsToKeep);

            List<Vector2> returnVector2s = new List<Vector2>();
            Vector2IndexsToKeep.Sort();
            foreach (Int32 index in Vector2IndexsToKeep)
            {
                returnVector2s.Add(points[index]);
            }

            return returnVector2s;
        }

        /// <summary>
        /// Douglases the peucker reduction.
        /// </summary>
        /// <param name="Vector2s">The Vector2s.</param>
        /// <param name="firstVector2">The first Vector2.</param>
        /// <param name="lastVector2">The last Vector2.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <param name="Vector2IndexsToKeep">The Vector2 index to keep.</param>
        private static void DouglasPeuckerReduction(List<Vector2>
            Vector2s, Int32 firstVector2, Int32 lastVector2, Double tolerance,
            ref List<Int32> Vector2IndexsToKeep)
        {
            Double maxDistance = 0;
            Int32 indexFarthest = 0;

            for (Int32 index = firstVector2; index < lastVector2; index++)
            {
                Double distance = PerpendicularDistance
                    (Vector2s[firstVector2], Vector2s[lastVector2], Vector2s[index]);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    indexFarthest = index;
                }
            }

            if (maxDistance > tolerance && indexFarthest != 0)
            {
                //Add the largest Vector2 that exceeds the tolerance
                Vector2IndexsToKeep.Add(indexFarthest);

                DouglasPeuckerReduction(Vector2s, firstVector2,
                indexFarthest, tolerance, ref Vector2IndexsToKeep);
                DouglasPeuckerReduction(Vector2s, indexFarthest,
                lastVector2, tolerance, ref Vector2IndexsToKeep);
            }
        }

        /// <summary>
        /// The distance of a Vector2 from a line made from Vector21 and Vector22.
        /// </summary>
        /// <param name="pt1">The PT1.</param>
        /// <param name="pt2">The PT2.</param>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public static Double PerpendicularDistance
            (Vector2 Vector21, Vector2 Vector22, Vector2 Vector2)
        {
            Double area = Math.Abs(.5 * (Vector21.X * Vector22.Y + Vector22.X *
            Vector2.Y + Vector2.X * Vector21.Y - Vector22.X * Vector21.Y - Vector2.X *
            Vector22.Y - Vector21.X * Vector2.Y));
            Double bottom = Math.Sqrt(Math.Pow(Vector21.X - Vector22.X, 2) +
            Math.Pow(Vector21.Y - Vector22.Y, 2));
            Double height = area / bottom * 2;

            return height;
        }
    }
}
