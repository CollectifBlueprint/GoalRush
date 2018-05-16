using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ball.Menus;
using LBE;
using LBE.Graphics.Sprites;
using Ball.Gameplay;
using Ball.Graphics;

namespace Ball.MainMenu.Scripts
{
    public class ControlsListScript : MenuScript
    {
        SpriteComponent m_gamepadCmp;
        SpriteComponent m_keyboardCmp;

        public override void Start()
        {
            var backgroundCmp = new SpriteComponent(Sprite.CreateFromTexture("Graphics/Menu/Background.png"), "MenuBackground");
            Menu.Owner.Attach(backgroundCmp);

			m_gamepadCmp = new SpriteComponent(Sprite.CreateFromTexture("Graphics/Menu/GpControls.png"), "MenuBackground");
            Menu.Owner.Attach(m_gamepadCmp);

            m_keyboardCmp = new SpriteComponent(Sprite.CreateFromTexture("Graphics/Menu/KbControls.png"), "MenuBackground");
            Menu.Owner.Attach(m_keyboardCmp);

            m_keyboardCmp.Visible = false;
        }

        public override void OnItemValid(string name, MenuController controller)
        {
            base.OnItemValid(name, controller);

            if (name == "MainMenu")
            {
                var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/MainMenu.lua::Menu");
                Game.MenuManager.StartMenu(menuDef);
            }
        }

        public override void OnCancel(string name, MenuController controller)
        {
            base.OnCancel(name, controller);

            var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/MainMenu.lua::Menu");
            Game.MenuManager.StartMenu(menuDef);
        }

        public override void OnItemSelect(string name, MenuController controller)
        {
            if (name == "Keyboard")
            {
                m_keyboardCmp.Visible = true;
                m_gamepadCmp.Visible = false;
            }

            if (name == "Gamepad")
            {
                m_keyboardCmp.Visible = false;
                m_gamepadCmp.Visible = true;
            }
        }
    }
}
