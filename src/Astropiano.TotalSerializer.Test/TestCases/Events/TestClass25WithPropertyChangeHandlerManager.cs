using Astropiano.TotalSerializer.Test.TestCases.Notifying;

namespace Astropiano.TotalSerializer.Test.TestCases.Events
{
    public class TestClass25WithPropertyChangeHandlerManager : NotifyingObject
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { SetValue(ref name, value); }
        }

        public PropertyChangedHandlerManager PropertyChangedHandlerManager = new PropertyChangedHandlerManager();

        public int ChangedCount;

        public TestClass25WithPropertyChangeHandlerManager other;
    }
}
