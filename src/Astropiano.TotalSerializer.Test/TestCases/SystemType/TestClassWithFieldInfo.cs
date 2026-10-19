using System.Diagnostics;
using System.Reflection;

namespace Astropiano.TotalSerializer.Test.TestCases.SystemType
{
    internal class TestClassWithFieldInfo
    {
        public FieldInfo MyFieldInfo { get; set; }
        public FieldInfo[] MyFieldInfoArray { get; set; }

        public static TestClassWithFieldInfo Create()
        {
            FieldInfo[] infos = new FieldInfo[3];
            for (int i = 0; i < infos.Length; i++)
            {
                infos[i] = typeof(Stopwatch).GetField(nameof(Stopwatch.Frequency));
            }
            
            return new TestClassWithFieldInfo
            {
                MyFieldInfo = typeof(Stopwatch).GetField(nameof(Stopwatch.Frequency)),
                MyFieldInfoArray = infos
            };
        }
    }
}