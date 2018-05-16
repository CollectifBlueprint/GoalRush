using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LBE.Input
{
    public interface IInput2D
    {
        Vector2 Value { get; }
        Vector2 PreviousValue { get; }
    }
}
