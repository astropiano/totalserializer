using System;
using System.Linq;
using FluentAssertions;
using Xunit;
using Astropiano.ReflectionTools;
using Astropiano.TotalSerializer.Test.TestCases;

namespace Astropiano.TotalSerializer.Test.Topics
{
    public class Primitives
    {
        private FieldsReadWriteCache _fieldsReadWriteCache = new FieldsReadWriteCache();
        
        [Fact(Skip = "WIP - due to changes in TypeNameWriter")]
        public void SerializePrimitiveTest()
        {
            TotalSerializer serializer = new TotalSerializer();
            TestClass1 obj = new TestClass1();
            obj.IntegerField = 121;
            obj.IntegerFieldWithLogic = 34;
            obj.IntegerProperty = 89;
            string json = serializer.Serialize(obj);
            json.Should().Contain("\"$type\":\"Astropiano.TotalSerializer.Test.TestCases.TestClass1, " +
                                      "Astropiano.TotalSerializer.Test\"", "Json: " + json);
            json.Should().Contain($"\"{nameof(TestClass1.IntegerField)}\":121", "Json: " + json);
            json.Should().Contain("\"integerFieldWithLogic\":34", "Json: " + json);
            json.Should().NotContain("\"IntegerProperty\"", "Json: " + json);
            json.Should().Contain("\"<IntegerProperty>k__BackingField\":89", "Json: " + json);
            json.Should().Contain("\"$id\":1", "Json: " + json);
            //check number of commas. Num properties + 2 ($id/$ref + $type)
            json.Count(x => x == ',').Should().Be(5, "Json: " + json);
            //Also make sure the getter logic did not execute
            object objC = obj;
            int backingFieldValue = (int)_fieldsReadWriteCache.Get(ref objC, "<IntegerProperty>k__BackingField");
            backingFieldValue.Should().Be(89);
        }
       

        [Fact]
        public void PrimitiveIntegerTest()
        {
            TotalSerializer serializer = new TotalSerializer();
            TestClass1 obj = new TestClass1();

            obj.IntegerField = 42;
            obj.IntegerProperty = int.MinValue;
            obj.IntegerFieldWithLogic = -1000;

            string json = serializer.Serialize(obj);

            Func<TestClass1, TestClass1, bool> check = (obj1, obj2) =>
            {
                obj1.Should().NotBeNull();
                obj2.Should().NotBeNull();
                obj1.IntegerField.Should().Be(obj2.IntegerField);
                obj1.IntegerProperty.Should().Be(obj2.IntegerProperty);
                obj1.GetIntegerFieldWithLogicFor().Should().Be(obj2.GetIntegerFieldWithLogicFor());
                return true;
            };

            {
                TestClass1 obj2 = serializer.Deserialize<TestClass1>(json);
                check(obj, obj2).Should().BeTrue();
            }

            {
                object o = serializer.Deserialize(json);
                TestClass1 obj2 = o as TestClass1;
                check(obj, obj2).Should().BeTrue();
            }

            {
                object o = serializer.Deserialize(json);
                TestClass1 obj2 = o as TestClass1;
                check(obj, obj2).Should().BeTrue();
            }
        }


        [Fact]
        public void PrimitiveDoubleTest()
        {
            TotalSerializer serializer = new TotalSerializer();
            TestClass2WithDoubles obj = new TestClass2WithDoubles();

            obj.DoubleField = Math.PI;
            obj.DoubleProperty = Math.E;
            obj.DoubleFieldWithLogic = 1.618033988749894848204586834365638117720309179805762862135;

            string json = serializer.Serialize(obj);

            Func<TestClass2WithDoubles, TestClass2WithDoubles, bool> check = (obj1, obj2) =>
            {
                obj1.Should().NotBeNull();
                obj2.Should().NotBeNull();
                obj1.DoubleField.Should().Be(obj2.DoubleField);
                obj1.DoubleProperty.Should().Be(obj2.DoubleProperty);
                obj1.DoubleFieldWithLogic.Should().Be(obj2.DoubleFieldWithLogic);
                return true;
            };

            {
                TestClass2WithDoubles obj2 = serializer.Deserialize<TestClass2WithDoubles>(json);
                check(obj, obj2).Should().BeTrue();
            }

            {
                object o = serializer.Deserialize(json);
                TestClass2WithDoubles obj2 = o as TestClass2WithDoubles;
                check(obj, obj2).Should().BeTrue();
            }
        }

        [Fact]
        public void PrimitiveDecimalTest()
        {
            TotalSerializer serializer = new TotalSerializer();
            TestClass3WithDecimals obj = new TestClass3WithDecimals();

            obj.DecimalField = 1.23456789m;
            obj.DecimalProperty = decimal.MinValue;
            obj.DecimalFieldWithLogic = decimal.MinusOne;

            string json = serializer.Serialize(obj);

            Func<TestClass3WithDecimals, TestClass3WithDecimals, bool> check = (obj1, obj2) =>
            {
                obj1.Should().NotBeNull();
                obj2.Should().NotBeNull();
                obj1.DecimalField.Should().Be(obj2.DecimalField);
                obj1.DecimalProperty.Should().Be(obj2.DecimalProperty);
                obj1.DecimalFieldWithLogic.Should().Be(obj2.DecimalFieldWithLogic);
                return true;
            };

            {
                TestClass3WithDecimals obj2 = serializer.Deserialize<TestClass3WithDecimals>(json);
                check(obj, obj2).Should().BeTrue();
            }

            {
                object o = serializer.Deserialize(json);
                TestClass3WithDecimals obj2 = o as TestClass3WithDecimals;
                check(obj, obj2).Should().BeTrue();
            }

            {
                object o = serializer.Deserialize(json);
                TestClass3WithDecimals obj2 = o as TestClass3WithDecimals;
                check(obj, obj2).Should().BeTrue();
            }
        }

        [Fact]
        public void PrimitiveStringTest()
        {
            TotalSerializer serializer = new TotalSerializer();
            TestClass4WithStrings obj = new TestClass4WithStrings();
            obj.StringField = "A field";
            obj.StringProperty = "A property.";
            obj.StringFieldWithLogic = "A property with logic.";

            string json = serializer.Serialize(obj);

            Func<TestClass4WithStrings, TestClass4WithStrings, bool> check = (obj1, obj2) =>
            {
                obj1.Should().NotBeNull();
                obj2.Should().NotBeNull();
                obj1.StringField.Should().Be(obj2.StringField);
                obj1.StringProperty.Should().Be(obj2.StringProperty);
                obj1.StringFieldWithLogic.Should().Be(obj2.StringFieldWithLogic);
                return true;
            };

            {
                TestClass4WithStrings obj2 = serializer.Deserialize<TestClass4WithStrings>(json);
                check(obj, obj2).Should().BeTrue();
            }

            {
                object o = serializer.Deserialize(json);
                TestClass4WithStrings obj2 = o as TestClass4WithStrings;
                check(obj, obj2).Should().BeTrue();
            }

            {
                object o = serializer.Deserialize(json);
                TestClass4WithStrings obj2 = o as TestClass4WithStrings;
                check(obj, obj2).Should().BeTrue();
            }
        }

        [Fact]
        public void StringEscapingTest()
        {
            TotalSerializer serializer = new TotalSerializer();
            TestClass4WithStrings obj = new TestClass4WithStrings();
            
            obj.StringField = "\\";

            string json = serializer.Serialize(obj);

            json.Should().Contain("\\\\");
            
            TestClass4WithStrings objD = serializer.Deserialize<TestClass4WithStrings>(json);
            
            obj.StringField.Should().Be(objD.StringField);
        }
    }
}