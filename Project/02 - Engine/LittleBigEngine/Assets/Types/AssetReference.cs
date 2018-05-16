using System;
using System.IO;

namespace LBE.Assets
{
    public struct AssetReference
    {
        public String Path;

        public AssetReference(String path)
        {
            Path = path;
        }

        public Stream Open()
        {
            return Engine.AssetManager.AssetSource.Open(Path);
        }
    }
}