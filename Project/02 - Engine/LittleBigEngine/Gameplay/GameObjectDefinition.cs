using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Assets;
using Microsoft.Xna.Framework;

namespace LBE.Gameplay
{    
    public class GameObjectDefinition
    {
        public String Name;
        public GameObjectComponent[] Components;
    }

    public class GameObjectInstance
    {
        public string Name;
        public GameObjectDefinition GameObjectDefinition;
        public Vector2 Position;
        public float Orientation;
    }
}
