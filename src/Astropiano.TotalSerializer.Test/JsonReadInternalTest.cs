using System;
using FluentAssertions;
using Xunit;
using Astropiano.TotalSerializer.Configuration;
using Astropiano.TotalSerializer.Test.TestCases;

namespace Astropiano.TotalSerializer.Test
{
    /// <summary>
    /// This class aims to test internal stateless methods
    /// </summary>
    public class JsonReadInternalTest
    {
        private Config _config;

        private JsonReader _jsonReader;

        public JsonReadInternalTest()
        {
            _config = new Config();
            _jsonReader = new JsonReader(_config);
        }
        
        [Fact]
        public void IsPropertyDeclarationTest()
        {
            char[] objectWithIntegerJsonCharArrays = "{\"Value\":2}".ToCharArray();
            int currentIndex = 1;
            int propertyNameEndIndex = 0;
                
            _jsonReader.IsPropertyDeclaration(
                objectWithIntegerJsonCharArrays, 
                ref currentIndex, 
                ref propertyNameEndIndex).Should().BeTrue();
            currentIndex.Should().Be(2);
            propertyNameEndIndex.Should().Be(7);
        }
        
        [Fact]
        public void IsPropertyDeclarationNegativeTest()
        {
            char[] objectWithIntegerJsonCharArrays = "{/*test*/}".ToCharArray();
            int currentIndex = 1;
            int propertyNameEndIndex = 0;
                
            _jsonReader.IsPropertyDeclaration(
                objectWithIntegerJsonCharArrays, 
                ref currentIndex, 
                ref propertyNameEndIndex).Should().BeFalse();
            currentIndex.Should().Be(1);
        }
        
        [Fact]
        public void IsTypeDeclarationTest()
        {
            char[] objectWithIntegerJsonCharArrays = "{\"$type\":\"TestClass\"}".ToCharArray();
            int currentIndex = 2;
                
            _jsonReader.IsTypeDeclaration(objectWithIntegerJsonCharArrays, ref currentIndex).Should().BeTrue();
            currentIndex.Should().Be(2);
        }
        
        [Fact]
        public void GetTypeNameTest()
        {
            char[] json = "{\"$type\":\"TestClass\"}".ToCharArray();
            int currentIndex = 8;

            _jsonReader.GetTypeName(json, ref currentIndex, out _).Should().Be("TestClass");
        }

        [Fact]
        public void GetTypeTest()
        {
            Type type = typeof(TestClass0);
            char[] json = ("{\"$type\":\"" +  type.Namespace + '.' + type.Name +',' 
                           + type.Assembly.GetName().Name + "\"}").ToCharArray();
            int currentIndex = 8;
            int iterator = 0;
                
            _jsonReader.ProcessTypeDeclaration(json, ref currentIndex, out iterator).Should().Be(type);
        }
    }
}