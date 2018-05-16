using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LBE.Assets.AssetTypes;
using LBE.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace LBE.Assets
{
    class CustomContentManager : ContentManager
    {
        public CustomContentManager(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        protected override Stream OpenStream(string assetName)
        {
            var filePath = assetName;
            if (!Path.HasExtension(assetName))
                filePath += ".xnb";

            return Engine.AssetManager.AssetSource.Open(filePath);
        }
    }

    public delegate void AssetLoadedEvent(IAsset asset);
    public partial class AssetManager : IEngineComponent
    {
        public event AssetLoadedEvent OnAssetLoaded;

        Dictionary<String, IAsset> m_assetInstances;
        public Dictionary<String, IAsset> AssetInstances
        {
            get { return m_assetInstances; }
        }

        Dictionary<Type, Object> m_assetTypes;
        public Dictionary<Type, Object> AssetTypes
        {
            get { return m_assetTypes; }
        }

        Dictionary<String, FileDependency> m_fileDependencies;

        ContentManager m_contentManager;
        public ContentManager ContentManager
        {
            get { return m_contentManager; }
        }

        String m_contentRoot;
        public String ContentRoot
        {
            get { return m_contentRoot; }
            set { SetContentRoot(value); }
        }

        IAssetSource m_assetSource;
        public IAssetSource AssetSource
        {
            get { return m_assetSource; }
            set { SetAssetReader(value); }
        }

        TypeDatabase m_typeDb;
        public TypeDatabase TypeDatabase
        {
            get { return m_typeDb; }
        }

        AssetDatabase m_assetDb;
        public AssetDatabase AssetDb
        {
            get { return m_assetDb; }
        }

        public override void Startup()
        {
            m_assetInstances = new Dictionary<string, IAsset>();
            m_assetTypes = new Dictionary<Type, object>();
            m_fileDependencies = new Dictionary<string, FileDependency>();

            m_typeDb = new TypeDatabase();
            m_assetDb = new AssetDatabase();            
            InitDb();

            RegisterAssetType<AssetList>(new AssetListLoader());
            RegisterAssetType<AssetDefinition>(new AssetDefinitionLoader());
            RegisterAssetType<AssetReference>(new AssetReferenceLoader());
        }

        public override void Shutdown()
        {
        }

        public override void StartFrame()
        {
            UpdateFileDependencies();
        }

        public void UpdateFileDependencies()
        {
            List<FileDependency> changedFiles = new List<FileDependency>();
            var filesTempList = m_fileDependencies.Values.ToList();
            foreach (var file in filesTempList)
            {
                if(file.PollChange())
                {
                    Engine.Log.Write(
                        String.Format("File \"{0}\" has changed.", file.FilePath));
                    Engine.Log.Write("Reloading dependencies:");
                    Engine.Log.IndentMore();
                    file.Reload();
                    Engine.Log.IndentLess();
                }
            }
        }

        public void RegisterAssetType<T>(BaseAssetLoader<T> assetType)
        {
            m_assetTypes.Add(assetType.Type, assetType);
        }

        private void SetContentRoot(string value)
        {
            m_contentRoot = Path.GetFullPath(value);
            m_contentManager = new ContentManager(Engine.Application.Services, Engine.AssetManager.ContentRoot);
        }

        void SetAssetReader(IAssetSource reader)
        {
            m_assetSource = reader;
            m_contentManager = new CustomContentManager(Engine.Application.Services);
        }

        public void DumpAllAsset(Stream stream)
        {
            foreach (var asset in m_assetInstances.Values)
            {
                StreamWriter sw = new StreamWriter(stream);
                //sw.WriteLine("Name: " + asset.Name);
                //sw.WriteLine("Path: " + asset.Path);
                sw.WriteLine(asset.Path);
                if (asset.Type.IsGenericType)
                    sw.WriteLine(asset.Type.GetGenericArguments()[0].Name);
                else 
                    sw.WriteLine(asset.Type.Name);
                sw.Flush();
            }
        }
    }
}
