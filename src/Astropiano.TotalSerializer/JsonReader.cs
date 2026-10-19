using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using Astropiano.ReflectionTools;
using Astropiano.TotalSerializer.CharArray;
using Astropiano.TotalSerializer.Configuration;
using Astropiano.TotalSerializer.Exceptions;
using Astropiano.TotalSerializer.Interfaces;
using Astropiano.TotalSerializer.Methods;
using Astropiano.TotalSerializer.SpecialSerialization;

[assembly: InternalsVisibleTo("Astropiano.TotalSerializer.Test")]

namespace Astropiano.TotalSerializer
{
    public class JsonReader : JsonReadWriteBase<ReadState>
    {
        private CharArrayConverter _charArrayConverter = new CharArrayConverter();

        private FieldsReadWriteCache _fieldsReadWriteCache = new FieldsReadWriteCache();

        /// <summary>
        /// Possible char to indicate the end of an e.g. $id JSON value. 
        /// 
        /// If it is the last one, it is usually the '}', else it is the ','.
        /// </summary>
        private static char[] PossibleIdRefSearchEnds = {',', '}'};

        private readonly ConcurrentDictionary<string, Type> _typeNameDictionary = new ConcurrentDictionary<string, Type>();

        
        private Config _config;
        
        public JsonReader(Config config)
        {
            _config = config;
            SetState(ReadState.OutsideObj);
        }


        public void Read(string json, ref object instance)
        {
            AllowState(ReadState.OutsideObj); 
            ReferenceStorage referenceStorage = new ReferenceStorage();
            Span<char> characters = json.ToCharArray();
            Read(characters, ref instance, ref referenceStorage);
            
            //special cases
            if (instance is RecreateType)
            {
                RecreateType recreateType = (RecreateType) instance;
                ProcessRecreateTypeWithoutWrite(ref instance, recreateType, _config);
            }
        }

