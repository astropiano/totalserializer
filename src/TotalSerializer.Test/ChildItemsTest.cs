using FluentAssertions;
using Xunit;
using TotalSerializer.Test.TestCases;

namespace TotalSerializer.Test
{
    public class ChildItemsTest
    {
        [Fact(Skip = "WIP - due to changes in TypeNameWriter")]
        public void OneChildStructureTest()
        {
            TestClass6WithChild obj = new TestClass6WithChild();
            obj.Integer = 35;
            obj.Child = new TestClass6WithChild();

            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(obj);
            json.Should().Be("{\"$type\":\"TotalSerializer.Test.TestCases.TestClass6WithChild, TotalSerializer.Test\",\"$id\":1,\"integer\":35,\"child\":{\"$type\":\"TotalSerializer.Test.TestCases.TestClass6WithChild, TotalSerializer.Test\",\"$id\":2,\"integer\":0,\"child\":null}}");
        }

        [Fact(Skip = "WIP - due to changes in TypeNameWriter")]
        public void SelfReferenceChildStructureTest()
        {
            TestClass6WithChild obj = new TestClass6WithChild();
            obj.Integer = 35;
            obj.Child = obj;

            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(obj);
            json.Should().Be("{\"$type\":\"TotalSerializer.Test.TestCases.TestClass6WithChild, TotalSerializer.Test\",\"$id\":1,\"integer\":35,\"child\":{\"$ref\":1}}");
        }
    }
}