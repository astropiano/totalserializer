using FluentAssertions;
using Xunit;
using System;
using System.Collections.Generic;

namespace Astropiano.TotalSerializer.Test.Topics
{
    public class ReadonlyFields
    {
        [Fact]
        public void Test1()
        {
            TotalSerializer serializer = new TotalSerializer();

            Parent p = Parent.Create();
            string json = serializer.Serialize(p);
            Parent d = serializer.Deserialize<Parent>(json);
            d.Should().NotBeNull("Could not deserialize.");
            p.Child.ReadonlyFieldContainer.Id.Should().Be(d.Child.ReadonlyFieldContainer.Id);
            d.Child.ReadonlyListContainer.ReadonlyList.Should().Contain("1", "Missing list item1.");
            d.Child.ReadonlyListContainer.ReadonlyList.Should().Contain("test", "Missing list item2.");
        }

        private class Parent
        {
            public Child Child { get; set; }

            public static Parent Create()
            {
                Parent parent = new Parent
                {
                    Child = new Child
                    {
                        ReadonlyFieldContainer = new ReadonlyFieldContainer(),
                        ReadonlyListContainer = new ReadonlyListContainer()
                    }
                };
                parent.Child.ReadonlyListContainer.ReadonlyList.AddRange(new string[] { "1", "test" });
                return parent;
            }
        }

        private class Child
        {
            public ReadonlyFieldContainer ReadonlyFieldContainer { get; set; }
            public ReadonlyListContainer ReadonlyListContainer { get; set; }
        }

        private class ReadonlyFieldContainer
        {
            public readonly string Id = Guid.NewGuid().ToString().Substring(1, 5);
        }

        private class ReadonlyListContainer
        {
            public readonly List<string> ReadonlyList = new List<string>();
        }
    }
}
