using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using LBE.Assets;
using Ionic.Zip;

namespace LBE.Graphics
{
    public class Texture2DLoader : BaseAssetLoader<Texture2D>
    {
        public override AssetLoadResult<Texture2D> Load(string path)
        {
            Texture2D instance = null;

            //System.Uri absoluteUri = new Uri(path);
            //System.Uri contentUri = new Uri(Engine.AssetManager.ContentRoot + "/");

            //Uri relativeUri = contentUri.MakeRelativeUri(absoluteUri);
            //String relativePath = relativeUri.ToString();

            //String dataPath = Path.Combine(Engine.AssetManager.ContentRoot, "content.dat");
            //var dataFileStream = File.OpenRead(dataPath);

            //ZipFile zf = ZipFile.Read(dataFileStream);
            //bool contains = zf.ContainsEntry(relativePath);
            //Engine.Log.Write(String.Format("Archive conttains path {0}? --- {1}", relativePath, contains));

            //var entry = zf[relativePath];
            //var reader = entry.OpenReader();
            
            //using (var fs = new FileStream(path, FileMode.Open))
            //    instance = Texture2D.FromStream(Engine.Renderer.Device, reader);

            using (var stream = Engine.AssetManager.AssetSource.Open(path))
                instance = Texture2D.FromStream(Engine.Renderer.Device, stream);

            var dependencies = new List<IAssetDependency>();
            dependencies.Add(Engine.AssetManager.AssetSource.CreateDependency(path));

            AssetLoadResult<Texture2D> result;
            result.Instance = instance;
            result.Dependencies = dependencies;

            return result;
        }

        public override void Unload(Texture2D content)
        {
            base.Unload(content);
        }
    }
}
