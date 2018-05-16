using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ball.Menus;
using LBE;
using LBE.Graphics.Sprites;

namespace Ball.MainMenu.Scripts
{
    public class MatchMenuScript : MenuScript
    {
        public override void Start()
        {
            var backgroundCmp = new SpriteComponent(Sprite.CreateFromTexture("Graphics/titleBackground.png"), "Background");
            Menu.Owner.Attach(backgroundCmp);

            Game.GameMusic.PlayMenuMusic();
        }

        public override void OnItemValid(string name, MenuController controller)
        {
            base.OnItemValid(name, controller);
        }
    }
}
