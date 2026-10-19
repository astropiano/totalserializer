﻿using System;
using System.Collections;
// ReSharper disable once RedundantUsingDirective
using System.Collections.Concurrent;
using System.Collections.Generic;
 using System.IO;
 using System.Reflection;
using Astropiano.ReflectionTools;
using Astropiano.TotalSerializer.Attributes;
using Astropiano.TotalSerializer.Configuration;
using Astropiano.TotalSerializer.Debug;
using Astropiano.TotalSerializer.Exceptions;
using Astropiano.TotalSerializer.Interfaces;
using Astropiano.TotalSerializer.Logger;
using Astropiano.TotalSerializer.Methods;
using Astropiano.TotalSerializer.SpecialSerialization;

namespace Astropiano.TotalSerializer
{
    public class TotalSerializer
    {
        public delegate void TotalSerializeObject(
            SpecialSerializationParameters specialSerializationParameters, 
            Type type, 
            object value,
            long? originalObjId = null);
        
        private static ITotalSerializerLogger _logger = LoggerFactory.Create<TotalSerializer>();

        
        private static readonly FieldsReadWriteCache FieldsReadWriteCache = new FieldsReadWriteCache();
        
        private Config _config;

        public TotalSerializer(Config config = null)
        {
            if (config == null)
            {
                config = new Config();
            }
            _config = config;
        }
        
        public string Serialize(object obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using(StreamWriter streamWriter = new StreamWriter(memoryStream))
            using(StreamReader reader = new StreamReader( memoryStream ))
            {
                Serialize(obj, streamWriter);
                streamWriter.Flush();
                memoryStream.Position = 0;
                return reader.ReadToEnd();    
            }
        }

        public void Serialize(object obj, StreamWriter streamWriter)
        {
            lock (obj)
            {
                Type objType = obj.GetType();
                JsonWriter writer = new JsonWriter(_config, streamWriter);
                JsonWriteInstanceMemory instanceMemory = new JsonWriteInstanceMemory();
                Stack<IProcessingLevel> processingStack = new Stack<IProcessingLevel>();
                if (objType.IsArray)
                {
                    SerializeArray(objType, obj, writer, instanceMemory, processingStack);
                }
                else
                {
                    SerializeObject(objType, obj, writer, instanceMemory, processingStack);    
                    
                }
            }
        }

        public object Deserialize(string json)
        {
            object obj = null;
            JsonReader reader = new JsonReader(_config);
            reader.Read(json, ref obj);
            return obj;
        }


        public T Deserialize<T>(string json) where T: class
        {
            object obj = Deserialize(json);
            return obj as T;
        }

        public T Copy<T>(T obj)
        {
            string json = Serialize(obj);
            return (T)Deserialize(json);
        }
        
        private void SerializerObjectCallback(
            SpecialSerializationParameters specialSerializationParameters,
            Type type,
            object value,
            long? originalObjId = null
            )
        {
            SerializeObject(
                type, 
                value, 
                specialSerializationParameters.Writer, 
                specialSerializationParameters.InstanceMemory,
                specialSerializationParameters.ProcessingLevels,
                specialSerializationParameters.OriginalObjId);
        }

        private void SerializeObject(Type type, object value, JsonWriter writer, JsonWriteInstanceMemory instanceMemory,
            Stack<IProcessingLevel> processingLevels,
            long? originalObjId = null)
        {
            if (originalObjId == null)
            {
#if !(NETCOREAPP)
                // Identifying the need for this special serializer must be done manually, we cannot check the type
                // but need to call IsTransparentProxy
                if (System.Runtime.Remoting.RemotingServices.IsTransparentProxy(value))
                {
                    ITotalSpecialSerializer transparentProxySerializer = new TransparentProxySerializer();
                    transparentProxySerializer.SerializeInstanceInternal(new SpecialSerializationParameters(
                        SerializerObjectCallback, writer, instanceMemory, processingLevels), type, value);
                    return;
                }
#endif
                // if originalObjId is not null means we already have a replacement instance, so we don't need to do
                // the following lookup
                ITotalSpecialSerializer totalSpecialSerializer = _config.TryGetSpecialSerializer(type);
                if (totalSpecialSerializer != null)
                {
                    totalSpecialSerializer.SerializeInstanceInternal(new SpecialSerializationParameters(
                        SerializerObjectCallback, writer, instanceMemory, processingLevels), type, value);
                    writer.Flush();
                    return;
                }

                IFactory factory = _config.TryGetRecreateTypeFactory(type);
                if (factory != null)
                {
                    RecreateType recreateType = new RecreateType(type);
                    SerializeObject(typeof(RecreateType), recreateType, writer, instanceMemory, processingLevels);
                    writer.Flush();
                    return;
                }
                
            }
            
            
            processingLevels.Push(new ObjectLevel(type));
            if (processingLevels.Count > 200)
            {
                //TODO: we are approaching a StackOverflowException. What should we do? 
            }
            
            writer.StartObject();

            long objId;
            if (originalObjId == null)
            {
                if (!instanceMemory.CheckIfFirstAndAddInstance(value, out objId))
                {
                    writer.WriteObjRef(objId);
                    writer.EndObject();
                    processingLevels.Pop();
                    writer.Flush();
                    return;
                }
            }
            else
            {
                //this is used for replacement types like in the case of Delegates or WeakReferences
                objId = originalObjId.Value;
            }
            
            FieldInfo[] fieldInfos = FieldInfoCache.GetFieldInfos(type);
            writer.WriteTypeInfo(type);
            writer.WriteSeparatorIfRequired();
            writer.WriteObjId(objId);
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                if (fieldInfo.IsLiteral)
                {
                    continue;
                }

                // TODO: check if we want to keep this
                if (PrimitiveChecker.IsUnsupportedType(fieldInfo.FieldType))
                {
                    continue;
                }
                
                if (!Attribute.IsDefined(fieldInfo, typeof(TotalSerializerIgnoreFieldAttribute)))
                {
                    SerializeField(ref value, fieldInfo, writer, instanceMemory, processingLevels);
                }
            }

