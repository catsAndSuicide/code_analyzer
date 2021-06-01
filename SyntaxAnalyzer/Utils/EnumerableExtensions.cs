using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SyntaxAnalyzer.Models.Grammar;
using SyntaxAnalyzer.Models.Tokens;

namespace SyntaxAnalyzer.Utils
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> WithNoLambda<T>(this IEnumerable<T> source) where T : Token
        {
            return source
                .Where(token => !Terminal.Lambda.Equals(token));
        }
        
        public static IEnumerable<Rule> WithNoLambdaRule(this IEnumerable<Rule> source)
        {
            return source
                .Where(rule => !rule.IsLambdaRule());
        }
        
        public static IEnumerable<Rule> WithNoRecursionRule(this IEnumerable<Rule> source)
        {
            return source
                .Where(rule => !rule.IsRecursionRule());
        }

        public static IEnumerable<Rule> WithOnlyProducingRules(this IEnumerable<Rule> source)
        {
            var rules = source.ToHashSet();
            while (true)
            {
                var previousRulesCount = rules.Count;
                var sources = rules
                    .Select(rule => rule.Source)
                    .ToHashSet();
                rules = rules
                    .Where(rule => rule.RuleTokens
                        .All(token => token is Terminal 
                                      || sources.Contains(token as NonTerminal)))
                    .ToHashSet();
                if (rules.Count == previousRulesCount)
                    return rules;
            }
        }

        public static bool HasDirectLeftRecursion(this IEnumerable<Rule> source)
        {
            return source.Any(rule => rule.HasDirectLeftRecursion());
        }
    }
}