using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Graphics.Sprites;

namespace Ball.Map
{
    public class RewardEffect
    {
        public virtual void ApplyReward() { }
    }
     
    public class Reward
    {
        public String Name;
        public String Description;
        public Sprite Icon;

        public String PlayerBonus;
        public RewardEffect Effect;
    }
}