            writer.EndObject();
            writer.Flush();
            processingLevels.Pop();
        }

        private void SerializeField(ref object obj, FieldInfo fieldInfo, JsonWriter writer, JsonWriteInstanceMemory instanceMemory, Stack<IProcessingLevel> processingLevels)
        {
            object value;
            Type valueType;
            try
            {
                value = FieldsReadWriteCache.Get(ref obj, fieldInfo);
                valueType = value?.GetType();
                if (valueType!= null && _config.IsIgnoredType(valueType))
                {
                    return;
                }
                
            }
            catch (Exception e)
            {
                if (_config.ErrorStrategy == ErrorStrategy.ThrowException)
                {
                    throw;
                }
                //Could not get value at all.
                _logger.Error($"Could not retrieve value of field {fieldInfo.Name}", e);
                return;
            }

            
            processingLevels.Push(new FieldLevel(fieldInfo));
            if (processingLevels.Count > 200)
            {
                //TODO: we are approaching a StackOverflowException. What should we do? 
            }
            
            try
            {
                writer.Flush();

                // Check here for primitive?!
                writer.WriteSeparatorIfRequired();
                writer.StartWriteProperty(fieldInfo.Name);
                if (PrimitiveChecker.IsPrimitive(valueType) && fieldInfo.FieldType != valueType)
                {
                    //This is sort of a shortcut to reduce number of checks that would happen twice
                    // because this is sort of calling SerializeItem with last parameter true
                    writer.WritePseudoPrimitive(valueType, value);         
                }
                else
                {
                    SerializeItem(value, valueType, writer, instanceMemory, processingLevels);
                }
                
                writer.EndWriteProperty();

                writer.Flush();
            }
            catch (Exception e)
            {
                if (e is UnsupportedTypeException)
                {
                    ProcessingStackDebugReporter.ReportUnsupportedTypeByStack(processingLevels);
                }

                if (_config.ErrorStrategy == ErrorStrategy.ThrowException)
                {
                    throw;
                }

                writer.RevertUnFlushed(); //TODO: write tests to ensure this works always    
            }
            finally
            {
                processingLevels.Pop();
            }
        }

