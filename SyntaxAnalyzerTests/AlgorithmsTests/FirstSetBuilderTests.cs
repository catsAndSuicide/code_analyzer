using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SyntaxAnalyzer.Algorithms;
using SyntaxAnalyzer.Models.Grammar;
using SyntaxAnalyzer.Utils;

namespace SyntaxAnalyzerTests.AlgorithmsTests
{
    [TestFixture]
    public class FirstSetBuilderTests
    {
        private readonly Grammar _arithmeticGrammar = File
            .ReadAllText(Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "..",
                "Files",
                "NoLL1ArithmeticGrammar.txt"))
            .ToGrammar();

        private readonly Grammar _fbnGrammar = File
            .ReadAllText(Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "..",
                "Files",
                "FloatBinaryNumbersGrammar.txt"))
            .ToGrammar();

        [TestCase(new[] {"+"}, "+")]
        [TestCase(new[] {"E"}, "(", "x")]
        [TestCase(new[] {"(", "E", ")"}, "(")]
        [TestCase(new[] {"T", "*", "F"}, "(", "x")]
        public void FirstSetBuilder_ShouldBuildFirstSet_ForArithmeticGrammar(
            string[] sourceValues,
            params string[] expectedValues)
        {
            CheckFirstSet(_arithmeticGrammar, sourceValues, expectedValues);
        }
        
        [TestCase(new[] {"0"}, "0")]
        [TestCase(new[] {"N"}, "0", "1")]
        [TestCase(new[] {"P"}, "0", "1", "Lambda")]
        [TestCase(new[] {"N", ",", "M"}, "0", "1")]
        public void FirstSetBuilder_ShouldBuildFirstSet_ForFBNGrammar(
            string[] sourceValues,
            params string[] expectedValues)
        {
            CheckFirstSet(_fbnGrammar, sourceValues, expectedValues);
        }

        private void CheckFirstSet(Grammar grammar, string[] sourceValues, string[] expectedValues)
        {
            var firstSetBuilder = new FirstSetBuilder(grammar);
            var source = sourceValues
                .Select(grammar.GetToken)
                .ToList();
            var expected = expectedValues
                .Select(grammar.GetToken)
                .ToHashSet();

            var actual = firstSetBuilder.GetFirstSet(source);

            actual.Should().BeEquivalentTo(expected);
        }
    }
}