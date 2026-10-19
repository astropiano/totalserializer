using System;
using System.Collections.Generic;
using System.Text;

namespace Astropiano.TotalSerializer.Test.TestCases.References
{
    internal class TestClass14ForReferences
    {
        public Item ItemALink1 { get; set; }
        public Item ItemALink2 { get; set; }
        public Item ItemALink3 { get; set; }
        public Item ItemBLink1 { get; set; }
        public Item ItemBLink2 { get; set; }
        public Item ItemBLink3 { get; set; }

        internal class Item
        {
            public string Name { get; set; }
            public SubItem[] SubItems { get; set; }
            public SubItem[] SubItemsSameRef { get; set; }
        }

        internal class SubItem
        {
            public string SubItemName { get; set; }
            public AlwaysTheSame TheSame { get; set; }
        }

        internal class AlwaysTheSame
        {
            public string Id { get; set; }
        }

        public static TestClass14ForReferences Create()
        {
            AlwaysTheSame alwaysTheSame = new AlwaysTheSame
            {
                Id = "3.16"
            };

            const int size = 20;
            SubItem[] subItems1 = CreateSubItemArray(size, alwaysTheSame);
            SubItem[] subItems2 = CreateSubItemArray(size, alwaysTheSame);

            Item itemA = new Item
            {
                Name = "ItemA",
                SubItems = subItems1,
                SubItemsSameRef = subItems1
            };
            Item itemB = new Item
            {
                Name = "ItemB",
                SubItems = subItems2,
                SubItemsSameRef = subItems2
            };

            return new TestClass14ForReferences
            {
                ItemALink1 = itemA,
                ItemALink2 = itemA,
                ItemALink3 = itemA,
                ItemBLink1 = itemB,
                ItemBLink2 = itemB,
                ItemBLink3 = itemB,
            };
        }

        private static SubItem[] CreateSubItemArray(int size, AlwaysTheSame alwaysTheSame)
        {
            SubItem[] subItems = new SubItem[size];
            for (int i = 0; i < subItems.Length; ++i)
            {
                subItems[i] = new SubItem
                {
                    SubItemName = "SubItem" + i,
                    TheSame = alwaysTheSame
                };
            }
            return subItems;
        }
    }
}