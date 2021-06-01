using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SyntaxAnalyzer.Analyzers;
using SyntaxAnalyzer.Models.Grammar;
using SyntaxAnalyzer.Models.Tokens;
using SyntaxAnalyzer.Utils;

namespace SyntaxAnalyzer
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var pathToGrammar = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "..",
                "Files",
                "InterfaceGrammar.txt");
            var pathToPatterns = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "..",
                "Files",
                "Patterns.txt");
            var pathToFile = args[0];
            var grammar = ParseFileToGrammar(pathToGrammar);
            var (patterns, priority) = ParseFileToPatterns(pathToPatterns);
            var analyzer = new Analyzer(grammar, patterns, priority);
            var code = File.ReadAllText(pathToFile);
            Console.WriteLine(analyzer.IsAccepted(code) ? "IsAccepted" : "IsNotAccepted");
        }

        private static Grammar ParseFileToGrammar(string path)
        {
            return File.ReadAllText(path).ToGrammar();
        }
        
        private static (Dictionary<Terminal, Regex> Patterns, 
            Dictionary<Terminal, int> Priority) ParseFileToPatterns(string path)
        {
            var lines = File.ReadLines(path).ToArray();
            var patterns = new Dictionary<Terminal, Regex>();
            var priority = new Dictionary<Terminal, int>();
            for (var i = 0; i < lines.Length; i++)
            {
                var splitLine = lines[i].Split('=');
                var terminalValue = splitLine[0].Substring(0, splitLine[0].Length - 1);
                var terminal = new Terminal(terminalValue);
                var pattern = splitLine[1].Substring(1);
                var regex = new Regex(pattern);
                patterns[terminal] = regex;
                priority[terminal] = i;
            }

            return (patterns, priority);
        }
    }
}