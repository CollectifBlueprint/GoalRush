using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE;
using LBE.Assets;
using LBE.Core;
using LBE.Gameplay;
using LBE.Graphics;
using LBE.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ball.Gameplay.Navigation.PotentialMaps;
using Ball.Gameplay.Navigation;

namespace Ball.Gameplay.Players.AI
{
    public class AssistPlayerPotentialSource : PotentialMapInfluence
    {
        public Player Assistee;
        public Player Assistant;
        public override float GetValue(NavigationCell navCell)
        {
            float distToAssistFallOff = 200;
            float distToAssistFallOffSq = distToAssistFallOff * distToAssistFallOff;
            float bestDistToAssist = 250;
            float bestDistToAssistSq = bestDistToAssist * bestDistToAssist;
            float distToAssistSq = Vector2.DistanceSquared(navCell.Position, Assistee.Position);
            float fallOff = Math.Abs(distToAssistSq - bestDistToAssistSq) / distToAssistFallOffSq;
            fallOff = 1 - LBE.MathHelper.Clamp(0, 1, fallOff);
            fallOff = LBE.MathHelper.Clamp(0, 1, 2 * fallOff);

            Goal goal = null;
            bool canShoot = false;
            float shootValue = 0;
            if (Game.Arena.RightGoal.Team == Assistee.Team)
            {
                goal = Game.Arena.LeftGoal;
                canShoot = navCell.CanShootLeft;
                shootValue = navCell.CanShootLeftValue;
            }
            else
            {
                goal = Game.Arena.RightGoal;
                canShoot = navCell.CanShootRight;
                shootValue = navCell.CanShootRightValue;
            }

            bool canAssist = navCell.CanSeePlayer[(int)Assistee.PlayerIndex];
            Team otherTeam = Assistee.Team.TeamID == TeamId.TeamOne ? Game.GameManager.Teams[1] : Game.GameManager.Teams[0];
            bool assistIntercept = true;

            float shootWeight = 0.3f;
            float assistValue = 0.0f;
            if (canAssist && assistIntercept)
                assistValue += 1 - shootWeight;
            if (canAssist && canShoot && assistIntercept)
                assistValue += shootWeight * shootValue;

            Vector2 assistDir = navCell.Position - Assistee.Position;
            Vector2 goalDir = goal.Position - Assistee.Position;
            if (Vector2.Dot(assistDir, goalDir) < 0)
                assistValue *= 0.5f;

            assistValue *= fallOff;
            return assistValue;
        }
    }
}
