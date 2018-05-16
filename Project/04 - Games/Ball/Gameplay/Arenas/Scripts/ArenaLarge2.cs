using Ball.Gameplay.Arenas;
using FarseerPhysics.Factories;
using LBE.Physics;
using Microsoft.Xna.Framework;
using LBE.Graphics.Sprites;
using LBE;
using System;
using LBE.Gameplay;
using FarseerPhysics.Dynamics;
using Ball.Gameplay;
using Ball.Physics;
using FarseerPhysics.Common;


namespace Content.Arenas
{
    public class ArenaLarge2 : ArenaScript
    {
       
        public override void OnInitGeometry()
        {
            var rbCmp = Arena.Owner.RigidBodyCmp;

            float width = ConvertUnits.ToSimUnits(Arena.ArenaHalfSize.X);
            float height = ConvertUnits.ToSimUnits(Arena.ArenaHalfSize.Y);
            float goalWidth = ConvertUnits.ToSimUnits(100);
            float goalHeight = ConvertUnits.ToSimUnits(80);

            rbCmp.Body.CollidesWith = (Category)CollisionType.All | (Category)CollisionType.Ball;
            rbCmp.Body.CollisionCategories = (Category)CollisionType.Wall;
        }

        public override void OnUpdate()
        {
        }
    }
}
