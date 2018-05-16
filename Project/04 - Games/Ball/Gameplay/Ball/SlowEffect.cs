using Ball.Gameplay.BallEffects;
using LBE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ball.Gameplay.BallEffects
{
    class SlowEffect : BallEffect
    {
        public override void Start()
        {
            if (Ball.LastPlayer == null)
            {
                Cancel();
                return;
            }
        }

        public override void Update()
        {
            Ball.BodyCmp.Body.LinearVelocity /= 10.0f;
        }

        public override void End()
        {
           
        }

        public override void OnPlayerTouchBall(Player player)
        {
            //Cancel();
        }

        public override void OnPlayerTakeBall(Player player)
        {
            Cancel();
        }
    }
}
