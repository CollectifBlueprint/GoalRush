using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Assets;

namespace LBE.Assets
{
    public class AssetDefinitionLoader : BaseAssetLoader<AssetDefinition>
    {
        public AssetDefinitionLoader()
        {
        }

        public override AssetLoadResult<AssetDefinition> Load(string path)
        {
            String[] subStrings = path.Split(new String[] { "::" }, 2, StringSplitOptions.None);
            String assetListPath = subStrings[0];
            String assetDefName = subStrings[1];

            Asset<AssetList> assetList = Engine.AssetManager.GetAsset<AssetList>(assetListPath);
            AssetDefinition assetDef = assetList.Content[assetDefName];

            List<IAssetDependency> dependencies = new List<IAssetDependency>();
            dependencies.Add(assetList);

            AssetLoadResult<AssetDefinition> result;
            result.Instance = assetDef;
            result.Dependencies = dependencies;

            return result;
        }
    }
}
