using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBE.Assets
{
    public delegate void OnChange();

    public interface IAssetDependency
    {
        event OnChange OnAssetChanged;
        void Reload();
    }

    public interface IAsset
    {
        String Name { get; }
        String Path { get; }
        Type Type { get; }
        Object Get();

        bool Load();
        bool Ready { get; }
        void Reload();

        event OnChange OnAssetChanged;
    }
}