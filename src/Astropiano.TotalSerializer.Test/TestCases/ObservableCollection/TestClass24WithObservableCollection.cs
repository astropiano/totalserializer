using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Astropiano.TotalSerializer.Test.TestCases.ObservableCollection
{
    public class TestClass24WithObservableCollection
    {
        public ObservableCollection<TestClass0> Collection;

        public bool ReceivedAnEvent;

        public void ListenToCollection(INotifyCollectionChanged notifyCollectionChanged)
        {
            notifyCollectionChanged.CollectionChanged += (s, a) => { ReceivedAnEvent = true; };
        }
    }
}