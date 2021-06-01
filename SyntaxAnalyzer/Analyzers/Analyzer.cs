using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SyntaxAnalyzer.Models.Grammar;
using SyntaxAnalyzer.Models.Tokens;

namespace SyntaxAnalyzer.Analyzers
{
    public class Analyzer
    {
        private readonly SyntaxAnalyzer _syntaxAnalyzer;
        private readonly LexicalAnalyzer _lexicalAnalyzer;
        public Analyzer(
            Grammar grammar, 
            Dictionary<Terminal, Regex> patterns, 
            Dictionary<Terminal, int> priority)
        {
            _lexicalAnalyzer = new LexicalAnalyzer(patterns, priority);
            _syntaxAnalyzer = new SyntaxAnalyzer(grammar);
        }
        public bool IsAccepted(string text)
        {
            try
            {
                var terminals = _lexicalAnalyzer.ToTerminals(text).ToList();
                return _syntaxAnalyzer.IsAccepted(terminals);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}