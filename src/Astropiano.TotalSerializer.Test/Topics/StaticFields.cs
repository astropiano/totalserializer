using FluentAssertions;
using Xunit;
using Astropiano.TotalSerializer.Test.TestCases.StaticFields;

namespace Astropiano.TotalSerializer.Test.Topics
{
    public class StaticFields
    {
        // We do not want to serialize static fields.
        [Fact]
        public void ClassWithStaticField()
        {
            TestClass19WithStaticField obj = new TestClass19WithStaticField();
            TestClass19WithStaticField.TestInteger = 10; //TODO consider that this value is stored in  each JSON portion!
            
            TotalSerializer serializer =  new TotalSerializer();
            string json = serializer.Serialize(obj);

            TestClass19WithStaticField.TestInteger = 5; //json should not set it back to 10
            
            TestClass19WithStaticField objD = serializer.Deserialize<TestClass19WithStaticField>(json);
            
            TestClass19WithStaticField.TestInteger.Should().Be(5);
        }
    }
}