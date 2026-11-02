using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TotalSerializer.ReflectionTools
{
    public class PropertyInfoCache
    {

        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyInfoArrayDictionary = new ();

        private static readonly ConcurrentDictionary<MemberKey, PropertyInfo> PropertyInfoDictionary = new ();

        public static void Clear()
        {
            PropertyInfoArrayDictionary.Clear();
            PropertyInfoDictionary.Clear();
        }

        public static int PropertyInfoCount()
        {
            return PropertyInfoDictionary.Count;
        }

        public static int PropertyInfoArrayCount()
        {
            return PropertyInfoArrayDictionary.Count;
        }

        public static PropertyInfo[] GetPropertyInfos(Type type)
        {
            PropertyInfo[] propertyInfos;

            if (!PropertyInfoArrayDictionary.TryGetValue(type, out propertyInfos))
            {
                if (type.IsInterface)
                {
                    var considered = new List<Type>();
                    var queue = new Queue<Type>();
                    var interfacePropertyInfos = new List<PropertyInfo>();
                    considered.Add(type);
                    queue.Enqueue(type);
                    while (queue.Count > 0)
                    {
                        var subType = queue.Dequeue();
                        foreach (var subInterface in subType.GetInterfaces())
                        {
                            if (considered.Contains(subInterface))
                            {
                                continue;
                            }

                            considered.Add(subInterface);
                            queue.Enqueue(subInterface);
                        }

                        var typeProperties = subType.GetProperties(
                            BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public |
                            BindingFlags.Instance);
                        var newPropertyInfos = typeProperties
                            .Where(x => !interfacePropertyInfos.Contains(x));
                        interfacePropertyInfos.InsertRange(0, newPropertyInfos);
                    }

                    propertyInfos = interfacePropertyInfos.ToArray();
                }
                else
                {
                    Type baseType = type.BaseType;
                    List<PropertyInfo> propertyInfosOfBaseTypes = new List<PropertyInfo>();

                    if (baseType != null && baseType != typeof(object))
                    {
                        propertyInfosOfBaseTypes.AddRange(GetPropertyInfos(baseType));
                    }

                    propertyInfosOfBaseTypes.AddRange(type.GetProperties(
                        BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.DeclaredOnly));

                    propertyInfos = propertyInfosOfBaseTypes.ToArray();
                }

                PropertyInfoArrayDictionary.TryAdd(type, propertyInfos);
            }

            return propertyInfos;
        }

        // TODO: write unit-tests, especially for interfaces
        public static PropertyInfo GetPropertyInfo(Type type, string fieldName)
        {
            PropertyInfo propertyInfo;
            MemberKey key = new MemberKey(type, fieldName);

            if (!PropertyInfoDictionary.TryGetValue(key, out propertyInfo))
            {
                if (type.IsInterface)
                {
                    var considered = new List<Type>();
                    var queue = new Queue<Type>();
                    considered.Add(type);
                    queue.Enqueue(type);
                    while (queue.Count > 0)
                    {
                        var subType = queue.Dequeue();
                        foreach (var subInterface in subType.GetInterfaces())
                        {
                            if (considered.Contains(subInterface))
                            {
                                continue;
                            }

                            considered.Add(subInterface);
                            queue.Enqueue(subInterface);
                        }

                        propertyInfo = subType.GetProperty(
                            fieldName,
                            BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public |
                            BindingFlags.Instance);
                        if (propertyInfo != null)
                        {
                            return propertyInfo;
                        }
                    }
                }
                else
                {
                    propertyInfo = type.GetProperty(
                        fieldName,
                        BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags
                            .Instance); // Here we don't need the flag 'DeclaredOnly' because we only need to check the base class in case we don't find it.

                    if (propertyInfo == null && type.BaseType != typeof(object) && type.BaseType != null)
                    {
                        propertyInfo = GetPropertyInfo(type.BaseType, fieldName);
                    }
                }

                PropertyInfoDictionary.TryAdd(key, propertyInfo);
            }

            return propertyInfo;
        }
    }
}
