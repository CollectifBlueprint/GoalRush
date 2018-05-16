using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBE.Log
{
    public class ConsoleLogOutput : ILogOutput
    {
        public void Write(string msg)
        {
            String indent = "".PadLeft(Engine.Log.IndentLevel * 2);
            System.Console.WriteLine(indent + msg);
        }

        public void Error(string msg)
        {
            String indent = "".PadLeft(Engine.Log.IndentLevel * 2);
            System.Console.WriteLine(indent + msg);
        }
    }
}
