using System.Collections.Generic;
using System.Linq;
using SyntaxAnalyzer.Models.Grammar;
using SyntaxAnalyzer.Models.Tokens;
using SyntaxAnalyzer.Utils;

namespace SyntaxAnalyzer.Algorithms
{
    public static class LambdaFreeGrammarBuilder
    {
        public static bool IsLambdaFreeGrammar(Grammar grammar)
        {
            var lambdaRules = grammar.GetAllLambdaRules();
            if (!lambdaRules.Any())
                return true;
            if (lambdaRules.Count > 1)
                return false;
            if (!lambdaRules.First().Source.Equals(grammar.Axiom))
                return false;
            return !grammar.GetAllRules().SelectMany(rule => rule.RuleTokens).Contains(grammar.Axiom);
        }
        
        public static Grammar Build(Grammar grammar)
        {
            if (IsLambdaFreeGrammar(grammar))
                return grammar;
            var annSet = BuildAnnSet(grammar); 
            var newRules = grammar
                .GetAllRules()
                .SelectMany(rule => GetRulePartition(rule, annSet))
                .WithNoRecursionRule()
                .WithNoLambdaRule()
                .WithOnlyProducingRules()
                .ToHashSet();

            var newGrammar = new Grammar();
            newGrammar.AddRules(newRules);
            
            if (annSet.Contains(grammar.Axiom))
            {
                var axiom = new NonTerminal(grammar.Axiom.Value + "&");
                newGrammar.Axiom = axiom;
                newGrammar.AddRule(axiom, new Rule(axiom, new List<Token> {grammar.Axiom}));
                newGrammar.AddRule(axiom, new Rule(axiom, new List<Token> {Terminal.Lambda}));
            }
            else
                newGrammar.Axiom = grammar.Axiom;
            
            return newGrammar;
        }

        private static HashSet<Rule> GetRulePartition(
            Rule rule,
            HashSet<NonTerminal> annSet,
            HashSet<Rule> partition = null)
        {
            if (partition == null)
                partition = new HashSet<Rule>();

            partition.Add(rule);

            if (rule.RuleTokens.Count == 1)
                return partition;

            for (var i = 0; i < rule.RuleTokens.Count; i++)
                if (rule.RuleTokens[i] is NonTerminal nonTerminal && annSet.Contains(nonTerminal))
                    partition.UnionWith(GetRulePartition(
                        rule.DeepCopy().RemoveToken(i), 
                        annSet, 
                        partition));

            return partition;
        }

        private static HashSet<NonTerminal> BuildAnnSet(Grammar grammar)
        {
            var annSet = grammar
                .GetAllLambdaRules()
                .Select(rule => rule.Source)
                .ToHashSet();

            while (true)
            {
                var newAnnSet = grammar
                    .GetAllRules()
                    .Where(rule => !annSet.Contains(rule.Source))
                    .Where(rule => rule.RuleTokens.All(token => token is NonTerminal nonTerminal
                                                  && annSet.Contains(nonTerminal)))
                    .Select(rule => rule.Source)
                    .ToHashSet();

                if (newAnnSet.Count == 0)
                    return annSet;

                annSet.UnionWith(newAnnSet);
            }
        }
    }
}