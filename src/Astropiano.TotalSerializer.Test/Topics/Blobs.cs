using FluentAssertions;
using Xunit;
using Astropiano.TotalSerializer.Test.TestCases.Blob;

namespace Astropiano.TotalSerializer.Test.Topics
{
    public class Blobs
    {
        [Fact]
        public void BlobTest()
        {
            TestClassWithBlob obj = new TestClassWithBlob();
            
            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(obj);

            TestClassWithBlob objD = serializer.Deserialize<TestClassWithBlob>(json);
            
            objD.BlobField.Should().BeEquivalentTo(obj.BlobField);
        }
    }
}