using Microsoft.Xna.Framework;

namespace LBE.Debug
{
    public class DebugBrush
    {
        public Color TextColor { get; set; }

        public Color LineColor { get; set; }
        public float LineAlpha
        {
            get { return LineColor.A; }
            set { LineColor = new Color(LineColor, value); }
        }

        public Color SurfaceColor { get; set; }
        public float SurfaceAlpha
        {
            get { return SurfaceColor.A; }
            set { SurfaceColor = new Color(SurfaceColor, value); }
        }

        public bool DrawWireframe { get; set; }
        public bool DrawSurface { get; set; }
    }
}