using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBE.Assets
{
    public struct AssetLoadResult<T>
    {
        public T Instance;
        public IEnumerable<IAssetDependency> Dependencies;
    }

    public interface IAssetLoader<T>
    {
        AssetLoadResult<T> Load(String path);
        void Unload(T content);
    }

    public abstract class BaseAssetLoader<T> : IAssetLoader<T>
    {
        Type m_type;
        public Type Type
        {
            get { return m_type; }
        }

        public BaseAssetLoader()
        {
            m_type = typeof(T);
        }

        public abstract AssetLoadResult<T> Load(String path);

        public virtual void Unload(T content)
        {
        }
    }
}
