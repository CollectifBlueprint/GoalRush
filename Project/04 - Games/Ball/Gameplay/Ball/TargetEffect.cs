using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Assets;
using Microsoft.Xna.Framework;
using LBE;

namespace Ball.Gameplay.BallEffects
{
    public class TargetEffectParameters
    {
        public float Strength = 12;
    }

    public class TargetEffect : BallEffect
    {
        Asset<TargetEffectParameters> m_params;
        public TargetEffectParameters Parameters
        {
            get { return m_params.Content; }
        }

        float m_strength = 1.0f;
        public float Strength
        {
            get { return m_strength; }
            set { m_strength = value; }
        }

        Player m_owner;

        public TargetEffect(Player owner)
        {
            m_owner = owner;
        }

        public override void Start()
        {
            m_params = new Asset<TargetEffectParameters>(new TargetEffectParameters());
        }

        public override void Update()
        {
            if (m_owner.Input == null)
                return;

            Vector2 dir = m_owner.Input.BallCtrl.Get();
            Vector2 ballSpeedDir = Ball.BodyCmp.Body.LinearVelocity;
            ballSpeedDir.Normalize();

            Vector2 impulseDir = dir - Vector2.Dot(dir, ballSpeedDir) * ballSpeedDir;

            Ball.BodyCmp.Body.ApplyLinearImpulse(impulseDir * Parameters.Strength * m_strength);
        }

        public override void OnPlayerTouchBall(Player player)
        {
            Cancel();
        }

        public override void OnPlayerTakeBall(Player player)
        {
            Cancel();
        }
    }
}
