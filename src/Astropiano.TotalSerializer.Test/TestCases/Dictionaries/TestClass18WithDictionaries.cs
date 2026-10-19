using System.Collections.Generic;

namespace Astropiano.TotalSerializer.Test.TestCases.Dictionaries
{
    public class TestClass18WithDictionaries
    {
        private Dictionary<int, int> _primitiveIntDictionary;

        public Dictionary<int, int> PrimitiveIntDictionary
        {
            get { return _primitiveIntDictionary; }
            set { _primitiveIntDictionary = value; }
        }
        
        
        private Dictionary<object, int> _objectDictionary;

        public Dictionary<object, int> ObjectDictionary
        {
            get { return _objectDictionary; }
            set { _objectDictionary = value; }
        }
    }
}