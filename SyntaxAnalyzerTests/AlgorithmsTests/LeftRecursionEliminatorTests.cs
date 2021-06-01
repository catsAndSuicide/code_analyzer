using System.IO;
using FluentAssertions;
using NUnit.Framework;
using SyntaxAnalyzer.Algorithms;
using SyntaxAnalyzer.Models.Grammar;
using SyntaxAnalyzer.Utils;

namespace SyntaxAnalyzerTests.AlgorithmsTests
{
    public class LeftRecursionEliminatorTests
    {
        private readonly Grammar _arithmeticGrammarWithLeftRecursion = File
            .ReadAllText(Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "..",
                "Files",
                "NoLL1ArithmeticGrammar.txt"))
            .ToGrammar();

        private readonly Grammar _arithmeticGrammarWithNoLeftRecursion = File
            .ReadAllText(Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "..",
                "Files",
                "LL1ArithmeticGrammar.txt"))
            .ToGrammar();

        [Test]
        public void LeftRecursionEliminator_ShouldEliminateLeftRecursion_InArithmeticGrammar()
        {
            LeftRecursionEliminator
                .Eliminate(_arithmeticGrammarWithLeftRecursion)
                .Should()
                .BeEquivalentTo(_arithmeticGrammarWithNoLeftRecursion);
        }
    }
}