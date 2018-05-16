using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ball.Menus;
using LBE;
using LBE.Graphics.Sprites;

namespace Ball.MainMenu.Scripts
{
    public class PlayMenuScript : MenuScript
    {
        Timer m_delayTimer;

        public override void Start()
        {
            var backgroundCmp = new SpriteComponent(Sprite.CreateFromTexture("Graphics/Menu/Background.png"), "MenuBackground");
            Menu.Owner.Attach(backgroundCmp);

            m_delayTimer = new Timer(Engine.GameTime, 100);
            m_delayTimer.Start();

            Game.GameMusic.PlayMenuMusic();
        }

        public override void OnItemValid(string name, MenuController controller)
        {
            base.OnItemValid(name, controller);

            if (m_delayTimer.Active)
                return;

            Game.MenuManager.QuitMenu();
            Game.GameManager.StartMatch(Game.GameSession.CurrentMatchInfo);
        }

        public override void OnCancel(string name, MenuController controller)
        {
            base.OnCancel(name, controller);

            var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/SelectMapMenu.lua::Menu");
            Game.MenuManager.StartMenu(menuDef);
        }
    }
}
