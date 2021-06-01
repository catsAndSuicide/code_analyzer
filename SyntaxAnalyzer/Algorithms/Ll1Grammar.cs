using System.Linq;
using SyntaxAnalyzer.Models.Grammar;
using SyntaxAnalyzer.Utils;

namespace SyntaxAnalyzer.Algorithms
{
    public static class Ll1Grammar
    {
        public static bool IsLl1Grammar(Grammar grammar)
        {
            var selectAlgorithm = new SelectSetBuilder(grammar);
            foreach (var nonTerminal in grammar.GetAllNonTerminals())
            {
                var selectSets = grammar
                    .GetRules(nonTerminal)
                    .Select(rule => selectAlgorithm.GetSelectSet(rule))
                    .ToList();
                for (var i = 0; i < selectSets.Count; i++)
                {
                    for (var j = 0; j < i; j++)
                    {
                        if (selectSets[i].Intersect(selectSets[j]).Any())
                            return false;
                    }
                }
            }

            return true;
        }
    }
}