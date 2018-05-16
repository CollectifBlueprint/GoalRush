using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBE
{
    public interface IEngineComponent
    {
        void Startup();
        void Shutdown();

        void StartFrame();
        void EndFrame();

        bool Started { get; set; }
    }
}
