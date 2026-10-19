using System;
using System.Collections.Concurrent;

namespace Astropiano.TotalSerializer.Test.TestCases.SystemType
{
    internal class TestClassWithType
    {
        public Type Type1 { get; set; }
        public Type[] TypeArray1 { get; set; }

        public static TestClassWithType Create()
        {
            Type type1 = (new UsedForType("test", new int[] { -1, -2 }, new double[][] { new double[] { 3.14159 }, new double[] { 1.6181 } }, new bool[2, 2] { { true, false }, { false, true } })).GetType();
            Type type2 = typeof(string[]);
            Type type3 = typeof(ConcurrentQueue<int>);

            return new TestClassWithType
            {
                //Type1 = new UsedForType("test", new int[] { -1,-2}, new double[][] { new double[] { 3.14159 }, new double[] { 1.6181 } }, new bool[2,2] { { true, false}, { false, true} })
                Type1 = type1,
                TypeArray1 = new Type[] { type2, type3, type2, type3 }
            };
        }

        internal class UsedForType : Tuple<string, int[], double[][], bool[,]>
        {
            public UsedForType(string item1, int[] item2, double[][] item3, bool[,] item4) : base(item1, item2, item3, item4)
            {
            }
        }
    }
}
