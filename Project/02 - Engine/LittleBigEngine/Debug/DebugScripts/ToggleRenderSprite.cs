using System;
using System.Collections.Generic;
using System.Text;
using LBE.Debug;

namespace LBE.Debug
{
    public class ToggleRenderSprite : ICommand
    {
        public void Execute()
        {
            Engine.Debug.Flags.RenderSprites = !Engine.Debug.Flags.RenderSprites; 
        }
    }
}
