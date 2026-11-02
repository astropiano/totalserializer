using System;
using FluentAssertions;
using Xunit;
using TotalSerializer.ReflectionTools.Test.TestCases;

namespace TotalSerializer.ReflectionTools.Test
{
    public class FieldWriterTest
    {
        [Fact]
        public void WriteValueTest()
        {
            Action act1 = () =>
            {
                var fieldWriter = new FieldWriter<FieldTestClass, string>(null, "asdf");
                fieldWriter.Should().NotBeNull();
            };
            act1.Should().Throw<ArgumentNullException>("There should be an ArgumentNullException because of the first argument which is null!");
            
            Action act2 = () =>
            {
                var fieldWriter = new FieldWriter<FieldTestClass, string>(typeof(FieldTestClass), (string)null);
                fieldWriter.Should().NotBeNull();
            };
            act2.Should().Throw<ArgumentNullException>("There should be an ArgumentNullException because of the second argument which is null!");

            FieldWriter<FieldTestClass, int> fieldReader =
                new FieldWriter<FieldTestClass, int>(typeof(FieldTestClass), "PublicIntegerValue");

            FieldTestClass f = new FieldTestClass();
            {
                const int v = 5;
                fieldReader.SetValue(ref f, v);
                f.PublicIntegerValue.Should().Be(v, "Wrong value!");
                var o = (object) v;
                object f1 = f;
                fieldReader.SetUntypedValue(ref f1, o);
                f.PublicIntegerValue.Should().Be((int)o, "Wrong object value!");
            }
            {
                const int v = 4;
                fieldReader.SetValue(ref f, v);
                f.PublicIntegerValue.Should().Be(v, "Wrong new value!");
                var o = (object) v;
                object f1 = f;
                fieldReader.SetUntypedValue(ref f1, o);
                f.PublicIntegerValue.Should().Be((int)o, "Wrong new object value!");
            }
        }

        
        [Fact]
        public void WriteStaticValueTest()
        {
            FieldWriter<WithStaticField, string> fieldReader = new FieldWriter<WithStaticField, string>(typeof(WithStaticField), "Name");

            WithStaticField f = new WithStaticField();
            {
                const string v = "Hello";
                fieldReader.SetValue(ref f, v);
                WithStaticField.Name.Should().Be(v, "Wrong value!");
                var o = (object)v;
                object f1 = f;
                fieldReader.SetUntypedValue(ref f1, o);
                WithStaticField.Name.Should().Be((string)o, "Wrong object value!");
            }
            {
                const string v = "World";
                fieldReader.SetValue(ref f, v);
                WithStaticField.Name.Should().Be(v, "Wrong new value!");
                var o = (object)v;
                object f1 = f;
                fieldReader.SetUntypedValue(ref f1, o);
                WithStaticField.Name.Should().Be((string)o, "Wrong new object value!");
            }
        }
    }
}
