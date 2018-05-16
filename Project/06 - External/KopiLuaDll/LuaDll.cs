using KopiLua;

namespace KopiLuaDll
{
	/*
	 * Lua types for the API, returned by lua_type function
	 */
	public enum LuaTypes 
	{
		LUA_TNONE=-1,
		LUA_TNIL=0,
		LUA_TNUMBER=3,
		LUA_TSTRING=4,
		LUA_TBOOLEAN=1,
		LUA_TTABLE=5,
		LUA_TFUNCTION=6,
		LUA_TUSERDATA=7,
		LUA_TLIGHTUSERDATA=2
	};


	/*
	 * Lua Garbage Collector options (param "what")
	 */
	public enum LuaGCOptions
	{
		LUA_GCSTOP = 0,
		LUA_GCRESTART = 1,
		LUA_GCCOLLECT = 2,
		LUA_GCCOUNT = 3,
		LUA_GCCOUNTB = 4,
		LUA_GCSTEP = 5,
		LUA_GCSETPAUSE = 6,
		LUA_GCSETSTEPMUL = 7
	};



	/*
	 * Special stack indexes
	 */
	public enum LuaIndexes 
	{
		LUA_REGISTRYINDEX=-10000,
		LUA_ENVIRONINDEX=-10001,	
		LUA_GLOBALSINDEX=-10002
	};


#if false
	/*
	 * Structure used by the chunk reader
	 */
	// [ StructLayout( LayoutKind.Sequential )]
	public ref struct ReaderInfo
	{
		public string chunkData;
		public bool finished;
	};


	/*
	 * Delegate for chunk readers used with lua_load
	 */
	public delegate string LuaChunkReader(LuaState luaState, ReaderInfo ^data, uint size);
#endif


	public class LuaDll 
	{
        // Not sure of the purpose of this, but I'm keeping it -kevinh
        static object tag = 0;

		// steffenj: BEGIN additional Lua API functions new in Lua 5.1

		public static int lua_gc(LuaState luaState, LuaGCOptions what, int data)
		{
			return Lua.LuaGC(luaState, (int) what, data);
		}

		public static string lua_typename(LuaState luaState, LuaTypes type)
		{
			return Lua.LuaTypeName(luaState, (int) type).ToString();
		}

		public static string luaL_typename(LuaState luaState, int stackPos)
		{
			return lua_typename(luaState, lua_type(luaState, stackPos));
		}

		public static void luaL_error(LuaState luaState, string message)
		{
			Lua.LuaLError(luaState, message);
		}

        public static void luaL_where(LuaState luaState, int level)
		{
			Lua.LuaLWhere(luaState, level);
		}


		// Not yet wrapped
		// static string luaL_gsub(LuaState luaState, string str, string pattern, string replacement);

#if false
		// the functions below are still untested
		static void lua_getfenv(LuaState luaState, int stackPos);
		static int lua_isfunction(LuaState luaState, int stackPos);
		static int lua_islightuserdata(LuaState luaState, int stackPos);
		static int lua_istable(LuaState luaState, int stackPos);
		static int lua_isuserdata(LuaState luaState, int stackPos);
		static int lua_lessthan(LuaState luaState, int stackPos1, int stackPos2);
		static int lua_rawequal(LuaState luaState, int stackPos1, int stackPos2);
		static int lua_setfenv(LuaState luaState, int stackPos);
		static void lua_setfield(LuaState luaState, int stackPos, string name);
		static int luaL_callmeta(LuaState luaState, int stackPos, string name);
		// steffenj: END additional Lua API functions new in Lua 5.1
#endif

		public static LuaState luaL_newstate()
		{
			return Lua.LuaLNewState();
		}

		// Not yet wrapped
		// static void lua_close(LuaState luaState);

		public static void luaL_openlibs(LuaState luaState)
		{
			Lua.LuaLOpenLibs(luaState);
		}

		// Not yet wrapped
		// static int lua_objlen(LuaState luaState, int stackPos);

		public static int luaL_loadstring(LuaState luaState, string chunk)
		{
			return Lua.LuaLLoadString(luaState, chunk);
		}

		public static int luaL_dostring(LuaState luaState, string chunk)
		{
			int result = luaL_loadstring(luaState, chunk);
			if (result != 0)
				return result;

			return lua_pcall(luaState, 0, -1, 0);
		}

		/// <summary>DEPRECATED - use luaL_dostring(LuaState luaState, string chunk) instead!</summary>
		public static int lua_dostring(LuaState luaState, string chunk)
		{
			return luaL_dostring(luaState, chunk);
		}

