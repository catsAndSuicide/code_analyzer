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
    public class FollowSetBuilderTests
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

        [TestCase("E", "End", "+", ")")]
        [TestCase("T", "End", "+", ")", "*")]
        [TestCase("F", "End", "+", ")", "*")]
        public void FirstSetBuilder_ShouldBuildFirstSet_ForArithmeticGrammar(
            string nonTerminalValue,
            params string[] expectedValues)
        {
            CheckFollowSet(_arithmeticGrammar, nonTerminalValue, expectedValues);
        }
        
        [TestCase("S", "End")]
        [TestCase("N", "End", ",")]
        [TestCase("P", "End", ",", "1")]
        [TestCase("M", "End")]
        public void FirstSetBuilder_ShouldBuildFirstSet_ForFBNGrammar(
            string nonTerminalValue,
            params string[] expectedValues)
        {
            CheckFollowSet(_fbnGrammar, nonTerminalValue, expectedValues);
        }

        private void CheckFollowSet(Grammar grammar, string sourceValue, string[] expectedValues)
        {
            var followSetBuilder = new FollowSetBuilder(grammar);
            var nonTerminal = new NonTerminal(sourceValue);
            var expected = expectedValues
                .Select(grammar.GetToken)
                .ToHashSet();

            var actual = followSetBuilder.GetFollowSet(nonTerminal).ToHashSet();

            actual.Should().BeEquivalentTo(expected);
        }
    }
}