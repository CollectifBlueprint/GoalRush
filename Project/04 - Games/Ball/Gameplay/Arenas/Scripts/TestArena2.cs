using System;
using System.Collections.Generic;
using Ball.Gameplay.Arenas;
using FarseerPhysics.Factories;
using LBE.Physics;
using Microsoft.Xna.Framework;
using LBE;
using LBE.Gameplay;
using LBE.Assets;
using Ball;
using FarseerPhysics.Dynamics;
using Ball.Physics;

namespace Content.Arenas
{
    public class TestArena2 : ArenaScript
    {
        public override void OnInitGeometry()
        {
            Engine.World.EventManager.AddListener((int)EventId.HalfTimeTransition, OnHalfTimeTransition);
            
            var rbCmp = Arena.Owner.RigidBodyCmp;

            float width = ConvertUnits.ToSimUnits(Arena.ArenaHalfSize.X);
            float height = ConvertUnits.ToSimUnits(Arena.ArenaHalfSize.Y);
            float goalWidth = ConvertUnits.ToSimUnits(100);
            float goalHeight = ConvertUnits.ToSimUnits(80);

            //Obstacle
            FixtureFactory.AttachCircle(
                ConvertUnits.ToSimUnits(40),
                0,
                rbCmp.Body,
                ConvertUnits.ToSimUnits(new Vector2(-300, 100)));

            //Obstacle
            FixtureFactory.AttachCircle(
                ConvertUnits.ToSimUnits(40),
                0,
                rbCmp.Body,
                ConvertUnits.ToSimUnits(new Vector2(-300, -100)));

            //Obstacle
            FixtureFactory.AttachCircle(
                ConvertUnits.ToSimUnits(40),
                0,
                rbCmp.Body,
                ConvertUnits.ToSimUnits(new Vector2(300, 100)));

            //Obstacle
            FixtureFactory.AttachCircle(
                ConvertUnits.ToSimUnits(40),
                0,
                rbCmp.Body,
                ConvertUnits.ToSimUnits(new Vector2(300, -100)));

            rbCmp.Body.CollidesWith = (Category)CollisionType.All | (Category)CollisionType.Ball;
            rbCmp.Body.CollisionCategories = (Category)CollisionType.Wall;

        }

        public override void OnUpdate()
        {
        }

        public void OnHalfTimeTransition(object arg)
        {
            if (Arena.Overlay != null && Arena.Overlay != null)
            {
                Arena.Overlay.Visible = false;
                Arena.OverlayAlt.Visible = true;
            }
        }
    }
}
