using System;
using System.Diagnostics;

namespace Astropiano.TotalSerializer.Test.TestCases.PseudoPrimitives
{
    internal class TestClassWithOneType
    {
        public Type MyType { get; set; }

        public static TestClassWithOneType Create()
        {
            return new TestClassWithOneType
            {
                MyType = typeof(Stopwatch)
            };
        }
    }
}
