using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorter.Core
{
    public static class SplitSorter
    {
        public static void SplitAndSort(string path, string tempFolderForPartsPath, 
            int splitSizeInLines,
            int sorterSaversCount,
            int boundedCapacitySplit)
        {
            int part = 0;
            using var stream = new StreamReader(path, Encoding.ASCII);
            using var blockingCollection = new BlockingCollection<(ColumnItem[], int)>(boundedCapacitySplit);
            
            var sorters = Enumerable.Range(0, sorterSaversCount).Select(e => (Action) SortAndSave).ToArray();
            var readers = new[] {(Action) Split};
            
            Console.WriteLine("--Start Splitting and sorting phase--");
            Console.WriteLine($"Sorting threads: {sorterSaversCount}; Readers:{readers.Length}");
            var stopwatchTotal = new Stopwatch();
            stopwatchTotal.Start();
            Parallel.Invoke(readers.Concat(sorters).ToArray());
            Console.WriteLine($"Parts: {part}; Time: {stopwatchTotal.ElapsedMilliseconds} ms");
            Console.WriteLine("--End Splitting and sorting phase--");
            
            void Split()
            {
                while (stream.Peek() >= 0)
                {
                    var items = new ColumnItem[splitSizeInLines];
                    int i = 0;
                    for (; i < splitSizeInLines && stream.Peek() >= 0; i++)
                    {
                        var line = stream.ReadLine();
                        var words = line.Split('.', 2);
                        items[i] = new ColumnItem
                        {
                            NumberPart = int.Parse(words[0]), 
                            StringPart = words[1]
                        };
                    }

                    if (i < splitSizeInLines) items = items.Take(i).ToArray();
                    blockingCollection.Add((items, part));
                    part++;
                }

                blockingCollection.CompleteAdding();
            }

            void SortAndSave()
            {
                try
                {
                    while (true)
                    {
                        var (items, part) = blockingCollection.Take();
                        Array.Sort(items, ColumnItemComparer.Instance);
                        using var writer = new BinaryWriter(File.Open($@"{tempFolderForPartsPath}\file-part{part}.txt", FileMode.Create), Encoding.ASCII);
                        foreach (var item in items)
                        {
                            writer.Write(item.StringPart);
                            writer.Write(item.NumberPart);
                        }
                    }
                }
                catch (InvalidOperationException)
                {
                }
            }
        }
    }
}