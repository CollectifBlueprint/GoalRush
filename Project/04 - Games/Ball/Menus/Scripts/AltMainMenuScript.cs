using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ball.Menus;
using LBE;
using LBE.Graphics.Sprites;
using LBE.Audio;
using Microsoft.Xna.Framework;

namespace Ball.MainMenu.Scripts
{
    public class AltMainMenuScript : MenuScript
    {
        SpriteComponent m_careerCmp;
        SpriteComponent m_versusCmp;
        SpriteComponent m_settingsCmp;
        SpriteComponent m_quitCmp;
        
        public override void Start()
        {
            var backgroundCmp = new SpriteComponent(Sprite.CreateFromTexture("Graphics/Menu/Background.png"), "MenuBackground");
            Menu.Owner.Attach(backgroundCmp);

            var artworkCmp = new SpriteComponent(Sprite.CreateFromTexture("Graphics/Menu/Artwork.png"), "MenuBackground");
            artworkCmp.Position = new Vector2(350, 0);
            Menu.Owner.Attach(artworkCmp);

            Vector2 basePos = Engine.Debug.EditVector2("MainMenuItemPos", Vector2.Zero);
            float offset = Engine.Debug.EditSingle("MainMenuItemOffset", 40);

            Game.GameMusic.PlayMenuMusic();
        }

        public override void Update()
        {

        }

        public override void OnItemValid(string name, MenuController controller)
        {
            base.OnItemValid(name, controller);

            if (name == "Play")
            {
                Engine.Log.Write("Proto, Go!");

                var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/SelectTeamMenu.lua::Menu");
                Game.MenuManager.StartMenu(menuDef);
                Game.GameSession.CurrentMatchInfo = new Gameplay.MatchStartInfo();
                var menuScript = (SelectTeamMenuScript)Game.MenuManager.CurrentMenu.Script;
                menuScript.AddController(controller);
            }

            if (name == "Controls")
            {
                Engine.Log.Write("Controls!");

                var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/ControlsMenu.lua::Menu");
                Game.MenuManager.StartMenu(menuDef);
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

                Engine.Application.ExitGame();
            }
        }
    }
}
