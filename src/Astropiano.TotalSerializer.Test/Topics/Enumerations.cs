using FluentAssertions;
using Xunit;
using Astropiano.TotalSerializer.Test.TestCases.Enumerations;

namespace Astropiano.TotalSerializer.Test.Topics
{
    public class Enumerations
    {
        [Fact]
        public void SimpleEnumTest()
        {
            TestClass22WithEnumeration obj = new TestClass22WithEnumeration();
            obj.EnumValue = TestClass22WithEnumeration.TestEnum.B;
            
            TotalSerializer totalSerializer =  new TotalSerializer();
            string json =  totalSerializer.Serialize(obj);

            TestClass22WithEnumeration objD = totalSerializer.Deserialize<TestClass22WithEnumeration>(json);
            objD.EnumValue.Should().Be(obj.EnumValue);
        }
            
        [Fact]
        public void ULongFlagEnumTest()
        {
            TestClass23WithFlagEnum obj = new TestClass23WithFlagEnum();
            obj.EnumValue = TestClass23WithFlagEnum.FlagEnum.A | TestClass23WithFlagEnum.FlagEnum.B;
            
            TotalSerializer totalSerializer =  new TotalSerializer();
            string json =  totalSerializer.Serialize(obj);

            TestClass23WithFlagEnum objD = totalSerializer.Deserialize<TestClass23WithFlagEnum>(json);
            objD.EnumValue.Should().Be(obj.EnumValue);
        }
    }
}