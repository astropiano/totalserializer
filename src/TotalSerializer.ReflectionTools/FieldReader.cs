using System;
using System.Reflection;
using System.Reflection.Emit;

namespace TotalSerializer.ReflectionTools
{
    /// <summary>
    /// Creates dynamic getters for given fields.
    /// </summary>
    /// <typeparam name="TInstance">The type of the instance.</typeparam>
    /// <typeparam name="TField">The type of the field.</typeparam>
    public class FieldReader<TInstance, TField> : IMemberReader<TInstance, TField>
    {
        private Func<TInstance, TField> _getter;

        public FieldReader(Type type, string fieldName) : this(type, FieldInfoCache.GetFieldInfo(type, fieldName))
        {
          
        }

        
        public FieldReader(Type type, FieldInfo fieldInfo)
        {
            if (null == type)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (null == fieldInfo)
            {
                throw new ArgumentNullException(nameof(fieldInfo));
            }
            _getter = CreateGetter(fieldInfo);
        }

        public TField GetValue(TInstance obj)
        {
            return _getter(obj);
        }

        public object GetUntypedValue(object obj)
        {
            if(null == obj)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return _getter((TInstance) obj); // We had an 'as' cast before, but I think if we don't have the correct type, the exception will be thrown later, 
            // so no performance boost when using 'as', so no performance loss when using a direct cast. 
        }
        
        private Func<TInstance, TField> CreateGetter(FieldInfo field)
        {
            if (null == field)
            {
                throw new ArgumentNullException(nameof(field));
            }

            string methodName;
            if (field.ReflectedType != null)
            {
                methodName = string.Concat(field.ReflectedType.FullName, ".get_", field.Name);    
            }
            else
            {
                methodName = string.Concat(".get_", field.Name);
            }
            
            Type fieldType = typeof(TField);
            Type instanceType = typeof(TInstance);
            if(null==fieldType)
            {
                throw new Exception();
            }
            DynamicMethod getterMethod = new DynamicMethod(methodName, fieldType, new [] { instanceType }, true);
            ILGenerator gen = getterMethod.GetILGenerator();
            if (field.IsStatic)
            {
                gen.Emit(OpCodes.Ldsfld, field);
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, field);
            }
            
            gen.Emit(OpCodes.Ret);
            return (Func<TInstance, TField>)getterMethod.CreateDelegate(typeof(Func<TInstance, TField>));
        }
        
    }
    
    
}