		public static void lua_createtable(LuaState luaState, int narr, int nrec)
		{
			Lua.LuaCreateTable(luaState, narr, nrec);
		}

		public static void lua_newtable(LuaState luaState)
		{
			lua_createtable(luaState, 0, 0);
		}

		public static int luaL_dofile(LuaState luaState, string fileName)
		{
			int result = Lua.LuaLLoadFile(luaState, fileName);
			if (result != 0)
				return result;

			return Lua.LuaPCall(luaState, 0, -1, 0);
		}

		public static void lua_getglobal(LuaState luaState, string name) 
		{
			lua_pushstring(luaState, name);
			Lua.LuaGetTable(luaState, (int) LuaIndexes.LUA_GLOBALSINDEX);
		}

		public static void lua_setglobal(LuaState luaState, string name)
		{
			lua_pushstring(luaState,name);
			lua_insert(luaState,-2);
			lua_settable(luaState, (int) LuaIndexes.LUA_GLOBALSINDEX);
		}

		public static void lua_settop(LuaState luaState, int newTop)
		{
			Lua.LuaSetTop(luaState, newTop);
		}

		public static void lua_pop(LuaState luaState, int amount)
		{
			lua_settop(luaState, -(amount) - 1);
		}

		public static void lua_insert(LuaState luaState, int newTop)
		{
			Lua.LuaInsert(luaState, newTop);
		}

		public static void lua_remove(LuaState luaState, int index)
		{
			Lua.LuaRemove(luaState, index);
		}

		public static void lua_gettable(LuaState luaState, int index)
		{
			Lua.LuaGetTable(luaState, index);
		}


		public static void lua_rawget(LuaState luaState, int index)
		{
			Lua.LuaRawGet(luaState, index);
		}


		public static void lua_settable(LuaState luaState, int index)
		{
			Lua.LuaSetTable(luaState, index);
		}


		public static void lua_rawset(LuaState luaState, int index)
		{
			Lua.LuaRawSet(luaState, index);
		}


		public static void lua_setmetatable(LuaState luaState, int objIndex)
		{
			Lua.LuaSetMetatable(luaState, objIndex);
		}


		public static int lua_getmetatable(LuaState luaState, int objIndex)
		{
			return Lua.LuaGetMetatable(luaState, objIndex);
		}


		public static int lua_equal(LuaState luaState, int index1, int index2)
		{
			return Lua.LuaEqual(luaState, index1, index2);
		}


		public static void lua_pushvalue(LuaState luaState, int index)
		{
			Lua.LuaPushValue(luaState, index);
		}


		public static void lua_replace(LuaState luaState, int index)
		{
			Lua.LuaReplace(luaState, index);
		}

		public static int lua_gettop(LuaState luaState)
		{
			return Lua.LuaGetTop(luaState);
		}


		public static LuaTypes lua_type(LuaState luaState, int index)
		{
			return (LuaTypes) Lua.LuaType(luaState, index);
		}

		public static bool lua_isnil(LuaState luaState, int index)
		{
			return lua_type(luaState,index)==LuaTypes.LUA_TNIL;
		}

		public static bool lua_isnumber(LuaState luaState, int index)
		{
			return lua_type(luaState,index)==LuaTypes.LUA_TNUMBER;
		}

		public static bool lua_isboolean(LuaState luaState, int index) 
		{
			return lua_type(luaState,index)==LuaTypes.LUA_TBOOLEAN;
		}

		public static int luaL_ref(LuaState luaState, int registryIndex)
		{
			return Lua.LuaLRef(luaState, registryIndex);
		}

		public static int lua_ref(LuaState luaState, int lockRef)
		{
			if(lockRef!=0) 
			{
				return luaL_ref(luaState, (int) LuaIndexes.LUA_REGISTRYINDEX);
			} 
			else return 0;
		}

		public static void lua_rawgeti(LuaState luaState, int tableIndex, int index)
		{
			Lua.LuaRawGetI(luaState, tableIndex, index);
		}

		public static void lua_rawseti(LuaState luaState, int tableIndex, int index)
		{
			Lua.LuaRawSetI(luaState, tableIndex, index);
		}


		public static object lua_newuserdata(LuaState luaState, int size)
		{
			return Lua.LuaNewUserData(luaState, (uint)size);
		}


		public static object lua_touserdata(LuaState luaState, int index)
		{
			return Lua.LuaToUserData(luaState, index);
		}

		public static void lua_getref(LuaState luaState, int reference)
		{
			lua_rawgeti(luaState, (int) LuaIndexes.LUA_REGISTRYINDEX,reference);
		}

