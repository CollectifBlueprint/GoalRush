using LBE;
using LBE.Assets;
using LBE.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ball.Audio
{
    public class GameMusic
    {
        Dictionary<String, Music> m_tracks;
        public Dictionary<String, Music> Tracks
        {
            get { return m_tracks; }
        }

		string m_currentTrack;

        public GameMusic()
        {
            m_tracks = new Dictionary<string, Music>();
           
            Asset<AssetList> assetListMusic = Engine.AssetManager.GetAsset<AssetList>("Audio/Music.lua");
            foreach (AssetDefinition assetDefMusic in assetListMusic.Content.Definitions)
            {
                AssetInstantiationResult musicDefinitionRes = AssetInstanciator.CreateInstance(assetDefMusic, typeof(MusicDefinition));
				MusicDefinition musicDefinition = (MusicDefinition)musicDefinitionRes.Instance;
                
				Music music = Music.Create(musicDefinition);
                m_tracks.Add(assetDefMusic.Name, music);
            }         
        }

        public void StopAll()
        {
			Engine.MusicManager.Stop();
        }


        public void PlayTitleMusic()
        {
			PlayMusic("TitleMusic");
        }

        public void PlayMenuMusic()
        {
			PlayMusic("MenuMusic");
        }

		public void PlayMatchMusic()
		{
			PlayMusic("MatchMusic");
		}

		public void PlayMusic(string trackName)
		{
			if ( !Engine.MusicManager.IsPlaying() ||  m_currentTrack != trackName) 
			{
				Engine.MusicManager.Play (m_tracks [trackName]);
				m_currentTrack = trackName;
			}
		}

        public void Dispose()
        {
			Engine.MusicManager.Stop();
        }

    }
}
