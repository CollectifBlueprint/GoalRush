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
    public class MusicComponent : GameObjectComponent
    {
        Music m_music;

        public Music Music
        {
            get { return m_music; }
        }

        public MusicComponent(string soundDefinitionPath)
        {
            m_music = Music.Create(soundDefinitionPath);
        }

        public MusicComponent(MusicDefinition musicDefinition)
        {
            m_music = Music.Create(musicDefinition);
        }

        public MusicComponent(Asset<MusicDefinition> musicAsset)
        {
            m_music = Music.Create(musicAsset);
        }

        public override void Update()
        {
        }

        public void Play(Music music)
        {
            Engine.MusicManager.Play(music);
        }

        public void Stop()
        {
            Engine.MusicManager.Stop();
        }

        public void Pause()
        {
            Engine.MusicManager.Pause();
        }

        public bool IsPlaying()
        {
            return Engine.MusicManager.GetState() == MediaState.Paused;
        }

    }
}
