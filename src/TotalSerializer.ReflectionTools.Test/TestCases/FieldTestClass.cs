namespace TotalSerializer.ReflectionTools.Test.TestCases
{
    public class FieldTestClass
    {
        public int PublicIntegerValue;
#pragma warning disable CS0649
        private int _privateIntegerValue;
#pragma warning restore CS0649

        public static int PublicStaticValue;

        public int PrivateIntegerValue
        {
            get { return _privateIntegerValue; }
        }
    }
}
