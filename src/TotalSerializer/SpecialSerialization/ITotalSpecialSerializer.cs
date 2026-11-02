using System;
using System.Collections.Generic;
using TotalSerializer.ReflectionTools;
using TotalSerializer.Interfaces;

namespace TotalSerializer.SpecialSerialization
{
    internal interface ITotalSpecialSerializer
    {
        IList<Type> GetHandledTypes();

        IList<Type> GetHandledGenericBaseTypes();

        Type GetReplacementType();
        
        void SerializeInstanceInternal(SpecialSerializationParameters specialSerializationParameters, Type type,
            object value);

        object DeserializeInstanceInternal(ref object replacementInstance, IFactory factory, 
            FieldsReadWriteCache fieldsReadWriteCache, ReferenceStorage referenceStorage);

        void DeserializeInstanceAndWriteInternal(ref object instance, ref TotalSpecialSerializerReplacement replacementInstance, 
            IFactory factory, FieldsReadWriteCache fieldsReadWriteCache, string propertyName, ReferenceStorage referenceStorage);
    }
}