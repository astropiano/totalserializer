using System;

namespace Astropiano.TotalSerializer.Test.TestCases
{
    internal class TestClass7WithCyclicReferences
    {
        public ClassA A { get; set; }
        public ClassB B { get; set; }

        public class ClassA
        {
            public Guid Oid { get; set; }
            public ClassB InnerB { get; set; }
        }

        public class ClassB
        {
            public DateTime ModificationDate { get; set; }
            public ClassA InnerA { get; set; }
        }
    }
}
