using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SyntaxAnalyzer.Models.Grammar;
using SyntaxAnalyzer.Models.Tokens;
using SyntaxAnalyzer.Utils;

namespace SyntaxAnalyzerTests.AnalyzersTests
{
    public class SyntaxAnalyzerTests
    {
        private readonly Grammar _noLl1ArithmeticGrammar = File
            .ReadAllText(Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "..",
                "Files",
                "NoLL1ArithmeticGrammar.txt"))
            .ToGrammar();

        private readonly Grammar _ll1ArithmeticGrammar = File
            .ReadAllText(Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "..",
                "Files",
                "LL1ArithmeticGrammar.txt"))
            .ToGrammar();
        
        private readonly Grammar _fbnGrammar = File
            .ReadAllText(Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "..",
                "Files",
                "FloatBinaryNumbersGrammar.txt"))
            .ToGrammar();

        [TestCase("x")]
        [TestCase("x", "+", "x")]
        [TestCase("(", "x", "*", "x", ")", "+", "x")]
        [TestCase("x", "*", "(", "x", "+", "x", ")")]
        public void SyntaxAnalyzer_ShouldAcceptWord_IfArithmeticLanguageContainsWordAndGrammarIsLL1(params string[] word)
        {
            CheckAnalyzerAcceptWord(_ll1ArithmeticGrammar, word, true);
        }
        
        [TestCase("x")]
        [TestCase("x", "+", "x")]
        [TestCase("(", "x", "*", "x", ")", "+", "x")]
        [TestCase("x", "*", "(", "x", "+", "x", ")")]
        public void SyntaxAnalyzer_ShouldAcceptWord_IfArithmeticLanguageContainsWordAndGrammarIsNotLL1(params string[] word)
        {
            CheckAnalyzerAcceptWord(_noLl1ArithmeticGrammar, word, true);
        }
        
        [TestCase("x", ")")]
        [TestCase("+", "x")]
        [TestCase("(", "x", "*", "x", "+", "x")]
        [TestCase("x", "*", "(", "x", "+", "x", ")", "x")]
        public void SyntaxAnalyzer_ShouldNotAcceptWord_IfArithmeticLanguageDoesNotContainWordAndGrammarIsLL1(params string[] word)
        {
            CheckAnalyzerAcceptWord(_ll1ArithmeticGrammar, word, false);
        }
        
        [TestCase("x", ")")]
        [TestCase("+", "x")]
        [TestCase("(", "x", "*", "x", "+", "x")]
        [TestCase("x", "*", "(", "x", "+", "x", ")", "x")]
        public void SyntaxAnalyzer_ShouldNotAcceptWord_IfArithmeticLanguageDoesNotContainWordAndGrammarIsNotLL1(params string[] word)
        {
            CheckAnalyzerAcceptWord(_noLl1ArithmeticGrammar, word, false);
        }
        
        [TestCase("0")]
        [TestCase("0", ",", "1")]
        [TestCase("1", "0", "0", "1")]
        [TestCase("1", "0", ",", "1", "0", "0", "1")]
        public void SyntaxAnalyzer_ShouldAcceptWord_IfFBNLanguageContainsWord(params string[] word)
        {
            CheckAnalyzerAcceptWord(_fbnGrammar, word, true);
        }
        
        [TestCase("0", "1")]
        [TestCase("0", "0", ",", "1")]
        [TestCase("1", ",", "0")]
        [TestCase("1", "0", ",", "0", ",", "0", "1")]
        public void SyntaxAnalyzer_ShouldNotAcceptWord_IfFBNLanguageDoesNotContainWord(params string[] word)
        {
            CheckAnalyzerAcceptWord(_fbnGrammar, word, false);
        }

        private static void CheckAnalyzerAcceptWord(Grammar grammar, IEnumerable<string> word, bool expected)
        {
            var analyzer = new SyntaxAnalyzer.Analyzers.SyntaxAnalyzer(grammar);
            var tokens = word
                .Select(grammar.GetToken)
                .Cast<Terminal>();

            var actual = analyzer.IsAccepted(tokens);

            actual.Should().Be(expected);
        }
    }
}