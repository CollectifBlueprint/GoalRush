using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Assets;

namespace LBE.Audio
{
    public class Music
    {
        Asset<MusicDefinition> m_asset;
        public MusicDefinition Definition
        {
            get { return m_asset.Content; }
        }

        public static Music Create(String musicDefinitionPath)
        {
            Asset<MusicDefinition> asset = Engine.AssetManager.GetAsset<MusicDefinition>(musicDefinitionPath);
            return new Music(asset);
        }


        public static Music Create(MusicDefinition musicDefinition)
        {
            Asset<MusicDefinition> asset = new Asset<MusicDefinition>(musicDefinition);
            return new Music(asset);
        }


        public static Music Create(Asset<MusicDefinition> musicAsset)
        {
            return new Music(musicAsset);
        }


        public Music(Asset<MusicDefinition> musicAsset)
        {
            m_asset = musicAsset;
            musicAsset.OnAssetChanged += new OnChange(musicAsset_OnAssetChanged);
            Reset(m_asset.Content);
        }


        void musicAsset_OnAssetChanged()
        {
            Reset(m_asset.Content);
        }


        void Reset(MusicDefinition musicDefinition)
        {
        }
    }
}
