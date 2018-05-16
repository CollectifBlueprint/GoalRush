using System;
using System.Collections.Generic;
using System.Text;
using LBE.Debug;

namespace LBE.Debug
{
    public class ToggleRenderDebug : ICommand
    {
        public void Execute()
        {
            Engine.Debug.Flags.RenderDebug = !Engine.Debug.Flags.RenderDebug; 
        }
    }
}
