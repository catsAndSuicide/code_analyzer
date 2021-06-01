using System.Linq;
using SyntaxAnalyzer.Models.Grammar;
using SyntaxAnalyzer.Models.Tokens;

namespace SyntaxAnalyzer.Utils
{
    public static class RuleExtensions
    {
        public static bool IsLambdaRule(this Rule rule)
        {
            return rule.RuleTokens.Count == 1 && rule.RuleTokens.Contains(Terminal.Lambda);
        }
        
        public static bool IsRecursionRule(this Rule rule)
        {
            return rule.RuleTokens.Count == 1 && rule.RuleTokens.Contains(rule.Source);
        }
        
        public static Rule RemoveToken(this Rule rule, int index)
        {
            rule.RuleTokens.RemoveAt(index);
            return rule;
        }

        public static Rule DeepCopy(this Rule rule)
        {
            return new Rule(rule.Source, rule.RuleTokens.ToList());
        }

        public static bool HasDirectLeftRecursion(this Rule rule)
        {
            return rule.RuleTokens[0].Equals(rule.Source);
        }
    }
}