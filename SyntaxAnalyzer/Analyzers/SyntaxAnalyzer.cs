using System.Collections.Generic;
using SyntaxAnalyzer.Algorithms;
using SyntaxAnalyzer.Models.Grammar;
using SyntaxAnalyzer.Models.PushdownAutomaton.Deterministic;
using SyntaxAnalyzer.Models.PushdownAutomaton.NonDeterministic;
using SyntaxAnalyzer.Models.Tokens;
using SyntaxAnalyzer.Utils;

namespace SyntaxAnalyzer.Analyzers
{
    public class SyntaxAnalyzer
    {
        private readonly PushdownAutomationAnalyzer _analyzer;
        
        public SyntaxAnalyzer(Grammar grammar)
        {
            if (!grammar.IsLl1())
                grammar = LeftRecursionEliminator.Eliminate(LambdaFreeGrammarBuilder.Build(grammar));
            _analyzer = grammar.IsLl1()
                ? new PushdownAutomationAnalyzer(
                    grammar,
                    new DeterministicPushdownAutomaton<Token>())
                : new PushdownAutomationAnalyzer(
                    grammar,
                    new NonDeterministicPushdownAutomaton<Token>());
        }
        
        public bool IsAccepted(IEnumerable<Terminal> tokens)
        {
            return _analyzer.IsAccepted(tokens);
        }
    }
}