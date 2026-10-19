using System.Collections.ObjectModel;
using FluentAssertions;
using Xunit;
using Astropiano.TotalSerializer.Test.TestCases;
using Astropiano.TotalSerializer.Test.TestCases.ObservableCollection;

namespace Astropiano.TotalSerializer.Test.Topics
{
    public class ObservableCollection
    {
        [Fact]
        public void SimpleObservableCollectionTest()
        {
            TestClass24WithObservableCollection obj = new TestClass24WithObservableCollection();
            obj.Collection = new ObservableCollection<TestClass0>();
            obj.Collection.Add(new TestClass0());
            
            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(obj);

            TestClass24WithObservableCollection
                objD = serializer.Deserialize<TestClass24WithObservableCollection>(json);
        }
        
        [Fact]
        public void AttachedObservableCollectionTest()
        {
            TestClass24WithObservableCollection obj = new TestClass24WithObservableCollection();
            obj.Collection = new ObservableCollection<TestClass0>();
            obj.Collection.Add(new TestClass0());
            obj.ListenToCollection(obj.Collection);
            
            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(obj);

            TestClass24WithObservableCollection
                objD = serializer.Deserialize<TestClass24WithObservableCollection>(json);
            
            objD.ReceivedAnEvent.Should().BeFalse();
            objD.Collection.Add(new TestClass0());
            objD.ReceivedAnEvent.Should().BeTrue();
        }
    }
}