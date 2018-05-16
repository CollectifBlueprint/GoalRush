using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Assets;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace LBE.Audio
{
    public class SoundEffectLoader : BaseAssetLoader<SoundEffect>
    {
        public override AssetLoadResult<SoundEffect> Load(string path)
        {
            SoundEffect instance = null;

            using (var stream = Engine.AssetManager.AssetSource.Open(path))
                instance = SoundEffect.FromStream(stream);

            var dependencies = new List<IAssetDependency>();
            dependencies.Add(Engine.AssetManager.AssetSource.CreateDependency(path));

            AssetLoadResult<SoundEffect> result;
            result.Instance = instance;
            result.Dependencies = dependencies;

            return result;
        }


        public override void Unload(SoundEffect content)
        {
            base.Unload(content);
        }
    }


}
