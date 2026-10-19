using System;
using FluentAssertions;
using Xunit;
using Astropiano.TotalSerializer.CharArray;
using Astropiano.TotalSerializer.Exceptions;

namespace Astropiano.TotalSerializer.Test.CharArray
{
    public class CharArrayComparerTest
    {
        [Fact]
        public void ContainsArrayTest()
        {
            char[] test1 = "Hello World".ToCharArray();
            char[] seek1 = "Hello".ToCharArray();
            char[] seek2 = "World".ToCharArray();
            CharArrayComparer.ContainsAt(test1, 0, seek1).Should().BeTrue();
            CharArrayComparer.ContainsAt(test1, 1, seek1).Should().BeFalse();
            CharArrayComparer.ContainsAt(test1, 6, seek2).Should().BeTrue();
            CharArrayComparer.ContainsAt(test1, 7, seek2).Should().BeFalse();
        }

        [Fact]
        public void SeekNextOccurenceTest()
        {
            char[] test1 = "Hello World".ToCharArray();

            int index = 0;
            int iterator = 0;
            CharArrayComparer.SeekNextOccurence(test1, 'H', ref iterator, out index);
            index.Should().Be(0);
            iterator.Should().Be(0);

            index = 0;
            iterator = 0;
            CharArrayComparer.SeekNextOccurence(test1, 'e', ref iterator, out index);
            index.Should().Be(1);
            iterator.Should().Be(1);

            index = 0;
            iterator = 0;
            CharArrayComparer.SeekNextOccurence(test1, 'W', ref iterator, out index);
            index.Should().Be(6);
            iterator.Should().Be(6);

            index = 0;
            iterator = 4;
            CharArrayComparer.SeekNextOccurence(test1, 'W', ref iterator, out index);
            index.Should().Be(6);
            iterator.Should().Be(6);

            index = 0;
            iterator = 0;
            CharArrayComparer.SeekNextOccurence(test1, ' ', ref iterator, out index);
            index.Should().Be(5);
            iterator.Should().Be(5);

            index = 0;
            iterator = 0;
            CharArrayComparer.SeekNextOccurence(test1, 'Z', ref iterator, out index);
            index.Should().Be(-1);
            iterator.Should().Be(11);

            index = 0;
            iterator = 100;
            CharArrayComparer.SeekNextOccurence(test1, 'Z', ref iterator, out index);
            index.Should().Be(-1);
            iterator.Should().Be(100);

            index = 0;
            iterator = 9;
            CharArrayComparer.SeekNextOccurence(test1, 'Z', ref iterator, out index);
            index.Should().Be(-1);
            iterator.Should().Be(11);
        }

        [Fact]
        public void SeekNextOccurenceWithOrLinkTest()
        {
            char[] test1 = "Hello World".ToCharArray();

            int index = 0;
            int iterator = 0;
            CharArrayComparer.SeekNextOccurenceWithOrLink(test1, new char[] { 'W', 'X'}, ref iterator, ref index);
            index.Should().Be(6, "W OR X is contained but not found!");
            iterator.Should().Be(6, "wrong iterator");

            index = 0;
            iterator = 0;
            CharArrayComparer.SeekNextOccurenceWithOrLink(test1, new char[] { 'Y', 'X'}, ref iterator, ref index);
            index.Should().Be(-1, "Neither Y NOR X are contained but were found!");
            iterator.Should().Be(11, "wrong iterator");
        }

        [Fact]
        public void SeekJsonScopeEndTest()
        {
            char[] test1 = "{\"MyInt\":123,\"MyObject\":{\"MyDouble\":3.14159}}".ToCharArray();
            int index = 0;
            int iterator = 1;
            CharArrayComparer.SeekJsonScopeEnd(test1, ref iterator, out index);
            index.Should().Be(12, "Wrong index found for first property.");
            iterator.Should().Be(12, "Wrong iterator found for first property.");
            iterator = index + 1;
            CharArrayComparer.SeekJsonScopeEnd(test1, ref iterator, out index);
            index.Should().Be(test1.Length - 1, "Wrong index found for second property.");
            iterator.Should().Be(test1.Length - 1, "Wrong iterator found for second property.");
            iterator = 1;
            CharArrayComparer.SeekNextOccurence(test1, '{', ref iterator, out index);
            iterator.Should().Be(24, "Wrong iterator");
            index.Should().Be(24, "Wrong index");
            iterator += 1;
            CharArrayComparer.SeekJsonScopeEnd(test1, ref iterator, out index);
            index.Should().Be(test1.Length - 2, "Wrong index found for inner property.");
            iterator.Should().Be(test1.Length - 2, "Wrong iterator found for inner property.");
        }

        [Fact]
        public void EnforceNextOccurenceTest()
        {
            string str1 = "{\"MyString\":\"J3.16\"}";
            char[] test1 = str1.ToCharArray();
            int index = str1.IndexOf(':');
            Action act1 = () => CharArrayComparer.EnforceNextOccurence(test1, ':', ref index);
            act1.Should().NotThrow();
            Action act2 = () => CharArrayComparer.EnforceNextOccurence(test1, '*', ref index);
            act2.Should().Throw<JsonFormatException>();
        }
    }
}
