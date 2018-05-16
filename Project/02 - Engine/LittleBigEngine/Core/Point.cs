using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

public struct Point
{
    public int X;
    public int Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static implicit operator Vector2(Point p)
    {
        return new Vector2(p.X, p.Y);
    }

    public static implicit operator Point(Vector2 v)
    {
        return new Point((int)v.X, (int)v.Y);
    }

    public static bool operator ==(Point p1, Point p2)
    {
        return p1.X == p2.X && p1.Y == p2.Y;
    }

    public static bool operator !=(Point p1, Point p2)
    {
        return !(p1 == p2);
    }

    public override bool Equals(object obj)
    {
        return (Point)obj == this;
    }

    public override int GetHashCode()
    {
        int hash = 1367;
        return X * hash - Y;
    }

    public static Point Zero
    {
        get { return new Point(); }
    }
}
