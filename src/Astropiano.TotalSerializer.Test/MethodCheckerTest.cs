using FluentAssertions;
using Xunit;

namespace Astropiano.TotalSerializer.Test
{
    public class MethodCheckerTest
    {
        [Fact]
        public void IsDelegateTest()
        {
            {
                bool b = MethodChecker.IsDelegate(typeof(MyClass.TestDelegate1));
                b.Should().BeTrue("TestDelegate1 should be a delegate.");
            }
            {
                bool b = MethodChecker.IsDelegate(typeof(MyClass));
                b.Should().BeFalse("MyClass should not be a delegate.");
            }
        }

        [Fact]
        public void IsMulticastDelegateTest()
        {
            MyClass o = new MyClass();
            MyClass.TestDelegate2 d1 = o.TestMethodA;
            MyClass.TestDelegate2 d2 = o.TestMethodB;
            MyClass.TestDelegate2 d3 = d1 + d2;
            MyClass.TestDelegate2 d4 = d3 - d1;

            {
                bool b = MethodChecker.IsMulticastDelegate(d1.GetType());
                b.Should().BeTrue("d1 should be a multicast delegate.");
            }

            {
                bool b = MethodChecker.IsMulticastDelegate(d2.GetType());
                b.Should().BeTrue("d2 should be a multicast delegate.");
            }

            {
                bool b = MethodChecker.IsMulticastDelegate(d3.GetType());
                b.Should().BeTrue("d3 should be a multicast delegate.");
            }

            {
                bool b = MethodChecker.IsMulticastDelegate(d4.GetType());
                b.Should().BeTrue("d4 should be a multicast delegate.");
            }

            /*
             // It seems that all delegates are multi-cast-delegates...
            {
                bool b = MethodChecker.IsMulticastDelegate(typeof(MyClass.TestDelegate1));
                Assert.That(b, Is.True, "MyClass.TestDelegate1 should be a multicast delegate.");
            }
            */

            {
                bool b = MethodChecker.IsMulticastDelegate(typeof(MyClass));
                b.Should().BeFalse("MyClass.TestDelegate1 should be a multicast delegate.");
            }
        }


        private class MyClass
        {
            public delegate string TestDelegate1(int i, double d);

            public delegate string TestDelegate2(int i);

            public string TestMethodA(int i)
            {
                return "_" + i;
            }

            public string TestMethodB(int i)
            {
                return "%" + i;
            }
        }
    }
}
