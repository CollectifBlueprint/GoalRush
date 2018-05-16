using LBE.Assets;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;

namespace LBE.Audio
{
    public class AudioStream
    {
        OggStream.OggStream m_stream;

        public float Volume
        {
            set { m_stream.Volume = value; }
            get { return m_stream.Volume; }
        }

        public bool Loop
        {
            set { m_stream.IsLooped = value; }
            get { return m_stream.IsLooped; }
        }

        Asset<AudioStreamDefinition> m_asset;
        public AudioStreamDefinition Definition
        {
            get { return m_asset.Content; }
        }

        public static AudioStream Create(String AudioStreamDefinitionPath)
        {
            Asset<AudioStreamDefinition> asset = Engine.AssetManager.GetAsset<AudioStreamDefinition>(AudioStreamDefinitionPath);
            return new AudioStream(asset);
        }


        public static AudioStream Create(AudioStreamDefinition AudioStreamDefinition)
        {
            Asset<AudioStreamDefinition> asset = new Asset<AudioStreamDefinition>(AudioStreamDefinition);
            return new AudioStream(asset);
        }


        public static AudioStream Create(Asset<AudioStreamDefinition> AudioStreamAsset)
        {
            return new AudioStream(AudioStreamAsset);
        }


        public AudioStream(Asset<AudioStreamDefinition> AudioStreamAsset)
        {
            m_asset = AudioStreamAsset;
            AudioStreamAsset.OnAssetChanged += new OnChange(AudioStreamAsset_OnAssetChanged);
            Reset(m_asset.Content);
			return;

			m_stream = new OggStream.OggStream(m_asset.Content.AssetReference.Open());

            m_stream.Volume = m_asset.Content.Volume;
            m_stream.IsLooped = m_asset.Content.Loop;
        }


        void AudioStreamAsset_OnAssetChanged()
        {
            Reset(m_asset.Content);
        }


        void Reset(AudioStreamDefinition AudioStreamDefinition)
        {
        }


        public void Play()
        {
			return; 
            m_stream.Volume = m_asset.Content.Volume * Engine.AudioStreamManager.MasterVolume;
            m_stream.Prepare();
            m_stream.Play();
        }

        public void Stop()
        {
			return;
            m_stream.Stop();
        }

        public void Pause()
        {
			return;
            m_stream.Pause();
        }

        public void Resume()
        {
			return;
            m_stream.Resume();
        }

        public AudioStreamState GetState()
        {
			return AudioStreamState.Error;

            switch (m_stream.GetState())
            {
                case ALSourceState.Initial:
                    return AudioStreamState.Initial;
                    break;
                case ALSourceState.Playing:
                    return AudioStreamState.Playing;
                    break;
                case ALSourceState.Paused:
                    return AudioStreamState.Paused;
                    break;
                case ALSourceState.Stopped:
                    return AudioStreamState.Stopped;
                    break;
            }

            return AudioStreamState.Error;
        }

    }
}
