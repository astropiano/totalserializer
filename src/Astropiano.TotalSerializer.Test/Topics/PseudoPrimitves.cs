using System;
using System.Globalization;
using FluentAssertions;
using Xunit;
using Astropiano.TotalSerializer.Test.TestCases;
using Astropiano.TotalSerializer.Test.TestCases.PseudoPrimitives;

namespace Astropiano.TotalSerializer.Test.Topics
{
    public class PseudoPrimitves
    {
        [Fact]
        public void DateTimeTest()
        {
            CultureInfo myCultureInfo = new CultureInfo("de-CH");
            string myString = "12 Juni 2008";
            DateTime myDatetime = DateTime.Parse(myString, myCultureInfo);
            TestClass12WithDateTime obj = new TestClass12WithDateTime() {DateTime = myDatetime};
            
            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(obj);

            TestClass12WithDateTime deserialized = serializer.Deserialize(json) as TestClass12WithDateTime;
            deserialized.Should().NotBeNull();
            deserialized.DateTime.Should().Be(obj.DateTime);
        }

        [Fact]
        public void DateTimeOnObjectTest()
        {
            CultureInfo myCultureInfo = new CultureInfo("de-CH");
            string myString = "12 Juni 2008";
            DateTime myDatetime = DateTime.Parse(myString, myCultureInfo);
            TestClass13WithObject obj = new TestClass13WithObject();
            obj.Obj = myDatetime;
            
            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(obj);
        }

        [Fact]
        public void TypeOnObject()
        {
            TestClassWithOneType testClassWithType = TestClassWithOneType.Create();

            Type t = testClassWithType.MyType.GetType();
            bool b = PseudoPrimitiveChecker.IsPseudoPrimitive(t);
            b.Should().BeTrue($"We should treat '{t.Name}' as pseudo-primitive");
        }
    }
}