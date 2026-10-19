using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Astropiano.TotalSerializer.Configuration;
using Astropiano.TotalSerializer.Test.TestCases.Dictionaries;

namespace Astropiano.TotalSerializer.Test.Topics
{
    public class IgnoredTypes
    {
        [Fact]
        public void IgnoredTypeGenericTest()
        {
            Config config = new Config();

            config.IgnoreType<Dictionary<int, int>>();
            
            TestClass18WithDictionaries obj = new TestClass18WithDictionaries();
            obj.PrimitiveIntDictionary = new Dictionary<int, int>();
            TotalSerializer totalSerializer = new TotalSerializer(config);

            string json = totalSerializer.Serialize(obj);
            
            json.Should().NotContain("_primitiveIntDictionary");
        }
        
        [Fact]
        public void IgnoredTypeTypedTest()
        {
            Config config = new Config();

            config.IgnoreType(typeof(Dictionary<int, int>));
            
            TestClass18WithDictionaries obj = new TestClass18WithDictionaries();
            obj.PrimitiveIntDictionary = new Dictionary<int, int>();
            TotalSerializer totalSerializer = new TotalSerializer(config);

            string json = totalSerializer.Serialize(obj);
            
            json.Should().NotContain("_primitiveIntDictionary");
        }
    }
}