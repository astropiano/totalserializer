using System;
using System.Collections.Generic;
using Astropiano.ReflectionTools;
using Astropiano.TotalSerializer.Interfaces;

namespace Astropiano.TotalSerializer.SpecialSerialization
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