using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ball.Gameplay.Players
{
    public class InvincibleParameters
    {
        public float InvincibleTimeMS = 100;
    }
    
    class PlayerInvincibleEffect : PlayerEffect
    {
        InvincibleParameters m_parameters;
        public InvincibleParameters Parameters
        {
            get { return m_parameters; }
            set { m_parameters = value; }
        }
    
        public override void Start()
        {
            Player.Properties.Invincible.Set();

            if (Player.Properties.Stunned.Value == true)
            {
                foreach (var effect in Player.Effects.ToArray())
                {
                    if (effect.GetType() == typeof(PlayerStunEffect))
                    {
                        Player.RemoveEffect(effect);
                    }
                }
            }
        }

        public override void Update()
        {
        }

        public override void End()
        {
            if (Player.Properties.Invincible.Value)
               Player.Properties.Invincible.Unset();
        }
    }
}
