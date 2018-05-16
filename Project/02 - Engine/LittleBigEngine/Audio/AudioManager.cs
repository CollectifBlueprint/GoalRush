using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;


namespace LBE.Audio
{
    public class AudioManager : BaseEngineComponent
    {
        float m_masterVolume;
        public float MasterVolume
        {
            set { m_masterVolume = value; }
            get { return m_masterVolume; }
        }

        float m_stereoWidth;
        public float StereoWidth
        {
            set { m_stereoWidth = value; }
            get { return m_stereoWidth; }
        }

        AudioListener m_audioListener;
        public AudioListener AudioListener
        {
            set { m_audioListener = value; }
            get { return m_audioListener; }
        }

        public override void Startup()
        {
            Engine.AssetManager.RegisterAssetType<SoundEffect>(new SoundEffectLoader());
            Engine.AssetManager.RegisterAssetType<Sound>(new SoundLoader());
        }


        public override void Shutdown()
        {
        }
    }
}
