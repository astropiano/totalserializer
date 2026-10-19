using System;

namespace Astropiano.TotalSerializer.Test.TestCases.Enumerations
{
    public class TestClass23WithFlagEnum
    {
        [Flags]
        public enum FlagEnum : ulong
        {
            A = 2,
            B = 4
        }

        public FlagEnum EnumValue;
    }
}