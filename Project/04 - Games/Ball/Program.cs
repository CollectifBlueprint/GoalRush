#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using LBE.Core;
#endregion

namespace Ball
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        private static Application app;

        [STAThread]
        static void Main()
        {
            using (app = new Application())
            {
                app.Engine.AddComponentDelayed(new Game());
                app.Run();
            }
        }
    }
}
