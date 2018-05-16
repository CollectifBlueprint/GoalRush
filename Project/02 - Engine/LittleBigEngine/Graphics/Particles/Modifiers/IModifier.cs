using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBE.Graphics.Particles.Modifiers
{
    public interface IModifier
    {
        void Modify(Particle[] particles, int from, int to, float dt);
    }
}
