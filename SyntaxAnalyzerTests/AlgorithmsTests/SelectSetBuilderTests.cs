using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SyntaxAnalyzer.Algorithms;
using SyntaxAnalyzer.Models.Grammar;
using SyntaxAnalyzer.Models.Tokens;
using SyntaxAnalyzer.Utils;

namespace SyntaxAnalyzerTests.AlgorithmsTests
{
    public class SelectSetBuilderTests
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

        [TestCase("E", new[] {"E", "+", "T"}, "(", "x")]
        [TestCase("E", new[] {"T"}, "(", "x")]
        [TestCase("F", new[] {"(", "E", ")"}, "(")]
        public void FirstSetBuilder_ShouldBuildFirstSet_ForArithmeticGrammar(
            string sourceValue,
            string[] destValues,
            params string[] expectedValues)
        {
            CheckSelectSet(_arithmeticGrammar, sourceValue, destValues, expectedValues);
        }
        
        [TestCase("S", new[] {"N", ",", "M"}, "0", "1")]
        [TestCase("N", new[] {"1", "P"}, "1")]
        [TestCase("P", new[] {"Lambda"}, ",", "1", "End")]
        public void FirstSetBuilder_ShouldBuildFirstSet_ForFBNGrammar(
            string sourceValue,
            string[] destValues,
            params string[] expectedValues)
        {
            CheckSelectSet(_fbnGrammar, sourceValue, destValues, expectedValues);
        }

        private void CheckSelectSet(Grammar grammar, string sourceValue, string[] destValues, 
            string[] expectedValues)
        {
            var selectSetBuilder = new SelectSetBuilder(grammar);
            var source = new NonTerminal(sourceValue);
            var ruleTokens = destValues.Select(grammar.GetToken).ToList();
            var rule = new Rule(source, ruleTokens);
            var expected = expectedValues
                .Select(grammar.GetToken)
                .ToHashSet();

            var actual = selectSetBuilder.GetSelectSet(rule);

            actual.Should().BeEquivalentTo(expected);
        }
    }
}