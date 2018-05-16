using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ball.Menus;
using LBE;
using LBE.Graphics.Sprites;
using Ball.Audio;

namespace Ball.MainMenu.Scripts
{
    public class OptionMenuScript : MenuScript
    {
        public override void Start()
        {
            var backgroundCmp = new SpriteComponent(Sprite.CreateFromTexture("Graphics/Menu/Background.png"), "MenuBackground");
            Menu.Owner.Attach(backgroundCmp);

            Menu.Items["Fullscreen"].ToggleState = Game.GameProfile.Options.FullScreen;
            Menu.Items["VSync"].ToggleState = Game.GameProfile.Options.VSync;
            Menu.Items["SFX"].SliderState = (int)(Game.GameProfile.Options.SfxVolume * Menu.Items["SFX"].SliderCount);
            Menu.Items["Music"].SliderState = (int)(Game.GameProfile.Options.MusicVolume * Menu.Items["Music"].SliderCount);
        }

        public override void OnItemValid(string name, MenuController controller)
        {
            base.OnItemValid(name, controller);

            if (name == "Accept")
            {
                var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/MainMenu.lua::Menu");
                Game.MenuManager.StartMenu(menuDef);

                Game.GameProfile.CommitChanges();
            }

            if (name == "Reset")
            {
                Game.GameProfile.ResetStats();
                Game.GameProfile.CommitChanges();

                Game.GameMusic.StopAll();
                var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/TitleScreenMenu.lua::Menu");
                Game.MenuManager.StartMenu(menuDef);
            }
        }

        public override void OnItemToggle(string name, MenuController controller)
        {
            base.OnItemToggle(name, controller);
            
            if (name == "Fullscreen")
            {
                Game.GameProfile.Options.FullScreen = !Game.GameProfile.Options.FullScreen;
                Menu.SetToggle(Menu.SelectedItem, Game.GameProfile.Options.FullScreen);
            }

            if (name == "VSync")
            {
                Game.GameProfile.Options.VSync = !Game.GameProfile.Options.VSync;
                Menu.SetToggle(Menu.SelectedItem, Game.GameProfile.Options.VSync);
            }           
        }

        public override void OnItemSlider(string name, MenuController controller)
        {
            base.OnItemSlider(name, controller);
                
            if (name == "SFX")
            {
                Game.GameProfile.Options.SfxVolume = (float)Menu.Items["SFX"].SliderState / (float)Menu.Items["SFX"].SliderCount;
                Engine.Audio.MasterVolume = (float)Math.Pow((float)Game.GameProfile.Options.SfxVolume,2.0f);
                //Engine.Audio.MasterVolume = AudioHelper.LinearVolumeTodBVolume(Game.GameProfile.Options.SfxVolume);
            }

            if (name == "Music")
            {
                Game.GameProfile.Options.MusicVolume = (float)Menu.Items["Music"].SliderState / (float)Menu.Items["Music"].SliderCount;
                Engine.MusicManager.MasterVolume = (float)Math.Pow(Game.GameProfile.Options.MusicVolume, 2.0f);
                
                Engine.AudioStreamManager.MasterVolume = AudioHelper.LinearVolumeTodBVolume(Game.GameProfile.Options.MusicVolume);
                //Game.GameMusic.Tracks["MenuStream"].Volume = Engine.AudioStreamManager.MasterVolume;
            }
        }

        public override void OnCancel(string name, MenuController controller)
        {
            base.OnCancel(name, controller);

            var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/MainMenu.lua::Menu");
            Game.MenuManager.StartMenu(menuDef);
        }
    }
}
