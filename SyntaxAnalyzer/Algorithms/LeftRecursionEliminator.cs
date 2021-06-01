using System.Collections.Generic;
using System.Linq;
using SyntaxAnalyzer.Models.Grammar;
using SyntaxAnalyzer.Models.Tokens;
using SyntaxAnalyzer.Utils;

namespace SyntaxAnalyzer.Algorithms
{
    public static class LeftRecursionEliminator
    {
        public static Grammar Eliminate(Grammar recursiveGrammar)
        {
            var transitions = recursiveGrammar.GetAllTransitions();
            var visited = new HashSet<NonTerminal>();
            
            foreach (var source in recursiveGrammar.GetAllNonTerminals())
            {
                foreach (var rule in transitions[source].ToHashSet())
                {
                    if (!(rule.RuleTokens[0] is NonTerminal nonTerminal) 
                        || !visited.Contains(nonTerminal)) 
                        continue;
                    transitions[source].Remove(rule);
                    foreach (var currentRule in transitions[nonTerminal].ToHashSet())
                        transitions[source].Add(new Rule(
                            source, 
                            currentRule.RuleTokens.Concat(rule.RuleTokens.Skip(1))
                                .ToList()));
                }

                EliminateDirectLeftRecursion(transitions, source);
                visited.Add(source);
            }

            var grammar = new Grammar(recursiveGrammar.Axiom);
            grammar.AddGrammarPart(transitions);
            return grammar;
        }

        private static void EliminateDirectLeftRecursion(
            Dictionary<NonTerminal, HashSet<Rule>> transitions, 
            NonTerminal nonTerminal)
        {
            if (!transitions.Values.SelectMany(rules => rules).HasDirectLeftRecursion())
                return;
            var alpha = new List<List<Token>>();
            var beta = new List<List<Token>>();
            
            foreach (var rule in transitions[nonTerminal].ToHashSet())
            {
                if (rule.HasDirectLeftRecursion())
                    alpha.Add(rule.RuleTokens.Skip(1).ToList());
                else
                    beta.Add(rule.RuleTokens);

                transitions[nonTerminal].Remove(rule);
            }

            var newNonTerminal = new NonTerminal(nonTerminal.Value + "#");
            transitions[newNonTerminal] = new HashSet<Rule>();
            
            foreach (var tokens in beta)
                transitions[nonTerminal].Add(new Rule(
                    nonTerminal, 
                    tokens.Concat(new List<Token> {newNonTerminal}).ToList()));

            foreach (var tokens in alpha)
                transitions[newNonTerminal].Add(new Rule(
                    newNonTerminal, 
                    tokens.Concat(new List<Token> {newNonTerminal}).ToList()));

            transitions[newNonTerminal].Add(new Rule(
                newNonTerminal, 
                new List<Token> {Terminal.Lambda}));
        }
    }
}