using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LBE.Core
{
    public struct Transform
    {
        public Vector2 Position;
        public float Orientation;

        public bool Absolute;

        public Transform(Vector2 position)
        {
            Position = position;
            Orientation = 0;
            Absolute = false;
        }

        public Transform(Vector2 position, float orientation)
        {
            Position = position;
            Orientation = orientation;
            Absolute = false;
        }
        
        public Transform(Vector2 position, bool absolute)
        {
            Position = position;
            Orientation = 0;
            Absolute = absolute;
        }

        public Transform(Vector2 position, float orientation, bool absolute)
        {
            Position = position;
            Orientation = orientation;
            Absolute = absolute;
        }

        public Transform Compose(Transform child)
        {
            if (child.Absolute)
                return child;

            Transform result = new Transform();
            result.Position = Position + child.Position.Rotate(Orientation);
            result.Orientation = Orientation + child.Orientation;
            return result;
        }

        public static implicit operator Transform(Vector2 position)
        {
            return new Transform(position);
        }
    }
}
