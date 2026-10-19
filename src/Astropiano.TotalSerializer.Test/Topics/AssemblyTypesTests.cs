using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Xunit;
using Astropiano.TotalSerializer.Configuration;
using Astropiano.TotalSerializer.Test.TestCases.SystemType;

namespace Astropiano.TotalSerializer.Test.Topics
{
    public class AssemblyTypesTests
    {
        private Config _config;
        
        public AssemblyTypesTests()
        {
            _config = new Config();
            _config.ErrorStrategy = ErrorStrategy.ThrowException;
        }
        
        [Fact(Skip = "WIP - The test is work in progress.")]
        public void OverallSystemTypeTest()
        {
            IList<Type> problematicsTypes = new List<Type>();
            TestAssembly(Assembly.Load("System"), problematicsTypes);
            TestAssembly(Assembly.Load("mscorlib"), problematicsTypes);
            TestAssembly(Assembly.Load("System.Reflection"), problematicsTypes);
            TestAssembly(Assembly.Load("System.Runtime"), problematicsTypes);
            TestAssembly(typeof(TimeSpan).Assembly, problematicsTypes);
            problematicsTypes.Should().BeEmpty($"Found {problematicsTypes.Count} problematic types.");
        }
        
        public static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        private void TestAssembly(Assembly assembly, IList<Type> problematicsTypes)
        {
            TotalSerializer serializer = new TotalSerializer();
            TestClassWithObjectField obj = new TestClassWithObjectField();
            TestClassWithObjectField objD;
            var types = GetLoadableTypes(assembly);
            foreach (var systemType in types)
            {
                try
                {
                    if (systemType.IsAbstract)
                    {
                        continue;
                    }
                    if (systemType.IsGenericType)
                    {
                        continue; //TODO: cover this in test
                    }
                    
                    var constructor = systemType.GetConstructor(Type.EmptyTypes);
                    if (constructor != null)
                    {
                        obj.Anything = constructor.Invoke(new object[0]);    
                    }
                    else
                    {
                        obj.Anything = _config.Factory.Create(systemType);
                    }
                    
                    obj.Anything.Should().NotBeNull();

                    objD = serializer.Copy(obj);
                    objD.Should().NotBeNull();
                }
                catch (Exception)
                {
                    problematicsTypes.Add(systemType);
                }
            }
        }
    }
}