using System.Collections.Generic;

namespace TotalSerializer.ReflectionTools.Test.TestCases
{
    class WithStaticField
    {
        static WithStaticField()
        {
            Names = new List<string>();
            Name = "Test";
        }

        public static List<string> Names;
        public static string Name;
    }
}
