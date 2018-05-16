using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LBE.Assets.Lua;

namespace LBE.Assets.AssetTypes
{
    public class AssetListLoader : BaseAssetLoader<AssetList>
    {
        public AssetListLoader()
        {
        }

        //static LuaManagerKopi lua;
        public override AssetLoadResult<AssetList> Load(String path)
        {
            AssetList assetList = new AssetList();

            ////Create Lua State
            //if (lua == null)
            //    lua = new LuaManagerKopi();
            LuaManagerKopi lua = new LuaManagerKopi();

            List<String> ignores = new List<string>();
            //Find the objects already in lua's global table
            foreach (var key in (lua["_G"] as LuaInterface.LuaTable).Keys)
            {
                ignores.Add(key.ToString());
            }

            //Run the file
            using(var stream = Engine.AssetManager.AssetSource.Open(path))
                lua.DoFile(stream);

            //For each object in lua's global table
            LuaInterface.LuaTable globals = lua["_G"] as LuaInterface.LuaTable;
            foreach (var key in globals.Keys)
            {
                String keyName = key.ToString();

                //If object was present before, we skip it
                if (ignores.Contains(keyName))
                {
                    continue;
                }

                // keys beginning by "_" are ignored. 
                if (keyName.StartsWith("_"))
                    continue;

                //Convert the lua table to an asset
                AssetDefinition asset = ConvertTable(keyName, globals[key] as LuaInterface.LuaTable);
                assetList.Definitions.Add(asset);

                String test = AssetDefinitionXMLHelper.ToXml(asset);
            }

            List<IAssetDependency> dependencies = new List<IAssetDependency>();
            dependencies.Add(Engine.AssetManager.AssetSource.CreateDependency(path));

            AssetLoadResult<AssetList> result;
            result.Instance = assetList;
            result.Dependencies = dependencies;

            return result;
        }

        AssetDefinition ConvertTable(String name, LuaInterface.LuaTable t)
        {
            String type = (String)t["_class"];

            AssetDefinition asset = new AssetDefinition(name, type);
            foreach (var key in t.Keys)
            {
                String keyName = key.ToString();
                Copy(asset.Fields, keyName, t[key]);
            }

            return asset;
        }

        void Copy(Dictionary<String, Object> t, String key, Object value)
        {
            if (value is LuaInterface.LuaTable)
            {
                t[key] = ConvertTable(key, value as LuaInterface.LuaTable);
            }
            else if (value is String)
            {
                String s = value as String;
                if (s[0] == '@')
                {
                    t[key] = new AssetReference(s.TrimStart('@'));
                }
                else
                {
                    t[key] = value;
                }
            }
            else
            {
                t[key] = value;
            }
        }
    }
}
