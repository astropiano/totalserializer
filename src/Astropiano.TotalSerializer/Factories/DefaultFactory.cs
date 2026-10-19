using System;
using System.Runtime.Serialization;
using Astropiano.ReflectionTools;
using Astropiano.TotalSerializer.Attributes;
using Astropiano.TotalSerializer.Interfaces;

namespace Astropiano.TotalSerializer.Factories
{
    public class DefaultFactory : IFactory
    {
        public object Create(Type objType)
        {
            
            if (!objType.IsValueType && !objType.IsAbstract)
            {
                var constructor = ConstructorInfoCache.GetParameterLessConstructorInfo(objType);
                if (constructor != null)
                {
                    if (Attribute.IsDefined(constructor, typeof(TotalSerializerUseConstructorAttribute)))
                    {
                        return constructor.Invoke(new object[0]);
                    }
                }
            }
            
            return FormatterServices.GetUninitializedObject(objType);
        }

        public T Create<T>(params object[] constructorParameters) where T : class
        {
            return Create(typeof(T), constructorParameters) as T;
        }

        public object Create(Type objType, params object[] constructorParameters)
        {
            if (!objType.IsValueType && !objType.IsAbstract)
            {
                Type[] parameterTypes = new Type[constructorParameters.Length];
                for (int i = 0; i < constructorParameters.Length; i++)
                {
                    parameterTypes[i] = constructorParameters[i].GetType();
                }

                var constructor = ConstructorInfoCache.GetConstructor(objType, parameterTypes);
                if (constructor != null)
                {
                    if (Attribute.IsDefined(constructor, typeof(TotalSerializerUseConstructorAttribute)))
                    {
                        return constructor.Invoke(new object[0]);
                    }
                }
            }
            return FormatterServices.GetUninitializedObject(objType);
        }

        public Array CreateArray(Type objType, int size)
        {
            Type elementType = objType.GetElementType();
            if (elementType == null)
            {
                throw new ArgumentException("Array type misses element type");
            }
            return Array.CreateInstance(elementType, size);
        }
        
        public Array CreateArray(Type objType, params int[] sizes)
        {
            Type elementType = objType.GetElementType();
            if (elementType == null)
            {
                throw new ArgumentException("Array type misses element type");
            }
            return Array.CreateInstance(elementType, sizes);
        }

        public object Activate(Type objType, params object[] constructorParameters)
        {
            return Activator.CreateInstance(objType, constructorParameters);
        }

        public T Create<T>()
        {
            return (T) FormatterServices.GetUninitializedObject(typeof(T));
        }
    }
}
