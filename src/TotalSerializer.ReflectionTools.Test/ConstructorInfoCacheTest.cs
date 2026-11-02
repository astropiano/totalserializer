using System;
using System.Reflection;
using FluentAssertions;
using Xunit;
using TotalSerializer.ReflectionTools.Test.TestCases;

namespace TotalSerializer.ReflectionTools.Test
{
    public class ConstructorInfoCacheTest
    {
        [Fact]
        public void GetParameterLessConstructorInfoTest()
        {
            Type type = typeof(ConstructorExample1);
            ConstructorInfo constructorInfo = ConstructorInfoCache.GetParameterLessConstructorInfo(type);
            constructorInfo.Should().NotBeNull();
            constructorInfo.ReflectedType.Should().Be(type, "Wrong type.");
        }

        [Fact]
        public void GetConstructorTest()
        {
            Type type = typeof(ConstructorExample1);
            {
                ConstructorInfo constructorInfo = ConstructorInfoCache.GetConstructor(type, new[] { typeof(string) });
                constructorInfo.Should().NotBeNull();
                constructorInfo.ReflectedType.Should().Be(type, "Wrong type.");
            }
            {
                ConstructorInfo constructorInfo = ConstructorInfoCache.GetConstructor(type, new[] { typeof(int) });
                constructorInfo.Should().NotBeNull();
                constructorInfo.ReflectedType.Should().Be(type, "Wrong type.");
            }
            {
                ConstructorInfo constructorInfo = ConstructorInfoCache.GetConstructor(type, new[] { typeof(string), typeof(int) });
                constructorInfo.Should().NotBeNull();
                constructorInfo.ReflectedType.Should().Be(type, "Wrong type.");
            }
        }
    }
}
