using FluentAssertions;
using Xunit;
using Astropiano.TotalSerializer.Test.TestCases.Methods;

namespace Astropiano.TotalSerializer.Test.Topics
{
    public class Methods
    {
        [Fact]
        public void SimpleMethod()
        {
            TestClass15WithMethodTarget target = new TestClass15WithMethodTarget();
            
            TestClass14WithMethod obj = new TestClass14WithMethod();
            obj.Method = target.SetTestVariableTrue;
            obj.TreeReference = target; //the target must be somewhere in the JSON tree.
            
            // make sure what we want to test works
            obj.Method.Target.Should().Be(target);
            target.TestVariable.Should().BeFalse();
            obj.Method();
            target.TestVariable.Should().BeTrue();
            
            // actual test
            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(obj);
            TestClass14WithMethod deserialized = serializer.Deserialize<TestClass14WithMethod>(json);
            target.TestVariable = false;
            target.TestVariable.Should().BeFalse();
            deserialized.Method.Target.Should().Be(deserialized.TreeReference);
            deserialized.Method();
            TestClass15WithMethodTarget deserializedTarget = deserialized.TreeReference as TestClass15WithMethodTarget;
            deserializedTarget.Should().NotBeNull();
            deserializedTarget.TestVariable.Should().BeTrue();
        }
        
        [Fact]
        public void MultiSignatureTest()
        {
            TestClass15WithMethodTarget target = new TestClass15WithMethodTarget();
            
            TestClass14WithMethod obj = new TestClass14WithMethod();
            obj.Method2 = target.CalculateSum;
            
            // actual test
            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(obj);
            TestClass14WithMethod deserialized = serializer.Deserialize<TestClass14WithMethod>(json);
            int result = deserialized.Method2(4,4);
            result.Should().Be(8);
        }
        
        [Fact]
        public void StaticMethodTest()
        {
            TestClass15WithMethodTarget target = new TestClass15WithMethodTarget();
            
            TestClass14WithMethod obj = new TestClass14WithMethod();
            obj.Method2 = TestClass15WithMethodTarget.CalculateSumStatic;
            
            // actual test
            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(obj);
            TestClass14WithMethod deserialized = serializer.Deserialize<TestClass14WithMethod>(json);
            int result = deserialized.Method2(4,4);
            result.Should().Be(8);
        }
    }
}