        private void Read(Span<char> characters, ref object instance, ref ReferenceStorage referenceStorage)
        {
            /* // This code is only to execute a consistency check. TODO: remove again
            char[] array = characters.ToArray();
            if (array.First() != '{' || array.Last() != '}')
            {
                throw new JsonFormatException("could not find starting '{' or ending '}'.");
            }
            */
            
            int operationEndIndex = 0; //shared for different operations
            for (int currentIndex = 0; currentIndex < characters.Length; currentIndex++)
            {
                if (IsOpenObjectClause(characters, ref currentIndex))
                {
                    ProcessOpenObjectClause();
                }
                else if (IsPropertyDeclaration(characters, ref currentIndex, ref operationEndIndex))
                {
                    if (IsIdDeclaration(characters, ref currentIndex))
                    {
                        ProcessIdDeclaration(characters, ref currentIndex, ref operationEndIndex, ref instance, ref referenceStorage);
                    }
                    else if (IsRefDeclaration(characters, ref currentIndex))
                    {
                        long refId = ProcessRefDeclaration(characters, ref currentIndex, ref operationEndIndex);
                        if (referenceStorage.TryGet(refId, out object referenced))
                        {
                            instance = referenced;
                        }
                        else
                        {
                            // Look whether we already have a postponed write
                            PostponedWrite postponedWrite;
                            if (!referenceStorage.TryGetPostponedWrite(refId, out postponedWrite))
                            {
                                postponedWrite = new PostponedWrite(refId);
                                referenceStorage.AddPostponedWrite(postponedWrite);
                            }
                            instance = postponedWrite;
                        }

                        /*
                        if (PostProcessAfterRead(ref instance))
                        {
                            // This means the instance has been changed in the post-processing and we should also update
                            // the reference storage
                            // TODO: write unit-test (e.g. same PropertyChangedHandler instance 3 times in an array)
                            referenceStorage.Update(refId, instance);
                        }
                        */
                        return;
                    }
                    else if (IsTypeDeclaration(characters, ref currentIndex))
                    {
                        //type declaration
                        //because otherwise operationEndIndex points to the end of the property name
                        currentIndex = operationEndIndex + 1;
                        Type type = ProcessTypeDeclaration(characters, ref currentIndex, out operationEndIndex);

                        if (PseudoPrimitiveChecker.IsPseudoPrimitive(type))
                        {
                            currentIndex+=3;
                            instance = ProcessPseudoPrimitive(type, characters, ref currentIndex, out operationEndIndex);
                        }
                        else if (PrimitiveChecker.IsPrimitive(type))
                        {
                            //If we have a primitive it could mean it got written like a pseud-primitive
                            currentIndex+=3;
                            instance = ProcessPseudoPrimitive(type, characters, ref currentIndex, out operationEndIndex);
                        }
                        else if (type.IsArray)
                        {
                            currentIndex+=3;
                            ProcessArray(type, ref instance, characters, ref currentIndex, out operationEndIndex, ref referenceStorage);
                            PostProcessAfterRead(ref instance, referenceStorage);
                            return;
                        }
/*                        
#if !(NETCOREAPP)
                        else if(type.IsGenericType && type.GetGenericTypeDefinition() ==  typeof(ConcurrentDictionary<,>))
                        {
                            currentIndex+=3;
                            ProcessDictionary(type, ref instance, characters, ref currentIndex, ref operationEndIndex, 
                                ref dictionary);
                            return;
                        }
#endif
*/
                        else
                        {
                            //normal type => use factory
                            try
                            {
                                instance = _config.Factory.Create(type);
                                //when creating replacement types we should write the real instance to dictionary, currently 
                                //    replacement type is added to the instance
                            }
                            catch (Exception e)
                            {
                                throw new Exception($"Factory could not create instance of type {type}", e);
                            }   
                        } 
                    }
                    else
                    {
                        //normal property
                        string propertyName;
                        object propertyValue;
                        GetPropertyNameAndValue(characters, ref currentIndex, out operationEndIndex, ref referenceStorage,
                            out propertyName, out propertyValue);

                        var totalSpecialSerializer = propertyValue != null ? _config.TryGetSpecialDeserializer(propertyValue.GetType()) : null;
                            
                        if (totalSpecialSerializer != null)
                        {
                            TotalSpecialSerializerReplacement replacement =
                                (TotalSpecialSerializerReplacement) propertyValue;
                            totalSpecialSerializer.DeserializeInstanceAndWriteInternal(ref instance, ref replacement, 
                                _config.Factory, _fieldsReadWriteCache, propertyName, referenceStorage);
                        }
                        else if (propertyValue is NormalDelegate)
                        {
                            NormalDelegate normalDelegate = (NormalDelegate) propertyValue;

                            if (normalDelegate.PostponedFieldWrites > 0)
                            {
                                object instanceForActionScope = instance;
                                normalDelegate.AddAction(() =>
                                {
                                    ProcessNormalDelegate(ref instanceForActionScope, ref propertyName, (NormalDelegate) propertyValue);    
                                });
                            }
                            else
                            {
                                ProcessNormalDelegate(ref instance, ref propertyName, (NormalDelegate) propertyValue);    
                            }
                        }
                        else if (propertyValue is PostponedFieldHolder)
                        {
                            EventMulticastDelegate eventMulticastDelegate = (EventMulticastDelegate) propertyValue;
                            if (eventMulticastDelegate.PostponedFieldWrites > 0)
                            {
                                object instanceForActionScope = instance;
                                eventMulticastDelegate.AddAction(() =>
                                {
                                    ProcessMultiCastDelegate(ref instanceForActionScope, ref propertyName, eventMulticastDelegate);    
                                });
                            }
                            else
                            {
                                ProcessMultiCastDelegate(ref instance, ref propertyName, eventMulticastDelegate);    
                            }
                            
                        }else if (propertyValue is RecreateType)
                        {
                            RecreateType recreateType = (RecreateType) propertyValue;
                            ProcessRecreateType(ref instance, ref propertyName, recreateType, _config);
                        }
                        else
                        {
                            if (propertyValue is PostponedWrite)
                            {
                                PostponedWrite postponedWrite = (PostponedWrite) propertyValue; 
                                postponedWrite.AddWrite(instance, propertyName, propertyValue);
                            }
                            else
                            {
                                _fieldsReadWriteCache.Set(ref instance, propertyName, ref propertyValue);    
                            }
                        }
                    }
                }
                else if(IsPropertySeparator(characters, ref currentIndex))
                {
                    
                }
                else if(IsFormattingCharacter(characters, ref currentIndex))
                {
                    
                }
                else 
                {
                    throw new JsonFormatException(string.Format("Now clue what this is: '{0}'.", characters[currentIndex]));
                }
                    
                
            }
            
            // we are done with parsing this JSON part

            PostProcessAfterRead(ref instance, referenceStorage);
        }

        
        // TODO: write unit-test (e.g. same PropertyChangedHandler instance 3 times in an array)
        private void PostProcessAfterRead(ref object instance, ReferenceStorage referenceStorage)
        {
            if (instance is EventMulticastDelegate)
            {
                EventMulticastDelegate multicastDelegate = instance as EventMulticastDelegate;
                Delegate del = ParseToMulticastDelegate(multicastDelegate);
                if (del != null && multicastDelegate.Type != null)
                {
                    long refId = referenceStorage.TryGetKey(multicastDelegate);
                    instance = Convert.ChangeType(del, multicastDelegate.Type);
                    if (refId != -1)
                    {
                        referenceStorage.Update(refId, instance);    
                    }
                }
                
            }
        }

