using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using LBE.Assets;

namespace LBE.Audio
{
    public class Sound
    {
        Asset<SoundDefinition> m_asset;
        public SoundDefinition Definition
        {
            get { return m_asset.Content; }
        }

        public static Sound Create(String soundDefinitionPath)
        {
            Asset<SoundDefinition> asset = Engine.AssetManager.GetAsset<SoundDefinition>(soundDefinitionPath);
            return new Sound(asset);
        }


        public static Sound Create(SoundDefinition soundDefinition)
        {
            Asset<SoundDefinition> asset = new Asset<SoundDefinition>(soundDefinition);
            return new Sound(asset);
        }


        public static Sound Create(Asset<SoundDefinition> soundAsset)
        {
            return new Sound(soundAsset);
        }
  

        public Sound(Asset<SoundDefinition> soundAsset)
        {
            m_asset = soundAsset;
            soundAsset.OnAssetChanged += new OnChange(soundAsset_OnAssetChanged);
            Reset(m_asset.Content);
        }


        void soundAsset_OnAssetChanged()
        {
            Reset(m_asset.Content);
        }


        void Reset(SoundDefinition spriteDefinition)
        {
        }
    }
}
