using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SyntaxAnalyzer.Models.Tokens;

namespace SyntaxAnalyzer.Analyzers
{
    public class LexicalAnalyzer
    {
        private readonly Dictionary<Terminal, Regex> _patterns;
        private readonly Dictionary<Terminal, int> _priority;
        public LexicalAnalyzer(Dictionary<Terminal, Regex> patterns, Dictionary<Terminal, int> priority)
        {
            _patterns = patterns;
            _priority = priority;
        }

        public IEnumerable<Terminal> ToTerminals(string text)
        {
            for (var i = 0; i < text.Length; i++)
            {
                if (char.IsWhiteSpace(text[i]) || Environment.NewLine.Contains(text[i]))
                    continue;
                var matches = _patterns
                    .Select(pair => Tuple.Create(pair.Key, pair.Value.Match(text, i)))
                    .Where(pair => pair.Item2.Success && pair.Item2.Index == i)
                    .ToList();
                if (!matches.Any())
                    throw new Exception();
                var matchPair =  matches
                    .GroupBy(pair => pair.Item2.Length)
                    .OrderBy(group => group
                        .OrderByDescending(pair => _priority[pair.Item1]))
                    .First().First();
                i += matchPair.Item2.Length - 1;
                yield return matchPair.Item1;
            }
        }
    }
}