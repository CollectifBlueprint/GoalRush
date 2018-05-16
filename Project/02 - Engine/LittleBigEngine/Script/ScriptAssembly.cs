using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.CSharp;
using System.IO;

namespace LBE.Script
{
    public class ScriptAssembly
    {
        private Assembly m_assembly;

        static List<Assembly> m_assemblies = new List<Assembly>();
        public static List<Assembly> Assemblies
        {
            get { return m_assemblies; }
        }

        public ScriptAssembly()
        {
        }

        public ScriptAssembly(Assembly assembly)
        {
            m_assembly = assembly;
        }

        public static ScriptAssembly BuildAssembly(String[] files)
        {
            var assemblies = new List<Assembly>();
            assemblies.Add(Assembly.GetExecutingAssembly());
            assemblies.Add(typeof(Microsoft.Xna.Framework.Vector2).Assembly);

            return BuildAssembly(files, assemblies.ToArray());
        }

        public static ScriptAssembly BuildAssembly(String[] files, Assembly[] assemblies)
        {
            var cSharpCodeProvider = new CSharpCodeProvider();

            CompilerParameters options = new CompilerParameters();
            options.GenerateExecutable = false;
            options.GenerateInMemory = true;

            if (assemblies != null)
                foreach (var assembly in assemblies)
                    options.ReferencedAssemblies.Add(assembly.Location);

            // Compile our code
            CompilerResults result;
            result = cSharpCodeProvider.CompileAssemblyFromFile(options, files);

            if (result.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    sb.AppendFormat("Error while compiling assembly: ");
                    sb.Append(error.ToString());
                    sb.AppendLine("");
                }
                throw new Exception(sb.ToString());
            }

            return new ScriptAssembly(result.CompiledAssembly);
        }

        public T GetInstance<T>(String typeName) where T : class
        {
            var types = m_assembly.GetTypes();
            var type = types.First(t => t.Name == typeName);
            var instance = m_assembly.CreateInstance(type.FullName);
            return instance as T;
        }

        public static T GetInstance<T>(ScriptSource source) where T : class
        {
            //Fix the files path with the content root
            var files = from file in source.Files
                        select Path.GetFullPath(Path.Combine(Engine.AssetManager.ContentRoot, file));
            var assembly = ScriptAssembly.BuildAssembly(files.ToArray(), m_assemblies.ToArray());
            return assembly.GetInstance<T>(source.MainType);
        }
    }
}
