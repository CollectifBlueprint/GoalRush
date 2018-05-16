using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using LBE;

namespace Ball.Gameplay.Navigation
{
    public class NavigationCell
    {
        public Point Index;
        public Vector2 Position;

        public bool CanShootLeft;
        public float CanShootLeftValue;

        public bool CanShootRight;
        public float CanShootRightValue;

        public bool[] CanSeePlayer;

        public NavigationCell[] Neighbours;
        public bool[] CanNavigateToNeighbour;
    }
}
