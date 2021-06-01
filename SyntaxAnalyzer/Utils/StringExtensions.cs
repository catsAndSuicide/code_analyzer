using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using SyntaxAnalyzer.Models.Grammar;
using SyntaxAnalyzer.Models.Tokens;

namespace SyntaxAnalyzer.Utils
{
    public static class StringExtensions
    {
        public static Grammar ToGrammar(this string input)
        {
            var grammar = new Grammar();
            var items = new Dictionary<NonTerminal, HashSet<Rule>>();
            var itemPairs = new Dictionary<string, string>();
            var axiom = "";
            foreach (var line in input.Split(new[] {Environment.NewLine}, StringSplitOptions.None))
            {
                var splitLine = line.Split(new[] {"->"}, StringSplitOptions.None);
                var nonTerminalValue = splitLine[0]
                    .Split(' ')
                    .Single(str => str.Length > 0);
                itemPairs[nonTerminalValue] = splitLine[1];
                if (axiom == "")
                    axiom = nonTerminalValue;
            }

            foreach (var pair in itemPairs)
            {
                var strItems = pair.Value.Split('|');
                var source = new NonTerminal(pair.Key);
                items[source] = strItems
                    .Select(str => str
                        .Split(' ')
                        .Where(val => val.Length > 0)
                        .Select(val => itemPairs.ContainsKey(val) ? (Token) new NonTerminal(val) : new Terminal(val))
                        .ToList()
                    )
                    .Select(list => new Rule(source, list))
                    .ToHashSet();
            }
            
            foreach (var pair in items) grammar.AddRules(pair.Key, pair.Value);
            grammar.Axiom = new NonTerminal(axiom);
            return grammar;
        }
    }
    
}