using FluentAssertions;
using Xunit;
using TotalSerializer.ReflectionTools.Extensions;
using TotalSerializer.ReflectionTools.Test.TestCases;

namespace TotalSerializer.ReflectionTools.Test
{
    public class ExtensionsTest
    {
        
        [Fact]
        public void ExtensionMethodTest()
        {
            FieldTestClass f = new FieldTestClass();
            f.PublicIntegerValue = 5;
            f.GetValue<int>(nameof(FieldTestClass.PublicIntegerValue)).Should().Be(5);
            
            f.SetValue(nameof(FieldTestClass.PublicIntegerValue), 2);
            f.PublicIntegerValue.Should().Be(2);
        }
    }
}
