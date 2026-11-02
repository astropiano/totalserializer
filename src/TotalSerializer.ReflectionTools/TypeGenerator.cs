using System;
using System.Reflection;

namespace TotalSerializer.ReflectionTools
{
    public static class TypeGenerator
    {
        public static Type GetType(string typename)
        {
            string[] split = typename.Split(',');
            
            if (split.Length == 2)
            {
                Assembly assembly;

#if !(DOTNET || PORTABLE40 || PORTABLE)
                // I don't like using obsolete methods but this is the only way
                // Assembly.Load won't check the GAC for a partial name
#pragma warning disable 618,612
                assembly = Assembly.LoadWithPartialName(split[1]);
#pragma warning restore 618,612
#elif DOTNET || PORTABLE
                assembly = Assembly.Load(new AssemblyName(split[1]));
#else
                assembly = Assembly.Load(split[1]);
#endif
                if (assembly != null) return assembly.GetType(split[0], true, true);
            }

            return Type.GetType(typename);
        }
    }
}
