using System.ComponentModel;
using System.Runtime.CompilerServices;
using Astropiano.TotalSerializer.Test.Annotations;

namespace Astropiano.TotalSerializer.Test.TestCases.Events
{
    public class TestClass16PropertyChangedSender : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void TriggerChangedEvent(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
    }
}