using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LBE.Debug
{

    public static class Colors
    {
        private static int index = 1;

        private static readonly PropertyInfo[] array =
           typeof(Color).GetProperties(BindingFlags.Public |
                                       BindingFlags.Static)
                        .Where(prop => prop.PropertyType == typeof(Color))
                        .ToArray();

        private static Color GetColorAtIndex()
        {
            return (Color)array[index].GetValue(null, null);
        }

        private static Color GetColorAt(int p_index)
        {
            return (Color)array[p_index].GetValue(null, null);
        }

     
        public static Color Next()
        {
            index++;
            if (index >= array.Length)
                index = 1;

            return GetColorAtIndex();
        }

        public static Color Previous()
        {
            index--;
            if (index < 1)
                index = array.Length - 1;

            return GetColorAtIndex();
        }

        public static Color Current()
        {
            Console.WriteLine(DebugStr());
            return GetColorAtIndex();
        }

        public static Color Random()
        {
            index = (int) Engine.Random.Next(0, array.Length);
            Console.WriteLine(DebugStr());
            return GetColorAtIndex();

        }

        public static string DebugStr()
        {
            Color c = (Color)array[index].GetValue(null, null);
            return "Color " + index + " / " + array.Length + " : " + c.ToString();
        }
    }

    class ToggleColorDebug : ICommand
    {
        public void Execute()
        {
            Engine.Debug.Flags.ColorEdit = !Engine.Debug.Flags.ColorEdit; 
        }
    }
}
