using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TotalSerializer.ReflectionTools
{
    public static class FieldInfoCache
    {
        private static ConcurrentDictionary<Type, FieldInfo[]> _fieldInfoArrayDictionary = new ConcurrentDictionary<Type, FieldInfo[]>();
        
        private static ConcurrentDictionary<MemberKey, FieldInfo> _fieldInfoDictionary = new ConcurrentDictionary<MemberKey, FieldInfo>();
        
        public static void Clear()
        {
            _fieldInfoArrayDictionary.Clear();
            _fieldInfoDictionary.Clear();
        }

        public static int FieldInfoCount()
        {
            return _fieldInfoDictionary.Count;
        }
        
        public static int FieldInfoArrayCount()
        {
            return _fieldInfoArrayDictionary.Count;
        }

        /// <summary>
        /// Returns non-static fieldInfos only.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FieldInfo[] GetFieldInfos(Type type)
        {
            if (!_fieldInfoArrayDictionary.TryGetValue(type, out var fieldInfos))
            {
                if (type.IsInterface)
                {
                    var considered = new List<Type>();
                    var queue = new Queue<Type>();
                    var propertyInfos = new List<FieldInfo>();
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

                        var typeProperties = subType.GetFields(
                            BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public |
                            BindingFlags.Instance);
                        var newPropertyInfos = typeProperties
                            .Where(x => !propertyInfos.Contains(x));
                        propertyInfos.InsertRange(0, newPropertyInfos);
                    }

                    fieldInfos = propertyInfos.ToArray();
                }
                else
                {
                    Type baseType = type.BaseType;
                    List<FieldInfo> propertyInfosOfBaseTypes = new List<FieldInfo>();

                    if (baseType != null && baseType != typeof(object))
                    {
                        propertyInfosOfBaseTypes.AddRange(GetFieldInfos(baseType));
                    }

                    propertyInfosOfBaseTypes.AddRange(type.GetFields(
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                        BindingFlags.DeclaredOnly));

                    fieldInfos = propertyInfosOfBaseTypes.ToArray();
                }

                _fieldInfoArrayDictionary.TryAdd(type, fieldInfos);
            }

            return fieldInfos;
        }

        // TODO: write unit-tests, especially for interfaces
        /// <summary>
        /// Does also return static fields.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static FieldInfo GetFieldInfo(Type type, string fieldName)
        {
            FieldInfo fieldInfo;
            MemberKey key = new MemberKey(type, fieldName);

            if (!_fieldInfoDictionary.TryGetValue(key, out fieldInfo))
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

                        fieldInfo = subType.GetField(
                            fieldName,
                            BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public |
                            BindingFlags.Instance | BindingFlags.Static);
                        if (fieldInfo != null)
                        {
                            return fieldInfo;
                        }
                    }
                }
                else
                {
                    // Here we don't need the flag 'DeclaredOnly' because we only need to check the base class in case we don't find it.
                    fieldInfo = type.GetField(
                        fieldName,
                        BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.Static); 

                    if (fieldInfo == null && type.BaseType != typeof(object) && type.BaseType != null)
                    {
                        fieldInfo = GetFieldInfo(type.BaseType, fieldName);
                    }
                }

                _fieldInfoDictionary.TryAdd(key, fieldInfo);
            }

            return fieldInfo;
        }
    }
}
