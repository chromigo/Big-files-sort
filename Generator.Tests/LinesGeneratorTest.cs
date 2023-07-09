using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Generator.Tests
{
    public class LinesGeneratorTest
    {
        private Regex lineRegex = new Regex(@"\d+\. [a-zA-Z0-9]");
        
        [Test]
        public void GenerateLines()
        {
            var availableWords = new DictionaryGenerator().Generate(10, 10).ToArray();
            var lines = new LinesGenerator().GenerateLines(10, availableWords).ToArray();
            Assert.AreEqual(10, lines.Length);
            Assert.True(lines.All(e => lineRegex.IsMatch(e)));
        }
    }
}