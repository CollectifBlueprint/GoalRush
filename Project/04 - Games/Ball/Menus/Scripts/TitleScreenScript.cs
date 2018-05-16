using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ball.Menus;
using LBE;
using LBE.Graphics.Sprites;
using Microsoft.Xna.Framework;
using LBE.Audio;

namespace Ball.MainMenu.Scripts
{
    public class TitleScreenScript : MenuScript
    {
        float m_startTime;
        SpriteComponent m_startPulseCmp;

        public override void Start()
        {
            var backgroundCmp = new SpriteComponent(Sprite.CreateFromTexture("Graphics/titleBackground.png"), "MenuBackground");
            Menu.Owner.Attach(backgroundCmp);

            var titleSpriteCmp = new SpriteComponent(Sprite.CreateFromTexture("Graphics/title.png"), "Menu");
            Menu.Owner.Attach(titleSpriteCmp);

            var startCmp = new SpriteComponent(Sprite.CreateFromTexture("Graphics/pressStartOff.png"), "Menu");
            startCmp.Position = new Vector2(0, -200);
            Menu.Owner.Attach(startCmp);

            m_startPulseCmp = new SpriteComponent(Sprite.CreateFromTexture("Graphics/pressStartOn.png"), "Menu");
            m_startPulseCmp.Position = new Vector2(0, -200);
            Menu.Owner.Attach(m_startPulseCmp);

            m_startTime = Engine.RealTime.TimeMS;

            Game.GameMusic.PlayMenuMusic();
            AudioStreamPlaylist playlist = new AudioStreamPlaylist();
            //playlist.Add(Game.GameMusic.Tracks["TitleStream"]);
            //playlist.Add(Game.GameMusic.Tracks["MenuStream"]);
            Engine.AudioStreamManager.Playlist = playlist;
            Engine.AudioStreamManager.Playlist.Play();
        }

        public override void Update()
        {
            float minAlpha = 0.4f;
            float pulseSpeed = Engine.Debug.EditSingle("TitlePulseSpeed", 0.0018f);
            float alpha = 0.5f - 0.5f * (float)Math.Cos((Engine.RealTime.TimeMS - m_startTime) * pulseSpeed);
            m_startPulseCmp.Sprite.Alpha = LBE.MathHelper.Clamp(minAlpha, 1, alpha);

            foreach (var ctrl in Game.MenuManager.Controllers)
            {
                if (ctrl.StartCtrl.KeyPressed() || ctrl.ValidCtrl.KeyPressed())
                {
                    var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/MainMenu.lua::Menu");
                    Game.MenuManager.StartMenu(menuDef);
                }
            }
        }
    }
}
