using System;
using System.Collections.Generic;
using Ball.Gameplay.Arenas;
using FarseerPhysics.Factories;
using LBE.Physics;
using Microsoft.Xna.Framework;
using LBE;
using FarseerPhysics.Dynamics;
using Ball.Physics;
using LBE.Assets;
using LBE.Gameplay;
using Ball.Gameplay.Arenas.Objects;
using FarseerPhysics.Common;
using Ball;

namespace Content.Arenas
{
    public class ArenaLarge : ArenaScript
    {
        public override void OnInitGeometry()
        {
            var rbCmp = Arena.Owner.RigidBodyCmp;

            float width = ConvertUnits.ToSimUnits(Arena.ArenaHalfSize.X);
            float height = ConvertUnits.ToSimUnits(Arena.ArenaHalfSize.Y);
            float goalWidth = ConvertUnits.ToSimUnits(100);
            float goalHeight = ConvertUnits.ToSimUnits(80);

            //Obstacle
            FixtureFactory.AttachCircle(
                ConvertUnits.ToSimUnits(75),
                0,
                rbCmp.Body,
                ConvertUnits.ToSimUnits(new Vector2(-450, 0)));

            //Obstacle
            FixtureFactory.AttachCircle(
                ConvertUnits.ToSimUnits(75),
                0,
                rbCmp.Body,
                ConvertUnits.ToSimUnits(new Vector2(450, 0)));

            rbCmp.Body.CollidesWith = (Category)CollisionType.All | (Category)CollisionType.Ball;
            rbCmp.Body.CollisionCategories = (Category)CollisionType.Wall;

            Engine.World.EventManager.AddListener((int)EventId.HalfTimeTransition, OnHalfTimeTransition);
           
            // //Obstacle
            // FixtureFactory.AttachCircle(
                // ConvertUnits.ToSimUnits(40),
                // 0,
                // rbCmp.Body,
                // ConvertUnits.ToSimUnits(new Vector2(-300, -100)));

            // //Obstacle
            // FixtureFactory.AttachCircle(
                // ConvertUnits.ToSimUnits(40),
                // 0,
                // rbCmp.Body,
                // ConvertUnits.ToSimUnits(new Vector2(300, 100)));

            // //Obstacle
            // FixtureFactory.AttachCircle(
                // ConvertUnits.ToSimUnits(40),
                // 0,
                // rbCmp.Body,
                // ConvertUnits.ToSimUnits(new Vector2(300, -100)));
        }

        public override void OnUpdate()
        {
            Engine.Debug.Screen.Brush.DrawSurface = true;
            Engine.Debug.Screen.Brush.SurfaceColor = Color.White;
            Engine.Debug.Screen.AddCircle(new Vector2(-450, 0), 75);
            Engine.Debug.Screen.AddCircle(new Vector2(450, 0), 75);

            Asset<LDSettings> launchersConfig = Engine.AssetManager.GetAsset<LDSettings>("Arenas/ArenaLarge/Arena.lua::LDSettings");
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

