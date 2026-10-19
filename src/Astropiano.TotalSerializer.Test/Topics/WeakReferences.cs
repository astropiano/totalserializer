using System;
using FluentAssertions;
using Xunit;
using Astropiano.TotalSerializer.Test.TestCases;
using Astropiano.TotalSerializer.Test.TestCases.WeakReferences;

namespace Astropiano.TotalSerializer.Test.Topics
{
    public class WeakReferences
    {
        [Fact(Skip = "We do not yet support this")]
        public void TestWeakReferenceAsRoot()
        {
            TestClass1 obj = new TestClass1();
            obj.IntegerField = 1;
            
            WeakReference weakReference = new WeakReference(obj);
            TotalSerializer serializer = new TotalSerializer();

            string json = serializer.Serialize(weakReference);
            WeakReference weakReferenceD = serializer.Deserialize<WeakReference>(json);
            TestClass1 objD = weakReferenceD.Target as TestClass1;
            
            objD.IntegerField.Should().Be(1);
        }
  
        [Fact]
        public void TestWeakReferenceFields()
        {
            TestClass22WithWeakReference obj = new TestClass22WithWeakReference();
            TestClass1 test1 = new TestClass1();
            test1.IntegerField = 1;
            TestClass1 test2 = new TestClass1();
            test2.IntegerField = 2;
            obj.UntypedWeakReference = new WeakReference(test1);
            obj.TypedWeakReference = new WeakReference<TestClass1>(test2);
            
            TotalSerializer serializer = new TotalSerializer();

            string json = serializer.Serialize(obj);

            TestClass22WithWeakReference objD = serializer.Deserialize<TestClass22WithWeakReference>(json);

            TestClass1 test1D = objD.UntypedWeakReference.Target as TestClass1;
            test1D.Should().NotBeNull();
            test1D.IntegerField.Should().Be(1);
            TestClass1 test2D;
            objD.TypedWeakReference.TryGetTarget(out test2D).Should().BeTrue();
            test2D.Should().NotBeNull();
            test2D.IntegerField.Should().Be(2);
        }

        [Fact]
        public void TestWeakReferenceNotAlive()
        {
            TestClass22WithWeakReference obj = CreateWeakReferencesTestCase();
            
            GC.Collect();
            GC.WaitForPendingFinalizers();
            
            obj.UntypedWeakReference.IsAlive.Should().BeFalse();
            obj.TypedWeakReference.TryGetTarget(out _).Should().BeFalse();
            
            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(obj);

            TestClass22WithWeakReference objD = serializer.Deserialize<TestClass22WithWeakReference>(json);

            objD.UntypedWeakReference.IsAlive.Should().BeFalse();
            objD.TypedWeakReference.TryGetTarget(out _).Should().BeFalse();
        }

        private TestClass22WithWeakReference CreateWeakReferencesTestCase()
        {
            TestClass22WithWeakReference obj = new TestClass22WithWeakReference();
            TestClass1 test1 = new TestClass1();
            test1.IntegerField = 1;
            TestClass1 test2 = new TestClass1();
            test2.IntegerField = 2;
            obj.UntypedWeakReference = new WeakReference(test1);
            obj.TypedWeakReference = new WeakReference<TestClass1>(test2);
            return obj;
        }
    }
}