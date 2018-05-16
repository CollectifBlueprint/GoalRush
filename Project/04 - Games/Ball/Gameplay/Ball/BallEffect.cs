using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using LBE;

namespace Ball.Gameplay.BallEffects
{
    public class BallEffect : GameplayEffect
    {
        Ball m_ball;
        public Ball Ball
        {
            get { return m_ball; }
        }

        public void Init(Ball ball)
        {
            m_ball = ball;
            Active = true;
        }

        public virtual void OnPlayerTouchBall(Player player)
        {
        }

        public virtual void OnPlayerTakeBall(Player player)
        {
        }

        public virtual void OnWallCollision(Fixture wall)
        {
        }
    }
}
