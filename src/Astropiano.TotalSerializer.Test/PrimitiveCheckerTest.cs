using FluentAssertions;
using Xunit;
using System;

namespace Astropiano.TotalSerializer.Test
{
    public class PrimitiveCheckerTest
    {
        [Fact]
        public void TestLong()
        {
            PrimitiveChecker.IsPrimitive(typeof(long)).Should().BeTrue();
            PrimitiveChecker.IsPrimitive(typeof(long?)).Should().BeTrue();
        }

        [Fact]
        public void TestInt()
        {
            PrimitiveChecker.IsPrimitive(typeof(int)).Should().BeTrue();
            PrimitiveChecker.IsPrimitive(typeof(int?)).Should().BeTrue();
        }

        [Fact]
        public void TestShort()
        {
            PrimitiveChecker.IsPrimitive(typeof(short)).Should().BeTrue();
            PrimitiveChecker.IsPrimitive(typeof(short?)).Should().BeTrue();
        }

        [Fact]
        public void TestByte()
        {
            PrimitiveChecker.IsPrimitive(typeof(byte)).Should().BeTrue();
            PrimitiveChecker.IsPrimitive(typeof(byte?)).Should().BeTrue();
        }

        [Fact]
        public void TestULong()
        {
            PrimitiveChecker.IsPrimitive(typeof(ulong)).Should().BeTrue();
            PrimitiveChecker.IsPrimitive(typeof(ulong?)).Should().BeTrue();
        }

        [Fact]
        public void TestUInt()
        {
            PrimitiveChecker.IsPrimitive(typeof(uint)).Should().BeTrue();
            PrimitiveChecker.IsPrimitive(typeof(uint?)).Should().BeTrue();
        }

        [Fact]
        public void TestUShort()
        {
            PrimitiveChecker.IsPrimitive(typeof(ushort)).Should().BeTrue();
            PrimitiveChecker.IsPrimitive(typeof(ushort?)).Should().BeTrue();
        }

        [Fact]
        public void TestDecimal()
        {
            PrimitiveChecker.IsPrimitive(typeof(decimal)).Should().BeTrue();
            PrimitiveChecker.IsPrimitive(typeof(decimal?)).Should().BeTrue();
        }

        [Fact]
        public void TestDouble()
        {
            PrimitiveChecker.IsPrimitive(typeof(double)).Should().BeTrue();
            PrimitiveChecker.IsPrimitive(typeof(double?)).Should().BeTrue();
        }

        [Fact]
        public void TestFloat()
        {
            PrimitiveChecker.IsPrimitive(typeof(float)).Should().BeTrue();
            PrimitiveChecker.IsPrimitive(typeof(float?)).Should().BeTrue();
        }

        [Fact]
        public void TestChar()
        {
            PrimitiveChecker.IsPrimitive(typeof(char)).Should().BeTrue();
            PrimitiveChecker.IsPrimitive(typeof(char?)).Should().BeTrue();
        }

        [Fact]
        public void TestGuid()
        {
            PrimitiveChecker.IsPrimitive(typeof(Guid)).Should().BeFalse();
            PrimitiveChecker.IsPrimitive(typeof(Guid?)).Should().BeFalse();
        }

        [Fact]
        public void TestDateTime()
        {
            PrimitiveChecker.IsPrimitive(typeof(DateTime)).Should().BeFalse();
            PrimitiveChecker.IsPrimitive(typeof(DateTime?)).Should().BeFalse();
        }

        [Fact]
        public void TestDateTimeOffset()
        {
            PrimitiveChecker.IsPrimitive(typeof(DateTimeOffset)).Should().BeFalse();
            PrimitiveChecker.IsPrimitive(typeof(DateTimeOffset?)).Should().BeFalse();
        }

        [Fact]
        public void TestTimeSpan()
        {
            PrimitiveChecker.IsPrimitive(typeof(TimeSpan)).Should().BeFalse();
            PrimitiveChecker.IsPrimitive(typeof(TimeSpan?)).Should().BeFalse();
        }

        [Fact]
        public void TestString()
        {
            PrimitiveChecker.IsPrimitive(typeof(string)).Should().BeTrue();
            PrimitiveChecker.IsPrimitive(typeof(String)).Should().BeTrue();
        }
    }
}
