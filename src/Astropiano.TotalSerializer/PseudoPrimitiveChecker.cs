using System;

namespace Astropiano.TotalSerializer
{
    public static class PseudoPrimitiveChecker
    {
        public static bool IsPseudoPrimitive(Type type)
        {
            if (type == typeof(DateTime) 
                || type == typeof(DateTimeOffset)
                || type == typeof(TimeSpan)
                || type == typeof(Guid)
                || typeof(Type).IsAssignableFrom(type) // System.RuntimeType is a concrete class that derives from System.Type (https://stackoverflow.com/questions/5737840/whats-the-difference-between-system-type-and-system-runtimetype-in-c)
                || type.IsEnum)
            {
                return true;
            }

            Type underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType == null)
            {
                return false;
            }
            else
            {
                return IsPseudoPrimitive(underlyingType);
            }
        }
    }
}