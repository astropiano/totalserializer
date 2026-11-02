
using System.Collections.Generic;
using TotalSerializer.Debug;

namespace TotalSerializer.SpecialSerialization
{
    public class SpecialSerializationParameters
    {
        internal TotalSerializer.TotalSerializeObject SerializerCallBack;
        internal JsonWriter Writer;
        internal JsonWriteInstanceMemory InstanceMemory;
        internal Stack<IProcessingLevel> ProcessingLevels;
        internal long? OriginalObjId;



        internal SpecialSerializationParameters(
            TotalSerializer.TotalSerializeObject serializerCallBack, 
            JsonWriter writer, JsonWriteInstanceMemory instanceMemory, Stack<IProcessingLevel> processingLevels)
        {
            SerializerCallBack = serializerCallBack;
            Writer = writer;
            InstanceMemory = instanceMemory;
            ProcessingLevels = processingLevels;
        }
    }
}