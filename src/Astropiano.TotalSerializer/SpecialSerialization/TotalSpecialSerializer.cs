using System;
using System.Collections.Generic;
using Astropiano.ReflectionTools;
using Astropiano.TotalSerializer.Interfaces;

namespace Astropiano.TotalSerializer.SpecialSerialization
{
    /// <summary>
    /// Please derive from this class to handle the serialization and deserialization of special types that would
    /// otherwise not be supported by TotalSerializer. This is done by providing a so called replacement type
    /// that will be serialized instead of the original type.
    /// </summary>
    /// <typeparam name="TReplacement"></typeparam>
    public abstract class TotalSpecialSerializer<TReplacement> :  ITotalSpecialSerializer where TReplacement :  TotalSpecialSerializerReplacement
    {
        public abstract IList<Type> GetHandledTypes();

        public virtual IList<Type> GetHandledGenericBaseTypes()
        {
            return null;
        }

        public Type GetReplacementType()
        {
            return typeof(TReplacement);
        }
        

        public abstract TReplacement SerializeInstance(Type type, object value);

        void ITotalSpecialSerializer.SerializeInstanceInternal(SpecialSerializationParameters specialSerializationParameters, Type type, object value)
        {
            //look whether we already have written
            long objId;
            if (!specialSerializationParameters.InstanceMemory.CheckIfFirstAndAddInstance(
                value, 
                out objId))
            {
                //We already have serialized this instance (or the placeholder instance for this instance)
                specialSerializationParameters.Writer.StartObject(); 
                specialSerializationParameters.Writer.WriteObjRef(objId);
                specialSerializationParameters.Writer.EndObject();
            }
            else
            {
                //We actually need to serialize this instance
                specialSerializationParameters.OriginalObjId = objId; //the placeholder instance should get written with this $id
                TReplacement replacementInstance =  SerializeInstance(type, value);
                replacementInstance.OriginalType = type;
                WriteInstance(specialSerializationParameters, replacementInstance);
            }
        }

        public abstract object DeserializeInstance(TReplacement replacementInstance, IFactory factory);

        object ITotalSpecialSerializer.DeserializeInstanceInternal(ref object replacementInstance, IFactory factory, 
            FieldsReadWriteCache fieldsReadWriteCache, ReferenceStorage referenceStorage)
        {
            TotalSpecialSerializerReplacement replacementInstanceCasted =
                replacementInstance as TotalSpecialSerializerReplacement;
            if (replacementInstanceCasted != null)
            {
                object deserializedInstance = DeserializeInstance((TReplacement)replacementInstance, factory);
            
                referenceStorage.Set(replacementInstanceCasted.RefId, deserializedInstance, fieldsReadWriteCache);
                
                return deserializedInstance;
            }
            
            throw new Exception($"Expected instance deriving from {nameof(TotalSpecialSerializerReplacement)}.");
            
        }

        void ITotalSpecialSerializer.DeserializeInstanceAndWriteInternal(ref object instance, 
            ref TotalSpecialSerializerReplacement replacementInstance, IFactory factory,
            FieldsReadWriteCache fieldsReadWriteCache, string propertyName, ReferenceStorage referenceStorage)
        {
            object deserializedInstance =  DeserializeInstance((TReplacement)replacementInstance, factory);
            
            fieldsReadWriteCache.Set(ref instance, propertyName, ref deserializedInstance);
            referenceStorage.Set(replacementInstance.RefId, deserializedInstance, fieldsReadWriteCache);
        }

        private void WriteInstance(SpecialSerializationParameters specialSerializationParameters, TReplacement replacementType)
        {
            specialSerializationParameters.SerializerCallBack(
                specialSerializationParameters,
                typeof(TReplacement), 
                replacementType);
        }
    }
}