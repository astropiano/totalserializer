using System;
using System.Collections.Generic;

namespace Astropiano.TotalSerializer.Performance.Test
{
    public class BigTestObject
    {
        public static BigTestObject Create(int width) {

            BigTestObject bigTestObject = new BigTestObject();

            List<NodeObject> list = new List<NodeObject>(width);
            for (int i = 0; i < width; ++i) {
                LeafObject leaf = new LeafObject();
                list.Add(new NodeObject(leaf, null));
            }
            NodeObject root = new NodeObject(null, list);
            bigTestObject.Tree = new TreeObject(root);
            return bigTestObject;
        }

        public TreeObject Tree { get; set; }
    }

    public class TreeObject
    {
        public TreeObject() : this(null) { }
        public TreeObject(NodeObject root) {
            Root = root;
        }
        public NodeObject Root { get; set; }
    }

    public class NodeObject
    {
        public NodeObject() : this(null, null) { }
        public NodeObject(LeafObject leaf, List<NodeObject> children) {
            Leaf = leaf;
            Children = children;
        }

        public LeafObject Leaf { get; set; }
        public List<NodeObject> Children { get; set; }
    }

    public class LeafObject
    {
        public LeafObject() : this(null, null, null, null) { }
        public LeafObject(Guid? oid, string info, double[] doubleArray, int[] intArray) {
            Oid = oid;
            Info = info;
            DoubleArray = doubleArray;
            IntArray = intArray;
        }

        public Guid? Oid { get; set; }
        public string Info { get; set; }
        public double[] DoubleArray { get; set; }
        public int[] IntArray { get; set; }

        public static LeafObject Create() {
            LeafObject leafObject = new LeafObject(Guid.NewGuid(), Guid.NewGuid().ToString(), new double[] { 1.0, 10.0, 100.0, 1000.0, 1e4, 1e5, 1e6, 1e7, 1e8, -2.0, -20.0, -200.0, -2000.0, -2e4, -2e5, -2e6, -2e7, -2e8, 0.3, 0.03, 0.003, 3e-4, 3e-5, 3e-6, 3e-7, 3e-8 }, new int[] { 1, 10, 100, 1000, 10000, 100000, 1000000, -2, -20, -200, -2000, -20000, -200000, -2000000 });
            return leafObject;
        }
    }
}