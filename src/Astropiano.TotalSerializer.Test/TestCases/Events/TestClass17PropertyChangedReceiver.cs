using System.ComponentModel;

namespace Astropiano.TotalSerializer.Test.TestCases.Events
{
    public class TestClass17PropertyChangedReceiver
    {
        public bool ChangedEvendHasFired;
        public int Count;

        public TestClass16PropertyChangedSender SenderInstance; //just so we can access it in the test
        public TestClass17PropertyChangedReceiver AnotherReceiver; //just so we can access it in the test
        
        public void ListenToChanges(INotifyPropertyChanged notifyPropertyChanged)
        {
            notifyPropertyChanged.PropertyChanged += UpdateLocaleField;
        }

        private void UpdateLocaleField(object sender, PropertyChangedEventArgs e)
        {
            ChangedEvendHasFired = true;
            Count++;
        }
    }
}