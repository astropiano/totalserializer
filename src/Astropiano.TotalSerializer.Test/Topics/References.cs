using FluentAssertions;
using Xunit;
using Astropiano.TotalSerializer.Test.TestCases;
using Astropiano.TotalSerializer.Test.TestCases.References;
using System;

namespace Astropiano.TotalSerializer.Test.Topics
{
    public class References
    {
        [Fact]
        public void ReferenceToRootElementTest()
        {
            TestClass13WithObject obj = new TestClass13WithObject();
            obj.Obj = obj;

            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(obj);
            TestClass13WithObject deserialized = (TestClass13WithObject)serializer.Deserialize(json);
            deserialized.Should().Be(deserialized.Obj);
        }

        [Fact]
        public void ManyReferences()
        {
            TestClass14ForReferences t = TestClass14ForReferences.Create();

            try
            {
                CheckReferences(t);
            }
            catch(Exception e)
            {
                throw new Exception("Check failed before serialization.", e);
            }

            TotalSerializer serializer = new TotalSerializer();
            string json = serializer.Serialize(t);
            //json.Should().Contain("\"$id\":25");
            //json.Should().Contain("\"$ref\":20");

            TestClass14ForReferences d = serializer.Deserialize<TestClass14ForReferences>(json);

            try
            {
                CheckReferences(d);
            }
            catch (Exception e)
            {
                throw new Exception("Check failed after serialization.", e);
            }
        }

        private static void CheckReferences(TestClass14ForReferences t)
        {
            CheckItemsForTheSameReference(t.ItemALink1, t.ItemALink2);
            CheckItemsForTheSameReference(t.ItemALink1, t.ItemALink3);
            CheckItemsForTheSameReference(t.ItemBLink1, t.ItemBLink2);
            CheckItemsForTheSameReference(t.ItemBLink1, t.ItemBLink3);
            CheckItemsForTheSameReference(t.ItemALink1, t.ItemBLink1);

            t.ItemALink1.Should().Be(t.ItemALink2, "A1 and A2 should be the same reference.");
            t.ItemALink1.Should().Be(t.ItemALink3, "A1 and A3 should be the same reference.");
            t.ItemBLink1.Should().Be(t.ItemBLink2, "B1 and B2 should be the same reference.");
            t.ItemBLink1.Should().Be(t.ItemBLink3, "B1 and B3 should be the same reference.");
            t.ItemALink1.Should().NotBe(t.ItemBLink1, "A1 and B1 should not be the same reference.");

            for(int i = 0;i<t.ItemALink1.SubItems.Length;i++)
            {
                t.ItemALink1.SubItems[i].Should().NotBe(t.ItemBLink1.SubItems[i], $"Sub items of A and B should be different, but they are not at index '{i}'.");
                t.ItemALink1.SubItems[i].Should().Be(t.ItemALink1.SubItemsSameRef[i], $"SubItems and SubItemsSameRef should be same, but they are not at index '{i}'.");
            }
        }

        private static void CheckItemsForTheSameReference(TestClass14ForReferences.Item item1, TestClass14ForReferences.Item item2)
        {
            item1.SubItems.Length.Should().BeGreaterThan(0, "No sub items 1!");
            item2.SubItems.Length.Should().BeGreaterThan(0, "No sub items 2!");
            for (int i = 0; i < item1.SubItems.Length; i++)
            {
                for (int j = 0; j < item2.SubItems.Length; j++)
                {
                    item1.SubItems[i].TheSame.Should().Be(item2.SubItems[j].TheSame, $"Wrong 'TheSame' for index '{i},{j}' for SubItems.");
                    item1.SubItemsSameRef[i].TheSame.Should().Be(item2.SubItemsSameRef[j].TheSame, $"Wrong 'TheSame' for index '{i},{j}' for SubItemsSameRef.");
                }
            }
        }
    }
}