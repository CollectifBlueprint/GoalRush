using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Gameplay;

namespace Ball.Gameplay.Arenas.Objects
{
    public interface IBallLauncher
    {    
        void Select();
        void Unselect();
        void Launch();
        void Scan();
    }
}
