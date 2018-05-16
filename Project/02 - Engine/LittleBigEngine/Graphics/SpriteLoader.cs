using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Assets;

namespace LBE.Graphics.Sprites
{
    public class SpriteLoader : BaseAssetLoader<Sprite>
    {
        public override AssetLoadResult<Sprite> Load(string path)
        {
            Asset<SpriteDefinition> m_spriteDefinitionAsset = Engine.AssetManager.GetAsset<SpriteDefinition>(path);
            Sprite instance = new Sprite(m_spriteDefinitionAsset);

            List<IAssetDependency> dependencies = new List<IAssetDependency>();
            dependencies.Add(m_spriteDefinitionAsset);

            AssetLoadResult<Sprite> result;
            result.Instance = instance;
            result.Dependencies = dependencies;

            return result;
        }

        public override void Unload(Sprite content)
        {
        }
    }
}
