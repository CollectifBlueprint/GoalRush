using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Gameplay;

using Microsoft.Xna.Framework;
using LBE.Assets;
using Microsoft.Xna.Framework.Media;

namespace LBE.Audio
{
    public class AudioStreamComponent : GameObjectComponent
    {
        AudioStream m_stream;
        public AudioStream Stream
        {
            get { return m_stream; }
        }

        public float Volume
        {
            set { m_stream.Volume = value; }
            get { return m_stream.Volume; }
        }

        public bool Loop
        {
            set { m_stream.Loop = value; }
            get { return m_stream.Loop; }
        }

      

        public AudioStreamComponent(string soundDefinitionPath)
        {
            m_stream = AudioStream.Create(soundDefinitionPath);
        }

        public AudioStreamComponent(AudioStreamDefinition musicDefinition)
        {
            //m_music = Music.Create(musicDefinition);
        }

        public AudioStreamComponent(Asset<AudioStreamDefinition> musicAsset)
        {
            //m_music = Music.Create(musicAsset);
        }

        public override void Update()
        {
        }

        public void Play()
        {
            m_stream.Play();
        }

        public void Stop()
        {
            m_stream.Stop();
        }

        public void Pause()
        {
            m_stream.Pause();
        }

        public void Resume()
        {
            m_stream.Resume();
        }
    }
}
