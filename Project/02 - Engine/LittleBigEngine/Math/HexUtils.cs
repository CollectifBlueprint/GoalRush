using System;
using Microsoft.Xna.Framework;

namespace LBE.Hex
{
    /// <summary>
    /// Provides a set of methods helping with the geometry of a hexagonal tiling, especially the conversion from and to Cartesian coordinates
    /// The scale and center of the tiling are given by setting that the circumcircle of the hexagon (0,0) is the unit circle.
    /// </summary>
    public class HexUtils
    {
        static readonly Vector2 m_uStep = 
            new Vector2(
                1.5f, 
                (float)Math.Sin(Math.PI / 3));

        static readonly Vector2 m_vStep = 
            new Vector2(
                0,
                2 * (float)Math.Sin(Math.PI / 3));

        /// <summary>
        /// The "positive" offset between two hex on the U axis 
        /// </summary>
        public static Vector2 UStep
        {
            get { return HexUtils.m_uStep; }
        }

        /// <summary>
        /// The "positive" offset between two hex on the V axis 
        /// </summary>
        public static Vector2 VStep
        {
            get { return HexUtils.m_vStep; }
        }

        /// <summary>
        /// Returns the center of an hexagon.
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Vector2 HexToVector(Hex hex)
        {
            Vector2 xy;
            xy.X = UStep.X * hex.U;
            xy.Y = VStep.Y * hex.V + UStep.Y * Math.Abs(hex.U % 2);

            return xy;
        }

        /// <summary>
        /// Returns the hexagon containing a specified point in space
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Hex VectorToHex(Vector2 v)
        {
            Hex hex = new Hex(0,0);

            //First, find approximate U and V coordinates using uStep and vStep
            float xRel = v.X / UStep.X;
            if (xRel > 0)
                xRel += 0.5f;
            else
                xRel -= 0.5f;

            float yOffset = UStep.Y * Math.Abs(hex.U % 2);
            float yRel = 0.5f * (v.Y - yOffset) / UStep.Y;
            if (yRel > 0)
                yRel += 0.5f;
            else
                yRel -= 0.5f;

            hex.U = (int)xRel;
            hex.V = (int)yRel;

            //The approximate UV can be wrong if the point lie on a corner of the unit square centered around the hex center
            //We test the 4 different edge cases
            Vector2 offset = (v - hex.ToVector());
            offset.X = offset.X / 1;
            offset.Y = offset.Y / 2 * (float)Math.Cos(Math.PI / 6.0f);

            if (offset.X + offset.Y > 1)
                return new Hex(hex.U + 1, hex.V + Math.Abs(hex.U % 2));
            else if (offset.X - offset.Y > 1)
                return new Hex(hex.U + 1, hex.V - 1 + Math.Abs(hex.U % 2));
            else if (-offset.X + offset.Y > 1)
                return new Hex(hex.U - 1, hex.V + Math.Abs(hex.U % 2));
            else if (-offset.X - offset.Y > 1)
                return new Hex(hex.U - 1, hex.V - 1 + Math.Abs(hex.U % 2));
            else
                return hex;
        }

        public static Hex Next(Hex hex, HexDirection direction)
        {
            Hex next = hex;

            switch (direction)
            {
                case HexDirection.Top:
                    next.V++;
                    break;
                case HexDirection.Bottom:
                    next.V--;
                    break;
                case HexDirection.TopLeft:
                    next.U--;
                    if (next.U % 2 == 0)
                        next.V++;
                    break;
                case HexDirection.TopRight:
                    next.U++;
                    if (next.U % 2 == 0)
                        next.V++;
                    break;
                case HexDirection.BottomLeft:
                    next.U--;
                    if (next.U % 2 != 0)
                        next.V--;
                    break;
                case HexDirection.BottomRight:
                    next.U++;
                    if (next.U % 2 != 0)
                        next.V--;
                    break;
            }

            return next;
        }
    }
}
