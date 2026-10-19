using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Astropiano.TotalSerializer.Configuration;
using Astropiano.TotalSerializer.Interfaces;

namespace Astropiano.TotalSerializer.Test.Topics
{
    public class RecreateTypeTest
    {
        private interface IMyMarkerInterface
        {
            
        }
        
        private class MyMarkerBaseType
        {
            
        }
        
        private class TypeWithRecreateInstance
        {
            public TypeToBReconstructed Instance;
        }
        private class TypeToBReconstructed : MyMarkerBaseType, IMyMarkerInterface
        {
            public int Value;
        }

        private class MyFactory : IFactory
        {
            public IList<Type> CreatedTypes = new List<Type>();
            
            public T Create<T>()
            {
                return (T) Create(typeof(T));
            }

            public object Create(Type objType)
            {
                CreatedTypes.Add(objType);
                return Activator.CreateInstance(objType);
            }

            public T Create<T>(params object[] constructorParameters) where T : class
            {
                throw new NotImplementedException();
            }

            public object Create(Type objType, params object[] constructorParameters)
            {
                throw new NotImplementedException();
            }

            public Array CreateArray(Type objType, int size)
            {
                throw new NotImplementedException();
            }

            public Array CreateArray(Type objType, params int[] sizes)
            {
                throw new NotImplementedException();
            }

            public object Activate(Type objType, params object[] constructorParameters)
            {
                throw new NotImplementedException();
            }
        }
        
        [Fact]
        public void RegisteredRecreateTypeTest()
        {
            Config config = new Config();
            MyFactory myFactory = new MyFactory();
            config.RegisterRecreateTypeFactory<TypeToBReconstructed>(myFactory);
            
            TotalSerializer totalSerializer = new TotalSerializer(config);
            
            myFactory.CreatedTypes.Count.Should().Be(0);
             
            TypeWithRecreateInstance obj = new TypeWithRecreateInstance();
            obj.Instance = new TypeToBReconstructed();
            obj.Instance.Value = 1;
            TypeWithRecreateInstance objD = totalSerializer.Copy(obj);
            
            myFactory.CreatedTypes.Count.Should().Be(1);
            obj.Should().NotBe(objD);
            objD.Instance.Value.Should().Be(0, "Value should have default value after being recreated."); 
        }
        
        [Fact]
        public void RegisteredRecreateTypeAsRootTest()
        {
            Config config = new Config();
            MyFactory myFactory = new MyFactory();
            config.RegisterRecreateTypeFactory<TypeToBReconstructed>(myFactory);
            
            TotalSerializer totalSerializer = new TotalSerializer(config);
            
            myFactory.CreatedTypes.Count.Should().Be(0);
             
            TypeToBReconstructed obj = new TypeToBReconstructed();
            obj.Value = 1;
            TypeToBReconstructed objD = totalSerializer.Copy(obj);
            
            myFactory.CreatedTypes.Count.Should().Be(1);
            obj.Should().NotBe(objD);
            objD.Value.Should().Be(0, "Value should have default value after being recreated."); 
        }

        [Fact]
        public void RegisteredRecreateTypeByMarkerInterfaceTest()
        {
            Config config = new Config();
            MyFactory myFactory = new MyFactory();
            config.RegisterRecreateTypeFactory<IMyMarkerInterface>(myFactory);
            
            TotalSerializer totalSerializer = new TotalSerializer(config);
            
            myFactory.CreatedTypes.Count.Should().Be(0);
             
            TypeToBReconstructed obj = new TypeToBReconstructed();
            obj.Value = 1;
            TypeToBReconstructed objD = totalSerializer.Copy(obj);
            
            myFactory.CreatedTypes.Count.Should().Be(1);
            obj.Should().NotBe(objD);
            objD.Value.Should().Be(0, "Value should have default value after being recreated."); 
        }
        
        [Fact]
        public void RegisteredRecreateTypeByMarkerBaseTypeTest()
        {
            Config config = new Config();
            MyFactory myFactory = new MyFactory();
            config.RegisterRecreateTypeFactory<MyMarkerBaseType>(myFactory);
            
            TotalSerializer totalSerializer = new TotalSerializer(config);
            
            myFactory.CreatedTypes.Count.Should().Be(0);
             
            TypeToBReconstructed obj = new TypeToBReconstructed();
            obj.Value = 1;
            TypeToBReconstructed objD = totalSerializer.Copy(obj);
            
            myFactory.CreatedTypes.Count.Should().Be(1);
            obj.Should().NotBe(objD);
            objD.Value.Should().Be(0, "Value should have default value after being recreated."); 
        }
    }
}