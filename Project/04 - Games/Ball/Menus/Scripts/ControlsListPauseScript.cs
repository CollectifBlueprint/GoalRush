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
    public class ControlsListPauseScript : MenuScript
    {
        SpriteComponent m_gamepadCmp;
        SpriteComponent m_keyboardCmp;

        ScreenFade m_screenFade;


        public override void Start()
        {
           
			m_gamepadCmp = new SpriteComponent(Sprite.CreateFromTexture("Graphics/Menu/GpControls.png"), "MenuBackground");
            Menu.Owner.Attach(m_gamepadCmp);

            m_keyboardCmp = new SpriteComponent(Sprite.CreateFromTexture("Graphics/Menu/KbControls.png"), "MenuBackground");
            Menu.Owner.Attach(m_keyboardCmp);

            m_keyboardCmp.Visible = false;

            m_screenFade = new ScreenFade();
            Menu.Owner.Attach(m_screenFade);
            
            m_screenFade.Opacity = 0.7f;
            m_screenFade.StartFade(ScreenFade.FadeType.FadeIn, 0, false);
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

            var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/PauseMenu.lua::Menu");
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

        public override void End()
        {
            Menu.Owner.Remove(m_screenFade);
        }
    }
}
