using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Assets;

namespace LBE.Audio
{
    public class SoundLoader : BaseAssetLoader<Sound>
    {

        public override AssetLoadResult<Sound> Load(string path)
        {
            Asset<SoundDefinition> soundDefinitionAsset = Engine.AssetManager.GetAsset<SoundDefinition>(path);
            Sound instance = new Sound(soundDefinitionAsset);

            List<IAssetDependency> dependencies = new List<IAssetDependency>();
            dependencies.Add(soundDefinitionAsset);

            AssetLoadResult<Sound> result;
            result.Instance = instance;
            result.Dependencies = dependencies;

            return result;
        }

        public override void Unload(Sound content)
        {
            base.Unload(content);
        }
    }
}
