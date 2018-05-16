using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LBE.Script;
using Microsoft.Xna.Framework.Graphics;
using LBE.Assets;
using System.Reflection;

namespace LBE.Script
{
    public class ScriptAssemblyLoader : BaseAssetLoader<ScriptAssembly>
    {
        List<Assembly> m_assemblies;
        public List<Assembly> Assemblies
        {
            get { return m_assemblies; }
            set { m_assemblies = value; }
        }

        public ScriptAssemblyLoader()
        {
            m_assemblies = new List<Assembly>();

            m_assemblies.Add(Assembly.GetExecutingAssembly());
            m_assemblies.Add(typeof(Microsoft.Xna.Framework.Vector2).Assembly);
        }

        public override AssetLoadResult<ScriptAssembly> Load(string path)
        {
            ScriptAssembly instance = null;

            var sourceAsset = Engine.AssetManager.GetAsset<ScriptSource>(path);

            //Fix the files path with the content root
            var files = from file in sourceAsset.Content.Files
                        select Path.GetFullPath(Path.Combine(Engine.AssetManager.ContentRoot, file));

            instance = ScriptAssembly.BuildAssembly(files.ToArray(), m_assemblies.ToArray());

            var dependencies = new List<IAssetDependency>();
            dependencies.Add(sourceAsset);
            foreach (var file in files)
            {
                dependencies.Add(Engine.AssetManager.GetFileDependency(file));
            }

            AssetLoadResult<ScriptAssembly> result;
            result.Instance = instance;
            result.Dependencies = dependencies;

            return result;
        }

        public override void Unload(ScriptAssembly content)
        {
        }
    }
}
