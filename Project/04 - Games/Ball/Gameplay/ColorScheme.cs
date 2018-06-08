using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Ball.Gameplay
{
    public class ColorSchemeData
    {
        public ColorScheme[] ColorSchemes;
    }
    
    public class ColorScheme
    {
        public Color Color1;
        public Color Color2;
        public bool Dark = false;
    }
}
