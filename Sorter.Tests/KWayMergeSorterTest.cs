using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using Sorter.Core;
using Sorter.Core.KWayMerge;

namespace Sorter.Tests
{
    public class KWayMergeSorterTest
    {
        [TestCaseSource(nameof(SortTestCases))]
        public void Sort(List<List<ColumnItem>> list, ColumnItem[] expectedSorted)
        {
            var readers = list.Select(listItem =>
            {
                var memoryStream = new MemoryStream();
                var writer = new BinaryWriter(memoryStream);
                foreach (var item in listItem)
                {
                    writer.Write(item.StringPart);
                    writer.Write(item.NumberPart);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                return new BinaryReader(memoryStream);
            }).ToArray();

            using var streamWriter = new StreamWriter("result.txt", false, Encoding.ASCII);
            KWayMergeSorter.Sort(readers, streamWriter);
            
            streamWriter.Close();
            foreach (var reader in readers)
            {
                reader.Dispose();
            }

            var results = File.ReadAllLines("result.txt", Encoding.ASCII);
            var expectations = expectedSorted.Select(e => string.Join(".", e.NumberPart, e.StringPart));
            results.Should().BeEquivalentTo(expectations);
        }

        public static IEnumerable<TestCaseData> SortTestCases => new[]
        {
            new TestCaseData(new List<List<ColumnItem>>
                {
                    new(Build(new []{("A", 4), ("A", 9)})), 
                    new(Build(new []{("A", 1), ("A", 7)})), 
                    new(Build(new []{("A", 3), ("A", 6)})), 
                },
                Build(new []{("A", 1), ("A", 3), ("A", 4), ("A", 6),("A", 7), ("A", 9)}))
        };
        private static ColumnItem[] Build(IEnumerable<(string, int)> items)
        {
            return items.Select(e => new ColumnItem {StringPart = e.Item1, NumberPart = e.Item2}).ToArray();
        }
    }
}