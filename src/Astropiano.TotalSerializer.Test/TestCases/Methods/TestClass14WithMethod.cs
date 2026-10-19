using System;

namespace Astropiano.TotalSerializer.Test.TestCases.Methods
{
    public class TestClass14WithMethod
    {
        private Action method;

        public Action Method
        {
            get { return method;}
            set { method = value; }
        }
        
        private Func<int, int, int> method2;

        public Func<int, int, int> Method2
        {
            get { return method2;}
            set { method2 = value; }
        }
        
        public object TreeReference;
        
    }
}