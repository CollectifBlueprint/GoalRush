using Microsoft.Xna.Framework;

namespace LBE.Hex
{
    public struct Hex
    {
        public int U,V;

        public Hex(int u, int v)
        {
            U = u;
            V = v;
        }

        public Vector2 ToVector()
        {
            return HexUtils.HexToVector(this);
        }

        public Vector2 ToVector(float scale)
        {
            return HexUtils.HexToVector(this);
        }
    }
}