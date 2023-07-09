using System;
using System.Diagnostics;
using Sorter.Core;
using Sorter.Core.KWayMerge;
using Sorter.Infra;

namespace Sorter
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = DefaultConfigurationBuilder.Build();
            
            var fileForSortPath = config["FileForSort"];
            var tempFolderForPartsPath = config["TempFolderForParts"];
            var sortedFilePath = config["SortedFile"];
            
            //размер частей на которые первоначально разбиваем большой файл.
            //Влияент на скорость разбиения и потребляемое кол-во памяти + на скорость работы второй части алгоритма
            //на больших размерах файлов желательно дробить на куски побольше(~100mb, ~1gb+)

            if (!int.TryParse(config["SplitSizeInLines"], out var splitSizeInLines) ||
                !int.TryParse(config["SortingThreads"], out var sortingThreads) ||
                !int.TryParse(config["BoundedCapacitySplit"], out var boundedCapacitySplit))
            {
                Console.WriteLine("Can't parse settings");
                return;
            }
            
            DirectoryPreparer.Prepare(tempFolderForPartsPath);
            
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            SplitSorter.SplitAndSort(fileForSortPath, tempFolderForPartsPath, splitSizeInLines, sortingThreads, boundedCapacitySplit);
            KWayMergeSorter.Sort(tempFolderForPartsPath, sortedFilePath);
            Console.WriteLine($"Total time: {stopwatch.ElapsedMilliseconds} ms");
            DirectoryPreparer.Prepare(tempFolderForPartsPath);
        }
    }
}