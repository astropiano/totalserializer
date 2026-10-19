using FluentAssertions;
using Xunit;
using Astropiano.TotalSerializer.Test.TestCases;
using Astropiano.TotalSerializer.Test.TestCases.Array;
using System.Collections.Generic;

namespace Astropiano.TotalSerializer.Test.Topics
{
    public class Arrays
    {
        [Fact]
        public void IntegerEmptyArraySerializationTest()
        {
            TestClass10WithIntegerArray obj = new TestClass10WithIntegerArray();
            obj.SetIntegerArray(new int[0]);
            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(obj);

            TestClass10WithIntegerArray objD = serializer.Deserialize<TestClass10WithIntegerArray>(json);
            
            objD.Should().NotBeNull();
            
            int[] arr = obj.GetIntegerArray();
            int[] arrD = obj.GetIntegerArray();

            for (int i = 0; i < arr.Length; i++)
            {
                arrD[i].Should().Be(arr[i]);
            }
        }
        
        [Fact]
        public void IntegerArraySerializationTest()
        {
            TestClass10WithIntegerArray obj = new TestClass10WithIntegerArray();
            obj.SetIntegerArray(new []{1,2,3,4});
            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(obj);
            
            TestClass10WithIntegerArray objD = serializer.Deserialize<TestClass10WithIntegerArray>(json);
            
            objD.Should().NotBeNull();

            int[] arr = obj.GetIntegerArray();
            int[] arrD = obj.GetIntegerArray();

            for (int i = 0; i < arr.Length; i++)
            {
                arrD[i].Should().Be(arr[i]);
            }
        }
        
        [Fact]
        public void IntegerArrayDeserializationTest()
        {
            TestClass10WithIntegerArray obj = new TestClass10WithIntegerArray();
            obj.SetIntegerArray(new []{1,2,3,4});
            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(obj);

            
            TestClass10WithIntegerArray objDeserialized = serializer.Deserialize<TestClass10WithIntegerArray>(json);
            int[] arr1 = obj.GetIntegerArray();
            int[] arr2 = objDeserialized.GetIntegerArray();
            arr2.Length.Should().Be(arr1.Length);
            for (int i = 0; i < arr2.Length; i++)
            {
                arr2[i].Should().Be(arr1[i]);
            }
        }
        
        [Fact]
        public void DirectArrayDeserializationTest()
        {
            TestClass10WithIntegerArray obj1 = new TestClass10WithIntegerArray();
            obj1.SetIntegerArray(new []{1,2,3,4});
            TestClass10WithIntegerArray obj2 = new TestClass10WithIntegerArray();
            obj2.SetIntegerArray(new []{7,8,9});

            {
                TestClass10WithIntegerArray[] emptyArray = { };
                TotalSerializer serializer = new TotalSerializer();
                string json = serializer.Serialize(emptyArray);
                TestClass10WithIntegerArray[] emptyArrayDeserialized = serializer.Deserialize<TestClass10WithIntegerArray[]>(json);
                emptyArrayDeserialized.Should().NotBeNull("emptyArrayDeserialized != null");
                emptyArrayDeserialized.Length.Should().Be(emptyArray.Length, "Wrong number of elements");
            }

            {
                TestClass10WithIntegerArray[] array = {obj1, obj2};
                TotalSerializer serializer = new TotalSerializer();
                string json = serializer.Serialize(array);
                TestClass10WithIntegerArray[] arrayDeserialized = serializer.Deserialize<TestClass10WithIntegerArray[]>(json);
                arrayDeserialized.Should().NotBeNull("emptyArrayDeserialized != null");
                arrayDeserialized.Length.Should().Be(array.Length, "Wrong number of elements");
                for (int i = 0; i < array.Length; i++)
                {
                    arrayDeserialized[i].GetIntegerArray().Length.Should().Be(array[i].GetIntegerArray().Length, $"Wrong number of integer array elements for item '{i}'.");
                    for (int j = 0; j < array[i].GetIntegerArray().Length; j++)
                    {
                        arrayDeserialized[i].GetIntegerArray()[j].Should().Be(array[i].GetIntegerArray()[j], $"Wrong integer at {j},{i}' for .");
                    }
                }
            }
        }

        [Fact]
        public void ObjectArrayAndObjectListSerializationTest()
        {
            TestClass11WithObjectArray obj = new TestClass11WithObjectArray();
            TestClass0 t1 = new TestClass0();
            TestClass0 t2 = new TestClass0();
            obj.SetObjectArray(new object[]
            {
                t1,t2,t1
            });
            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(obj);

            TestClass11WithObjectArray objD = serializer.Deserialize<TestClass11WithObjectArray>(json);
            objD.Should().NotBeNull();
          
            
            obj = new TestClass11WithObjectArray();
            IList<object> list = new List<object>()
            {
                t1, t2, t1
            };
            obj.SetObjectArray(list);
            json = serializer.Serialize(obj);

            TestClass11WithObjectArray deserializedObj = serializer.Deserialize<TestClass11WithObjectArray>(json);
            deserializedObj.Should().NotBeNull();
            List<object> deserializedList = deserializedObj.GetObjectArray() as List<object>;
            deserializedList.Should().NotBeNull();
            deserializedList.Count.Should().Be(3);
            deserializedList[0].Should().Be(deserializedList[2]);
        }

