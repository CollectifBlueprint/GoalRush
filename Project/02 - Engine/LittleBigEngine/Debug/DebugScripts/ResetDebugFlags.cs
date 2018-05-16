using System;
using System.Collections.Generic;
using System.Text;
using LBE.Debug;

namespace LBE.Debug
{
    public class ResetDebugFlags : ICommand
    {
        public void Execute()
        {
            Engine.Debug.Flags = new DebugFlags();
        }
    }
}
