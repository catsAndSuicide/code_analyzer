using System.Collections.Generic;
using SyntaxAnalyzer.Models.Grammar;
using SyntaxAnalyzer.Models.Tokens;
using SyntaxAnalyzer.Utils;

namespace SyntaxAnalyzer.Algorithms
{
    public class FirstSetBuilder
    {
        private readonly Dictionary<Token, HashSet<Terminal>> _firstSets;
        private readonly Grammar _grammar;

        public FirstSetBuilder(Grammar grammar)
        {
            _grammar = grammar;
            _firstSets = new Dictionary<Token, HashSet<Terminal>>();
            BuildFirstSetsForTokens();
        }
        
        public HashSet<Terminal> GetFirstSet(Token token)
        {
            return _firstSets[token];
        }

        public HashSet<Terminal> GetFirstSet(List<Token> tokens)
        {
            if (tokens.Count == 1)
                return GetFirstSet(tokens[0]);
            
            var firstSet = new HashSet<Terminal>();
            for (var i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                firstSet.UnionWith(_firstSets[token].WithNoLambda());
                if (token is Terminal || !_firstSets[token].Contains(Terminal.Lambda))
                    break;

                if (i == tokens.Count - 1)
                    firstSet.Add(Terminal.Lambda);
            }

            return firstSet;
        }

        private void BuildFirstSetsForTokens()
        {
            InitializeFirstSets();
            var stabilized = false;
            while (!stabilized)
            {
                stabilized = true;
                foreach (var rule in _grammar.GetAllRules().WithNoLambdaRule())
                {
                    var previousCount = _firstSets[rule.Source].Count;
                    for (var i = 0; i < rule.RuleTokens.Count; i++)
                    {
                        var destToken = rule.RuleTokens[i];
                        _firstSets[rule.Source].UnionWith(_firstSets[destToken].WithNoLambda());
                        if (!_firstSets[destToken].Contains(Terminal.Lambda)) break;

                        if (i == rule.RuleTokens.Count - 1)
                            _firstSets[rule.Source].Add(Terminal.Lambda);
                    }

                    if (_firstSets[rule.Source].Count > previousCount)
                        stabilized = false;
                }
            }
        }

        private void InitializeFirstSets()
        {
            foreach (var token in _grammar.GetAllTokens())
            {
                _firstSets[token] = new HashSet<Terminal>();
                if (token is Terminal terminal)
                    _firstSets[terminal].Add(terminal);
                else if (_grammar.HasLambdaRule(token as NonTerminal)) 
                    _firstSets[token].Add(Terminal.Lambda);
            }
        }
    }
}