		// Unwrapped
		// public static void luaL_unref(LuaState luaState, int registryIndex, int reference);

		public static void lua_unref(LuaState luaState, int reference) 
		{
			Lua.LuaLUnref(luaState, (int) LuaIndexes.LUA_REGISTRYINDEX,reference);
		}

		public static bool lua_isstring(LuaState luaState, int index)
		{
			return Lua.LuaIsString(luaState, index) != 0;
		}


		public static bool lua_iscfunction(LuaState luaState, int index)
		{
			return Lua.LuaIsCFunction(luaState, index);
		}

		public static void lua_pushnil(LuaState luaState)
		{
			Lua.LuaPushNil(luaState);
		}



		public static void lua_call(LuaState luaState, int nArgs, int nResults)
		{
			Lua.LuaCall(luaState, nArgs, nResults);
		}

		public static int lua_pcall(LuaState luaState, int nArgs, int nResults, int errfunc)
		{
			return Lua.LuaPCall(luaState, nArgs, nResults, errfunc);
		}

		// public static int lua_rawcall(LuaState luaState, int nArgs, int nResults)

		public static LuaNativeFunction lua_tocfunction(LuaState luaState, int index)
		{
			return Lua.LuaToCFunction(luaState, index);
		}

		public static double lua_tonumber(LuaState luaState, int index)
		{
			return Lua.LuaToNumber(luaState, index);
		}


		public static bool lua_toboolean(LuaState luaState, int index)
		{
			return Lua.LuaToBoolean(luaState, index) != 0;
		}

		// unwrapped
		// was out strLen
		// public static IntPtr lua_tolstring(LuaState luaState, int index, [Out] int ^ strLen);

		public static string lua_tostring(LuaState luaState, int index)
		{
			// FIXME use the same format string as lua i.e. LUA_NUMBER_FMT
			LuaTypes t = lua_type(luaState,index);
			
			if(t == LuaTypes.LUA_TNUMBER)
				return string.Format("{0}", lua_tonumber(luaState, index));
			else if(t == LuaTypes.LUA_TSTRING)
			{
				uint strlen;
				return Lua.LuaToLString(luaState, index, out strlen).ToString();
			}
			else if(t == LuaTypes.LUA_TNIL)
				return null;			// treat lua nulls to as C# nulls
			else
				return "0";	// Because luaV_tostring does this
		}

        public static void lua_atpanic(LuaState luaState, LuaNativeFunction panicf)
		{
			Lua.LuaAtPanic(luaState, (LuaNativeFunction)panicf);
		}

		public static void lua_pushstdcallcfunction(LuaState luaState, LuaNativeFunction function)
		{
			Lua.LuaPushCFunction(luaState, function);
		}


#if false
		// not yet implemented
        public static void lua_atlock(LuaState luaState, LuaCSFunction^ lockf)
		{
			IntPtr p = Marshal::GetFunctionPointerForDelegate(lockf);
			Lua.lua_atlock(luaState, (lua_CFunction) p.ToPointer());
		}

        public static void lua_atunlock(LuaState luaState, LuaCSFunction^ unlockf);
#endif

		public static void lua_pushnumber(LuaState luaState, double number)
		{
			Lua.LuaPushNumber(luaState, number);
		}

		public static void lua_pushboolean(LuaState luaState, bool value)
		{
			Lua.LuaPushBoolean(luaState, value ? 1 : 0);
		}

#if false
		// Not yet wrapped
		public static void lua_pushlstring(LuaState luaState, string str, int size)
		{
			char *cs = (char *) Marshal::StringToHGlobalAnsi(str).ToPointer();

			//

			Marshal::FreeHGlobal(IntPtr(cs));
		}
#endif


		public static void lua_pushstring(LuaState luaState, string str)
		{
			Lua.LuaPushString(luaState, str);
		}


		public static int luaL_newmetatable(LuaState luaState, string meta)
		{
			return Lua.LuaLNewMetatable(luaState, meta);
		}


		public static void lua_getfield(LuaState luaState, int stackPos, string meta)
		{
			Lua.LuaGetField(luaState, stackPos, meta);
		}

		public static void luaL_getmetatable(LuaState luaState, string meta)
		{
			lua_getfield(luaState, (int) LuaIndexes.LUA_REGISTRYINDEX, meta);
		}

		public static object luaL_checkudata(LuaState luaState, int stackPos, string meta)
		{
			return Lua.LuaLCheckUData(luaState, stackPos, meta);
		}