        [Fact]
        public void NullElementInArrayTest()
        {
            TestClass11WithObjectArray obj = new TestClass11WithObjectArray();
            object[] objectArray = new object[] {null, null};
            obj.SetObjectArray(objectArray);
            
            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(obj);
            TestClass11WithObjectArray objD = serializer.Deserialize<TestClass11WithObjectArray>(json);
            
            objD.Should().NotBeNull();
            object[] objectArrayD = objD.GetObjectArray() as object[];
            objectArrayD.Should().NotBeNull();
            objectArrayD.Length.Should().Be(2);
            foreach (object o in objectArrayD)
            {
                o.Should().BeNull();
            }
        }

        // This test does also test direct serialization of arrays
        [Fact]
        public void TwoDimensionalJaggedArrayTest()
        {
            
            const int sizeX = 3;
            const int sizeY = 3;
            int[][] twoDimensionalArray = new int[sizeX][];
            for (int x = 0; x < sizeX; x++)
            {
                twoDimensionalArray[x] = new int[sizeY];
                for (int y = 0; y < sizeY; y++)
                {
                    twoDimensionalArray[x][y] = x + y;
                }    
            }
            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(twoDimensionalArray);


            int[][] twoDimensionalArrayD = serializer.Deserialize<int[][]>(json);
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    twoDimensionalArrayD[x][y].Should().Be(x + y);
                }    
            }
        }
        
        [Fact]
        public void TwoDimensionalMultidimensionalArrayTest()
        {
            
            const int sizeX = 3;
            const int sizeY = 3;
            int[,] twoDimensionalArray = new int[sizeX,sizeY];
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    twoDimensionalArray[x,y] = x + y;
                }    
            }
            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(twoDimensionalArray);


            int[,] twoDimensionalArrayD = serializer.Deserialize<int[,]>(json);
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    twoDimensionalArrayD[x,y].Should().Be(x + y);
                }    
            }
        }
        
        // This tes makes sure the order is correctly preserved
        [Fact]
        public void TwoDimensionalMultidimensionalArrayWithCharTest()
        {
            
            char[,] twoDimensionalArray = new char[2,2];

            twoDimensionalArray[0, 0] = 'a';
            twoDimensionalArray[0, 1] = 'b';
            twoDimensionalArray[1, 0] = 'c';
            twoDimensionalArray[1, 1] = 'd';

            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(twoDimensionalArray);


            char[,] twoDimensionalArrayD = serializer.Deserialize<char[,] >(json);
            
            twoDimensionalArrayD[0, 0].Should().Be('a');
            twoDimensionalArrayD[0, 1].Should().Be('b');
            twoDimensionalArrayD[1, 0].Should().Be('c');
            twoDimensionalArrayD[1, 1].Should().Be('d');
        }

        [Fact(Skip = "Error: System.InvalidCastException : Object cannot be stored in an array of this type.")]
        public void ArrayWithDelegates()
        {
            const int size = 1;
            ArrayWithDelegateTestClass o = ArrayWithDelegateTestClass.Create(size);

            const int index = 0;
            o.PropertyMarkerArray[index].Should().Be("", "Initially marker should be string.Empty.");
            o.DelegateArray[index](index, o);
            o.PropertyMarkerArray[index].Should().Be("marked_0", "After executing delegate marker should no longer be empty.");
            
            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(o);

            ArrayWithDelegateTestClass deserialized = serializer.Deserialize<ArrayWithDelegateTestClass>(json);

            deserialized.PropertyMarkerArray[index].Should().Be("marked_0", "After deserialization of the deserialized object marker should still be there.");
            deserialized.ResetMarkers(size);
            deserialized.PropertyMarkerArray[index].Should().Be("", "After reset on the deserialized object marker should again be empty.");
            deserialized.DelegateArray[index](index, o);
            deserialized.PropertyMarkerArray[index].Should().Be("marked_0", "After executing delegate on the deserialized object marker should no longer be empty.");
            deserialized.PropertyMarkerArray[index].Should().Be("test");
        }

        [Fact]
        public void StringArrayWithSpecialCharacters()
        {
            TestArray(new []{"{1,[."});
            TestArray(new []{"}}"});
            TestArray(new []{"]78"});
            TestArray(new []{",,,"});
            TestArray(new []{"]\",sds"});
        }

        private void TestArray<T>(T[] stringArray)
        {
            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(stringArray);

            T[] stringArrayD = serializer.Deserialize<T[]>(json);
            AssertArrayElementEquality(stringArray, stringArrayD);
        }

        private void AssertArrayElementEquality<T>(T[] one, T[] two)
        {
            for (int i = 0; i < one.Length; i++)
            {
                two[i].Should().Be(one[i]);
            }
        }

        [Fact]
        public void DifferentPrimitivesInObjectArray()
        {
            TotalSerializer serializer = new TotalSerializer();
            object[] objArr =  new object[]  {1, "2"};
            string json = serializer.Serialize(objArr);
            object[] objArrD = serializer.Deserialize<object[]>(json);
            
            objArrD.Should().BeEquivalentTo(objArr);

        }

        [Fact]
        public void ReferenceHandlingOfArray()
        {
            object[] array = new object[1];
            array[0] = array;
            TotalSerializer serializer = new TotalSerializer();
            object[] arrayCopy = serializer.Copy(array);
            ((object)arrayCopy).Should().BeSameAs(arrayCopy[0]);
        }
    }
}