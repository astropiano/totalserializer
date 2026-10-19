using FluentAssertions;
using Xunit;
using Astropiano.TotalSerializer.Test.TestCases;
using Astropiano.TotalSerializer.Configuration;
using System;
using System.IO;
using Astropiano.TotalSerializer.Factories;

namespace Astropiano.TotalSerializer.Test
{
    public class TotalSerializerTest
    {
        [Fact(Skip = "WIP - due to changes in TypeNameWriter")]
        public void EmtpyClassTest()
        {
            TotalSerializer serializer = new TotalSerializer();
            TestClass0 emptyObj = new TestClass0();
            string json = serializer.Serialize(emptyObj);
            json.Should().Be("{\"$type\":\"Astropiano.TotalSerializer.Test.TestCases.TestClass0, Astropiano.TotalSerializer.Test\",\"$id\":1}");
            TestClass0 emptyObj2 = serializer.Deserialize<TestClass0>(json);
            emptyObj2.Should().NotBeNull();
            object emptyObj3 = serializer.Deserialize(json);
            emptyObj3.GetType().Should().Be(typeof(TestClass0));
        }
        
        [Fact]
        public void FormattedStringTest()
        {
            TotalSerializer serializer = new TotalSerializer();
            const string json = "\r\n\r\n\t{\"$type\":\"Astropiano.TotalSerializer.Test.TestCases.TestClass0,Astropiano.TotalSerializer.Test\",\"$id\":1}\t\r\r";
            TestClass0 emptyObj2 = serializer.Deserialize<TestClass0>(json);
            emptyObj2.Should().NotBeNull();
            object emptyObj3 = serializer.Deserialize(json);
            emptyObj3.GetType().Should().Be(typeof(TestClass0));
        }
        
        [Fact]
        public void FormattedStringWithWhiteSpaceAndNewLineTest()
        {
            TotalSerializer serializer = new TotalSerializer();
            const string json = "\r\n\r\n \t{\"$type\": \"Astropiano.TotalSerializer.Test.TestCases.TestClass0, Astropiano.TotalSerializer.Test\" ,\r\n\"$id\":1}\t\r\r";
            TestClass0 emptyObj2 = serializer.Deserialize<TestClass0>(json);
            emptyObj2.Should().NotBeNull();
            object emptyObj3 = serializer.Deserialize(json);
            emptyObj3.GetType().Should().Be(typeof(TestClass0));
        }

        [Fact]
        public void PrimitiveClassWithoutParameterlessConstructorTest()
        {
            Config config = new Config();
            config.Factory = new TestFactory5();
            TotalSerializer serializer = new TotalSerializer(config);

            Action act = () => { (new DefaultFactory()).Create<TestClass5WithoutParameterlessConstructor>(); };
            act.Should().NotThrow("The test is broken - this class should not have a parameterless constructor.");

            TestClass5WithoutParameterlessConstructor obj = TestClass5WithoutParameterlessConstructor.Create();
            obj.TestField = 42;
            string json = serializer.Serialize(obj);

            Func<TestClass5WithoutParameterlessConstructor, TestClass5WithoutParameterlessConstructor, bool> check = (obj1, obj2) =>
            {
                obj1.Should().NotBeNull();
                obj2.Should().NotBeNull();
                obj2.TestField.Should().Be(obj.TestField);
                return true;
            };

            {
                TestClass5WithoutParameterlessConstructor obj2 = serializer.Deserialize<TestClass5WithoutParameterlessConstructor>(json);
                check(obj, obj2).Should().BeTrue();
            }

            {
                object o = serializer.Deserialize(json);
                TestClass5WithoutParameterlessConstructor obj2 = o as TestClass5WithoutParameterlessConstructor;
                check(obj, obj2).Should().BeTrue();
            }

            {
                object o = serializer.Deserialize(json);
                TestClass5WithoutParameterlessConstructor obj2 = o as TestClass5WithoutParameterlessConstructor;
                check(obj, obj2).Should().BeTrue();
            }
        }

