using System.Diagnostics;
using System.Reflection;

namespace Astropiano.TotalSerializer.Test.TestCases.SystemType
{
    internal class TestClassWithPropertyInfo
    {
        public PropertyInfo MyPropertyInfo { get; set; }
        public PropertyInfo[] MyPropertyInfoArray { get; set; }

        public static TestClassWithPropertyInfo Create()
        {
            PropertyInfo[] infos = new PropertyInfo[3];
            for (int i = 0; i < infos.Length; i++)
            {
                infos[i] = typeof(Stopwatch).GetProperty(nameof(Stopwatch.ElapsedTicks));
            }
            
            return new TestClassWithPropertyInfo
            {
                MyPropertyInfo = typeof(Stopwatch).GetProperty(nameof(Stopwatch.Elapsed)),
                MyPropertyInfoArray = infos
            };
        }
    }
}