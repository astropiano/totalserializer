namespace TotalSerializer.ReflectionTools.Test.TestCases
{
    public class ConstructorExample1
    {
        public ConstructorExample1() : this("", 0)
        {

        }

        public ConstructorExample1(string s) : this(s, 0)
        {

        }
        public ConstructorExample1(int i) : this("", i)
        {

        }

        public ConstructorExample1(string s, int i)
        {
            S = s;
            I = i;
        }


        public string S { get; set; }
        public int I { get; set; }
    }
}
