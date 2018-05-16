using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Assets;
using LBE.Physics;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;

namespace LBE.Graphics.Sprites
{
    public class CollisionDefinitionLoader : BaseAssetLoader<CollisionDefinition>
    {
        public override AssetLoadResult<CollisionDefinition> Load(string path)
        {
            List<IAssetDependency> dependencies = new List<IAssetDependency>();

            if (!path.EndsWith(".png") && !path.EndsWith(".col"))
            {
                Engine.Log.Error("Can't load collision \"" + path + "\", unknow format");
            }

            String pngPath = Path.ChangeExtension(path, ".png");
            pngPath = Engine.AssetManager.AssetSource.GetFullPath(pngPath);
            String colPath = Path.ChangeExtension(path, ".png.col");
            colPath = Engine.AssetManager.AssetSource.GetFullPath(colPath);


            bool rebuildCol = false;
            if (Engine.AssetManager.AssetSource.Exists(pngPath))
            {
                DateTime lastSourceEditTime = DateTime.MinValue;
                var colFileExist = Engine.AssetManager.AssetSource.Exists(colPath);
                if (!colFileExist || File.GetLastWriteTimeUtc(colPath) < lastSourceEditTime)
                    rebuildCol = true;

                if (rebuildCol)
                {
                    Asset<Texture2D> textureAsset = Engine.AssetManager.GetAsset<Texture2D>(pngPath);
                    var colDef = CollisionDefinitionHelper.FromTexture(textureAsset.Content, 2.0f);

                    // Save the collision
                    using (var file = File.Create(colPath))
                        Serialize(file, colDef);
                }
            }

            CollisionDefinition instance = null;
            using (var stream = Engine.AssetManager.AssetSource.Open(colPath))
                instance = Deserialize(stream);

            dependencies.Add(Engine.AssetManager.AssetSource.CreateDependency(colPath));
            AssetLoadResult<CollisionDefinition> result = new AssetLoadResult<CollisionDefinition>();
            result.Instance = instance;
            result.Dependencies = dependencies;

            return result;


            ////Load collision from file
            //CollisionDefinition instance = null;
            //if (Engine.AssetManager.AssetSource.Exists(colPath))
            //{
            //    using (var stream = Engine.AssetManager.AssetSource.Open(colPath))
            //        instance = Deserialize(stream);

            //    dependencies.Add(Engine.AssetManager.AssetSource.CreateDependency(colPath));
            //}
            //else
            //{
            //    Asset<Texture2D> textureAsset = Engine.AssetManager.GetAsset<Texture2D>(pngPath);
            //    instance = CollisionDefinitionHelper.FromTexture(textureAsset.Content, 2.0f);

            //    dependencies.Add(textureAsset);
            //}

            //AssetLoadResult<CollisionDefinition> result = new AssetLoadResult<CollisionDefinition>();
            //result.Instance = instance;
            //result.Dependencies = dependencies;

            //return result;

            //CollisionTextureDefinition textureDefinition = null;
            //DateTime lastSourceEditTime = DateTime.MinValue;

            //if (path.EndsWith(".png"))
            //{
            //    textureDefinition = new CollisionTextureDefinition();
            //    textureDefinition.Texture = new AssetReference(path);
            //    dependencies.Add(Engine.AssetManager.GetFileDependency(path));

            //    lastSourceEditTime = File.GetLastWriteTimeUtc(path);
            //}
            //else if(path.Contains(".lua::"))
            //{
            //    var defAsset = Engine.AssetManager.GetAsset<CollisionTextureDefinition>(path);
            //    dependencies.Add(defAsset);

            //    textureDefinition = defAsset.Content;

            //    var textureEditTime = File.GetLastWriteTimeUtc(defAsset.Content.Texture.Path);
            //    var definitionEditTime = File.GetLastWriteTimeUtc(path);

            //    if(textureEditTime > definitionEditTime)
            //        lastSourceEditTime = textureEditTime;
            //    else
            //        lastSourceEditTime = definitionEditTime;
            //}
            //else
            //{
            //    Engine.Log.Error("Can't load collision \"" + path + "\", unknow format");
            //}

            //String buildedColPath = path + ".col";
            //if (!File.Exists(buildedColPath) || File.GetLastWriteTimeUtc(buildedColPath) < lastSourceEditTime)
            //{
            //    Engine.Log.Write("Building collisions from texture: " + textureDefinition.Texture.Path);
            //    Texture2D sourceTexture = Engine.AssetManager.Get<Texture2D>(textureDefinition.Texture.Path);
            //    CollisionDefinition colDef = CollisionDefinitionHelper.FromTexture(sourceTexture, textureDefinition.Tolerance, textureDefinition.Transform);

            //    //Save the collision
            //    using (var file = File.Create(buildedColPath))
            //        Serialize(file, colDef);
            //}

            ////Load collision from file
            //CollisionDefinition instance = null;
            //using (var file = File.OpenRead(buildedColPath))
            //    instance = Deserialize(file);

            //AssetLoadResult<CollisionDefinition> result = new AssetLoadResult<CollisionDefinition>();
            //result.Instance = instance;
            //result.Dependencies = dependencies;

            //return result;
        }

        public void Serialize(Stream stream, CollisionDefinition colDef)
        {
            var writer = new BinaryWriter(stream);
            writer.Write(colDef.Entries.Count());
            foreach (var item in colDef.Entries)
            {
                writer.Write(item.Indices.Count());
                foreach (var i in item.Indices)
                    writer.Write(i);

                writer.Write(item.Vertices.Count());
                foreach (var v in item.Vertices)
                {
                    writer.Write(v.X);
                    writer.Write(v.Y);
                }
            }
        }

        public CollisionDefinition Deserialize(Stream stream)
        {
            var reader = new BinaryReader(stream);
            var colDef = new CollisionDefinition();

            var nItem = reader.ReadInt32();
            colDef.Entries = new CollisionDefinitionEntry[nItem];
            for (int iItem = 0; iItem < nItem; iItem++)
            {
                colDef.Entries[iItem] = new CollisionDefinitionEntry();

                var nIndices = reader.ReadInt32();
                colDef.Entries[iItem].Indices = new int[nIndices];
                for (int iIndices = 0; iIndices < nIndices; iIndices++)
                {
                    colDef.Entries[iItem].Indices[iIndices] = reader.ReadInt32();
                }

                var nVertices = reader.ReadInt32();
                colDef.Entries[iItem].Vertices = new Vector2[nVertices];
                for (int iVertice = 0; iVertice < nVertices; iVertice++)
                {
                    colDef.Entries[iItem].Vertices[iVertice].X = reader.ReadSingle();
                    colDef.Entries[iItem].Vertices[iVertice].Y = reader.ReadSingle();
                }
            }

            return colDef;
        }

        public override void Unload(CollisionDefinition content)
        {
        }
    }
}
