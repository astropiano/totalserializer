using System.Reflection;

namespace TotalSerializer.ReflectionTools
{
    public interface IFieldsReadWriteCache
    {
        object Get(ref object instance, string fieldName);
        T Get<T>(ref object instance, string fieldName);
        
        object Get(ref object instance, FieldInfo fieldInfo);
        void Set(ref object instance, string fieldName, ref object value);
        void Set<TInstance, TValue>(ref TInstance instance, string fieldName, ref TValue value);
        void Set<TInstance, TValue>(ref TInstance instance, FieldInfo fieldInfo, ref TValue value);
        
        void Set<TInstance, TValue>(ref TInstance instance, string fieldName, TValue value);
        void Set(ref object instance, string fieldName, object value);
        void Set(ref object instance, FieldInfo fieldInfo, ref object value);
    }
}
