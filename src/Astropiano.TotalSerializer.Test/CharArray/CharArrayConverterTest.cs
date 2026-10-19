using FluentAssertions;
using Xunit;
using Astropiano.TotalSerializer.CharArray;

namespace Astropiano.TotalSerializer.Test.CharArray
{
    public class CharArrayConverterTest
    {
        private CharArrayConverter charArrayConverter = new CharArrayConverter();
        
        [Fact]
        public void GetPartialStringTest()
        {
            char[] test = "Hello World".ToCharArray();
            int iter = 0;
            int start = 0;
            int end = 5;
            string partial = charArrayConverter.GetPartialString(test, start, end);
            partial.Should().Be("Hello");
            start = 6;
            end = 11;
            partial = charArrayConverter.GetPartialString(test, start, end);
            partial.Should().Be("World");

            // Too large 'end' index - should not crash:
            int endTooLarge = 1000;
            partial = charArrayConverter.GetPartialString(test, start, endTooLarge);
            partial.Should().Be("World");
        }
    }
}