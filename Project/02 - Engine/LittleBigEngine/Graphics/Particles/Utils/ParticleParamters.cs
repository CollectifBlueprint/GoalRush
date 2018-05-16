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
using System.ComponentModel;

namespace LBE.Graphics.Particles.Utils
{
    public class ParticleParameterSingleConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(float))
                return true;

            if (sourceType == typeof(double))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is float)
                return new ParticleParameterSingle() { Value = (float)value };

            if (value is double)
                return new ParticleParameterSingle() { Value = (float)(double)value };

            return base.ConvertFrom(context, culture, value);
        }
    }

    public class ParticleParameterVector2Converter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(Vector2))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is Vector2)
                return new ParticleParameterVector2() { Value = (Vector2)value };

            return base.ConvertFrom(context, culture, value);
        }
    }

    [TypeConverter(typeof(ParticleParameterSingleConverter))]
    public struct ParticleParameterSingle
    {
        public float Value;
        public float Variation;

        public float Get()
        {
            float t = Engine.Random.NextFloat(-1, 1);
            //t = t * t * Math.Sign(t); //Skew the distribution around 0
            return Value + t * Variation;
        }

        public static implicit operator ParticleParameterSingle(float value)
        {
            return new ParticleParameterSingle() { Value = value };
        }
    }

    [TypeConverter(typeof(ParticleParameterVector2Converter))]
    public struct ParticleParameterVector2
    {
        public Vector2 Value;
        public Vector2 Variation;

        public Vector2 Get()
        {
            float tx = Engine.Random.NextFloat(-1, 1);
            float ty = Engine.Random.NextFloat(-1, 1);
            //t = t * t * Math.Sign(t); //Skew the distribution around 0
            return Value + new Vector2(tx, ty) * Variation;
        }

        public static implicit operator ParticleParameterVector2(Vector2 value)
        {
            return new ParticleParameterVector2() { Value = value };
        }
    }

    public struct ModifierRangeSingle
    {
        public float MinValue;
        public float MaxValue;

        public float MinTime;
        public float MaxTime;

        public float Get(float t)
        {
            return MinValue + (MaxValue - MinValue) * LBE.MathHelper.LinearStep(MinTime, MaxTime, t);
        }
    }

    public struct ModifierRangeVector2
    {
        public Vector2 MinValue;
        public Vector2 MaxValue;

        public float MinTime;
        public float MaxTime;

        public Vector2 Get(float t)
        {
            return MinValue + (MaxValue - MinValue) * LBE.MathHelper.LinearStep(MinTime, MaxTime, t);
        }
    }
}
