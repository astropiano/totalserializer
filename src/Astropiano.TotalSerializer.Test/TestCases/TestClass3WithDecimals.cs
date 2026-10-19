namespace Astropiano.TotalSerializer.Test.TestCases
{
    public class TestClass3WithDecimals
    {
        public decimal DecimalField;

        public decimal DecimalProperty { get; set; }

        private decimal decimalFieldWithLogic;

        public decimal DecimalFieldWithLogic
        {
            set
            {
                decimalFieldWithLogic = value;
            }
            get
            {
                return decimalFieldWithLogic + 1.0m;
            }
        }
    }
}
