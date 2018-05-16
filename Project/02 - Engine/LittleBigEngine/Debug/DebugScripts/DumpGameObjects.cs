using System;
using System.Collections.Generic;
using System.Text;
using LBE.Debug;

namespace LBE.Script
{
    public class DumpGameObjects : ICommand
    {
        public void Execute()
        {
            Engine.Log.Write(
                String.Format("There is {0} game objects in the Engine.World", Engine.World.GameObjects.Count));
            foreach (var go in Engine.World.GameObjects)
            {
                Engine.Log.Write(
                    String.Format("*{0}[{1}]", go.Tag, go.Name));
                foreach (var cmp in go.Components)
                {
                    Engine.Log.Write(
                        String.Format(" {0}[{1}]", cmp.GetType().Name, cmp.Name));
                }
            }
        }
    }
}
