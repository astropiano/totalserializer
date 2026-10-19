namespace Astropiano.TotalSerializer.Test.TestCases
{
    public class TestClass4WithStrings
    {
        public string StringField;

        public string StringProperty { get; set; }

        private string stringFieldWithLogic;

        public string StringFieldWithLogic
        {
            set
            {
                stringFieldWithLogic = value + "_SET_";
            }
            get
            {
                return stringFieldWithLogic + "_GET_";
            }
        }
    }
}
