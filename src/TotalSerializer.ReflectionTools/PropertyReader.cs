using System;
using System.Reflection;
using System.Reflection.Emit;

namespace TotalSerializer.ReflectionTools
{
    /// <summary>
    /// Creates dynamic getters for given properties.
    /// </summary>
    /// <typeparam name="TInstance">The type of the instance.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    public class PropertyReader<TInstance, TProperty> : IMemberReader<TInstance, TProperty>
    {
        private Func<TInstance, TProperty> _getter;

        public PropertyReader(Type type, string propertyName) : this(type, PropertyInfoCache.GetPropertyInfo(type, propertyName))
        {

        }

        public PropertyReader(Type type, PropertyInfo propertyInfo)
        {
            if (null == type)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (null == propertyInfo)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }
            _getter = CreateGetter(propertyInfo);
        }

        public TProperty GetValue(TInstance obj)
        {
            return _getter(obj);
        }

        public object GetUntypedValue(object obj)
        {
            if (null == obj)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return _getter((TInstance)obj);
        }

        private Func<TInstance, TProperty> CreateGetter(PropertyInfo property)
        {
            MethodInfo getter = property.GetMethod;
            if (null == getter)
            {
                throw new ArgumentException($"Property '{property.Name}' has no getter.", nameof(property));
            }

            string methodName;
            if (property.ReflectedType != null)
            {
                methodName = string.Concat(property.ReflectedType.FullName, ".", getter.Name);
            }
            else
            {
                methodName = getter.Name;
            }

            DynamicMethod getterMethod = new DynamicMethod(methodName, typeof(TProperty), new[] { typeof(TInstance) }, true);
            ILGenerator gen = getterMethod.GetILGenerator();
            if (getter.IsStatic)
            {
                gen.Emit(OpCodes.Call, getter);
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(getter.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, getter);
            }
            gen.Emit(OpCodes.Ret);
            return (Func<TInstance, TProperty>)getterMethod.CreateDelegate(typeof(Func<TInstance, TProperty>));
        }
    }
}
