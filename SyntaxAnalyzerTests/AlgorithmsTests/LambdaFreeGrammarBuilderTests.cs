using System.IO;
using FluentAssertions;
using NUnit.Framework;
using SyntaxAnalyzer.Algorithms;
using SyntaxAnalyzer.Models.Grammar;
using SyntaxAnalyzer.Utils;

namespace SyntaxAnalyzerTests.AlgorithmsTests
{
    public class LambdaFreeGrammarBuilderTests
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
        
        [Test]
        public void IsLambdaGrammar_ShouldReturnTrue_ForArithmeticGrammar()
        {
            LambdaFreeGrammarBuilder
                .IsLambdaFreeGrammar(_arithmeticGrammar)
                .Should()
                .BeTrue();
        }
        
        [Test]
        public void IsLambdaGrammar_ShouldReturnFalse_ForFBNGrammarGrammar()
        {
            LambdaFreeGrammarBuilder
                .IsLambdaFreeGrammar(_fbnGrammar)
                .Should()
                .BeFalse();
        }

        [Test]
        public void LambdaFreeGrammarBuilder_ShouldReturnSameGrammar_ForLambdaFreeGrammar()
        {
            LambdaFreeGrammarBuilder
                .Build(_arithmeticGrammar)
                .Should()
                .BeEquivalentTo(_arithmeticGrammar);
        }
        
        [Test]
        public void LambdaFreeGrammarBuilder_ShouldReturnLambdaFreeGrammar_ForNotLambdaFreeGrammar()
        {
            var lambdaFreeGrammar = LambdaFreeGrammarBuilder.Build(_fbnGrammar);
            
            LambdaFreeGrammarBuilder
                .IsLambdaFreeGrammar(lambdaFreeGrammar)
                .Should()
                .BeTrue();

        }
    }
}