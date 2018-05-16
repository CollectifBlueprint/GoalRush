using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE;
using Microsoft.Xna.Framework;
using Ball.Gameplay.Players;
using LBE.Assets;

namespace Ball.Gameplay
{

    public class PlayerShieldUpParameters
    {
        public ColorScheme ColorScheme;
    }

    public class PlayerShieldUpEffect : PlayerEffect
    {
        public override void Start()
        {
            Player.ShieldUpFxSpriteCmp.Visible = true;
            Player.ShieldUpFxSpriteCmp.Sprite.SetAnimation("ShieldUp");
            Player.ShieldUpFxSpriteCmp.Sprite.Playing = true;

            return;
        }

        public override void Update()
        {
        }

        public override void End()
        {

        }

    }
}
