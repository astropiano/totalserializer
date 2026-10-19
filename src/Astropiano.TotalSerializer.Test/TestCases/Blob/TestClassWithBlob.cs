namespace Astropiano.TotalSerializer.Test.TestCases.Blob
{
    public class TestClassWithBlob
    {
        public string BlobField;

        public TestClassWithBlob()
        {
            BlobField = "{\"$id\":\"1\",\"$type\":\"MyAssembly.MyNameSpace.MyType, MyAssembly\",\"myValue\":2}";
        }
    }
}