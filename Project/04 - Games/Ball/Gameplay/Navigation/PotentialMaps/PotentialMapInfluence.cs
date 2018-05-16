using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ball.Gameplay.Navigation.PotentialMaps
{
    public class PotentialMapInfluence
    {
        public virtual float GetValue(NavigationCell navCell)
        {
            return 0;
        }
    }

    public class PotentialMapInfluencePoint : PotentialMapInfluence
    {
        public Vector2 Position;
        public float Radius;
        public float Attenuation;

        public float Value;

        protected float GetWeight(Vector2 pos)
        {
            float distToSourceSq = Vector2.DistanceSquared(Position, pos);

            float fastDistSq = distToSourceSq / (Radius * Radius);
            float fastAttenuation = LBE.MathHelper.Clamp(0.01f, 1, Attenuation);
            float weight = 1 / fastAttenuation * (1 - fastDistSq); 
            return LBE.MathHelper.Clamp(0, 1, weight);
        }

        public override float GetValue(NavigationCell navCell)
        {
            return GetWeight(navCell.Position) * Value;
        }
    }
}
