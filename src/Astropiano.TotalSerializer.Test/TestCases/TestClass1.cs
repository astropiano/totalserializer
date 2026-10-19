namespace Astropiano.TotalSerializer.Test.TestCases
{
    public class TestClass1
    {
        public int IntegerField;

        public int IntegerProperty { get; set; }

        private int integerFieldWithLogic;

        public int IntegerFieldWithLogic
        {
            set
            {
                integerFieldWithLogic = value;
            }
            get
            {
                return integerFieldWithLogic++;
            }
        }

        public int GetIntegerFieldWithLogicFor()
        {
            return integerFieldWithLogic;
        }
    }
}
