using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Script;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using LBE.Physics;
using LBE.Gameplay;
using Ball.Gameplay.Arenas.Objects;

namespace Ball.Gameplay.Arenas
{
    public enum ArenaSize
    {
        Normal,
        Large,
    }

    public class ArenaList
    {
        public String[] Arenas;
    }

    public class ArenaStartInfo
    {
        public Vector2[] PlayerPosition;

        public bool UseBallLauncher;
        public Vector2 BallPosition;
    }

    public class ArenaPreview
    {
        public String Name;
        public String Description;

        public ArenaSize Size = ArenaSize.Normal;

        public Texture2D Preview;
    }

    public class ArenaDescription
    {
        public ArenaScript Script;

        // BackGround
        public Texture2D Material;
        public Texture2D AO;

        // TODO del
        public Texture2D Background;
        public Texture2D Overlay;
        public Texture2D OverlayAlt;

        public LDDefinition LD;
        public LDSettings LDSettings;

        public ArenaStartInfo FirstHalfStartInfo;
        public ArenaStartInfo SecondHalfStartInfo;

        public CollisionDefinition Geometry;

        public Vector2 Size;
        public float Scale = 1;

        public bool DefaultGeometry = false;
    }
}
