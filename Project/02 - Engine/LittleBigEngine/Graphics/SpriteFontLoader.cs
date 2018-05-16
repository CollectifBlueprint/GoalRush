using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using LBE.Assets;
using Microsoft.Xna.Framework.Content;

namespace LBE.Graphics
{
    public class SpriteFontLoader : BaseAssetLoader<SpriteFont>
    {
        public override AssetLoadResult<SpriteFont> Load(string path)
        {
            SpriteFont instance = null;

            instance = Engine.AssetManager.ContentManager.Load<SpriteFont>(path);

            var dependencies = new List<IAssetDependency>();
            dependencies.Add(Engine.AssetManager.AssetSource.CreateDependency(path));

            AssetLoadResult<SpriteFont> result;
            result.Instance = instance;
            result.Dependencies = dependencies;

            return result;
        }

        public override void Unload(SpriteFont content)
        {
            base.Unload(content);
        }
    }
}
