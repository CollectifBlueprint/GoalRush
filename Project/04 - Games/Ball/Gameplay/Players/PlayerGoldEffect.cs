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

    public class PlayerGoldParameters
    {
        public ColorScheme ColorScheme;
    }
    
    public class PlayerGoldEffect : PlayerEffect
    {
        PlayerGoldParameters m_parameters;
        public PlayerGoldParameters Parameters
        {
            get { return m_parameters; }
            set { m_parameters = value; }
        }
      
        public override void Start()
        {
            Player.GoldFxSpriteCmp.Visible = true;
            Player.GoldFxSpriteCmp.Sprite.SetAnimation("GoldFx");
            Player.GoldFxSpriteCmp.Sprite.Playing = true;

            return;

            Player.SetColor(Player.ColorElement.Body, m_parameters.ColorScheme.Color1);
            Player.SetColor(Player.ColorElement.Body2, m_parameters.ColorScheme.Color2);
            Player.ResetColors();
        }

        public override void Update()
        {
        }

        public override void End()
        {

        }

    }
}
