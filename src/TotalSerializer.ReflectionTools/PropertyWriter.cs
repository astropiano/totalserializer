using System;
using System.Reflection;
using System.Reflection.Emit;

namespace TotalSerializer.ReflectionTools
{
    /// <summary>
    /// Creates dynamic setters for given properties.
    /// </summary>
    /// <typeparam name="TInstance">The type of the instance.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    public class PropertyWriter<TInstance, TProperty> : IMemberWriter<TInstance, TProperty>
    {
        delegate void RefAction<T1, T2>(ref T1 arg1, T2 arg2);

        private Action<TInstance, TProperty> _setter;

        private RefAction<TInstance, TProperty> _structSetter;

        public PropertyWriter(Type type, string propertyName) : this(type, PropertyInfoCache.GetPropertyInfo(type, propertyName))
        {

        }

        public PropertyWriter(Type type, PropertyInfo propertyInfo)
        {
            if (null == type)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (null == propertyInfo)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }
            if (typeof(TInstance).IsValueType)
            {
                _structSetter = CreateRefDelegate<TInstance, TProperty>(propertyInfo);
                _setter = null;
            }
            else
            {
                _setter = CreateSetter(propertyInfo);
                _structSetter = null;
            }
        }

        public void SetValue(ref TInstance obj, TProperty value)
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
            TProperty parsedValue = TypeParser.Parse<TProperty>(value);
            if (typeof(TInstance).IsValueType)
            {
                TInstance typedObj = (TInstance)obj;
                _structSetter(ref typedObj, parsedValue);
                obj = typedObj;
            }
            else
            {
                _setter((TInstance)obj, parsedValue);
            }
        }

        private Action<TInstance, TProperty> CreateSetter(PropertyInfo property)
        {
            MethodInfo setter = property.SetMethod;
            if (null == setter)
            {
                throw new ArgumentException($"Property '{property.Name}' has no setter.", nameof(property));
            }

            string methodName;
            if (property.ReflectedType != null)
            {
                methodName = string.Concat(property.ReflectedType.FullName, ".", setter.Name);
            }
            else
            {
                methodName = setter.Name;
            }

            DynamicMethod setterMethod = new DynamicMethod(methodName, null, new[] { typeof(TInstance), typeof(TProperty) }, true);
            ILGenerator gen = setterMethod.GetILGenerator();
            if (setter.IsStatic)
            {
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Call, setter);
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(setter.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, setter);
            }
            gen.Emit(OpCodes.Ret);
            return (Action<TInstance, TProperty>)setterMethod.CreateDelegate(typeof(Action<TInstance, TProperty>));
        }

        private RefAction<T1, T2> CreateRefDelegate<T1, T2>(PropertyInfo propertyInfo)
        {
            MethodInfo setter = propertyInfo.SetMethod;
            if (null == setter)
            {
                throw new ArgumentException($"Property '{propertyInfo.Name}' has no setter.", nameof(propertyInfo));
            }
            if (propertyInfo.ReflectedType == null)
            {
                throw new ArgumentException($"{nameof(propertyInfo.PropertyType)} must not be null.");
            }

            string methodName = propertyInfo.ReflectedType.FullName + ".setref_" + propertyInfo.Name;
            DynamicMethod setterMethod = new DynamicMethod(methodName, null,
                new[] { typeof(T1).MakeByRefType(), typeof(T2) }, true);
            ILGenerator gen = setterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            if (!propertyInfo.ReflectedType.IsValueType)
            {
                gen.Emit(OpCodes.Ldind_Ref);
            }
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(setter.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, setter);
            gen.Emit(OpCodes.Ret);
            return (RefAction<T1, T2>)setterMethod.CreateDelegate(typeof(RefAction<T1, T2>));
        }
    }
}
