using FluentAssertions;
using Xunit;
using Astropiano.TotalSerializer.Test.TestCases.IgnoreFieldsByAttribute;

namespace Astropiano.TotalSerializer.Test.Topics
{
    public class IgnoreFieldsByAttribute
    {
        [Fact]
        public void TestCanIgnoreFields()
        {
            TestClass21WithIgnoredFields obj = new TestClass21WithIgnoredFields();
            obj.Ignored = 1;
            obj.NotIgnored = 1;
            
            TotalSerializer serializer = new TotalSerializer();

            string json = serializer.Serialize(obj);

            TestClass21WithIgnoredFields objD = serializer.Deserialize<TestClass21WithIgnoredFields>(json);
            
            objD.NotIgnored.Should().Be(1);
            objD.Ignored.Should().Be(0);
        }
    }
}