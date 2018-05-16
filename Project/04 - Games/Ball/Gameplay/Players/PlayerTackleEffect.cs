using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE;
using Microsoft.Xna.Framework;

namespace Ball.Gameplay
{
    public class PlayerTackleEffect : PlayerEffect
    {
        public float Strength;
        public Vector2 Direction;

        public override void Start()
        {
            Player.Owner.RigidBodyCmp.Body.ApplyLinearImpulse(Strength * Direction);

            PlayerTackleFx ghostCmp = new PlayerTackleFx();
            Player.Owner.Attach(ghostCmp);

            Player.Properties.LastTackleTimeMS = Engine.GameTime.TimeMS;
            Player.Properties.Tackling.Set();

            Engine.World.EventManager.ThrowEvent((int)EventId.PlayerDash, this);
        }
        
        public override void End()
        {
            if (Player.Properties.Tackling)
            {
                Player.Properties.Tackling.Unset();
                if(Player.Properties.Shield) Player.Properties.Shield.Unset();
            }
        }
    }
}
