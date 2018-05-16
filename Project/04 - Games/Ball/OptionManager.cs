using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Gameplay;
using LBE;
using Ball.Audio;

namespace Ball
{
    public class OptionManager
    {
        public OptionManager()
        {
            Init();
        }
        
        public void Init()
        {
            Engine.AudioStreamManager.MasterVolume = AudioHelper.LinearVolumeTodBVolume(Game.GameProfile.Options.MusicVolume);
            Engine.MusicManager.MasterVolume = AudioHelper.LinearVolumeTodBVolume(Game.GameProfile.Options.MusicVolume);
            Engine.Audio.MasterVolume = AudioHelper.LinearVolumeTodBVolume(Game.GameProfile.Options.SfxVolume);
            Engine.Audio.StereoWidth = 600.0f;
        }

        public void Update()
        {
            bool applyDeviceChanges = false;

            //Fullscreen
            if (Game.GameProfile.Options.FullScreen != Engine.Application.GraphicsDeviceManager.IsFullScreen)
            {
                if (Game.GameProfile.Options.FullScreen)
                    Engine.Renderer.SetFullscreen();
                else
                    Engine.Renderer.SetWindowed(Game.ConfigParameters.ResolutionX, Game.ConfigParameters.ResolutionY);
            }

            //VSync
            if (Game.GameProfile.Options.VSync != Engine.Application.GraphicsDeviceManager.SynchronizeWithVerticalRetrace)
            {
                Engine.Application.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = Game.GameProfile.Options.VSync;
                Engine.Log.Write("Vsync: " + Engine.Application.GraphicsDeviceManager.SynchronizeWithVerticalRetrace);
                applyDeviceChanges = true;
            }

            if (applyDeviceChanges)
                Engine.Application.GraphicsDeviceManager.ApplyChanges();

            //Audio
            //Engine.Audio.MasterVolume = Game.GameProfile.Options.SfxVolume;
            //Engine.MusicManager.MasterVolume = Game.GameProfile.Options.MusicVolume;
        }
    }
}
