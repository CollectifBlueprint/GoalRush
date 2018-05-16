using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE;
using Microsoft.Xna.Framework;
using LBE.Graphics.Sprites;

namespace Ball.Gameplay
{
    public class PlayerBashEffect : PlayerEffect
    {
        public float Strength;
        public Vector2 Direction;

        static List<SpriteComponent> m_sprites = new List<SpriteComponent>();

        public override void Start()
        {
            AddMark();

            Player.Stun();
            Player.Owner.RigidBodyCmp.Body.ApplyLinearImpulse(Strength * Direction);
            Player.Properties.Bashed.Set();            
        }

        private void AddMark()
        {
            var markColor = Player.PlayerColors[2];
            markColor = Color.Lerp(markColor, Color.White, 0.35f);

            var sprite = Sprite.Create("Graphics/Marks/Marks-assets/MarkSprites.lua::Dash");
            SpriteComponent spriteCmp = new SpriteComponent(sprite, "GroundOverlay3");
            spriteCmp.Orientation = -LBE.MathHelper.Angle(Vector2.UnitY, Direction);
            spriteCmp.Position = Player.Position;
            sprite.Color = markColor;
            sprite.AnimationIndex = Engine.Random.Next(0, 4);

            float decay = 0.93f;
            float minAlpha = 0.01f;
            foreach (var cmp in m_sprites)
                cmp.Sprite.Alpha = minAlpha + (cmp.Sprite.Alpha - minAlpha) * decay;

            m_sprites.Add(spriteCmp);

            Game.GameManager.Arena.Owner.Attach(spriteCmp);
        }

        public override void End()
        {
            if (Player.Properties.Bashed.Value == true)
                Player.Properties.Bashed.Unset();
        }
    }
}