        private void ProcessRecreateTypeWithoutWrite(ref object value, RecreateType recreateType, Config config)
        {
            IFactory factory = config.TryGetRecreateTypeFactory(recreateType.Type);
            if (factory != null)
            {
                value= factory.Create(recreateType.Type);
            }
            else
            {
                throw new Exception($"Could not find recreate factory for type {recreateType.Type}. Please use" +
                                    " the same configuration when deserializing!");
            }
        }

        private void ProcessRecreateType(ref object instance, ref string propertyName, RecreateType recreateType, Config config)
        {
            object propertyValue = null;
            ProcessRecreateTypeWithoutWrite(ref propertyValue, recreateType, config);
            _fieldsReadWriteCache.Set(ref instance, propertyName, ref propertyValue);
        }


        private void ProcessNormalDelegate(
            ref object instance,
            ref string propertyName,
            NormalDelegate normalDelegate)
        {
            Type[] parameterTypes;
            if (normalDelegate.Signature == null)
            {
                parameterTypes = new Type[0];
            }
            else
            {
                parameterTypes = new Type[normalDelegate.Signature.Length];
                for (int i = 0; i < normalDelegate.Signature.Length; i++)
                {
                    parameterTypes[i] = TypeGenerator.GetType(normalDelegate.Signature[i]);
                }
            }
            
            MethodInfo methodInfo = MethodInfoCache.GetMethodInfo(normalDelegate.DeclaringType,
                normalDelegate.MethodName, parameterTypes);
            Type delegateType = TypeGenerator.GetType(normalDelegate.MethodType);
            Delegate d = Delegate.CreateDelegate(delegateType,
                normalDelegate.Target, methodInfo);
            object objD = d; //otherwise we cannot pass ref
            _fieldsReadWriteCache.Set(ref instance, propertyName, ref objD);
        }

        // TODO: if we reactivate NormalDelegate (which currently has a bug) we need a similar implementation for NormalDelegate
        private Delegate ParseToMulticastDelegate(EventMulticastDelegate eventMulticastDelegate)
        {
            if (eventMulticastDelegate.MultiCastTargets != null && eventMulticastDelegate.MultiCastTargets.Length > 0)
            {
                Type[] parameterTypes;
                if (eventMulticastDelegate.Signature == null)
                {
                    parameterTypes = new Type[0];
                }
                else
                {
                    parameterTypes = new Type[eventMulticastDelegate.Signature.Length];
                    for (int i = 0; i < eventMulticastDelegate.Signature.Length; i++)
                    {
                        parameterTypes[i] = TypeGenerator.GetType(eventMulticastDelegate.Signature[i]);
                        if (parameterTypes[i] == null)
                        {
                            throw new JsonFormatException($"Could not read type: {eventMulticastDelegate.Signature[i]}");
                        }
                    }
                }

                Type delegateType = TypeGenerator.GetType(eventMulticastDelegate.MethodType);
                
                MethodInfo methodInfo = MethodInfoCache.GetMethodInfo(
                    eventMulticastDelegate.MultiCastTargets[0].DeclaringType,
                    eventMulticastDelegate.MultiCastTargets[0].MethodName, parameterTypes);

                Delegate multicastDelegate = Delegate.CreateDelegate(delegateType,
                    eventMulticastDelegate.MultiCastTargets[0].Target, methodInfo);
                int c = 1;
                while (c < eventMulticastDelegate.MultiCastTargets.Length)
                {
                    methodInfo = MethodInfoCache.GetMethodInfo(
                        eventMulticastDelegate.MultiCastTargets[c].DeclaringType,
                        eventMulticastDelegate.MultiCastTargets[c].MethodName, parameterTypes);
                    Delegate nextMulticastDelegate = Delegate.CreateDelegate(delegateType,
                        eventMulticastDelegate.MultiCastTargets[c].Target, methodInfo);
                    multicastDelegate = Delegate.Combine(multicastDelegate, nextMulticastDelegate);
                    c++;
                }

                return multicastDelegate;
            }

            return null;
        }
        
