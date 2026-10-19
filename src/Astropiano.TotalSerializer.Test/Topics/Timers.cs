using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Astropiano.TotalSerializer.Test.TestCases.Timers;
#if (NETFRAMEWORK)
using System.Windows.Forms;
#endif

namespace Astropiano.TotalSerializer.Test.Topics
{
    public class Timers
    {
        [Fact]
        public async Task SystemTimersTimer()
        {
            TestClassWithSystemTimersTimer t = TestClassWithSystemTimersTimer.Create();

            t.TimerUpdateCounter = 0;

            TimeSpan waitTime = TimeSpan.FromMilliseconds(20);
            TimeSpan timeOut = TimeSpan.FromSeconds(20);
            CancellationToken ct = new CancellationTokenSource(timeOut).Token;

            while (!ct.IsCancellationRequested)
            {
                if (t.TimerUpdateCounter > 0)
                {
                    break;
                }

                await Task.Delay(waitTime, ct).ConfigureAwait(false);
            }

            t.TimerUpdateCounter.Should().BeGreaterThan(0, "Timer not active on initial object - error in test!");

            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(t);
            TestClassWithSystemTimersTimer d = serializer.Deserialize<TestClassWithSystemTimersTimer>(json);
            d.Timer.Should().NotBeNull("Missing timer after deserialization!");
            d.TimerUpdateCounter = 0;
            while (!ct.IsCancellationRequested)
            {
                if (d.TimerUpdateCounter > 0)
                {
                    break;
                }

                await Task.Delay(waitTime, ct).ConfigureAwait(false);
            }

            d.TimerUpdateCounter.Should().BeGreaterThan(0, "Timer is not active on deserialized object!");
        }

        [Fact(Skip = "Not implemented yet.")]
        public async Task SystemThreadingTimer()
        {
            TestClassWithSystemThreadingTimer t = TestClassWithSystemThreadingTimer.Create();

            t.Timer.Should().NotBeNull("No timer defined!");
            t.TimerUpdateCounter = 0;

            TimeSpan waitTime = TimeSpan.FromMilliseconds(20);
            TimeSpan timeOut = TimeSpan.FromSeconds(20);
            CancellationToken ct = new CancellationTokenSource(timeOut).Token;
            while (!ct.IsCancellationRequested)
            {
                if (t.TimerUpdateCounter > 0)
                {
                    break;
                }

                await Task.Delay(waitTime, ct).ConfigureAwait(false);
            }

            t.TimerUpdateCounter.Should().BeGreaterThan(0, "Timer not active on initial object - error in test!");

            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(t);
            TestClassWithSystemThreadingTimer d = serializer.Deserialize<TestClassWithSystemThreadingTimer>(json);
            d.Timer.Should().NotBeNull("Missing timer after deserialization!");
            d.TimerUpdateCounter = 0;
            while (!ct.IsCancellationRequested)
            {
                if (d.TimerUpdateCounter > 0)
                {
                    break;
                }

                await Task.Delay(waitTime, ct).ConfigureAwait(false);
            }

            d.TimerUpdateCounter.Should().BeGreaterThan(0, "Timer is not active on deserialized object!");
        }


#if (NETFRAMEWORK)
        [Fact(Skip = "Not implemented yet.")]
        public async Task SystemWindowsFormTimer()
        {
            TestClassWithSystemWindowsFormTimer t = new TestClassWithSystemWindowsFormTimer();
            
            t.Timer.Should().NotBeNull("No timer defined!");
            t.TimerUpdateCounter = 0;

            TimeSpan waitTime = TimeSpan.FromMilliseconds(20);
            TimeSpan timeOut = TimeSpan.FromSeconds(20);
            CancellationToken ct = new CancellationTokenSource(timeOut).Token;
            while (!ct.IsCancellationRequested)
            {
                if (t.TimerUpdateCounter > 0)
                {
                    break;
                }

                await Task.Delay(waitTime, ct).ConfigureAwait(false);
            }

            t.TimerUpdateCounter.Should().BeGreaterThan(0, "Timer not active on initial object - error in test!");

            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(t);
            TestClassWithSystemWindowsFormTimer d = serializer.Deserialize<TestClassWithSystemWindowsFormTimer>(json);
            d.Timer.Should().NotBeNull("Missing timer after deserialization!");
            d.TimerUpdateCounter = 0;
            while (!ct.IsCancellationRequested)
            {
                if (d.TimerUpdateCounter > 0)
                {
                    break;
                }

                await Task.Delay(waitTime, ct).ConfigureAwait(false);
            }

            d.TimerUpdateCounter.Should().BeGreaterThan(0, "Timer is not active on deserialized object!");
            d.Timer.Should().NotBeNull("Timer on deserialized object is missing!");
            d.Timer.Interval.Should().Be(t.Timer.Interval, "Wrong interval!");
        }
#endif
    }
}