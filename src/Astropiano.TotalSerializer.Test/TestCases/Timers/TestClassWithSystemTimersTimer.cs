using System.Timers;

namespace Astropiano.TotalSerializer.Test.TestCases.Timers
{
    public class TestClassWithSystemTimersTimer
    {
        public Timer Timer { get; set; }

        public int TimerUpdateCounter { get; set; }

        public static TestClassWithSystemTimersTimer Create()
        {
            var t = new TestClassWithSystemTimersTimer();
            
            Timer timer = new Timer(100);
            timer.Elapsed += (sender, args) =>
            {
                t.TimerUpdateCounter++;
            }; 
            timer.AutoReset = true;
            timer.Enabled = true;

          

            t.Timer = timer;

            return t;
        }
    }
}