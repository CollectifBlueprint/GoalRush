using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE;
using LBE.Assets;
using LBE.Core;
using LBE.Gameplay;
using LBE.Graphics;
using LBE.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using LuaInterface;
using System.IO;

namespace LBE.Assets.Lua
{
    public class LuaManagerKopi
    {
        protected LuaInterface.Lua m_luaState;
        public LuaInterface.Lua LuaState
        {
            get { return m_luaState; } 
        }

        public Object this[String s]
        {
            get { return m_luaState[s]; }
            set { m_luaState[s] = value; }
        }

        public LuaManagerKopi()
        {
            m_luaState = new LuaInterface.Lua();
        }

        public void DoFile(Stream stream)
        {
            try
            {
                StreamReader sr = new StreamReader(stream);
                String buffer = sr.ReadToEnd();
                m_luaState.DoString(buffer);
            }
            catch (Exception e)
            {
                Engine.Log.Error(">>A .Net Exception occurred:");
                Engine.Log.Error(">>" + e.Message);
                throw e;
            }
        }

        public void DoFile(String path)
        {
            using (var stream = File.OpenRead(path))
                DoFile(stream);
        }
    }
}
