using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace TotalSerializer.ReflectionTools
 {
     public class FieldsReadWriteCache : IFieldsReadWriteCache
     {
         ConcurrentDictionary<MemberKey, IMemberReader> _readers = new ConcurrentDictionary<MemberKey, IMemberReader>();
         
         ConcurrentDictionary<MemberKey, IMemberWriter> _writers = new ConcurrentDictionary<MemberKey, IMemberWriter>();

         private MethodInfo _readerCreatorMethod;
         private MethodInfo _propertyReaderCreatorMethod;
         private MethodInfo _writerCreatorMethod;
         private MethodInfo _propertyWriterCreatorMethod;

         /// <summary>
         /// Use this register your logger
         /// </summary>
         public Action<object> LogError;

         public void Clear()
         {
             _readers.Clear();
             _writers.Clear();
         }

         public int ReadersCount()
         {
             return _readers.Count;
         }
         
         public int WritersCount()
         {
             return _writers.Count;
         }

         public FieldsReadWriteCache()
         {
             _readerCreatorMethod = MethodInfoCache.GetMethodInfo(GetType(), nameof(CreateReader));
             _propertyReaderCreatorMethod = MethodInfoCache.GetMethodInfo(GetType(), nameof(CreatePropertyReader));
             _writerCreatorMethod = MethodInfoCache.GetMethodInfo(GetType(), nameof(CreateWriter));
             _propertyWriterCreatorMethod = MethodInfoCache.GetMethodInfo(GetType(), nameof(CreatePropertyWriter));
         }
 
         public object Get(ref object instance, string fieldName)
         {
             FieldInfo fieldInfo = FieldInfoCache.GetFieldInfo(instance.GetType(), fieldName);
             if (fieldInfo == null)
             {
                 PropertyInfo propertyInfo = PropertyInfoCache.GetPropertyInfo(instance.GetType(), fieldName);
                 return Get(ref instance, propertyInfo);
             }
             return Get(ref instance, fieldInfo);
         }

         public T Get<T>(ref object instance, string fieldName)
         {
             return (T) Get(ref instance, fieldName);
         }

         public object Get(ref object instance, FieldInfo fieldInfo)
         {
             Type instanceType = instance.GetType();
             MemberKey memberKey = new MemberKey(instanceType, fieldInfo.Name);
             if (!_readers.TryGetValue(memberKey, out var reader))
             {
                 //TODO: try/catch
                 MethodInfo mi = _readerCreatorMethod.MakeGenericMethod(instanceType, fieldInfo.FieldType);
                 object result = mi.Invoke(this, new object[]{instanceType, fieldInfo});
                 reader = result as IMemberReader;
                 if (reader == null)
                 {
                     throw new Exception($"Could not create instance of {nameof(IMemberReader)}");
                 }
               
                 _readers.TryAdd(memberKey, reader);
             }

             var value = reader.GetUntypedValue(instance);
             return value;
         }

         public object Get(ref object instance, PropertyInfo propertyInfo)
         {
             Type instanceType = instance.GetType();
             MemberKey memberKey = new MemberKey(instanceType, propertyInfo.Name);
             if (!_readers.TryGetValue(memberKey, out var reader))
             {
                 MethodInfo mi = _propertyReaderCreatorMethod.MakeGenericMethod(instanceType, propertyInfo.PropertyType);
                 object result = mi.Invoke(this, new object[]{instanceType, propertyInfo});
                 reader = result as IMemberReader;
                 if (reader == null)
                 {
                     throw new Exception($"Could not create instance of {nameof(IMemberReader)}");
                 }

                 _readers.TryAdd(memberKey, reader);
             }

             var value = reader.GetUntypedValue(instance);
             return value;
         }

         private IMemberReader CreateReader<TInstance, TField>(Type instanceType, FieldInfo fieldInfo)
         {
             IMemberReader<TInstance, TField> reader = new FieldReader<TInstance, TField>(instanceType, fieldInfo);
             return reader;
         }

         private IMemberReader CreatePropertyReader<TInstance, TProperty>(Type instanceType, PropertyInfo propertyInfo)
         {
             IMemberReader<TInstance, TProperty> reader = new PropertyReader<TInstance, TProperty>(instanceType, propertyInfo);
             return reader;
         }

         public void Set<TInstance, TValue>(ref TInstance instance, string fieldName, ref TValue value)
         {
             FieldInfo fieldInfo = FieldInfoCache.GetFieldInfo(instance.GetType(), fieldName);
             Set(ref instance, fieldInfo, ref value);
         }

         public void Set<TInstance, TValue>(ref TInstance instance, FieldInfo fieldInfo, ref TValue value)
         {
             object instanceAsObject = instance;
             object valueAsObject = value;
             Set(ref instanceAsObject, fieldInfo, ref valueAsObject);
         }

         public void Set<TInstance, TValue>(ref TInstance instance, string fieldName, TValue value)
         {
             Set(ref instance, fieldName, ref value);
         }

         public void Set(ref object instance, string fieldName, object value)
         {
             Set(ref instance, fieldName, ref value);
         }

         public void Set(ref object instance, string fieldName, ref object value)
         {
             FieldInfo fieldInfo = FieldInfoCache.GetFieldInfo(instance.GetType(), fieldName);
             if (fieldInfo == null)
             {
                 PropertyInfo propertyInfo = PropertyInfoCache.GetPropertyInfo(instance.GetType(), fieldName);
                 Set(ref instance, propertyInfo, ref value);
                 return;
             }
             Set(ref instance, fieldInfo, ref value);
         }
         
         public void Set(ref object instance, FieldInfo fieldInfo, ref object value)
         {
             if (fieldInfo.IsLiteral)
             {
                 return; //cannot write this type of fields
             }
             IMemberWriter writer;
             Type instanceType = instance.GetType();
             MemberKey memberKey = new MemberKey(instanceType, fieldInfo.Name);
             if (!_writers.TryGetValue(memberKey, out writer))
             {
                 //TODO: try/catch
#if !(NETCOREAPP)
                 if (fieldInfo.IsInitOnly)
                 {
                     writer = new ReflectionMemberWriter(fieldInfo);
                 }
                 else
#endif
                 {
                     object result = _writerCreatorMethod.MakeGenericMethod(instanceType, fieldInfo.FieldType)
                         .Invoke(this, new object[] {instanceType, fieldInfo});
                     writer = result as IMemberWriter;
                     if (writer == null)
                     {
                         throw new Exception($"Could not create instance of {nameof(IMemberWriter)}");
                     }
                 }

                 _writers.TryAdd(memberKey, writer);
             }

             writer.SetUntypedValue(ref instance, value);

             if (Attribute.IsDefined(fieldInfo, typeof(PostWriteActionAttribute)))
             {
                 var attribute = fieldInfo.GetCustomAttribute<PostWriteActionAttribute>();
                 if(attribute != null)
                 {
                     var methodInfo = MethodInfoCache.GetMethodInfo(fieldInfo.DeclaringType, attribute.Action);
                     if (methodInfo != null)
                     {
                         try
                         {
                             methodInfo.Invoke(instance, new object[0]);
                         }
                         catch (Exception e)
                         {
                             LogError?.Invoke(e);
                         }
                     }    
                 }
             }
         }
         
         private IMemberWriter CreateWriter<TInstance, TField>(Type instanceType, FieldInfo fieldInf)
         {
             IMemberWriter writer = new FieldWriter<TInstance, TField>(instanceType, fieldInf);
             return writer;
         }

         public void Set(ref object instance, PropertyInfo propertyInfo, ref object value)
         {
             IMemberWriter writer;
             Type instanceType = instance.GetType();
             MemberKey memberKey = new MemberKey(instanceType, propertyInfo.Name);
             if (!_writers.TryGetValue(memberKey, out writer))
             {
                 object result = _propertyWriterCreatorMethod.MakeGenericMethod(instanceType, propertyInfo.PropertyType)
                     .Invoke(this, new object[] {instanceType, propertyInfo});
                 writer = result as IMemberWriter;
                 if (writer == null)
                 {
                     throw new Exception($"Could not create instance of {nameof(IMemberWriter)}");
                 }

                 _writers.TryAdd(memberKey, writer);
             }

             writer.SetUntypedValue(ref instance, value);
         }

         private IMemberWriter CreatePropertyWriter<TInstance, TProperty>(Type instanceType, PropertyInfo propertyInfo)
         {
             IMemberWriter writer = new PropertyWriter<TInstance, TProperty>(instanceType, propertyInfo);
             return writer;
         }
     }
 }
