using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE;
using Microsoft.Xna.Framework;

namespace Ball.Gameplay
{
    public class PlayerDisableMagnetEffect : PlayerEffect
    {
        public override void Start()
        {
            Player.Properties.MagnetDisabled.Set();
        }

        public override void End()
        {
            if (Player.Properties.MagnetDisabled.Value == true)
                Player.Properties.MagnetDisabled.Unset();
        }
    }
}
