using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Sorter.Core;
using Sorter.Core.KWayMerge;

namespace Sorter.Tests
{
    public class TournamentTreeTest
    {
        [TestCaseSource(nameof(SortTestCases))]
        public void Sort(ColumnItem[] inputs, ColumnItem[] expectedSorted)
        {
            var tournamentTree = new TournamentTree(inputs);
            var result = tournamentTree.Sort();
            result.Should().BeEquivalentTo(expectedSorted);
        }

        public static IEnumerable<TestCaseData> SortTestCases => new[]
        {
            new TestCaseData(new ColumnItem[0], new ColumnItem[0]),
            new TestCaseData(Build(new []{("1", 0)}), Build(new []{("1", 0)})),
            new TestCaseData(Build(new []{("1", 0), ("2", 0)}), Build(new []{("1", 0), ("2", 0)})),
            new TestCaseData(Build(new []{("2", 0), ("1", 0)}), Build(new []{("1", 0), ("2", 0)})),
            new TestCaseData(Build(new []{("2", 0), ("1", 0), ("3", 0)}), Build(new []{("1", 0), ("2", 0), ("3", 0)})),
            new TestCaseData(Build(new []{("1", 0), ("1", 0), ("1", 0), ("1", 0)}), Build(new []{("1", 0), ("1", 0), ("1", 0), ("1", 0)})),
            new TestCaseData(Build(new []{("5", 0), ("1", 0), ("9", 0), ("3", 0)}), Build(new []{("1", 0), ("3", 0), ("5", 0), ("9", 0)})),
            
            new TestCaseData(Build(new []{("d", 0), ("c", 0), ("b", 0), ("a", 0)}), Build(new []{("a", 0), ("b", 0), ("c", 0), ("d", 0)})),
            new TestCaseData(Build(new []{("D", 0), ("C", 0), ("B", 0), ("A", 0)}), Build(new []{("A", 0), ("B", 0), ("C", 0), ("D", 0)})),
        };

        private static ColumnItem[] Build(IEnumerable<(string, int)> items)
        {
            return items.Select(e => new ColumnItem {StringPart = e.Item1, NumberPart = e.Item2}).ToArray();
        }
        [Test]
        //Sorted 500000 items for ~2076 ms
        public void SortTime()
        {
            const int sizeOfTestCollection = 500_000;
            
            var random = new Random();
            var stopwatch = Stopwatch.StartNew();
            var bigCollection = Enumerable.Range(0, sizeOfTestCollection).Select(e => new ColumnItem{NumberPart = random.Next(0, int.MaxValue)}).ToArray();
            Console.WriteLine($"Generate {sizeOfTestCollection} random items for {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"Expected time ~{Math.Log2(sizeOfTestCollection)} iteration");
            stopwatch.Restart();
            var tournamentTree = new TournamentTree(bigCollection);
            Console.WriteLine($"Generate tree from {sizeOfTestCollection} items for {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Restart();
            var result = tournamentTree.Sort();
            Console.WriteLine($"Sorted {sizeOfTestCollection} items for {stopwatch.ElapsedMilliseconds} ms");
            Assert.Less(stopwatch.ElapsedMilliseconds, 3000);
        }
        [Test]
        public void PopRootAndInsert()
        {
            var tournamentTree = new TournamentTree(Build(new []{((string)null, 5), (null, 1), (null, 9), (null, 3)}));
            tournamentTree.PopRoot().Should().Be(new ColumnItem{NumberPart = 1});
            tournamentTree.InsertLeaf(new ColumnItem{NumberPart = 2});
            tournamentTree.PopRoot().Should().Be(new ColumnItem{NumberPart = 2});
            tournamentTree.InsertLeaf(ColumnItem.Sentinel); // need this to make the calculation for the next root to happen
            tournamentTree.PopRoot().Should().Be(new ColumnItem{NumberPart = 3});
            tournamentTree.InsertLeaf(new ColumnItem{NumberPart = 1});
            tournamentTree.PopRoot().Should().Be(new ColumnItem{NumberPart = 1});
        }
    }
}