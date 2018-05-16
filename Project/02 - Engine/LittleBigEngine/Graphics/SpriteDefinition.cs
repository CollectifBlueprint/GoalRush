using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Drawing;

namespace LBE.Graphics.Sprites
{
    public class SpriteAnimationDefinition
    {
        public String Name = "Default";

        public Texture2D Texture;

        public Point FrameCount = new Point(1,1);
        public int StartIndex = 0;
        public int EndIndex = -1;

        public float FrameTime = 0.0f;
        public bool Loop = false;
    }

    public class SpriteDefinition
    {
        public SpriteAnimationDefinition[] Animations;
        public String DefaultAnimation = "Default";

        public Texture2D Texture;

        public float AnimationSpeed = 1.0f;
        public float Scale = 1.0f;
        public float Orientation = 0.0f;
        public Vector3 Color = new Vector3(1,1,1);
        public float Alpha = 1.0f;
    }
}
