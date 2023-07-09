using System.Collections.Generic;
using System.Linq;

namespace Sorter.Core.KWayMerge
{
    public class TournamentTree
    {
        private readonly List<List<Node>> nodes = new();
        public int NodeCount => nodes.Count;
        public int? MissingLeafIndex;

        public TournamentTree(ColumnItem[] dataArr)
        {
            // store entire tree as array of arrays in a bottom-up manner
            // index 0 = leaves, index 1 = bottom branches, index 1++ = higher branches all the way to the root
            nodes.Add(dataArr.Select(e => new Node {Value = e}).ToList());

            // construct the rest of the tournament tree
            int level = 0;
            for (; nodes[level].Count > 1; level++)
            {
                // create next level
                nodes.Add(new List<Node>(nodes[level].Count));

                // assign this level's tournament victors to next level
                for (var i = 0; i < nodes[level].Count; i = i + 2)
                {
                    var leftIndex = i;
                    var rightIndex = i + 1;
                    var leftVal = nodes[level][leftIndex].Value;
                    var rightVal = rightIndex < nodes[level].Count
                        ? nodes[level][rightIndex].Value
                        : ColumnItem.Sentinel;

                    // store the node value along with its index in the lower level so we can track it backward when removing the root
                    nodes[level + 1].Add(ColumnItemComparer.Instance.Compare(leftVal, rightVal) < 0
                        ? new Node {Value = leftVal, Index = leftIndex}
                        : new Node {Value = rightVal, Index = rightIndex});
                }
            }
        }

        public ColumnItem PopRoot()
        {
            // working our way back, remove all the branches that the root element came from
            var topLevel = nodes.Count - 1;
            var rootNodeValue = nodes[topLevel][0].Value;
            if (rootNodeValue.IsSentinel())
                return rootNodeValue;
            // loop until we reach a node without a descendant (a leaf node)
            int level = topLevel;
            int i = 0;
            var currentNode = nodes[level][i];
            for (;; level--, i = currentNode.Index.Value, currentNode = nodes[level][i])
            {
                nodes[level][i] = null;

                if (level == 0)
                {
                    MissingLeafIndex = i;
                    break;
                }
            }

            return rootNodeValue;
        }

        public void InsertLeaf(ColumnItem leafValue)
        {
            // insert the leaf into the blank spot
            nodes[0][MissingLeafIndex.Value] = new Node {Value = leafValue};

            // rebuild the missing branches
            var leftIndex = MissingLeafIndex.Value;
            for (var level = 0; level < nodes.Count - 1; level++)
            {
                var rightIndex = leftIndex - 1;
                var nextLevelIndex = rightIndex / 2;
                if (leftIndex % 2 == 0)
                {
                    rightIndex = leftIndex + 1;
                    nextLevelIndex = leftIndex / 2;
                }

                var leftVal = nodes[level][leftIndex].Value;
                var rightVal = rightIndex < nodes[level].Count ? nodes[level][rightIndex].Value : ColumnItem.Sentinel;
                nodes[level + 1][nextLevelIndex] = ColumnItemComparer.Instance.Compare(leftVal, rightVal) < 0
                    ? new Node {Value = leftVal, Index = leftIndex}
                    : new Node {Value = rightVal, Index = rightIndex};
                leftIndex = nextLevelIndex;
            }

            MissingLeafIndex = null;
        }

        public ColumnItem[] Sort()
        {
            // return the full sorted array by repeatedly popping the root node
            // and pushing Infinity as sentinel value for the missing leaves
            var sorted = new ColumnItem[nodes[0].Count];
            for (var i = 0; i < nodes[0].Count; i++)
            {
                sorted[i] = PopRoot();
                InsertLeaf(ColumnItem.Sentinel);
            }

            return sorted;
        }

        private class Node//todo struct
        {
            public ColumnItem Value;
            public int? Index;
        }
    }
}