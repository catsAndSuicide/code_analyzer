using System.Collections.Generic;
using System.Linq;
using SyntaxAnalyzer.Models.Grammar;
using SyntaxAnalyzer.Models.Tokens;
using SyntaxAnalyzer.Utils;

namespace SyntaxAnalyzer.Algorithms
{
    public class FollowSetBuilder
    {
        private readonly Dictionary<NonTerminal, HashSet<Terminal>> _followSets;
        private readonly Grammar _grammar;

        public FollowSetBuilder(Grammar grammar)
        {
            _grammar = grammar;
            _followSets = new Dictionary<NonTerminal, HashSet<Terminal>>();
            BuildFollowSets();
        }

        public IEnumerable<Terminal> GetFollowSet(NonTerminal token)
        {
            return _followSets[token];
        }

        private void BuildFollowSets()
        {
            foreach (var nonTerminal in _grammar.GetAllNonTerminals()) 
                _followSets[nonTerminal] = new HashSet<Terminal>();

            var firstSetAlgorithm = new FirstSetBuilder(_grammar);
            _followSets[_grammar.Axiom].Add(Terminal.End);
            var stabilized = false;
            while (!stabilized)
            {
                stabilized = true;
                foreach (var rule in _grammar.GetAllRules().WithNoLambdaRule())
                {
                    var ann = true;
                    for (var i = rule.RuleTokens.Count - 1; i >= 0; i--)
                    {
                        var token = rule.RuleTokens[i];
                        if (token is NonTerminal nonTerminal)
                        {
                            var previousCount = _followSets[nonTerminal].Count;
                            _followSets[nonTerminal]
                                .UnionWith(firstSetAlgorithm
                                    .GetFirstSet(rule.RuleTokens.Skip(i + 1).ToList())
                                    .WithNoLambda());
                            if (ann)
                                _followSets[nonTerminal]
                                    .UnionWith(_followSets[rule.Source]
                                    .WithNoLambda());
                            if (_followSets[nonTerminal].Count > previousCount)
                                stabilized = false;
                        }

                        if (!firstSetAlgorithm.GetFirstSet(token).Contains(Terminal.Lambda))
                            ann = false;
                    }
                }
            }
        }
    }
}