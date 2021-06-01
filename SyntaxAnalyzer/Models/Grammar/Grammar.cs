using System.Collections.Generic;
using System.Linq;
using SyntaxAnalyzer.Models.Tokens;

namespace SyntaxAnalyzer.Models.Grammar
{
    public class Grammar
    {
        private readonly Dictionary<NonTerminal, HashSet<Rule>> _grammar =
            new Dictionary<NonTerminal, HashSet<Rule>>();

        public Grammar()
        {
        }

        public Grammar(NonTerminal axiom)
        {
            Axiom = axiom;
        }

        public NonTerminal Axiom { get; set; }

        public HashSet<Rule> GetRules(NonTerminal nonTerminal)
        {
            return _grammar[nonTerminal];
        }

        public HashSet<Rule> GetAllRules()
        {
            return _grammar.Values.SelectMany(items => items).ToHashSet();
        }

        public void AddRule(NonTerminal source, Rule rule)
        {
            if (!_grammar.ContainsKey(source)) _grammar[source] = new HashSet<Rule>();
            _grammar[source].Add(rule);
        }

        public void AddRules(NonTerminal source, HashSet<Rule> rules)
        {
            if (!_grammar.ContainsKey(source))
                _grammar[source] = rules;
            else
                _grammar[source].UnionWith(rules);
        }
        
        public override bool Equals(object obj)
        {
            if (!(obj is Grammar)) return false;

            var grammar = (Grammar) obj;

            return Axiom.Equals(grammar.Axiom) && GetAllRules().SetEquals(grammar.GetAllRules());
        }
    }
}