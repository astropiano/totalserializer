namespace Astropiano.TotalSerializer.Test.TestCases.Methods
{
    public class TestClass15WithMethodTarget
    {
        public bool TestVariable;
        
        public void SetTestVariableTrue()
        {
            TestVariable = true;
        }

        public int CalculateSum(int a, int b)
        {
            return a + b;
        }
        
        public static int CalculateSumStatic(int a, int b)
        {
            return a + b;
        }
        
        public int CalculateSum(int a, int b, int c)
        {
            return a + b + c;
        }

        
    }
}