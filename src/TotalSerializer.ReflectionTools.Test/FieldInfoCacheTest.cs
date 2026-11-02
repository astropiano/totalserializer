using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using TotalSerializer.ReflectionTools.Test.TestCases;

namespace TotalSerializer.ReflectionTools.Test
{
    public class FieldInfoCacheTest
    {
        ConcurrentQueue<FieldInfo> _fieldInfoQueue = new ConcurrentQueue<FieldInfo>();
        ConcurrentQueue<FieldInfo[]> _fieldInfosQueue = new ConcurrentQueue<FieldInfo[]>();
        private const int Total = 3000;

        [Fact]
        public async Task ParallelGetFieldInfoTest()
        {
            const string fieldName1 = "_privateIntegerValue";

            FieldInfoCache.Clear();
            FieldInfoCache.FieldInfoCount().Should().Be(0, "Wrong count after clear!");

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < Total; ++i)
            {
                tasks.Add(GetFieldInfoAsync(i, typeof(FieldTestClass), fieldName1));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            _fieldInfoQueue.Count.Should().Be(Total, "Wrong number of field infos!");
            FieldInfo[] list = _fieldInfoQueue.ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                FieldInfo f = list[i];
                f.Should().NotBeNull($"Field is null for item '{i}'!");
                f.Name.Should().Be(fieldName1, "Wrong name!");
            }

            FieldInfoCache.FieldInfoCount().Should().BeGreaterThan(0, "Wrong field count after test!");
        }

        [Fact(Skip = "I don't understand the interface implementation.")]
        public async Task ParallelGetFieldInfoForInterfaceTest()
        {
            string fieldName1 = "modificationDate";

            FieldInfoCache.Clear();
            FieldInfoCache.FieldInfoCount().Should().Be(0, "Wrong count after clear!");

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < Total; ++i)
            {
                tasks.Add(GetFieldInfoAsync(i, typeof(IFieldExample2), fieldName1));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            _fieldInfoQueue.Count.Should().Be(Total, "Wrong number of field infos!");
            FieldInfo[] list = _fieldInfoQueue.ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                FieldInfo f = list[i];
                f.Should().NotBeNull($"Field is null for item '{i}'!");
                f.Name.Should().Be(fieldName1, "Wrong name!");
            }

            FieldInfoCache.FieldInfoCount().Should().BeGreaterThan(0, "Wrong field count after test!");
        }

        [Fact]
        public async Task ParallelGetFieldInfosTest()
        {
            const string fieldName1 = "_privateIntegerValue";
            const string propertyName1 = "PublicIntegerValue";

            FieldInfoCache.Clear();
            FieldInfoCache.FieldInfoArrayCount().Should().Be(0, "Wrong count after clear!");

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < Total; ++i)
            {
                tasks.Add(GetFieldInfosAsync(i, typeof(FieldTestClass)));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            string[] names = {fieldName1, propertyName1};
            _fieldInfosQueue.Count.Should().Be(Total, "Wrong number of field infos!");
            FieldInfo[][] lists = _fieldInfosQueue.ToArray();
            for (int i = 0; i < lists.Length; ++i)
            {
                lists[i].Length.Should().Be(names.Length, "Wrong number of field infos!");
                for (int j = 0; j < lists[i].Length; ++j)
                {
                    FieldInfo f = lists[i][j];
                    f.Should().NotBeNull($"Field is null for item '{i}.{j}'!");
                    names.FirstOrDefault(l => l == f.Name).Should().NotBeNull("Wrong name!");
                }
            }

            FieldInfoCache.FieldInfoArrayCount().Should().BeGreaterThan(0, "Wrong array field count after test!");
        }

        [Fact(Skip = "I don't understand the interface implementation.")]
        public async Task ParallelGetFieldInfosForInterfaceTest()
        {
            string fieldName1 = "modificationDate";
            string fieldName2 = "lifeTime";

            FieldInfoCache.Clear();
            FieldInfoCache.FieldInfoArrayCount().Should().Be(0, "Wrong count after clear!");

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < Total; ++i)
            {
                tasks.Add(GetFieldInfosAsync(i, typeof(IFieldExample2)));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            string[] names = {fieldName1, fieldName2};
            _fieldInfosQueue.Count.Should().Be(Total, "Wrong number of field infos!");
            FieldInfo[][] lists = _fieldInfosQueue.ToArray();
            for (int i = 0; i < lists.Length; ++i)
            {
                lists[i].Length.Should().Be(names.Length, "Wrong number of field infos!");
                for (int j = 0; j < lists[i].Length; ++j)
                {
                    FieldInfo f = lists[i][j];
                    f.Should().NotBeNull($"Field is null for item '{i}.{j}'!");
                    names.FirstOrDefault(l => l == f.Name).Should().NotBeNull("Wrong name!");
                }
            }

            FieldInfoCache.FieldInfoArrayCount().Should().BeGreaterThan(0, "Wrong array field count after test!");
        }


        private async Task GetFieldInfoAsync(int i, Type t, string fieldName)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(Total + 10 - i)).ConfigureAwait(false);
            FieldInfo fieldInfo = FieldInfoCache.GetFieldInfo(t, fieldName);
            fieldInfo.Should().NotBeNull("Could not get field info!");
            _fieldInfoQueue.Enqueue(fieldInfo);
        }


        private async Task GetFieldInfosAsync(int i, Type t)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(Total + 10 - i)).ConfigureAwait(false);
            var fieldInfos = FieldInfoCache.GetFieldInfos(t);
            fieldInfos.Should().NotBeNull("Could not get field info!");
            fieldInfos.Length.Should().NotBe(0, "Could not get enough field info!");
            _fieldInfosQueue.Enqueue(fieldInfos);
        }

/*
        [Test]
        public void NoReadOnlyFlagTest()
        {
            var fieldInfos = FieldInfoCache.GetFieldInfos(typeof(ReadonlyFieldTest));
            Assert.AreEqual(1, fieldInfos.Length);
            fieldInfos = FieldInfoCache.GetFieldInfos(typeof(ReadonlyFieldTest), FieldFlags.NoReadOnly);
            Assert.AreEqual(0, fieldInfos.Length);     }*/
    }
}
