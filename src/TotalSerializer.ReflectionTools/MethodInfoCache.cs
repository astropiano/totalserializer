using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace TotalSerializer.ReflectionTools
{
    public class MethodInfoCache
    {
        private static ConcurrentDictionary<MemberKey, MethodInfo> _methodInfos = new ConcurrentDictionary<MemberKey, MethodInfo>();

        public static void Clear()
        {
            _methodInfos.Clear();
        }

        public static int Count()
        {
            return _methodInfos.Count;
        }

        public static MethodInfo GetMethodInfo(Type type, string methodName)
        {
            MethodInfo methodInfo;
            MemberKey key = new MemberKey(type, methodName);
            if (!_methodInfos.TryGetValue(key, out methodInfo))
            {
                methodInfo = type.GetMethod(methodName, 
                    BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                _methodInfos.TryAdd(key, methodInfo);
            }

            return methodInfo;
        }

        public static MethodInfo GetMethodInfo(Type type, string methodName, params Type[] parameterTypes)
        {
            MethodInfo methodInfo;
            string parametersKeyParts = "";
            foreach (Type parameterType in parameterTypes)
            {
                parametersKeyParts  = "%" + TypeNameWriter.GetTypeNameForJson(parameterType);
            }
            MemberKey key = new MemberKey(type, methodName + parametersKeyParts);
            if (!_methodInfos.TryGetValue(key, out methodInfo))
            {
                MethodInfo[] methodInfoArray = type.GetMethods(BindingFlags.FlattenHierarchy | BindingFlags.Public |
                                                               BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                methodInfo = methodInfoArray.Where((m) =>
                {
                    if (m.Name != methodName) return false;
                    var parameters = m.GetParameters();
                    if ((parameterTypes.Length == 0))
                        return parameters.Length == 0;
                    if (parameters.Length != parameterTypes.Length)
                        return false;
                    for (int i = 0; i < parameterTypes.Length; i++)
                    {
                        if (parameters[i].ParameterType != parameterTypes[i])
                            return false;
                    }
                    return true;
                }).FirstOrDefault();

                if (methodInfo == null  && type.BaseType != typeof(object) && type.BaseType != null)
                {
                    methodInfo = GetMethodInfo(type.BaseType, methodName, parameterTypes);
                }
                
                _methodInfos.TryAdd(key, methodInfo);
            }

            return methodInfo;
        }
    }
}
