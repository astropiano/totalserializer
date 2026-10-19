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
    public class SystemType
    {
        [Fact]
        public void TypePropertyAndArrayTest()
        {
            TestClassWithType t = TestClassWithType.Create();
            TotalSerializer serializer = new TotalSerializer();

            string json = serializer.Serialize(t);
            TestClassWithType d = serializer.Deserialize<TestClassWithType>(json);

            d.Should().NotBeNull("Could not deserialize.");
            t.Type1.FullName.Should().Be(d.Type1.FullName, "Wrong full name of type 1");
            t.TypeArray1.Length.Should().Be(d.TypeArray1.Length, "Wrong length of array");
            for(int i = 0;i<t.TypeArray1.Length;i++)
            {
                t.TypeArray1[i].FullName.Should().Be(d.TypeArray1[i].FullName, $"Wrong type at '{i}'.");
            }
        }
        
        [Fact]
        public void PropertyInfoAndArrayTest()
        {
            TestClassWithPropertyInfo t = TestClassWithPropertyInfo.Create();
            TotalSerializer serializer = new TotalSerializer();

            string json = serializer.Serialize(t);
            TestClassWithPropertyInfo d = serializer.Deserialize<TestClassWithPropertyInfo>(json);

            d.Should().NotBeNull("Could not deserialize.");
            d.MyPropertyInfo.Should().NotBeNull("Missing property");
            t.MyPropertyInfo.Name.Should().Be(d.MyPropertyInfo.Name, "Wrong name");
            t.MyPropertyInfo.IsSpecialName.Should().Be(d.MyPropertyInfo.IsSpecialName, "Wrong IsSpecialName");
            t.MyPropertyInfo.CanRead.Should().Be(d.MyPropertyInfo.CanRead, "Wrong CanRead");
            d.MyPropertyInfoArray.Should().NotBeNull("missing array");
            t.MyPropertyInfoArray.Length.Should().Be(d.MyPropertyInfoArray.Length, "Wrong length of array");
            for(int i = 0;i<t.MyPropertyInfoArray.Length;i++)
            {
                t.MyPropertyInfoArray[i].Name.Should().Be(d.MyPropertyInfoArray[i].Name, $"Wrong name at '{i}'.");
            }
        }
        
        [Fact]
        public void FieldInfoAndArrayTest()
        {
            TestClassWithFieldInfo t = TestClassWithFieldInfo.Create();
            TotalSerializer serializer = new TotalSerializer();

            string json = serializer.Serialize(t);
            TestClassWithFieldInfo d = serializer.Deserialize<TestClassWithFieldInfo>(json);

            d.Should().NotBeNull("Could not deserialize.");
            d.MyFieldInfo.Should().NotBeNull("property is missing");
            t.MyFieldInfo.Name.Should().Be(d.MyFieldInfo.Name, "Wrong name");
            t.MyFieldInfo.IsSpecialName.Should().Be(d.MyFieldInfo.IsSpecialName, "Wrong IsSpecialName");
            t.MyFieldInfo.FieldType.Should().Be(d.MyFieldInfo.FieldType, "Wrong FieldType");
            d.MyFieldInfoArray.Should().NotBeNull("missing array");
            t.MyFieldInfoArray.Length.Should().Be(d.MyFieldInfoArray.Length, "Wrong length of array");
            for(int i = 0;i<t.MyFieldInfoArray.Length;i++)
            {
                t.MyFieldInfoArray[i].Name.Should().Be(d.MyFieldInfoArray[i].Name, $"Wrong name at '{i}'.");
            }
        }

        
        
    }
}