        [Fact]
        public void CyclicReferencesTest()
        {
            TotalSerializer serializer = new TotalSerializer();
            TestClass7WithCyclicReferences obj = new TestClass7WithCyclicReferences();

            obj.A = new TestClass7WithCyclicReferences.ClassA();
            obj.A.Oid = Guid.Parse("E0E9F237-B9AE-4984-883F-7D5B9868D1DD");

            obj.B = new TestClass7WithCyclicReferences.ClassB();
            obj.B.ModificationDate = DateTime.Parse("2019-02-05");

            obj.A.InnerB = obj.B;
            obj.B.InnerA = obj.A;

            string json = serializer.Serialize(obj);

            Func<TestClass7WithCyclicReferences, TestClass7WithCyclicReferences, bool> check = (obj1, obj2) =>
            {
                obj1.Should().NotBeNull();
                obj2.Should().NotBeNull();
                obj2.A.Oid.Should().Be(obj1.A.Oid);
                obj2.B.ModificationDate.Should().Be(obj1.B.ModificationDate);

                obj2.B.InnerA.Should().Be(obj2.A);
                obj2.A.InnerB.Should().Be(obj2.B);
                return true;
            };

            {
                TestClass7WithCyclicReferences obj2 = serializer.Deserialize<TestClass7WithCyclicReferences>(json);
                check(obj, obj2).Should().BeTrue();
            }

            {
                object o = serializer.Deserialize(json);
                TestClass7WithCyclicReferences obj2 = o as TestClass7WithCyclicReferences;
                check(obj, obj2).Should().BeTrue();
            }

            {
                object o = serializer.Deserialize(json);
                TestClass7WithCyclicReferences obj2 = o as TestClass7WithCyclicReferences;
                check(obj, obj2).Should().BeTrue();
            }
        }

        [Fact]
        public void ManyPropertiesTest()
        {
            TotalSerializer serializer = new TotalSerializer();
            TestClass8WithManyProperties obj = new TestClass8WithManyProperties();

            obj.Oid = Guid.Parse("E0E9F237-B9AE-4984-883F-7D5B9868D1DD");
            obj.OidWithNullValue = null;
            obj.Duration = TimeSpan.MaxValue;
            obj.Amount = 1.234m;
            obj.Is = true;
            obj.FixedDate = new DateTimeOffset(DateTime.Now);
            obj.ModificationDate = DateTime.FromBinary(987654321L);
            obj.Byte = 200;

            string json = serializer.Serialize(obj);

            Func<TestClass8WithManyProperties, TestClass8WithManyProperties, bool> check = (obj1, obj2) =>
            {
                obj1.Should().NotBeNull();
                obj2.Should().NotBeNull();
                obj2.Oid.Should().Be(obj1.Oid);
                obj2.OidWithNullValue.Should().Be(obj1.OidWithNullValue);
                obj2.Duration.Should().Be(obj1.Duration);
                obj2.Amount.Should().Be(obj1.Amount);
                obj2.Is.Should().Be(obj1.Is);
                obj2.FixedDate.Should().Be(obj1.FixedDate);
                obj2.ModificationDate.Should().Be(obj1.ModificationDate);
                obj2.Byte.Should().Be(obj1.Byte);
                return true;
            };

            {
                TestClass8WithManyProperties obj2 = serializer.Deserialize<TestClass8WithManyProperties>(json);
                check(obj, obj2).Should().BeTrue();
            }

            {
                object o = serializer.Deserialize(json);
                TestClass8WithManyProperties obj2 = o as TestClass8WithManyProperties;
                check(obj, obj2).Should().BeTrue();
            }

            {
                object o = serializer.Deserialize(json);
                TestClass8WithManyProperties obj2 = o as TestClass8WithManyProperties;
                check(obj, obj2).Should().BeTrue();
            }
        }

        [Fact]
        public void SingleComplexPropertyTest()
        {
            TotalSerializer serializer = new TotalSerializer();
            TestClass9WithSubClass obj = new TestClass9WithSubClass();

            obj.SubInstance = new TestClass9WithSubClass.SubClass9();
            obj.SubInstance.TestInt = 42;

            string json = serializer.Serialize(obj);

            Func<TestClass9WithSubClass, TestClass9WithSubClass, bool> check = (obj1, obj2) =>
            {
                obj1.Should().NotBeNull();
                obj2.Should().NotBeNull();
                obj2.SubInstance.Should().NotBeNull();
                obj2.SubInstance.TestInt.Should().Be(obj1.SubInstance.TestInt);
                return true;
            };

            {
                TestClass9WithSubClass obj2 = serializer.Deserialize<TestClass9WithSubClass>(json);
                check(obj, obj2).Should().BeTrue();
            }

            {
                object o = serializer.Deserialize(json);
                TestClass9WithSubClass obj2 = o as TestClass9WithSubClass;
                check(obj, obj2).Should().BeTrue();
            }

            {
                object o = serializer.Deserialize(json);
                TestClass9WithSubClass obj2 = o as TestClass9WithSubClass;
                check(obj, obj2).Should().BeTrue();
            }
        }

        [Fact]
        public void SerializeToStreamTest()
        {
            TotalSerializer serializer = new TotalSerializer();
            TestClass9WithSubClass obj = new TestClass9WithSubClass();
            string json = serializer.Serialize(obj);
            
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);

            serializer.Serialize(obj, streamWriter);
            
            // convert stream to string
            streamWriter.Flush();
            memoryStream.Position = 0;
            StreamReader reader = new StreamReader( memoryStream );
            string text = reader.ReadToEnd();
            
            json.Should().Be(text);
        }
    }
}
