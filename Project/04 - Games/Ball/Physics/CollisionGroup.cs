using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ball.Physics
{
    [Flags]
    public enum CollisionType
    {
        None = 0,
        All = int.MaxValue,
        Default = 1,
        Player = 2,
        TeamA = 4,
        TeamB = 8,
        Wall = 16,
        PlayerShield = 32,
        Ball = 64,
        Cat8 = 128,
        Cat9 = 256,
        Cat10 = 512,
        Cat11 = 1024,
        Cat12 = 2048,
        Cat13 = 4096,
        Cat14 = 8192,
        Cat15 = 16384,
        Cat16 = 32768,
        Cat17 = 65536,
        Cat18 = 131072,
        Cat19 = 262144,
        Cat20 = 524288,
        Cat21 = 1048576,
        Cat22 = 2097152,
        Cat23 = 4194304,
        Cat24 = 8388608,
        Cat25 = 16777216,
        Cat26 = 33554432,
        Cat27 = 67108864,
        Cat28 = 134217728,
        Cat29 = 268435456,
        Cat30 = 536870912,
        Gameplay = 1073741824
    }
}
