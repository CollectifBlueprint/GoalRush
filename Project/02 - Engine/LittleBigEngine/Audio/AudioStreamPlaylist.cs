using LBE.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBE.Audio
{
    public class AudioStreamPlaylist
    {
        List<AudioStream> m_streams;
        int m_current = 0;

        AudioStreamState m_previousStreamState;

        bool m_isPlaying = false;

        public AudioStreamPlaylist()
        {
            m_streams = new List<AudioStream>(32);
        }

        public void Update()
        {
            if (!m_isPlaying)
                return;

            if (m_previousStreamState == AudioStreamState.Playing
                && m_streams[m_current].GetState() == AudioStreamState.Stopped)
            {
                m_current++;
                if (m_current >= m_streams.Count)
                    m_current = 0;

                m_streams[m_current].Play();
            }

            m_previousStreamState = m_streams[m_current].GetState();
        }


        public void Add(AudioStream stream)
        {
            m_streams.Add(stream);
        }


        public void Play()
        {
            Play(0);
        }

        public void Play(int streamIndex)
        {
            m_streams[streamIndex].Play();
            m_isPlaying = true;
        }

        public void Stop()
        {
            m_streams[m_current].Stop();
            m_isPlaying = false;
        }

        public void Dispose()
        {
            Stop();

        }
    }
}