        private void ProcessMultiCastDelegate(ref object instance,
            ref string propertyName,
            EventMulticastDelegate eventMulticastDelegate)
        {
            object del = ParseToMulticastDelegate(eventMulticastDelegate);
            if (del != null)
            {
                _fieldsReadWriteCache.Set(ref instance, propertyName, ref del);
            }
        }

        private bool IsOpenObjectClause(Span<char> characters, ref int currentIndex)
        {
            if (characters[currentIndex] == '{')
            {
                return true;
            }

            return false;
        }

        private bool IsPropertySeparator(Span<char> characters, ref int currentIndex)
        {
            if (characters[currentIndex] == ',')
            {
                return true;
            }

            return false;
        }

        private bool IsFormattingCharacter(Span<char> characters, ref int currentIndex)
        {
            if (characters[currentIndex] == ' ' || characters[currentIndex] == '\n' || characters[currentIndex] == '\r' || characters[currentIndex] == '\t')
            {
                return true;
            }

            return false;
        }

        private void ProcessOpenObjectClause()
        {
            SetState(ReadState.ReadingObj);
        }

        /// <summary>
        /// Example:
        /// Before operation:
        /// {"Value":2}
        ///
        /// </summary>
        /// <param name="characters">Character array</param>
        /// <param name="currentIndex">Start index. Index of the first character of the name at the end (in case of true).</param>
        /// <param name="propertyNameEndIndex"></param>
        /// <returns></returns>
        /// <exception cref="JsonFormatException"></exception>
        internal bool IsPropertyDeclaration(Span<char> characters, ref int currentIndex, ref int propertyNameEndIndex)
        {
            if (characters[currentIndex] == DoubleQuote)
            {
                currentIndex++;
                int iterator = currentIndex;
                CharArrayComparer.SeekNextOccurence(characters, DoubleQuote, ref iterator, out var seekIndex);
                if (seekIndex == -1)
                {
                    throw new JsonFormatException($"Could not find closing \" in string: {characters.ToString()}");
                }
                propertyNameEndIndex = seekIndex;
                if (propertyNameEndIndex == currentIndex)
                {
                    throw new JsonFormatException("Property name is empty.");
                }

                return true;
            }
            return false;
        }


        /// <summary>
        ///
        /// Note: Does not move currentIndex.
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="currentIndex">Parameter will not change.</param>
        /// <returns></returns>
        internal bool IsTypeDeclaration(Span<char> characters, ref int currentIndex)
        {
            if (CharArrayComparer.ContainsAt(characters, currentIndex, TypeDeclaration))
            {
                return true;
            }

            return false;
        }


        internal string GetTypeName(Span<char> characters, ref int currentIndex, out int seekIndex)
        {
            CharArrayComparer.EnforceNextOccurence(characters, ':', ref currentIndex);
            currentIndex++;
            CharArrayComparer.EnforceNextOccurence(characters, '\"', ref currentIndex);
            currentIndex++;
            int typeNameStart = currentIndex;
            CharArrayComparer.SeekNextOccurence(characters, '\"', ref currentIndex, out seekIndex);
            int typeNameEnd = seekIndex;

            string name = _charArrayConverter.GetPartialString(characters, typeNameStart, typeNameEnd);
            return name;
        }

        internal Type ProcessTypeDeclaration(Span<char> characters, ref int currentIndex, out int iterator)
        {
            SetState(ReadState.ReadingTypeInfo);
            string name = GetTypeName(characters, ref currentIndex, out iterator);
            Type type = GetTypeFromName(name);
            if (type == null)
            {
                throw new JsonFormatException(string.Format("Could not parse given type '{0}'.", name));
            }
            return type;
        }

        private Type GetTypeFromName(string name)
        {
            return _typeNameDictionary.GetOrAdd(name, n => TypeGenerator.GetType(n));
        }

