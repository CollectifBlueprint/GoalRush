using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Gameplay;
using LBE;
using Microsoft.Xna.Framework;
using LBE.Assets;
using LBE.Core;
using LBE.Graphics.Sprites;

namespace Ball.Gameplay.Arenas.Objects
{
    public class LauncherComponent : GameObjectComponent, IBallLauncher
    {
        protected Transform m_transform;
        public Transform Transform
        {
            get { return m_transform; }
            set { m_transform = value; }
        }

        protected Vector2 m_ballSpawnOffset = Vector2.Zero;
        protected Vector2 m_direction = Vector2.UnitX;


        public LauncherComponent()
        {
        }

        public override void Start()
        {
            Game.Arena.Launchers.Add(this);
        }

        public override void Update()
        {
            Transform parent = new Transform(Owner.Position, Owner.Orientation);
            Transform world = parent.Compose(m_transform);

            Engine.Debug.Screen.ResetBrush();
            Engine.Debug.Screen.Brush.DrawSurface = false;
            Engine.Debug.Screen.AddSquare(world.Position, 8);
            Engine.Debug.Screen.AddSquare(world.Position + m_ballSpawnOffset.Rotate(world.Orientation), 4);
            Engine.Debug.Screen.AddArrow(world.Position, world.Position + m_direction.Rotate(world.Orientation) * 24);
        }

        public override void End()
        {
            Game.Arena.Launchers.Remove(this);
        }


        public virtual void Select()
        {
            SpriteComponent sprite = Owner.FindComponent<SpriteComponent>("LauncherSprite");
            if (sprite == null)
                return;

            sprite.Sprite.Playing = true;
            sprite.Sprite.SetAnimation("Selected");
        }

        public virtual void Unselect()
        {
            SpriteComponent sprite = Owner.FindComponent<SpriteComponent>("LauncherSprite");
            if (sprite == null)
                return;

            sprite.Sprite.Playing = true;
            sprite.Sprite.SetAnimation("Normal");
        }

        public virtual void Launch()
        {
            
        }

        public virtual void Scan()
        {
        }

        public Transform GetWorldTransform()
        {
            Transform parent = new Transform(Owner.Position, Owner.Orientation);
            Transform world = parent.Compose(m_transform);
            return world;
        }

        public override void Enable(bool value)
        { 
            if (value == false)
                Unselect();
        }

    }
}
