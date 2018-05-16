using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Assets;
using Microsoft.Xna.Framework;

namespace Ball.Gameplay.BallEffects
{
    public class PushParameters
    {
        public float Strenght = 1000;
    }

    public class PushEffect : BallEffect
    {
        Asset<PushParameters> m_params;
        public PushParameters Parameters
        {
            get { return m_params.Content; }
        }

        Vector2 m_ballDirection;

        public override void Start()
        {
            m_params = new Asset<PushParameters>(new PushParameters());
        }

        public override void Update()
        {
            m_ballDirection = Ball.BodyCmp.Body.LinearVelocity;
            m_ballDirection.Normalize();
        }

        public override void OnPlayerTakeBall(Player player)
        {
            if (m_ballDirection != Vector2.Zero)
                player.Owner.RigidBodyCmp.Body.ApplyLinearImpulse(m_ballDirection * Parameters.Strenght);

            Cancel();
        }
    }
}
