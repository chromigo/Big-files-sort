using System;
using System.Collections.Generic;
using System.Linq;

namespace Generator
{
    public class DictionaryGenerator
    {
        private const string UpperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string LowerChars = "abcdefghijklmnopqrstuvwxyz";
        private const string DigitsChars = "0123456789";
        private const string Alphabet = UpperChars + LowerChars + DigitsChars;
        private readonly Random _random = new ();
        
        public IEnumerable<string> Generate(int maxWordSize, int count)
        {
            return Enumerable
                .Range(1, count)
                .Select(_ => GenerateAvailableWord(maxWordSize));
        }
        private string GenerateAvailableWord(int maxWordSize)
        {
            const int minWordSize = 3;
            var wordSize = _random.Next(minWordSize, maxWordSize);
            var stringChars = new char[wordSize];
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = Alphabet[_random.Next(Alphabet.Length)];
            }

            return new String(stringChars);
        }
    }
}