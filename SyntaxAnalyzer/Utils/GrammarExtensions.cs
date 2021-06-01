using System.Collections.Generic;
using System.Linq;
using SyntaxAnalyzer.Algorithms;
using SyntaxAnalyzer.Models.Grammar;
using SyntaxAnalyzer.Models.Tokens;

namespace SyntaxAnalyzer.Utils
{
    public static class GrammarExtensions
    {
        public static bool HasLambdaRule(this Grammar grammar, NonTerminal nonTerminal)
        {
            return grammar
                .GetRules(nonTerminal)
                .Any(rule => rule.IsLambdaRule());
        }
        
        public static HashSet<NonTerminal> GetAllNonTerminals(this Grammar grammar)
        {
            return grammar.GetAllTokensOfType<NonTerminal>();
        }

        public static HashSet<Terminal> GetAllTerminals(this Grammar grammar)
        {
            return grammar.GetAllTokensOfType<Terminal>();
        }

        private static HashSet<T> GetAllTokensOfType<T>(this Grammar grammar)
        {
            return grammar
                .GetAllTokens()
                .Where(token => token is T)
                .Cast<T>()
                .ToHashSet();
        }

        public static HashSet<Token> GetAllTokens(this Grammar grammar)
        {
            return grammar
                .GetAllRules()
                .SelectMany(rule => rule.RuleTokens.Union(new[] {rule.Source}))
                .ToHashSet();
        }

        public static HashSet<Rule> GetAllLambdaRules(this Grammar grammar)
        {
            return grammar
                .GetAllRules()
                .Where(rule => rule.IsLambdaRule())
                .ToHashSet();
        }

        public static void AddRules(this Grammar grammar, HashSet<Rule> rules)
        {
            foreach (var rule in rules)
            {
                grammar.AddRule(rule.Source, rule);
            }
        }

        public static Dictionary<NonTerminal, HashSet<Rule>> GetAllTransitions(this Grammar grammar)
        {
            var transitions = new Dictionary<NonTerminal, HashSet<Rule>>();
            
            foreach (var rule in grammar.GetAllRules())
            {
                if (!transitions.ContainsKey(rule.Source))
                    transitions[rule.Source] = new HashSet<Rule>();
                transitions[rule.Source].Add(rule);
            }

            return transitions;
        }

        public static void AddGrammarPart(this Grammar grammar, 
            Dictionary<NonTerminal, HashSet<Rule>> grammarPart)
        {
            foreach (var pair in grammarPart)
            {
                grammar.AddRules(pair.Key, pair.Value);
            }
        }

        public static bool IsLl1(this Grammar grammar)
        {
            return Ll1Grammar.IsLl1Grammar(grammar);
        }

        public static Token GetToken(this Grammar grammar, string value)
        {
            if (value == "End")
                return Terminal.End;
            if (value == "Lambda")
                return Terminal.Lambda;
            return grammar
                .GetAllTokens()
                .First(token => token.Value == value);
        }
    }
}