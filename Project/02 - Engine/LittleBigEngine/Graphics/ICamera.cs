using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LBE.Graphics.Camera
{
    public interface ICamera
    {
        int Width { get; }
        int Height { get; }

        Matrix View { get; }
        Matrix Projection { get; }

        Point WorldToScreen(Vector2 world);
        Vector2 ScreenToWorld(Point screen);
    }
}
