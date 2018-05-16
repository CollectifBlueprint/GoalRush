using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace LBE.Assets
{
    public partial class AssetManager : BaseEngineComponent
    {
        public const String AssetDbPAth = "07 - Output/Assets.xml";
        public const String AssetOptimPath = "07 - Output/AssetLoadTime.txt";

        int m_loadingRecursionLevel = 0;

        public T Get<T>(String path, bool registerAsset = true)
        {
            return GetAsset<T>(path, registerAsset).Content;
        }

        public Asset<T> GetAsset<T>(String path, bool registerAsset = true)
        {
            m_loadingRecursionLevel++;

            Stopwatch m_loadTimeWatch = new Stopwatch();
            m_loadTimeWatch.Start();

            String systemPath = path;
            if (Path.IsPathRooted(path))
            {
                systemPath = path;
            }
            else if (!systemPath.StartsWith(m_contentRoot))
            {
                //Add content root to path
                systemPath = Path.Combine(new String[] { m_contentRoot, path });
            }

            //Make sure we have consistent use of slashes
            //systemPath = systemPath.Replace("\\", "/");

            //Create a identifying key using type and path
            String assetKey = Hash.GetMd5Sum(typeof(T).Name + ":" + systemPath);

            //If asset instance already exists, return it
            if (m_assetInstances.ContainsKey(assetKey))
            {
                m_loadingRecursionLevel--;
                return m_assetInstances[assetKey] as Asset<T>;
            }

            Engine.Log.Write("Loading new asset:");

            System.Uri absoluteUri = new Uri(systemPath);
            System.Uri contentUri = new Uri(m_contentRoot + "/./");

            Uri relativeUri = contentUri.MakeRelativeUri(absoluteUri);
            String relativePath = relativeUri.ToString();

            Engine.Log.IndentMore();
            //Engine.Log.Write(
            //    String.Format("* Key: {0}", assetKey));
            Engine.Log.Write(
                String.Format("* Path: \"{0}\"", relativePath));
            Engine.Log.Write(
                String.Format("* Type: \"{0}\"", typeof(T).Name));

            Asset<T> newAsset = null;
            if (m_assetTypes.ContainsKey(typeof(T)))
            {
                //Find loader associated with T
                IAssetLoader<T> assetLoader = m_assetTypes[typeof(T)] as BaseAssetLoader<T>;
                newAsset = new Asset<T>(relativePath, assetKey, assetLoader);
            }
            else if (systemPath.Contains("::") && typeof(T) != typeof(AssetDefinition))
            {
                //Use asset definition instanciator
                IAssetLoader<T> assetLoader = new AssetInstanceLoader<T>();
                newAsset = new Asset<T>(relativePath, assetKey, assetLoader);
            }
            else
            {
                //Find any suitable loadable
                Type loaderType = m_assetTypes.Keys.FirstOrDefault(t => { return t.IsAssignableFrom(typeof(T)); });
                if (loaderType != null)
                {
                    var assetLoader = m_assetTypes[loaderType] as IAssetLoader<T>;
                    newAsset = new Asset<T>(relativePath, assetKey, assetLoader);
                }
            }

            if (newAsset == null)
            {
                Engine.Log.Error(
                       String.Format("Couldn't find a suitable Loader"));

                m_loadingRecursionLevel--;
                return null;
            }

            //Engine.Log.Write(
            //    String.Format("* Loader: \"{0}\"", newAsset.Loader.GetType().Name));

            //Load the asset from memory
            Engine.Log.IndentMore();
            bool loadResult = newAsset.Load();
            Engine.Log.IndentLess();

            if (!loadResult)
            {
                Engine.Log.Error(
                    String.Format("An error occurred while loading the asset"));

                m_loadingRecursionLevel--;
                return null;
            }

            //Engine.Log.Write(
            //    String.Format("Loading successful"));

            //Register the new asset
            if (registerAsset)
            {
                m_assetInstances.Add(assetKey, newAsset);
            }

            //Fire OnAssetLoaded event
            if (OnAssetLoaded != null) OnAssetLoaded(newAsset);

            Engine.Log.IndentLess();

            if (newAsset != null)
            {
                var added = m_assetDb.Add(relativePath, typeof(T).Name);
                if (added)
                {

                    var dir = Path.GetDirectoryName(AssetDbPAth);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    using (var file = File.Create(AssetDbPAth))
                        m_assetDb.Save(file);
                }
            }

            m_loadTimeWatch.Stop();
            m_loadingRecursionLevel--;

            var time = m_loadTimeWatch.ElapsedMilliseconds / 1000.0f;
            using (var fs = File.Open(AssetOptimPath, FileMode.Append))
            {
                var line = String.Format("{0};{1};{2};{3}", path, newAsset.Type.Name, time, m_loadingRecursionLevel);
                var sw = new StreamWriter(fs);
                sw.WriteLine(line);
                sw.Flush();
            }

            return newAsset;
        }

        public void InitDb()
        {
            if (!File.Exists(AssetDbPAth))
                return;

            using (var file = File.OpenRead(AssetDbPAth))
                m_assetDb.Load(file);
        }

        public FileDependency GetFileDependency(String path)
        {
            if (!m_fileDependencies.Keys.Contains(path))
            {
                m_fileDependencies[path] = new FileDependency(path);
            }
            return m_fileDependencies[path];
        }
    }
}
