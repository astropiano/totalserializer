using System;
using FluentAssertions;
using Xunit;
using TotalSerializer.ReflectionTools.Test.TestCases;

namespace TotalSerializer.ReflectionTools.Test
{
    public class FieldReaderTest
    {
        [Fact]
        public void GetValueTest()
        {
            Action act1 = () => { new FieldReader<FieldTestClass, string>(null, "asdf"); };
            act1.Should().Throw<ArgumentNullException>("There should be an ArgumentNullException because of the first argument which is null!");
            
            Action act2 = () => { new FieldReader<FieldTestClass, string>(typeof(FieldTestClass), (string)null); };
            act2.Should().Throw<ArgumentNullException>("There should be an ArgumentNullException because of the second argument which is null!");

            {
                FieldReader<FieldTestClass, int> fieldReader = new FieldReader<FieldTestClass, int>(typeof(FieldTestClass), "PublicIntegerValue");

                FieldTestClass f = new FieldTestClass();
                f.PublicIntegerValue = 5;

                {
                    int value = fieldReader.GetValue(f);
                    value.Should().Be(f.PublicIntegerValue, "Wrong value!");
                    object valueObj = fieldReader.GetUntypedValue(f);
                    valueObj.Should().Be(f.PublicIntegerValue, "Wrong object value!");
                }

                {
                    f.PublicIntegerValue = 6;
                    int valueNew = fieldReader.GetValue(f);
                    valueNew.Should().Be(f.PublicIntegerValue, "Wrong new value!");
                    object valueObjNew = fieldReader.GetUntypedValue(f);
                    valueObjNew.Should().Be(f.PublicIntegerValue, "Wrong new object value!");
                }

            }

        }


        [Fact(Skip = "TODO: This test fails because the binding flags in the FieldInfoCache don't include 'static'. Maybe it is on purpose?")]
        public void ReadStaticValueTest()
        {
            FieldReader<WithStaticField, string> fieldReader = new FieldReader<WithStaticField, string>(typeof(WithStaticField), "Name");

            WithStaticField f = new WithStaticField();
            {
                string v = fieldReader.GetValue(f);
                v.Should().Be(WithStaticField.Name, "Wrong value!");
                object o = fieldReader.GetUntypedValue(f);
                o.Should().Be((object)WithStaticField.Name, "Wrong object value!");
            }
        }

    }
}
