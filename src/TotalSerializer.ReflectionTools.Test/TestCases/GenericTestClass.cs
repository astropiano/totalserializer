namespace TotalSerializer.ReflectionTools.Test.TestCases
{
    public class GenericTestClass<T>
    {
        public T Data;
        
        private T _data;

        public T PublicData
        {
            get { return _data; }
            set { _data = value; }
        }
    }
}
