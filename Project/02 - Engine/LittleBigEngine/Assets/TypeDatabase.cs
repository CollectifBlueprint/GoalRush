using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LBE.Assets
{
    public class TypeDatabase
    {
        Dictionary<String, Type> m_shortNameLookUpTable;

        public TypeDatabase()
        {
            m_shortNameLookUpTable = new Dictionary<string, Type>();
        }

        public void AddAssembly(Assembly ass)
        {
            foreach (var type in ass.GetExportedTypes())
            {
                //Multiple occurence of a type name will be marked as null
                if (!m_shortNameLookUpTable.ContainsKey(type.Name))
                    m_shortNameLookUpTable[type.Name] = type;
                else
                    m_shortNameLookUpTable[type.Name] = null;
            }
        }

        public Type GetType(String name)
        {
            //Try a fast look up
            if (m_shortNameLookUpTable.ContainsKey(name))
            {
                var type = m_shortNameLookUpTable[name];
                if (type != null)
                    return type;
            }
            //Browse all assemblies
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var a in assemblies)
            {
                foreach (var type in a.GetTypes())
                {
                    if (type.Name == name)
                        return type;
                }
            }

            Engine.Log.Exception(String.Format("Type \"{0}\" does not exist.", name));

            return null;
        }
    }
}
