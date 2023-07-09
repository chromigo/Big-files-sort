using System;
using System.Collections.Generic;
using System.Linq;

namespace Generator
{
    public class LinesGenerator
    {
        private Random _random = new ();
        
        public IEnumerable<string> GenerateLines(int linesCount, string[] dictionaryWords)
        {
            return Enumerable
                .Range(0, linesCount)
                .Select(_ =>
                {
                    var word = dictionaryWords[_random.Next(0, dictionaryWords.Length)];
                    return GenerateLine(word);
                });
        }
        
        private string GenerateLine(string word)
        {
            return $"{_random.Next(0, int.MaxValue/2)}. {word}";
        }
    }
}