using System;
using System.Collections.Generic;
using System.Text;
using LBE.Debug;

namespace LBE.Debug
{
    public class TogglePhysicsDebug : ICommand
    {
        public void Execute()
        {
            Engine.Debug.Flags.ShowPhysics = !Engine.Debug.Flags.ShowPhysics; 
        }
    }
}
