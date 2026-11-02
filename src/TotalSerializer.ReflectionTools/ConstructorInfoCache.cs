using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace TotalSerializer.ReflectionTools
{
    public static class ConstructorInfoCache
    {
        private static ConcurrentDictionary<Type, ConstructorInfo> _parameterLessConstructors = new ConcurrentDictionary<Type, ConstructorInfo>();
        
        private static ConcurrentDictionary<MemberKey, ConstructorInfo> _constructors = new ConcurrentDictionary<MemberKey, ConstructorInfo>();


        public static ConstructorInfo GetParameterLessConstructorInfo(Type type)
        {
            if (!_parameterLessConstructors.TryGetValue(type, out var constructorInfo))
            {
                constructorInfo = type.GetConstructor(Type.EmptyTypes);
                _parameterLessConstructors.TryAdd(type, constructorInfo);
            }

            return constructorInfo;
        }

        public static ConstructorInfo GetConstructor(Type type, Type[] parameterTypes)
        {
            string memberKeyPropertyName = "";
            foreach (var parameterType in parameterTypes)
            {
                memberKeyPropertyName += "%" + parameterType.Name;
            }
            
            MemberKey key = new MemberKey(type, memberKeyPropertyName);
                
            if (!_constructors.TryGetValue(key, out var constructorInfo))
            {
                // docs.microsoft.com/en-us/dotnet/api/system.type.getconstructor
                // "You must specify either BindingFlags.Instance or BindingFlags.Static in order to get a return."
                constructorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null,  parameterTypes, null);
                _constructors.TryAdd(key, constructorInfo);
            }

            return constructorInfo;
        }
    }
}
