using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Assets;
using Microsoft.Xna.Framework.Media;

namespace LBE.Audio
{
    public class SongLoader : BaseAssetLoader<Song>
    {
        public override AssetLoadResult<Song> Load(string path)
        {
            Song instance = null;

            var relPath = path.Replace(Engine.AssetManager.ContentRoot + "\\", "");

            //instance = Engine.AssetManager.ContentManager.Load<Song>(path);
			//string songFileName = @"content/SongName.mp3";
			//Engine.AssetManager.ContentRoot
			var contentPath = Path.Combine(Engine.AssetManager.ContentRoot, path);
			var uri = new Uri(contentPath, UriKind.Relative);
			string name = Path.GetFileNameWithoutExtension (path);
			instance = Song.FromUri(name, uri);

            var dependencies = new List<IAssetDependency>();
			dependencies.Add(Engine.AssetManager.GetFileDependency(contentPath));

            AssetLoadResult<Song> result;
            result.Instance = instance;
            result.Dependencies = dependencies;

            return result;
        }


        public override void Unload(Song content)
        {
            base.Unload(content);
        }
    }
}
