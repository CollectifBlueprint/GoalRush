using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Gameplay;
using Microsoft.Xna.Framework;
using LBE;

namespace Ball.Menus
{
    public class MenuManager
    {
        MenuController[] m_controllers;
        public MenuController[] Controllers
        {
            get { return m_controllers; }
            set { m_controllers = value; }
        }

        Menu m_currentMenu;
        public Menu CurrentMenu
        {
            get { return m_currentMenu; }
        }

        public MenuManager()
        {
            m_controllers = new MenuController[5];
            m_controllers[0] = MenuController.Keyboard();
            m_controllers[1] = MenuController.Gamepad(PlayerIndex.One);
            m_controllers[2] = MenuController.Gamepad(PlayerIndex.Two);
            m_controllers[3] = MenuController.Gamepad(PlayerIndex.Three);
            m_controllers[4] = MenuController.Gamepad(PlayerIndex.Four);
        }

        public void StartMenu(MenuDefinition menuDef)
        {
            QuitMenu();

            //foreach (var camera in Engine.Renderer.Cameras)
            //    camera.Position = Vector2.Zero;

            GameObject menuRoot = new GameObject("Menu Root");
            menuRoot.Tag = "Menu";

            m_currentMenu = new Menu(menuDef);
            menuRoot.Attach(m_currentMenu);
        }

        public void QuitMenu()
        {
            if (m_currentMenu != null)
            {
                m_currentMenu.Owner.Kill();
            }
        }
    }
}
