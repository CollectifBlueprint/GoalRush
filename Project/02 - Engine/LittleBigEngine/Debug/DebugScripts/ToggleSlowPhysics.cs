using System;
using System.Collections.Generic;
using System.Text;
using LBE.Debug;

namespace LBE.Debug
{
    public class ToggleSlowPhysics : ICommand
    {
        public void Execute()
        {
            Engine.Debug.Flags.SlowPhysics = !Engine.Debug.Flags.SlowPhysics; 
        }
    }
}
