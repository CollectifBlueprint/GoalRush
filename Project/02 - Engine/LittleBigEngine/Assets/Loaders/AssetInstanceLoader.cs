using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework;
using System.ComponentModel;

namespace LBE.Assets
{
    public class AssetInstanceLoader<T> : BaseAssetLoader<T>
    {
        List<IAssetDependency> m_currentInstanceReferences;

        public override AssetLoadResult<T> Load(String path)
        {
            String[] subStrings = path.Split(new String[] { "::" }, 2, StringSplitOptions.None);
            String assetListPath = subStrings[0];
            String assetDefName = subStrings[1];

            Asset<AssetList> assetList = Engine.AssetManager.GetAsset<AssetList>(assetListPath);
            AssetDefinition assetDef = assetList.Content[assetDefName];

            Engine.Log.Assert(assetDef != null, String.Format("Can't find asset definition {0} in file {1}!", assetDefName, assetListPath));

            m_currentInstanceReferences = new List<IAssetDependency>();

            var instanceResult = AssetInstanciator.CreateInstance(assetDef, typeof(T));
            T instance = (T)instanceResult.Instance;

            List<IAssetDependency> dependencies = new List<IAssetDependency>();
            dependencies.Add(assetList);
            dependencies.AddRange(instanceResult.Dependencies);

            m_currentInstanceReferences.Clear();
            m_currentInstanceReferences = null;

            AssetLoadResult<T> result;
            result.Instance = instance;
            result.Dependencies = dependencies;

            return result;
        }        
    }
}
