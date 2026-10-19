namespace Astropiano.TotalSerializer.Test.TestCases
{
    public class TestClass2WithDoubles
    {
        public double DoubleField;

        public double DoubleProperty { get; set; }

        private double doubleFieldWithLogic;

        public double DoubleFieldWithLogic
        {
            set
            {
                doubleFieldWithLogic = value;
            }
            get
            {
                return doubleFieldWithLogic + 1.0;
            }
        }
    }
}
