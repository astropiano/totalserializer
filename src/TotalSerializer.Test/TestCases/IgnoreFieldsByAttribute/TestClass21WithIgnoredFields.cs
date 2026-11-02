using TotalSerializer.Attributes;

namespace TotalSerializer.Test.TestCases.IgnoreFieldsByAttribute
{
    public class TestClass21WithIgnoredFields
    {
        public int NotIgnored;
        
        [TotalSerializerIgnoreField]
        public int Ignored;
    }
}