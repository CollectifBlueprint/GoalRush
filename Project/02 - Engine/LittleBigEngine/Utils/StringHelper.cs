using System;

namespace LBE
{
    public static class StringHelper
    {
        public const String DefaultString = "null";
        public static string NullSafeToString(this object obj)
        {
            return obj != null ? obj.ToString() : DefaultString;
        }
    }
}
