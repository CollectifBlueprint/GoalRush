using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LBE.Utils
{
    /// <summary>
    /// Provide some functions for color manipulation.
    /// Formulas taken from http://www.easyrgb.com/
    /// </summary>
    public static class ColorHelper
    {
        public static Color SetSaturation(this Color color, float saturation)
        {
            Vector3 hsl = ColorHelper.RGBtoHSL(color.ToVector3());
            hsl.Y = saturation;
            return new Color(ColorHelper.HSLtoRGB(hsl));
        }

        public static Vector3 RGBtoHSL(Vector3 color)
        {
            float h = 0, s = 0, l = 0;

            float min = Math.Min(Math.Min(color.X, color.Y), color.Z);
            float max = Math.Max(Math.Max(color.X, color.Y), color.Z);
            float delta = max - min;

            l = (max + min) * 0.5f;

            if (delta == 0)
            {
                h = 0.0f;
                s = 0.0f;
            }
            else
            {
                if (l < 0.5)
                {
                    s = delta / (max + min);
                }
                else
                {
                    s = delta / (2.0f - max - min);
                }

                float deltaR = (((max - color.X) / 6.0f) + (delta / 2.0f)) / delta;
                float deltaG = (((max - color.Y) / 6.0f) + (delta / 2.0f)) / delta;
                float deltaB = (((max - color.Z) / 6.0f) + (delta / 2.0f)) / delta;

                if (color.X == max)
                {
                    h = deltaB - deltaG;
                }
                else if (color.Y == max)
                {
                    h = 1.0f / 3.0f + deltaR - deltaB;
                }
                else if (color.Z == max)
                {
                    h = 2.0f / 3.0f + deltaG - deltaR;
                }

                if (h < 0)
                {
                    h += 1;
                }
                if (h > 1)
                {
                    h -= 1;
                }
            }
            return new Vector3(h, s, l);
        }

        public static Vector3 HSLtoRGB(Vector3 color)
        {
            float r, g, b;
            if (color.Y == 0)                       
            {
                r = color.Z;                      
                g = color.Z;
                b = color.Z;
            }
            else
            {
                float q, p;

                if (color.Z < 0.5)
                {
                    p = color.Z * (1 + color.Y);
                }
                else
                {
                    p = (color.Z + color.Y) - (color.Y * color.Z);
                }

                q = 2 * color.Z - p;

                r = HueToRGB(q, p, color.X + (1.0f / 3.0f));
                g = HueToRGB(q, p, color.X);
                b = HueToRGB(q, p, color.X - (1.0f / 3.0f));
            }
            return new Vector3(r, g, b);
        }

        public static float HueToRGB(float q, float p, float h)
        {
            if (h < 0) h += 1;
            if (h > 1) h -= 1;

            if ((6.0f * h) < 1) return (q + (p - q) * 6.0f * h);
            if ((2.0f * h) < 1) return (p);
            if ((3.0f * h) < 2) return (q + (p - q) * ((2.0f / 3.0f) - h) * 6.0f);
            
            return (q);
        }
    }
}
