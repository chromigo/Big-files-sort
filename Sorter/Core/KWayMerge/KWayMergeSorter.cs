using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Sorter.Core.KWayMerge
{
    public static class KWayMergeSorter
    {
        public static void Sort(string tempFolderForParts, string sortedFilePath)
        {
            var paths = Directory.GetFiles(tempFolderForParts, "file-part*.txt");
            var streamReaders = paths
                .Select(path => new BinaryReader(File.Open(path, FileMode.Open), Encoding.ASCII))
                .ToArray();
            using var streamWriter = new StreamWriter(sortedFilePath, false, Encoding.ASCII);
            Sort(streamReaders, streamWriter);

            foreach (var streamReader in streamReaders)
            {
                streamReader.Dispose();
            }
        }
        public static void Sort(BinaryReader[] inputReaders, StreamWriter streamWriter)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var tree = BuildTree(inputReaders);
            Console.WriteLine("--Start Tournament phase--");
            Console.WriteLine($"Tree nodes: {tree.NodeCount}");
            Sorting();
            Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine("--End Tournament phase--");

            void Sorting()
            {
                while (true)
                {
                    var columnItem = tree.PopRoot();
                    if (columnItem.IsSentinel()) break;

                    streamWriter.WriteLine(columnItem.NumberPart + "." + columnItem.StringPart);
                    
                    var reader = inputReaders[tree.MissingLeafIndex.Value];
                    if (reader.PeekChar() > -1)
                    {
                        tree.InsertLeaf(new ColumnItem
                            { StringPart = reader.ReadString(), NumberPart = reader.ReadInt32() });
                    }
                    else
                    {
                        tree.InsertLeaf(ColumnItem.Sentinel);
                    }
                }
            }
        }

        private static TournamentTree BuildTree(BinaryReader[] inputReaders)
        {
            var initialValues = inputReaders
                .Select(reader =>
                {
                    if (reader.PeekChar() > -1)
                        return new ColumnItem
                        {
                            StringPart = reader.ReadString(),
                            NumberPart = reader.ReadInt32()
                        };
                    throw new Exception("Empty stream");
                }).ToArray();
            return new TournamentTree(initialValues);
        }
    }
}