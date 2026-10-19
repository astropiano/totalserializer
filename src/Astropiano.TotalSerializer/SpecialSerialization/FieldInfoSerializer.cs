using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Astropiano.ReflectionTools;
using Astropiano.TotalSerializer.Interfaces;

namespace Astropiano.TotalSerializer.SpecialSerialization
{
    internal class FieldInfoSerializer : TotalSpecialSerializer<FieldInfoSerializer.FieldInfoData>
    {
        private ConcurrentDictionary<string, ConcurrentDictionary<string, FieldInfo>> _cache = new ConcurrentDictionary<string, ConcurrentDictionary<string, FieldInfo>>();
        
        public override FieldInfoData SerializeInstance(Type type, object value)
        {
            FieldInfoData data = new FieldInfoData();
            FieldInfo propertyInfo = (FieldInfo)value;
            data.Name = propertyInfo.Name;
            data.Type = TypeNameWriter.GetTypeNameForJson(propertyInfo.DeclaringType);
            return data;
        }

        public override object DeserializeInstance(FieldInfoData fieldInfoData, IFactory factory)
        {
            ConcurrentDictionary<string, FieldInfo> cacheEntry = _cache.GetOrAdd(fieldInfoData.Type, _ =>
            {
                Type typeForPropertyInfo = TypeGenerator.GetType(fieldInfoData.Type);
                ConcurrentDictionary<string, FieldInfo> concurrentDictionary = new ConcurrentDictionary<string, FieldInfo>();
                FieldInfo[] fields = typeForPropertyInfo.GetFields();
                foreach (FieldInfo field in fields)
                {
                    concurrentDictionary.TryAdd(field.Name, field);
                }

                return concurrentDictionary;
            });

            FieldInfo fieldInfo = cacheEntry.GetOrAdd(fieldInfoData.Name,
                _ =>
                {
                    Type typeForPropertyInfo = TypeGenerator.GetType(fieldInfoData.Type);
                    FieldInfo f = typeForPropertyInfo.GetField(fieldInfoData.Name);
                    return f;
                });

            return fieldInfo;
        }

        public override IList<Type> GetHandledTypes()
        {
            return new List<Type> {typeof(FieldInfo)};
        }
        
        
        
        /// <summary>
        /// Internal data object to contain serializable data of
        /// <see cref="FieldInfo"/> to be able to serialize it.
        /// </summary>
        internal class FieldInfoData : TotalSpecialSerializerReplacement
        {
            /// <summary>
            /// The name of the FieldInfo (<see cref="FieldInfo.Name"/>).
            /// </summary>
            public string Name;
        
            /// <summary>
            /// The string representation of the type (<see cref="System.Type"/>) where
            /// the FieldInfo comes from.
            /// </summary>
            public string Type;
        }
    }
}