        private void SerializeItem(object value, Type type, JsonWriter writer, JsonWriteInstanceMemory instanceMemory, 
            Stack<IProcessingLevel> processingLevels, bool writePrimitivesWithTypeInfo = false)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }
            
            if (PrimitiveChecker.IsUnsupportedType(type))
            {
                throw new UnsupportedTypeException("We cannot (yet) serialize fields of type: {type}");
            }
            if (PrimitiveChecker.IsPrimitive(type))
            {
                try
                {
                    if (writePrimitivesWithTypeInfo)
                    {
                        //we use this in case the element type is different to the array element type
                        writer.WritePseudoPrimitive(type, value);        
                    }
                    else
                    {
                        writer.WritePrimitive(type, value);    
                    }
                    
                }
                catch (NotImplementedException)
                {
                    throw new UnsupportedTypeException("NotImplemented. We cannot (yet) serialize primitive fields of type: {type}");    
                }
            }
            else if (PseudoPrimitiveChecker.IsPseudoPrimitive(type))
            {
                writer.WritePseudoPrimitive(type, value);
            }
            else if (type.IsArray)
            {
                SerializeArray(type, value, writer, instanceMemory, processingLevels);
            }
            else if (MethodChecker.IsDelegate(type))
            {
                Delegate del =  value as Delegate;
                if (del != null)
                {
                    //look whether we already have written
                    long objId;
                    if (!instanceMemory.CheckIfFirstAndAddInstance(del, out objId))
                    {
                        writer.StartObject(); 
                        writer.WriteObjRef(objId);
                        writer.EndObject();
                    }
                    else
                    {
                        ParameterInfo[] parameters = del.Method.GetParameters();
                        string[] parameterTypes = new string[parameters.Length];
                        int i = 0;
                        foreach (ParameterInfo parameterInfo in parameters)
                        {
                            parameterTypes[i++] = TypeNameWriter.GetTypeNameForJson(parameterInfo.ParameterType);
                        }

                        string methodType = TypeNameWriter.GetTypeNameForJson(type);
                        
                        MulticastDelegate multicastDelegate = value as MulticastDelegate;
                        Delegate[] invocationList = null;
                        if (multicastDelegate != null)
                        {
                            invocationList = multicastDelegate.GetInvocationList();
                        }
                        
                        if (invocationList != null && invocationList.Length > 0) //currently always use EventMultiCastDelegate.... :-( because of a bug
                        {
                          
                            var intermediateEventInvocationList =
                                new List<EventMulticastDelegate.EventMultiCastTarget>();
                            for (int j = 0; j < invocationList.Length; j++)
                            {
                                Delegate invocationListItem = invocationList[j];
                                if (invocationListItem.Target == null || !_config.IsIgnoredType(invocationListItem.Target.GetType()))
                                {
                                    intermediateEventInvocationList.Add(new EventMulticastDelegate.EventMultiCastTarget(
                                        invocationListItem.Target,
                                        invocationListItem.Method.Name,
                                        invocationListItem.Method.DeclaringType
                                    ));    
                                }
                            }

                            var eventMulticastDelegates = intermediateEventInvocationList.ToArray();
                                
                            
                            EventMulticastDelegate eventMulticastDelegate = new EventMulticastDelegate(
                                value.GetType(), // e.g. PropertyChangedEventHandler
                                parameterTypes,
                                methodType,
                                eventMulticastDelegates
                                );
                            SerializeObject(typeof(EventMulticastDelegate), eventMulticastDelegate,  writer, instanceMemory, processingLevels, objId);
                        }
                        else
                        {
                            NormalDelegate  normalDelegate = new NormalDelegate(
                                del.Method.Name, 
                                del.Target,
                                del.Method.DeclaringType,
                                parameterTypes,
                                methodType
                            );
                            SerializeObject(typeof(NormalDelegate), normalDelegate, writer, instanceMemory, processingLevels, objId);
                        }
                    }
                }
            }
            else
            {
                SerializeObject(value.GetType(), value, writer, instanceMemory, processingLevels);
            }
        }
        
        private void SerializeArray(Type type, object value, JsonWriter writer, JsonWriteInstanceMemory instanceMemory, Stack<IProcessingLevel> processingLevels)
        {
            long objId;
            if (!instanceMemory.CheckIfFirstAndAddInstance(value, out objId))
            {
                writer.StartObject(); 
                writer.WriteObjRef(objId);
                writer.EndObject();
                return;
            }
            
            writer.StartObject();
            writer.WriteTypeInfo(type);
            writer.WriteSeparatorIfRequired();
            writer.WriteObjId(objId);
            writer.WriteSeparatorIfRequired();
            
            Array array = value as Array;
            if (array != null && array.Rank >= 0)
            {
                writer.StartWriteProperty("$dim");
                writer.StartWriteArray();
                for (int i = 0; i < array.Rank; i++)
                {
                    writer.WriteSeparatorIfRequired();
                    writer.WritePrimitive(typeof(int), array.GetLength(i));    
                }
                writer.EndWriteArray();
                writer.EndWriteProperty();
                writer.WriteSeparatorIfRequired();
            }
            
            writer.StartWriteProperty("$content");
            writer.StartWriteArray();
            
            IEnumerable enumerable = value as IEnumerable;
            if (enumerable != null)
            {
                Type elementType = type.GetElementType();
                bool writePrimitivesWithTypeInfo;
                Type itemType;
                foreach (var item in enumerable)
                {
                    writer.WriteSeparatorIfRequired();
                    if (item == null)
                    {
                        writer.WriteNullValue();
                    }
                    else
                    {
                        if (_config.IsIgnoredType(item.GetType()))
                        {
                            //TODO: solve this issue ->  need to correct $dim, or write $dim later (separate JsonWriter?)
                            throw new NotImplementedException("Array with forbidden types are not yet supported.");
                        }

                        if (!writer.InState(WriteState.WritingArray))
                        {
                            throw new Exception("Unexpected write state"); //TODO: make InState protected again after fixing the bug
                        }

                        itemType = item.GetType();
                        writePrimitivesWithTypeInfo = itemType != elementType;
                        SerializeItem(item, itemType , writer, instanceMemory, processingLevels, writePrimitivesWithTypeInfo);
                    }
                }
            }

            writer.EndWriteArray();
            writer.EndWriteProperty();
            writer.EndObject();
            writer.Flush();
        }

      
    }
}
