using System.Collections.Generic;

namespace LBE
{
    public static class PointHelper
    {
        public static IEnumerable<Point> Neighbours(this Point p)
        {
            var nexts = new Point[8];
            nexts[0] = new Point(p.X + 1, p.Y);
            nexts[1] = new Point(p.X, p.Y + 1);
            nexts[2] = new Point(p.X - 1, p.Y);
            nexts[3] = new Point(p.X, p.Y - 1);
            nexts[4] = new Point(p.X + 1, p.Y + 1);
            nexts[5] = new Point(p.X + 1, p.Y - 1);
            nexts[6] = new Point(p.X - 1, p.Y + 1);
            nexts[7] = new Point(p.X - 1, p.Y - 1);

            return nexts;
        }
    }
}
