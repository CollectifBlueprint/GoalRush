using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ball.Gameplay.Arenas.Objects
{
    public enum LaunchersSelectionType
    {
        Random,
        KickOffTeam,
        Central
    }
    
    public class LDSettings
    {
        public LaunchersSelectionType LaunchersSelection; 
    }



}
