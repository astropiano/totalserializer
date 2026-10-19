using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Astropiano.TotalSerializer.Test.TestCases;
using Astropiano.TotalSerializer.Test.TestCases.Dictionaries;

namespace Astropiano.TotalSerializer.Test.Topics
{
    public class Dictionaries
    {
        // Covers case with struct properties (see svn log)
        [Fact]
        public void PrimitiveIntDictionaryTest()
        {
            TestClass18WithDictionaries obj = new TestClass18WithDictionaries();
            obj.PrimitiveIntDictionary = new Dictionary<int, int>();
            obj.PrimitiveIntDictionary.Add(1,2);
            obj.PrimitiveIntDictionary.Add(2,3);
            obj.PrimitiveIntDictionary.Add(3,5);
            obj.PrimitiveIntDictionary.Add(4,7);
            obj.PrimitiveIntDictionary.Add(5,11);
            obj.PrimitiveIntDictionary.Add(6,13);
            obj.PrimitiveIntDictionary.Add(7,17);
            
            TotalSerializer totalSerializer = new TotalSerializer();
            var json = totalSerializer.Serialize(obj);

            TestClass18WithDictionaries objD = totalSerializer.Deserialize<TestClass18WithDictionaries>(json);
            objD.PrimitiveIntDictionary[1].Should().Be(2);
            objD.PrimitiveIntDictionary[2].Should().Be(3);
            objD.PrimitiveIntDictionary[3].Should().Be(5);
            objD.PrimitiveIntDictionary[4].Should().Be(7);
            objD.PrimitiveIntDictionary[5].Should().Be(11);
            objD.PrimitiveIntDictionary[6].Should().Be(13);
            objD.PrimitiveIntDictionary[7].Should().Be(17);
        }

        [Fact]
        public void DictionaryErrorTest()
        {
            TestClass18WithDictionaries obj = new TestClass18WithDictionaries();
            obj.PrimitiveIntDictionary = new Dictionary<int, int>();
            obj.PrimitiveIntDictionary.Add(1,2);
            obj.PrimitiveIntDictionary.Add(2,3);
            obj.PrimitiveIntDictionary.Add(3,5);
            obj.PrimitiveIntDictionary.Add(4,7);
            obj.PrimitiveIntDictionary.Add(5,11);
            obj.PrimitiveIntDictionary.Add(6,13);
            obj.PrimitiveIntDictionary.Add(7,17);
            
            TotalSerializer totalSerializer = new TotalSerializer();
            var json1 = totalSerializer.Serialize(obj);
            TestClass18WithDictionaries objD = totalSerializer.Deserialize<TestClass18WithDictionaries>(json1);
            var json2 = totalSerializer.Serialize(objD);
            
            json1.Should().Be(json2);
        }
        
        [Fact]
        public void DictionaryObjectErrorTest()
        {
            TestClass18WithDictionaries obj = new TestClass18WithDictionaries();
            obj.ObjectDictionary = new Dictionary<object, int>();
            
            obj.ObjectDictionary.Add(new TestClass0(), 2);
            obj.ObjectDictionary.Add(new TestClass0(),3);
            obj.ObjectDictionary.Add(new TestClass0(),5);
            obj.ObjectDictionary.Add(new TestClass0(),7);
            obj.ObjectDictionary.Add(new TestClass0(),11);
            obj.ObjectDictionary.Add(new TestClass0(),13);
            obj.ObjectDictionary.Add(new TestClass0(),17);
            
            TotalSerializer totalSerializer = new TotalSerializer();
            var json1 = totalSerializer.Serialize(obj);
            TestClass18WithDictionaries objD = totalSerializer.Deserialize<TestClass18WithDictionaries>(json1);
            var json2 = totalSerializer.Serialize(objD);
            
            json1.Should().Be(json2);
        }

        [Fact]
        public void DictionaryObjectKeyTest()
        {
            TestClass13WithObject obj = new TestClass13WithObject();
            TestClass18WithDictionaries subObj = new TestClass18WithDictionaries();
            subObj.ObjectDictionary = new Dictionary<object, int>();
            subObj.ObjectDictionary.Add(obj, 12);
            obj.Obj = subObj;
            
            TotalSerializer totalSerializer =new TotalSerializer();
            var json = totalSerializer.Serialize(obj);
            TestClass13WithObject objD = totalSerializer.Deserialize<TestClass13WithObject>(json);
            TestClass18WithDictionaries subObjD = objD.Obj as TestClass18WithDictionaries;
            
            subObjD.Should().NotBeNull();
            subObjD.ObjectDictionary.Keys.First().Should().Be(objD);
            subObjD.ObjectDictionary[objD].Should().Be(12);
        }
        
        // Covers classes without parameter-less constructors
        [Fact]
        public void PrimitiveIntDictionaryWithReadingKeysTest()
        {
            TestClass18WithDictionaries obj = new TestClass18WithDictionaries();
            obj.PrimitiveIntDictionary = new Dictionary<int, int>();
            obj.PrimitiveIntDictionary.Add(1,2);

            var keys = obj.PrimitiveIntDictionary.Keys;
            keys.Should().NotBeNull();
            
            TotalSerializer totalSerializer = new TotalSerializer();
            var json = totalSerializer.Serialize(obj);

            TestClass18WithDictionaries objD = totalSerializer.Deserialize<TestClass18WithDictionaries>(json);
            objD.PrimitiveIntDictionary[1].Should().Be(2);
        }

        [Fact]
        public void ConcurrentDictionaryTest()
        {
            ConcurrentDictionary<int, int> dict = new ConcurrentDictionary<int, int>();
            dict.TryAdd(1, 2);
            
            TotalSerializer totalSerializer = new TotalSerializer();
            var json = totalSerializer.Serialize(dict);

            ConcurrentDictionary<int, int> dictD = totalSerializer.Deserialize<ConcurrentDictionary<int, int>>(json);
            dictD[1].Should().Be(2);
        }
        
        [Fact]
        public void ConcurrentDictionaryMultiEntryTest()
        {
            ConcurrentDictionary<int, int> dict = new ConcurrentDictionary<int, int>();
            dict.TryAdd(1, 2);
            dict.TryAdd(5, 20);
            
            TotalSerializer totalSerializer = new TotalSerializer();
            var json = totalSerializer.Serialize(dict);

            ConcurrentDictionary<int, int> dictD = totalSerializer.Deserialize<ConcurrentDictionary<int, int>>(json);
            dictD[1].Should().Be(2);
            dictD[5].Should().Be(20);
        }

        [Fact]
        public void ConcurrentDictionaryWithRefTest()
        {
            TestClass0 obj1 = new TestClass0();
            TestClass0 obj2 = new TestClass0();

            ConcurrentDictionary<int, TestClass0> dict = new ConcurrentDictionary<int, TestClass0>();
            
            dict.TryAdd(1, obj1);
            dict.TryAdd(2, obj2);
            dict.TryAdd(3, obj1);
            
            TotalSerializer totalSerializer = new TotalSerializer();
            var json = totalSerializer.Serialize(dict);

            ConcurrentDictionary<int, TestClass0> dictD = totalSerializer.Deserialize<ConcurrentDictionary<int, TestClass0>>(json);
            dictD[1].Should().Be(dictD[3]);
        }
        
        
    }
}