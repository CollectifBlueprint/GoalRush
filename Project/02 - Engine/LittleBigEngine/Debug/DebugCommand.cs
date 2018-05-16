using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using LBE.Debug;
using LBE.Input;

namespace LBE.Debug
{
    public interface ICommand
    {
        void Execute();
    }

    public class DebugCommand
    {
        public DebugCommandDefinition Definition;
        public KeyControl Control;
        public ICommand Command;
    }

    public class DebugCommandDefinition
    {
        public ICommand Script;
        public String Name;
        public String Description;
        public Keys Key;
    }
}
