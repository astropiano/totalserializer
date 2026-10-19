using System;
using System.Collections;
using System.Collections.Generic;
using Astropiano.TotalSerializer.Interfaces;

namespace Astropiano.TotalSerializer.SpecialSerialization
{
    public class DictionarySerializer : TotalSpecialSerializer<DictionarySerializer.DictionaryData>
    { 
        
        public class DictionaryData  :  TotalSpecialSerializerReplacement
        {
            public class KeyValuePair
            {
                public KeyValuePair(object key, object value)
                {
                    Key = key;
                    Value = value;
                }

                public object Key { get; private set; }
                public object Value { get; private set; }
            }

            public DictionaryData(IDictionary dictionary, Type type)
            {
                Type = type;
                var keys = dictionary.Keys;
                int length = keys.Count;
                KeyValuePairs = new KeyValuePair[length];
                int i = 0;
                foreach (var key in keys)
                {
                    KeyValuePairs[i] = new KeyValuePair(key, dictionary[key]);
                    i++;
                }
            }
            
            public KeyValuePair[] KeyValuePairs { get; private set; }

            public Type Type { get; private set; }
        }

        public override IList<Type> GetHandledTypes()
        {
            return new List<Type>()
            {
                typeof(Dictionary<,>)
            };
        }
        
        public override IList<Type> GetHandledGenericBaseTypes()
        {
            return new List<Type>() {typeof(Dictionary<,>)};
        }

        public override DictionaryData SerializeInstance(Type type, object value)
        {
            IDictionary dictionary = value as IDictionary;
            if (dictionary != null)
            {
                DictionaryData data = new DictionaryData(dictionary, dictionary.GetType());
                return data;
            }
            throw new ArgumentException("Expected value to be of type Dictionary.");
        }

        public override object DeserializeInstance(DictionaryData replacementInstance, IFactory factory)
        {
            //TODO: value types of dictionaries seem not to be correct. Here an int becomes a string... :-(
            IDictionary dictionary = factory.Activate(replacementInstance.Type) as IDictionary;
            if (dictionary != null)
            {
                foreach (var keyValuePair in replacementInstance.KeyValuePairs)
                {
                    dictionary.Add(keyValuePair.Key, keyValuePair.Value);
                }

                return dictionary;
            }
            throw new Exception("Expected IDictionary instance from factory.");

        }
    }
}