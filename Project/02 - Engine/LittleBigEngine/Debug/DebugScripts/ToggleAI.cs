using System;
using System.Collections.Generic;
using System.Text;
using LBE.Debug;

namespace LBE.Script
{
    public class ToggleAI : ICommand
    {
        public void Execute()
        {
            Engine.Debug.Flags.EnableAI = !Engine.Debug.Flags.EnableAI; 
        }
    }
}
