using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using LBE.Assets;
using System.Diagnostics;

namespace LBE.Graphics
{
    public class EffectLoader : BaseAssetLoader<Effect>
    {
        public override AssetLoadResult<Effect> Load(string path)
        {
            Effect instance = null;

            //var mgfxPath = Path.ChangeExtension(path, "2mgfx");

            //using (var stream = Engine.AssetManager.AssetSource.Open(mgfxPath))
            //{
            //    //instance = new Effect(Engine.Renderer.Device, path, stream);
            //    var bytes = new byte[stream.Length];
            //    stream.Read(bytes, 0, (int)stream.Length);
            //    instance = new Effect(Engine.Renderer.Device, bytes);
            //}

            //List<IAssetDependency> dependencies = new List<IAssetDependency>();
            //dependencies.Add(Engine.AssetManager.AssetSource.CreateDependency(mgfxPath));

            //AssetLoadResult<Effect> result;
            //result.Instance = instance;
            //result.Dependencies = dependencies;

            //return result;

            path = Path.Combine(Engine.AssetManager.ContentRoot, path);
                
            String effectFilePath = path;
            String mgfxFilePath = Path.ChangeExtension(effectFilePath, "2mgfx");

            var lastEffectWriteTime = File.GetLastWriteTimeUtc(path);
            var lastMgfxWriteTime = DateTime.MinValue;

            if (File.Exists(mgfxFilePath))
                lastMgfxWriteTime = File.GetLastWriteTimeUtc(mgfxFilePath);

            //Execute 2Mgfx to create the binary file
            if (lastEffectWriteTime > lastMgfxWriteTime)
            {
                String execPath = Path.GetFullPath(@"01 - MonoGame\Tools\2MGFX\bin\Windows\AnyCPU\Release\2MGFX.exe");
                String error = "";

                if (!File.Exists(execPath))
                {
                    error = "2MGFX.exe does not exist. If you're running the project from a non-windows platform it may not be available. If so please run the project from a windows platform first";
                }
                else
                {
                    ProcessStartInfo startInfos = new ProcessStartInfo();
                    startInfos.FileName = Path.GetFullPath(execPath);
                    startInfos.Arguments =
                        "\"" + Path.GetFullPath(effectFilePath) + "\"" +
                        " " +
                        "\"" + Path.GetFullPath(mgfxFilePath) + "\"" +
                        " /DEBUG";
                    startInfos.RedirectStandardOutput = true;
                    startInfos.RedirectStandardError = true;
                    startInfos.UseShellExecute = false;

                    var process = System.Diagnostics.Process.Start(startInfos);
                    process.WaitForExit();

                    String output = process.StandardOutput.ReadToEnd();
                    error = process.StandardError.ReadToEnd();
                }
                if (error.Length > 0)
                {
                    throw new Exception(error);
                }
            }

            instance = new Effect(Engine.Renderer.Device, File.ReadAllBytes(mgfxFilePath));

            var dependencies = new List<IAssetDependency>();
            dependencies.Add(Engine.AssetManager.GetFileDependency(path));

            AssetLoadResult<Effect> result;
            result.Instance = instance;
            result.Dependencies = dependencies;

            return result;
        }

        public override void Unload(Effect content)
        {
            base.Unload(content);
        }
    }
}
