using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml.Serialization;

namespace LBE.Assets
{
    public class AssetDatabaseEntry
    {
        public String Path;
        public String Type;
    }

    public class AssetDatabase
    {
        List<AssetDatabaseEntry> m_assets;
        public List<AssetDatabaseEntry> Assets
        {
            get { return m_assets; }
        }

        public AssetDatabase()
        {
            m_assets = new List<AssetDatabaseEntry>();
        }

        public void LoadAll()
        {
            foreach (var entry in m_assets.ToArray())
            {
                bool loaded = false;
                loaded = loaded || Load(entry);
                loaded = loaded || Load(entry);

                if (!loaded)
                    m_assets.Remove(entry);
            }
        }

        public bool Load(AssetDatabaseEntry entry)
        {
            try
            {
                MethodInfo getMethod = typeof(AssetManager).GetMethod("Get");
                MethodInfo generic = getMethod.MakeGenericMethod(Engine.AssetManager.TypeDatabase.GetType(entry.Type));
                object objData = generic.Invoke(Engine.AssetManager, new Object[] { entry.Path, true });                
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool Add(String path, String type)
        {
            var asset = m_assets.Find(entry => path == entry.Path);
            if (asset != null && asset.Type == type)
                return false;

            if (asset != null && asset.Type != type)
            {
                asset.Type = type;
                return true;
            }
            else
            {
                m_assets.Add(new AssetDatabaseEntry() { Path = path, Type = type });
                return true;
            }

        }

        public void Remove(String path)
        {
            var asset = m_assets.Find(entry => path == entry.Path);
            if (asset != null)
                m_assets.Remove(asset);
        }

        public void Save(Stream stream)
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<AssetDatabaseEntry>));
            xs.Serialize(stream, m_assets);            
        }

        public void Load(Stream stream)
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<AssetDatabaseEntry>));
            m_assets = (List<AssetDatabaseEntry>)xs.Deserialize(stream); 
        }
    }
}
