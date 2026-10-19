using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Astropiano.ReflectionTools;
using Astropiano.TotalSerializer.Interfaces;

namespace Astropiano.TotalSerializer.SpecialSerialization
{
    internal class PropertyInfoSerializer : TotalSpecialSerializer<PropertyInfoSerializer.PropertyInfoData>
    {
        private ConcurrentDictionary<string, ConcurrentDictionary<string, PropertyInfo>> _cache = new ConcurrentDictionary<string, ConcurrentDictionary<string, PropertyInfo>>();
        
        public override PropertyInfoData SerializeInstance(Type type, object value)
        {
            PropertyInfoData data = new PropertyInfoData();
            PropertyInfo propertyInfo = (PropertyInfo)value;
            data.Name = propertyInfo.Name;
            data.Type = TypeNameWriter.GetTypeNameForJson(propertyInfo.DeclaringType);

            return data;
        }

        public override object DeserializeInstance(PropertyInfoData propertyInfoData, IFactory factory)
        {
            ConcurrentDictionary<string, PropertyInfo> cacheEntry = _cache.GetOrAdd(propertyInfoData.Type, _ =>
            {
                Type typeForPropertyInfo = TypeGenerator.GetType(propertyInfoData.Type);
                ConcurrentDictionary<string, PropertyInfo> concurrentDictionary = new ConcurrentDictionary<string, PropertyInfo>();
                PropertyInfo[] properties = typeForPropertyInfo.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    concurrentDictionary.TryAdd(property.Name, property);
                }

                return concurrentDictionary;
            });

            PropertyInfo propertyInfo = cacheEntry.GetOrAdd(propertyInfoData.Name,
                _ =>
                {
                    Type typeForPropertyInfo = TypeGenerator.GetType(propertyInfoData.Type);
                    PropertyInfo p = typeForPropertyInfo.GetProperty(propertyInfoData.Name);
                    return p;
                });

            return propertyInfo;
        }

        public override IList<Type> GetHandledTypes()
        {
            return new List<Type>() {typeof(PropertyInfo)};
        }
        
        
        
        /// <summary>
        /// Internal data object to contain serializable data of
        /// <see cref="PropertyInfo"/> to be able to serialize it.
        /// </summary>
        internal class PropertyInfoData :  TotalSpecialSerializerReplacement
        {
            /// <summary>
            /// The name of the PropertyInfo (<see cref="MemberInfo.Name"/>).
            /// </summary>
            public string Name;
        
            /// <summary>
            /// The string representation of the type (<see cref="System.Type"/>) where
            /// the PropertyInfo comes from.
            /// </summary>
            public string Type;
        }
    }
}