using System;
using System.Collections.Generic;
using Astropiano.TotalSerializer.Interfaces;

namespace Astropiano.TotalSerializer.SpecialSerialization
{
    public abstract class TypedTotalSpecialSerializer<THandled, TReplacement> : TotalSpecialSerializer<TReplacement> where TReplacement : TotalSpecialSerializerReplacement where THandled : class
    {
        public sealed override IList<Type> GetHandledTypes()
        {
            return new List<Type>(){typeof(THandled)};
        }

        public abstract TReplacement SerializeInstanceTyped(Type type, THandled value);
        
        public abstract THandled DeserializeInstanceTyped(TReplacement replacementInstance, IFactory factory);


        public sealed override TReplacement SerializeInstance(Type type, object value)
        {
            THandled handledValue = value as THandled;
            if (handledValue != null)
            {
                return SerializeInstanceTyped(type, handledValue);
            }
            
            throw new Exception($"Unexpected type. This serializer should only handle {typeof(THandled)}");
        }

        public sealed override object DeserializeInstance(TReplacement replacementInstance, IFactory factory)
        {
            return DeserializeInstanceTyped(replacementInstance, factory);
        }
    }
}