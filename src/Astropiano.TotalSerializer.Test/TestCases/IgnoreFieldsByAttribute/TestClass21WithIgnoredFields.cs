using Astropiano.TotalSerializer.Attributes;

namespace Astropiano.TotalSerializer.Test.TestCases.IgnoreFieldsByAttribute
{
    public class TestClass21WithIgnoredFields
    {
        public int NotIgnored;
        
        [TotalSerializerIgnoreField]
        public int Ignored;
    }
}