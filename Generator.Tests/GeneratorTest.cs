using System.Linq;
using NUnit.Framework;

namespace Generator.Tests
{
    public class GeneratorTest
    {
        [Test]
        public void GenerateAvailableWords()
        {
            var words = new DictionaryGenerator().Generate(10, 10).ToArray();
            Assert.AreEqual(10, words.Length);
            Assert.True(words.All(e => e.Length is <= 10 and >= 3));
        }
    }
}