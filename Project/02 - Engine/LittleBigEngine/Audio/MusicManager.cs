using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;


namespace LBE.Audio
{
    public class MusicManager : BaseEngineComponent
    {
        Music m_music;
        
        float m_masterVolume;
        public float MasterVolume
        {
            set 
            { 
                m_masterVolume = value;
                SetMasterVolume(value);
            }
            get { return m_masterVolume; }
        }

        public override void Startup()
        {
            Engine.AssetManager.RegisterAssetType<Song>(new SongLoader());
        }

        public override void Shutdown()
        {            
            MediaPlayer.Stop();
        }

        public void Play(Music music)
        {
            m_music = music;
			MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = music.Definition.Volume * m_masterVolume;
            MediaPlayer.Play(music.Definition.Song);
        }

        public void Stop()
        {
            MediaPlayer.Stop();
        }

        public void Pause()
        {
            MediaPlayer.Pause();
        }

        public bool IsPlaying()
        {
            return MediaPlayer.State == MediaState.Playing;
        }

        public MediaState GetState()
        {
            return MediaPlayer.State;
        }

        void SetMasterVolume(float volume)
        {
            if (IsPlaying())
            {
                MediaPlayer.Volume = m_music.Definition.Volume * m_masterVolume;
            }
        }
    }
}
