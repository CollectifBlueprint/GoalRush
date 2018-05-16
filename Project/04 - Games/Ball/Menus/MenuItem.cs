using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using LBE.Graphics.Sprites;

namespace Ball.Menus
{
    public enum MenuItemType
    {
        Button,
        Toggle,
        Slider,
    }

    public class MenuItemDefinition
    {
        public String Name;
        public String Text;
        public bool Enabled = true;

        public MenuItemType Type = MenuItemType.Button;
    }

    public class MenuItem
    {
        public String Name;
        public Vector2 Position;
        public String Text;
        public MenuItemType Type;

        public bool Enabled;

        public bool ToggleState;
        public int SliderState = 4;
        public int SliderCount = 8;

        public TextComponent TextComponent;

        public TextComponent[] SliderComponents;

        public TextComponent ToggleOnTextComponent;
        public TextComponent ToggleOffTextComponent;
    }
}
