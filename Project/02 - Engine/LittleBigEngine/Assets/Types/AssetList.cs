using System;
using System.Collections.Generic;

namespace LBE.Assets
{
    public class AssetList
    {
        List<AssetDefinition> m_definitions;
        public List<AssetDefinition> Definitions
        {
            get { return m_definitions; }
            set { m_definitions = value; }
        }

        public AssetDefinition this[String name]
        {
            get { return m_definitions.Find(assetDef => assetDef.Name == name); } 
        }

        public AssetList()
        {
            m_definitions = new List<AssetDefinition>();
        }
    }
}