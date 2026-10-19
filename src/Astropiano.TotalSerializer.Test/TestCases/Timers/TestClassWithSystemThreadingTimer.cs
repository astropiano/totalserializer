using System.Threading;

namespace Astropiano.TotalSerializer.Test.TestCases.Timers
{
    public class TestClassWithSystemThreadingTimer
    {
        public Timer Timer { get; set; }

        public int TimerUpdateCounter { get; set; }

        private void Callback(object o)
        {
            // By updating this field we compare an earlier value
            // with a newer value and then we know the timer is still alive
            // on the deserialized object.
            TimerUpdateCounter++;
        }

        public static TestClassWithSystemThreadingTimer Create()
        {
            TestClassWithSystemThreadingTimer t = new TestClassWithSystemThreadingTimer();

            Timer timer = new Timer(t.Callback, t, 100, 100);

            t.Timer = timer;

            return t;
        }
    }
}