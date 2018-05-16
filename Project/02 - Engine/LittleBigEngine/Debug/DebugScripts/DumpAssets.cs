using System;
using System.Collections.Generic;
using System.Text;
using LBE.Debug;
using System.IO;

namespace LBE.Script
{
    public class DumpAssets : ICommand
    {
        public void Execute()
        {
            var file = File.Create("test.txt");
            Engine.AssetManager.DumpAllAsset(file);
            file.Close();
        }
    }
}