		public static bool luaL_getmetafield(LuaState luaState, int stackPos, string field)
		{
			return Lua.LuaLGetMetafield(luaState, stackPos, field) != 0;
		}

		// wrapper not yet implemented
		// public static int lua_load(LuaState luaState, LuaChunkReader chunkReader, ref ReaderInfo data, string chunkName);

		public static int luaL_loadbuffer(LuaState luaState, string buff, string name)
		{
			return Lua.LuaLLoadBuffer(luaState, buff, (uint)buff.Length, name);
		}

		public static int luaL_loadfile(LuaState luaState, string filename)
		{
			return Lua.LuaLLoadFile(luaState, filename);
		}

		public static void lua_error(LuaState luaState)
		{
			Lua.LuaError(luaState);
		}


		public static bool lua_checkstack(LuaState luaState,int extra)
		{
			return Lua.LuaCheckStack(luaState, extra) != 0;
		}


		public static int lua_next(LuaState luaState,int index)
		{
			return Lua.LuaNext(luaState, index);
		}




		public static void lua_pushlightuserdata(LuaState luaState, object udata)
		{
			Lua.LuaPushLightUserData(luaState, udata);
		}

		public static int luanet_rawnetobj(LuaState luaState,int obj)
		{
            byte[] bytes = lua_touserdata(luaState, obj) as byte[];
            return fourBytesToInt(bytes);
		}

        // Starting with 5.1 the auxlib version of checkudata throws an exception if the type isn't right
		// Instead, we want to run our own version that checks the type and just returns null for failure
		private static object checkudata_raw(LuaState L, int ud, string tname)
		{
			object p = Lua.LuaToUserData(L, ud);

			if (p != null) 
			{  /* value is a userdata? */
				if (Lua.LuaGetMetatable(L, ud) != 0) 
				{ 
					bool isEqual;

					/* does it have a metatable? */
					Lua.LuaGetField(L, (int) LuaIndexes.LUA_REGISTRYINDEX, tname);  /* get correct metatable */

					isEqual = Lua.LuaRawEqual(L, -1, -2) != 0;

					// NASTY - we need our own version of the lua_pop macro
					// lua_pop(L, 2);  /* remove both metatables */
					Lua.LuaSetTop(L, -(2) - 1);

					if (isEqual)   /* does it have the correct mt? */
						return p;
				}
			}
		  
	        return null;
		}


        public static int luanet_checkudata(LuaState luaState, int ud, string tname)
		{
			object udata=checkudata_raw(luaState, ud, tname);

            if (udata != null) return fourBytesToInt(udata as byte[]);
		    return -1;
		}


		public static bool luaL_checkmetatable(LuaState luaState,int index)
		{
			bool retVal=false;

			if(lua_getmetatable(luaState,index)!=0) 
			{
				lua_pushlightuserdata(luaState, tag);
				lua_rawget(luaState, -2);
				retVal = !lua_isnil(luaState, -1);
				lua_settop(luaState, -3);
			}
			return retVal;
		}

		public static object luanet_gettag() 
		{
			return tag;
		}

		public static void luanet_newudata(LuaState luaState,int val)
		{
			byte[] userdata = lua_newuserdata(luaState, sizeof(int)) as byte[];
            intToFourBytes(val, userdata);
		}

		public static int luanet_tonetobject(LuaState luaState,int index)
		{
			byte[] udata;

			if(lua_type(luaState,index)==LuaTypes.LUA_TUSERDATA) 
			{
				if(luaL_checkmetatable(luaState, index)) 
				{
				    udata=lua_touserdata(luaState,index) as byte[];
				    if(udata!=null)
                    {
				    	return fourBytesToInt(udata);
                    }
				}

			    udata=checkudata_raw(luaState,index, "luaNet_class") as byte[];
			    if(udata!=null) return fourBytesToInt(udata);

			    udata=checkudata_raw(luaState,index, "luaNet_searchbase") as byte[];
			    if(udata!=null) return fourBytesToInt(udata);

			    udata=checkudata_raw(luaState,index, "luaNet_function") as byte[];
			    if(udata!=null) return fourBytesToInt(udata);
			}
			return -1;
		}

        private static int fourBytesToInt(byte[] bytes)
        {
            return bytes[0] + (bytes[1] << 8) + (bytes[2] << 16) + (bytes[3] << 24);
        }

        private static void intToFourBytes(int val, byte[] bytes)
        {
			// gfoot: is this really a good idea?
            bytes[0] = (byte)val;
            bytes[1] = (byte)(val >> 8);
            bytes[2] = (byte)(val >> 16);
            bytes[3] = (byte)(val >> 24);
        }
	};
}
