using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using OpenTK.Audio;
//using NVorbis.OpenTKSupport;
using LBE;

namespace LBE.Audio
{

    public enum AudioStreamState
    {
        Initial = 0,
        Playing,
        Paused,
        Stopped,
        Error
    }

    public class AudioStreamManager : BaseEngineComponent
    {

        float m_masterVolume;
        public float MasterVolume
        {
            set { m_masterVolume = value; }
            get { return m_masterVolume; }
        }

        OggStream.OggStreamer m_oggStreamer;
        public OggStream.OggStreamer OggStreamer
        {
            get { return m_oggStreamer; }
        }

        AudioStreamPlaylist m_playlist = null;
        public AudioStreamPlaylist Playlist
        {
            set { m_playlist = value; }
            get { return m_playlist; }
        }


        public override void Startup()
        {
            m_oggStreamer = new OggStream.OggStreamer();
        }

        public override void Shutdown()
        {
            m_oggStreamer.Dispose();
        }

        public override void StartFrame()
        {
            if (m_playlist != null)
                m_playlist.Update();
        }
    }
}
