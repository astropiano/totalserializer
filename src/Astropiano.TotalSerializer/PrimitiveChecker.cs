using System;

namespace Astropiano.TotalSerializer
{
    public static class PrimitiveChecker
    {
        public static bool IsPrimitive(Type type)
        {
            if (type == null)
            {
                return false;
            }
            
            if (type.IsPrimitive || type == typeof(decimal) || type == typeof(string))
            {
                return true;
            }

            Type underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType == null)
            {
                return false;
            }

            return IsPrimitive(underlyingType);
        }

        public static bool IsUnsupportedType(Type type)
        {
            if (type == typeof(IntPtr) || type == typeof(UIntPtr))
            {
                return true;
            }

            return false;
        }

    }
}