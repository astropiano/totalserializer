using System;

namespace Astropiano.TotalSerializer
{
    public static class MethodChecker
    {
        public static bool IsDelegate(Type type)
        {
            if (typeof(Delegate).IsAssignableFrom(type))
            {
                return true;
            }

            return false;
        }

        public static bool IsMulticastDelegate(Type type)
        {
            if (typeof(MulticastDelegate).IsAssignableFrom(type))
            {
                return true;
            }
            return false;
        }
    }
}