        /// <summary>
        /// Check if we are inside an $id declaration. 
        /// 
        /// Note: Does not move currentIndex.
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="currentIndex">Parameter will not change.</param>
        /// <returns></returns>
        private bool IsIdDeclaration(Span<char> characters, ref int currentIndex)
        {
            if (CharArrayComparer.ContainsAt(characters, currentIndex, IdDeclaration))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if we are inside an ref declaration. 
        /// 
        /// Note: Does not move currentIndex.
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="currentIndex">Parameter will not change.</param>
        /// <returns></returns>
        private bool IsRefDeclaration(Span<char> characters, ref int currentIndex)
        {
            if (CharArrayComparer.ContainsAt(characters, currentIndex, RefDeclaration))
            {
                return true;
            }

            return false;
        }

        private long ProcessRefDeclaration(Span<char> characters, ref int currentIndex, ref int iterator)
        {
            SetState(ReadState.ReadingIdInfo);
            long id = GetRef(characters, ref currentIndex, ref iterator);
            return id;
        }


        private long GetRef(Span<char> characters, ref int currentIndex, ref int seekIndex)
        {
            CharArrayComparer.EnforceNextOccurence(characters, '$', ref currentIndex);
            currentIndex++;
            CharArrayComparer.EnforceNextOccurence(characters, 'r', ref currentIndex);
            currentIndex++;
            CharArrayComparer.EnforceNextOccurence(characters, 'e', ref currentIndex);
            currentIndex++;
            CharArrayComparer.EnforceNextOccurence(characters, 'f', ref currentIndex);
            currentIndex++;
            CharArrayComparer.EnforceNextOccurence(characters, DoubleQuote, ref currentIndex);
            currentIndex++;
            CharArrayComparer.EnforceNextOccurence(characters, ':', ref currentIndex);
            currentIndex++;
            int idStart = currentIndex;
            CharArrayComparer.SeekNextOccurenceWithOrLink(characters, PossibleIdRefSearchEnds, ref currentIndex, ref seekIndex);
            int idEnd;
            if (seekIndex == -1)
            {
                //assume we have only a small Span<char> e.g. from an array item
                idEnd = characters.Length;
            }
            else
            {
                idEnd = seekIndex;
            }
            

            string refAsString = _charArrayConverter.GetPartialString(characters, idStart, idEnd);
            if (long.TryParse(refAsString, out var id))
            {
                return id;
            }

            throw new JsonFormatException($"Could not parse $ref, input was '{refAsString}'.");
        }






        private void ProcessIdDeclaration(Span<char> characters, ref int currentIndex, ref int iterator, ref object instance, 
            ref ReferenceStorage referenceStorage)
        {
            SetState(ReadState.ReadingIdInfo);
            long id = GetId(characters, ref currentIndex, ref iterator);
            TotalSpecialSerializerReplacement totalSpecialSerializerReplacement = instance as TotalSpecialSerializerReplacement;
            // we do not write the replacement instances to the store only real instances
            if (totalSpecialSerializerReplacement != null)
            {
                totalSpecialSerializerReplacement.RefId = id;
            }
            else
            {
                referenceStorage.Set(id, instance, _fieldsReadWriteCache);
            }
        }


        private long GetId(Span<char> characters, ref int currentIndex, ref int seekIndex)
        {
            CharArrayComparer.EnforceNextOccurence(characters, '$', ref currentIndex);
            currentIndex++;
            CharArrayComparer.EnforceNextOccurence(characters, 'i', ref currentIndex);
            currentIndex++;
            CharArrayComparer.EnforceNextOccurence(characters, 'd', ref currentIndex);
            currentIndex++;
            CharArrayComparer.EnforceNextOccurence(characters, DoubleQuote, ref currentIndex);
            currentIndex++;
            CharArrayComparer.EnforceNextOccurence(characters, ':', ref currentIndex);
            currentIndex++;
            int idStart = currentIndex;
            CharArrayComparer.SeekNextOccurenceWithOrLink(characters, PossibleIdRefSearchEnds, ref currentIndex, ref seekIndex);
            if (seekIndex == -1)
            {
                throw new JsonFormatException("Could not parse $id because end could not be found.");
            }
            int idEnd = seekIndex;

            string idAsString = _charArrayConverter.GetPartialString(characters, idStart, idEnd);
            long id;
            if (long.TryParse(idAsString, out id))
            {
                return id;
            }

            throw new JsonFormatException($"Could not parse $id, input was '{idAsString}'.");
        }

        private string GetPropertyName(Span<char> characters, ref int currentIndex, out int seekIndex)
        {
            int propertyNameStart = currentIndex;
            CharArrayComparer.SeekNextOccurence(characters, DoubleQuote, ref currentIndex, out seekIndex);
            if (seekIndex < 0)
            {
                throw new JsonFormatException(
                    "Could not find ending double quote while searching for a property name.");
            }
            int propertyNameEnd = seekIndex;
            return _charArrayConverter.GetPartialString(characters, propertyNameStart, propertyNameEnd);
        }
        
        private void GetPropertyNameAndValue(
            Span<char> characters, 
            ref int currentIndex, 
            out int seekIndex, 
            ref ReferenceStorage referenceStorage, 
            out string propertyName,
            out object propertyValue)
        {
            propertyName = GetPropertyName(characters, ref currentIndex, out seekIndex);

            currentIndex++;
            CharArrayComparer.EnforceNextOccurence(characters, ':', ref currentIndex);
            currentIndex++;

            int propertyValueStart = currentIndex;
            //check if first character is DoubleQuote
            int propertyValueEnd;
            if (characters[currentIndex] == DoubleQuote)
            {
                propertyValueStart++;
                //we have a a string property, need to seek for end of string...
                CharArrayComparer.SeekNextStringScopedOccurence(characters, DoubleQuote, ref currentIndex, out propertyValueEnd);
                currentIndex++;
                Span<char> property = _charArrayConverter.GetPartial(characters, propertyValueStart, propertyValueEnd);
                propertyValue = property.ToString();
            }
            else
            {
                CharArrayComparer.SeekJsonScopeEnd(characters, ref currentIndex, out seekIndex);
                if (seekIndex < 0)
                {
                    throw new JsonFormatException($"Could not find end marker for value of property '{propertyName}'!");
                }
                propertyValueEnd = seekIndex;
                Span<char> property = _charArrayConverter.GetPartial(characters, propertyValueStart, propertyValueEnd);
                propertyValueEnd = 0; //this is now our seek-index
                CharArrayComparer.SeekNextOccurence(property, '{', ref propertyValueEnd, out seekIndex);
                if (seekIndex > -1)
                {
                    propertyValue = null;
                    Read(property, ref propertyValue, ref referenceStorage);
                }
                else
                {
                    //number but not a string
                    propertyValue = property.ToString();
                }
            }
        }

      

        private object ProcessPseudoPrimitive(Type type, Span<char> characters, ref int currentIndex, out int seekIndex)
        {
            string propertyName = GetPropertyName(characters, ref currentIndex, out seekIndex);

            currentIndex++;
            CharArrayComparer.EnforceNextOccurence(characters, ':', ref currentIndex);
            currentIndex++;

            int propertyValueStart = currentIndex;
            CharArrayComparer.SeekJsonScopeEnd(characters, ref currentIndex, out seekIndex);
            if (seekIndex < 0)
            {
                // We could not find ',' as end-marker. Maybe it is the last property, then we have to seek for a '}'.
                currentIndex = propertyValueStart; // Go back and search again.
                CharArrayComparer.SeekNextOccurence(characters, '}', ref currentIndex, out seekIndex);
                if (seekIndex < 0)
                {
                    throw new JsonFormatException($"Could not find end marker for value of property '{propertyName}'!");
                }
            }
            int propertyValueEnd = seekIndex;

            Span<char> property = _charArrayConverter.GetPartial(characters, propertyValueStart, propertyValueEnd);

            return TypeParser.ParseString(type, property.ToString());
        }

        
        private void ProcessArray(Type type, ref object instance, Span<char> characters, ref int currentIndex, 
            out int seekIndex, ref ReferenceStorage referenceStorage)
        {
            //this should be $content (or $dim in case of a multidimensional array)
            string propertyName = GetPropertyName(characters, ref currentIndex, out seekIndex);
            int[] dimensions = null;

            long objId = 0;
            
            if (propertyName == "$ref")
            {
                currentIndex++;
                CharArrayComparer.EnforceNextOccurence(characters, ':', ref currentIndex);
                currentIndex++;
                
                int refStart = currentIndex;
                
                //we already should have the instance in the referenceStorage
                CharArrayComparer.SeekJsonScopeEnd(characters, ref currentIndex, out seekIndex);
                if (seekIndex < 0)
                {
                    throw new JsonFormatException($"Could not find end marker for value of array '{propertyName}'!");
                }
                int refEnd = seekIndex;
                int jumpAfterOperation = seekIndex; //needed because other values are passed as ref and get changed
                Span<char> refChars = _charArrayConverter.GetPartial(characters, refStart, refEnd);
                if (!long.TryParse(refChars.ToString(), out objId))
                {   
                    throw new JsonFormatException($"Could not parse '$ref' to long!");
                }
                if (!referenceStorage.TryGet(objId, out instance))
                {
                    throw new Exception($"Instance with $id={objId} should be in memory, but seems it is not.");
                }
                currentIndex = jumpAfterOperation + 2;
                return;
            }
            if (propertyName == "$id")
            {
                currentIndex++;
                CharArrayComparer.EnforceNextOccurence(characters, ':', ref currentIndex);
                currentIndex++;
                
                int idStart = currentIndex;
                
                //we have to add the instance to the reference storage
                CharArrayComparer.SeekJsonScopeEnd(characters, ref currentIndex, out seekIndex);
                if (seekIndex < 0)
                {
                    throw new JsonFormatException($"Could not find end marker for value of array '{propertyName}'!");
                }
                int idEnd = seekIndex;
                Span<char> idChars = _charArrayConverter.GetPartial(characters, idStart, idEnd);
                if (!long.TryParse(idChars.ToString(), out objId))
                {   
                    throw new JsonFormatException($"Could not parse '$id' to long!");
                }
    
                currentIndex = seekIndex+2;
            }

            propertyName = GetPropertyName(characters, ref currentIndex, out seekIndex);

            if (propertyName != "$content")
            {
                if (propertyName == "$dim")
                {
                    //we are in the case of a multi-dimensional array
                    currentIndex++;
                    CharArrayComparer.EnforceNextOccurence(characters, ':', ref currentIndex);
                    currentIndex++;

                    int dimStart = currentIndex;

                    CharArrayComparer.SeekJsonScopeEnd(characters, ref currentIndex, out seekIndex);
                    if (seekIndex < 0)
                    {
                        throw new JsonFormatException($"Could not find end marker for value of array '{propertyName}'!");
                    }
                    int dimEnd = seekIndex;
                    int jumpAfterOperation = seekIndex; //needed because other values are passed as ref and get changed
                    Span<char> dimensionsChars = _charArrayConverter.GetPartial(characters, dimStart, dimEnd);    

                    object dimensionArrayInstance = null;
                    CreateArrayAndArrayItems(typeof(int[]), null, ref dimensionArrayInstance, dimensionsChars, out currentIndex, ref seekIndex, out dimStart,
                        out dimEnd, -1, ref referenceStorage);
                    dimensions = (int[]) dimensionArrayInstance;
                    
                    currentIndex = jumpAfterOperation + 2;

                    propertyName = GetPropertyName(characters, ref currentIndex, out seekIndex);
                    if (propertyName != "$content")
                    {
                        throw new JsonFormatException("Expected to find '$content' as property-name.");    
                    }
                    
                }
                else
                {
                    throw new JsonFormatException("Expected to find '$content' as property-name.");    
                }
                
            }
             
            currentIndex++;
            CharArrayComparer.EnforceNextOccurence(characters, ':', ref currentIndex);
            currentIndex++;
            
            int propertyValueStart = currentIndex;

            CharArrayComparer.SeekJsonScopeEnd(characters, ref currentIndex, out seekIndex);
            if (seekIndex < 0)
            {
                throw new JsonFormatException($"Could not find end marker for value of array '{propertyName}'!");
            }
            int propertyValueEnd = seekIndex;
            

            characters = _charArrayConverter.GetPartial(characters, propertyValueStart, propertyValueEnd);
            
            
            CreateArrayAndArrayItems(type, dimensions, ref instance, characters, out currentIndex, ref seekIndex, out propertyValueStart,
                out propertyValueEnd, objId, ref referenceStorage);
        }
        
        private void CreateArrayAndArrayItems(
            Type type, 
            int[] dimensions,
            ref object instance, 
            Span<char> arrayContent,
            out int currentIndex, 
            ref int seekIndex, 
            out int arrayItemsStart,
            out int  arrayItemsEnd,
            long objId,
            ref ReferenceStorage referenceStorage)
        {
            currentIndex = 0;
            arrayItemsStart = 0;
            arrayItemsEnd = 0;
            int iterator = 0;
            
            Array arrayInstance;
            int[] dimensionsIterator = null;
            if (dimensions != null)
            {
                //create multi-dimensional array
                arrayInstance = _config.Factory.CreateArray(type, dimensions);
                dimensionsIterator = (int[]) _config.Factory.CreateArray(typeof(int[]), dimensions.Length);

                arrayItemsEnd = 1;
                foreach (int dimension in dimensions)
                {
                    arrayItemsEnd *= dimension;
                }
                arrayItemsEnd--;
            }
            else
            {
                CharArrayComparer.EnforceNextOccurence(arrayContent, '[', ref currentIndex);
                currentIndex++;
                CharArrayComparer.CountJsonScopedOccurence(arrayContent, ',', ref currentIndex, out arrayItemsEnd);
           
                if (arrayItemsEnd == 0)
                {
                    //array has no ',' character, so size is either 0 or 1
                    CharArrayComparer.SeekNextJsonScopedOccurence(arrayContent, ']', ref iterator, out seekIndex);
                    if (seekIndex == 1)
                    {
                        // empty array
                        instance =  _config.Factory.CreateArray(type, 0); //create empty array
                        return; // array size is zero, no further processing is required.    
                    }
                }
                arrayInstance =  _config.Factory.CreateArray(type, arrayItemsEnd + 1);    
            }

            if (objId != -1)
            {
                referenceStorage.Set(objId, arrayInstance, _fieldsReadWriteCache);    
            }
            

            IList array = arrayInstance;

            currentIndex = 0;
            CharArrayComparer.EnforceNextOccurence(arrayContent, '[', ref currentIndex);
            currentIndex++;
            Type elementType = type.GetElementType();
            //from now the variable instance is used for array items!
            arrayItemsEnd++;

            for (arrayItemsStart = 0; arrayItemsStart < arrayItemsEnd; arrayItemsStart++)
            {
                iterator = currentIndex;
                if (arrayItemsStart == arrayItemsEnd - 1)
                {
                    //This should seek for a scoped occurence!
                    CharArrayComparer.SeekNextJsonScopedOccurence(arrayContent, ']', ref iterator, out seekIndex);
                }
                else
                {
                    CharArrayComparer.SeekNextJsonScopedOccurence(arrayContent, ',', ref iterator, out seekIndex);
                }
                if (seekIndex == -1)
                {
                    throw new JsonFormatException("Could not find end of array content");
                }
                Span<char> arrayItem = _charArrayConverter.GetPartial(arrayContent, currentIndex, seekIndex);
                
                if (ShouldHandleArrayItemByRead(arrayItem))
                {
                    Read(arrayItem, ref instance, ref referenceStorage);
                }
                else
                {
                    instance = TypeParser.ParseString(elementType, arrayItem.ToString());
                }

                if (dimensions == null)
                {
                    array[arrayItemsStart] = instance; //single dimensional case    
                }
                else
                {
                    
                    if (instance != null)
                    {
                        //check whether this type is covered by the plugin system and we currently see a replacement instance
                        ITotalSpecialSerializer totalSpecialSerializer =
                            _config.TryGetSpecialDeserializer(instance.GetType());
                        if (totalSpecialSerializer != null)
                        {
                                
                            instance = totalSpecialSerializer.DeserializeInstanceInternal(ref instance, _config.Factory, 
                                _fieldsReadWriteCache, referenceStorage);
                        }    
                    }

                 
                    if (instance is PostponedWrite)
                    {
                        PostponedWrite postponedWrite = (PostponedWrite) instance;
                        postponedWrite.AddArrayWrite(arrayInstance, instance, dimensionsIterator);
                    }
                    else
                    {
                        arrayInstance.SetValue(instance, dimensionsIterator);
                    }
                   
                    IncreaseMultiDimension(ref dimensionsIterator, ref dimensions);
                }
                
                currentIndex+=arrayItem.Length;
                currentIndex++;
            }
           

            instance = arrayInstance;
        }

        private void IncreaseMultiDimension(ref int[] multiDimensionIndex, ref int[] dimensions)
        {
            for (int i = multiDimensionIndex.Length -1; i >= 0; i--)
            {
                if (multiDimensionIndex[i] < dimensions[i] -1)
                {
                    multiDimensionIndex[i] ++ ;
                    break;
                }
                multiDimensionIndex[i] = 0;
            }
        }
        private bool ShouldHandleArrayItemByRead(Span<char> arrayItem)
        {
            if (arrayItem.Length > 0
                && ( arrayItem[0] == '{' || arrayItem[0] == '['))
            {
                return true;
            }

            return false;
        }

    }
}