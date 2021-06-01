using System;
using System.Collections.Generic;
using System.Linq;
using SyntaxAnalyzer.Models.Tokens;

namespace SyntaxAnalyzer.Models.Grammar
{
    public class Rule
    {
        public Rule(NonTerminal source, List<Token> tokens)
        {
            Source = source;
            RuleTokens = tokens;
        }

        public NonTerminal Source { get; }

        public List<Token> RuleTokens { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is Rule)) return false;

            var item = (Rule) obj;

            if (RuleTokens.Count != item.RuleTokens.Count) return false;

            if (RuleTokens.Where((t, i) => !t.Equals(item.RuleTokens[i])).Any()) return false;

            return Source.Equals(item.Source);
        }

        public override int GetHashCode()
        {
            var rulesHashCode = 0;
            var module = (int) (Math.Pow(10, 9) + 7);
            foreach (var ruleToken in RuleTokens)
            {
                rulesHashCode += ruleToken.GetHashCode();
                rulesHashCode %= module;
            }

            return Source.GetHashCode() * 257 +
                   rulesHashCode * 643;
        }
    }
}