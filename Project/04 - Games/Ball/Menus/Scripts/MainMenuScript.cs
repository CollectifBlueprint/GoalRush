using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ball.Menus;
using LBE;
using LBE.Graphics.Sprites;
using LBE.Audio;

namespace Ball.MainMenu.Scripts
{
    public class MainMenuScript : MenuScript
    {
        public override void Start()
        {
            var backgroundCmp = new SpriteComponent(Sprite.CreateFromTexture("Graphics/titleBackground.png"), "MenuBackground");
            Menu.Owner.Attach(backgroundCmp);

            Game.GameMusic.PlayMenuMusic();
        }

        public override void OnItemValid(string name, MenuController controller)
        {
            base.OnItemValid(name, controller);

            if (name == "Play")
            {
                Engine.Log.Write("Proto, Go!");

                var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/SelectTeamMenu.lua::Menu");
                Game.MenuManager.StartMenu(menuDef);

                var menuScript = (SelectTeamMenuScript)Game.MenuManager.CurrentMenu.Script;
                menuScript.AddController(controller);
            }

            if (name == "Options")
            {
                Engine.Log.Write("Proto, Options!");

                var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/OptionMenu.lua::Menu");
                Game.MenuManager.StartMenu(menuDef);
            }

            if (name == "Quit")
            {
				Engine.Log.Write("Quit");
                Engine.Application.Exit();
            }
        }
    }
}
