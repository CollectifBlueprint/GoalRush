using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ball.Gameplay.Navigation.PotentialMaps;
using Ball.Gameplay.Navigation;

namespace Ball.Gameplay.Players.AI
{
    public class CanShootLeftPotentialSource : PotentialMapInfluence
    {
        public override float GetValue(NavigationCell navCell)
        {
            float acc = navCell.CanShootLeftValue;
            float accCount = 1;
            int iNeigh = 0;
            foreach (var nextCell in navCell.Neighbours)
            {
                if (navCell.CanNavigateToNeighbour[iNeigh])
                {
                    acc += nextCell.CanShootLeftValue;
                    accCount++;
                }
                iNeigh++;
            }
            return acc / accCount;
        }
    }

    public class CanShootRightPotentialSource : PotentialMapInfluence
    {
        public override float GetValue(NavigationCell navCell)
        {
            float acc = navCell.CanShootRightValue;
            float accCount = 1;
            int iNeigh = 0;
            foreach (var nextCell in navCell.Neighbours)
            {
                if (navCell.CanNavigateToNeighbour[iNeigh])
                {
                    acc += nextCell.CanShootRightValue;
                    accCount++;
                }
                iNeigh++;
            }
            return acc / accCount;
        }
    } 
}
