namespace Astropiano.TotalSerializer.Test.TestCases.Array
{
    public class TestClass11WithObjectArray
    {
        private object _objectArray;

        public void SetObjectArray(object input)
        {
            _objectArray = input;
        }
        
        public object GetObjectArray()
        {
            return _objectArray;
        }
    }
}