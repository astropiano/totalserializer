#if (NETFRAMEWORK)
using System;
using System.Windows.Forms;

namespace Astropiano.TotalSerializer.Test.TestCases.Timers
{
    internal class TestClassWithSystemWindowsFormTimer
    {
        public Timer Timer { get; set; }
        
        public int TimerUpdateCounter { get; set; }

        public static TestClassWithSystemWindowsFormTimer Create()
        {
            Timer timer = new Timer();
            
            timer.Interval = 100; // specify interval time as you want
            timer.Tick += new EventHandler((sender, args) => { ((TestClassWithSystemWindowsFormTimer) sender).TimerUpdateCounter++; });
            timer.Start();

            TestClassWithSystemWindowsFormTimer t = new TestClassWithSystemWindowsFormTimer();
            t.Timer = timer;
            return t;
        }
    }
}

#endif
