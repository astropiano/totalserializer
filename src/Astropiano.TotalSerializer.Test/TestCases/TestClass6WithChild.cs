namespace Astropiano.TotalSerializer.Test.TestCases
{
    public class TestClass6WithChild
    {
        private int integer;

        public int Integer
        {
            get { return integer; }
            set { integer = value; }
        }
        
        private TestClass6WithChild child;

        public TestClass6WithChild Child
        {
            get { return child; }
            set { child = value; }
        }
    }
}