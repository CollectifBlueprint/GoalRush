using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ball.Gameplay
{
    public class PlayerEffect : GameplayEffect
    {
        Player m_player;
        public Player Player
        {
            get { return m_player; }
        }

        public void Init(Player player)
        {
            m_player = player;
            Active = true;
        }

        public virtual void OnPlayerTouchBall(Ball ball)
        {
        }

        public virtual void OnPlayerTakeBall(Ball ball)
        {
        }
    }
}
