using System.Collections.Generic;
using System.Linq;
using SyntaxAnalyzer.Models.Grammar;
using SyntaxAnalyzer.Models.Tokens;
using SyntaxAnalyzer.Utils;

namespace SyntaxAnalyzer.Algorithms
{
    public class SelectSetBuilder
    {
        private readonly FirstSetBuilder _firstSetAlgorithm;
        private readonly FollowSetBuilder _followSetBuilderAlgorithm;

        public SelectSetBuilder(Grammar grammar)
        {
            _firstSetAlgorithm = new FirstSetBuilder(grammar);
            _followSetBuilderAlgorithm = new FollowSetBuilder(grammar);
        }

        public HashSet<Terminal> GetSelectSet(Rule rule)
        {
            var firstSet = _firstSetAlgorithm.GetFirstSet(rule.RuleTokens);
            if (!firstSet.Contains(Terminal.Lambda))
                return firstSet;
            var followSet = _followSetBuilderAlgorithm.GetFollowSet(rule.Source);
            return firstSet
                .WithNoLambda()
                .Union(followSet)
                .ToHashSet();
        }
    }
}