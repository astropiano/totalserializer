using System;

namespace Astropiano.TotalSerializer.Interfaces
{
    /// <summary>
    /// This interfaces describes the methods a custom factory should implement. 
    /// </summary>
    public interface IFactory
    {
        /// <summary>
        /// Creates an instance of type <see cref="T"/>.
        /// </summary>
        /// <typeparam name="T">Generic type to be created.</typeparam>
        /// <returns>A new instance.</returns>
        T Create<T>();

        /// <summary>
        /// Creates an instance of type <paramref name="objType"/>.
        /// </summary>
        /// <param name="objType">Type to be created.</param>
        /// <returns>A new instance.</returns>
        object Create(Type objType);

        T Create<T>(params object[] constructorParameters)  where T : class;
        
        object Create(Type objType, params object[] constructorParameters);
        
        Array CreateArray(Type objType, int size);

        Array CreateArray(Type objType, params int[] sizes);

        object Activate(Type objType, params object[] constructorParameters);
    }
}
