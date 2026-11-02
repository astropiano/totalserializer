using System;
using System.Reflection;
using System.Reflection.Emit;

namespace TotalSerializer.ReflectionTools
{
    public class FieldWriter<TInstance, TField> : IMemberWriter<TInstance, TField>
    {
        delegate void RefAction<T1, T2>(ref T1 arg1, T2 arg2);
        
        private Action<TInstance, TField> _setter;
        
        private RefAction<TInstance, TField> _structSetter;

        public FieldWriter(Type type, string fieldName) : this(type, FieldInfoCache.GetFieldInfo(type, fieldName))
        {
          
        }
        public FieldWriter(Type type,  FieldInfo fieldInfo)
        {
            if (null == type)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (null == fieldInfo)
            {
                throw new ArgumentNullException(nameof(fieldInfo));
            }
            if (typeof(TInstance).IsValueType)
            {
                _structSetter = CreateRefDelegate<TInstance, TField>(fieldInfo);
                _setter = null;
            }
            else
            {
                _setter = CreateSetter(fieldInfo);
                _structSetter = null;
            }
            
        }
        
        public void SetValue(ref TInstance obj, TField value)
        {
            if (typeof(TInstance).IsValueType)
            {
                _structSetter(ref obj, value);
            }
            else
            {
                _setter(obj, value);
            }
            
        }
        
        public void SetUntypedValue(ref object obj, object value)
        {
            TField parsedValue = TypeParser.Parse<TField>(value);
            if (typeof(TInstance).IsValueType)
            {
                TInstance typedObj = (TInstance) obj;
                _structSetter(ref typedObj, parsedValue);
                obj = typedObj;
            }
            else
            {

                _setter((TInstance)obj, parsedValue);
            }
        }
        
        private Action<TInstance, TField> CreateSetter(FieldInfo field)
        {
            if(null == field)
            {
                throw new ArgumentNullException(nameof(field));
            }

            if (field.ReflectedType == null)
            {
                throw new ArgumentException($"{nameof(field.FieldType)} must not be null.");
            }

            string methodName = field.ReflectedType.FullName+".set_"+field.Name;
            DynamicMethod setterMethod = new DynamicMethod(methodName, null, new[]{typeof(TInstance),typeof(TField)},true);
            ILGenerator gen = setterMethod.GetILGenerator();
            if (field.IsStatic)
            {
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Stsfld, field);
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Stfld, field);
            }
            gen.Emit(OpCodes.Ret);
            return (Action<TInstance, TField>)setterMethod.CreateDelegate(typeof(Action<TInstance, TField>));
        }


        private RefAction<T1, T2> CreateRefDelegate<T1, T2>(FieldInfo fieldInfo)
        {
            if(null == fieldInfo)
            {
                throw new ArgumentNullException(nameof(fieldInfo));
            }

            if (fieldInfo.ReflectedType == null)
            {
                throw new ArgumentException($"{nameof(fieldInfo.FieldType)} must not be null.");
            }
            
            var dynamicMethod = new DynamicMethod(String.Empty, typeof(void), new[] { fieldInfo.ReflectedType.MakeByRefType(), fieldInfo.FieldType }, true);
            var ilGenerator = dynamicMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Stfld, fieldInfo);
            ilGenerator.Emit(OpCodes.Ret);
            return (RefAction<T1, T2>)dynamicMethod.CreateDelegate(typeof(RefAction<T1, T2> ));
        }
        
    }
}
