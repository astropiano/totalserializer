using System;
using Astropiano.TotalSerializer.Interfaces;

namespace Astropiano.TotalSerializer.Test.TestCases
{
    /// <summary>
    /// An empty class without empty constructor. 
    /// </summary>
    public class TestClass5WithoutParameterlessConstructor
    {
        private TestClass5WithoutParameterlessConstructor(int dummy)
        {

        }

        public static TestClass5WithoutParameterlessConstructor Create()
        {
            return new TestClass5WithoutParameterlessConstructor(1);
        }

        public int TestField { get; set; }
    }

    internal class TestFactory5 : IFactory
    {
        public T Create<T>()
        {
            if(typeof(T) == typeof(TestClass5WithoutParameterlessConstructor))
            {
                object o = TestClass5WithoutParameterlessConstructor.Create();
                return (T) o;
            }
            return Activator.CreateInstance<T>();
        }

        public object Create(Type objType)
        {
            return TestClass5WithoutParameterlessConstructor.Create();
        }

        public T Create<T>(params object[] constructorParameters) where T : class
        {
            throw new NotImplementedException();
        }

        public object Create(Type objType, params object[] constructorParameters)
        {
            throw new NotImplementedException();
        }

        public System.Array CreateArray(Type objType, int size)
        {
            throw new NotImplementedException();
        }

        public System.Array CreateArray(Type objType, params int[] sizes)
        {
            throw new NotImplementedException();
        }

        public object Activate(Type objType, params object[] constructorParameters)
        {
            throw new NotImplementedException();
        }
    }
}
