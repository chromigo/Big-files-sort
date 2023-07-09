using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Generator.Infra;

namespace Generator
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = DefaultConfigurationBuilder.Build();
            var dictSizeRaw = config["DictionarySize"];
            var maxWordSizeRaw = config["MaxWordSize"];
            var linesToGenerateRaw = config["GenerateLinesCount"];
            var savePathRaw = config["SavePath"];

            if (!int.TryParse(linesToGenerateRaw, out var linesCount) || !int.TryParse(dictSizeRaw, out var dictSize) || !int.TryParse(maxWordSizeRaw, out var maxWordSize))
            {
                Console.WriteLine("Cant parse settings");
                return;
            }

            var linesGenerator = new LinesGenerator();
            var generator = new DictionaryGenerator();
            var dictionaryWords = generator.Generate(maxWordSize, dictSize).ToArray();
            
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            using var stream = new StreamWriter(savePathRaw, false, Encoding.ASCII);
            foreach (var line in linesGenerator.GenerateLines(linesCount, dictionaryWords))
            {
                stream.WriteLine(line);    
            }
            stream.Close();
            stopwatch.Stop();
            Console.WriteLine($"Generated: {linesCount} lines. Dict size: {dictSizeRaw}. Time: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}