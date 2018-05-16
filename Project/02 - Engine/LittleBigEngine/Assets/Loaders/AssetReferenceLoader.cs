using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBE.Assets
{
 
    public class AssetReferenceLoader : BaseAssetLoader<AssetReference>
    {

        public override AssetLoadResult<AssetReference> Load(string path)
        {
            AssetReference instance = new AssetReference(path);

            List<IAssetDependency> dependencies = new List<IAssetDependency>();
            dependencies.Add(Engine.AssetManager.AssetSource.CreateDependency(path));

            AssetLoadResult<AssetReference> result;
            result.Instance = instance;
            result.Dependencies = dependencies;

            return result;
        }

        public override void Unload(AssetReference content)
        {
            base.Unload(content);
        }
    }

}
