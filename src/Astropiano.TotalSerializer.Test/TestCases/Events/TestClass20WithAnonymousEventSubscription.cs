using System.ComponentModel;

namespace Astropiano.TotalSerializer.Test.TestCases.Events
{
    public class TestClass20WithAnonymousEventSubscription
    {
        public bool ReceivedEvent;
        public int Count;

        public TestClass16PropertyChangedSender TreeReference; //just used for test
        
        public void SubScribeTo(TestClass16PropertyChangedSender sender)
        {
            sender.PropertyChanged += (o, args) =>
            {
                ReceivedEvent = true;
                Count++;
            };
        }
    }
}