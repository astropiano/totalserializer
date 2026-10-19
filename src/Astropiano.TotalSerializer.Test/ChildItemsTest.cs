using FluentAssertions;
using Xunit;
using Astropiano.TotalSerializer.Test.TestCases;

namespace Astropiano.TotalSerializer.Test
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
            json.Should().Be("{\"$type\":\"Astropiano.TotalSerializer.Test.TestCases.TestClass6WithChild, Astropiano.TotalSerializer.Test\",\"$id\":1,\"integer\":35,\"child\":{\"$type\":\"Astropiano.TotalSerializer.Test.TestCases.TestClass6WithChild, Astropiano.TotalSerializer.Test\",\"$id\":2,\"integer\":0,\"child\":null}}");
        }

        [Fact(Skip = "WIP - due to changes in TypeNameWriter")]
        public void SelfReferenceChildStructureTest()
        {
            TestClass6WithChild obj = new TestClass6WithChild();
            obj.Integer = 35;
            obj.Child = obj;

            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(obj);
            json.Should().Be("{\"$type\":\"Astropiano.TotalSerializer.Test.TestCases.TestClass6WithChild, Astropiano.TotalSerializer.Test\",\"$id\":1,\"integer\":35,\"child\":{\"$ref\":1}}");
        }
    }
}