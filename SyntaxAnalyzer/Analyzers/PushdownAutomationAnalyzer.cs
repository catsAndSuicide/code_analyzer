using System.Collections.Generic;
using System.Linq;
using SyntaxAnalyzer.Algorithms;
using SyntaxAnalyzer.Models.Grammar;
using SyntaxAnalyzer.Models.PushdownAutomaton;
using SyntaxAnalyzer.Models.PushdownAutomaton.State;
using SyntaxAnalyzer.Models.Tokens;
using SyntaxAnalyzer.Utils;

namespace SyntaxAnalyzer.Analyzers
{
    public class PushdownAutomationAnalyzer
    {
        private readonly IPushdownAutomation<Token> _automation;
        private readonly Grammar _grammar;
        private readonly SelectSetBuilder _selectSetBuilder;
        
        public PushdownAutomationAnalyzer(Grammar grammar, IPushdownAutomation<Token> automation)
        {
            _grammar = grammar;
            _selectSetBuilder = new SelectSetBuilder(grammar);
            _automation = automation;
            InitializeAutomation(automation);
        }
        
        public bool IsAccepted(IEnumerable<Terminal> tokens)
        {
            return _automation
                .Run(tokens, new List<Token> {_grammar.Axiom, StackEndToken.Instance})
                .IsAcceptableInput();
        }
        
        private void InitializeAutomation(IPushdownAutomation<Token> automation)
        {
            foreach (var terminal in _grammar.GetAllTerminals().WithNoLambda())
            {
                var startState = new StartState<Token>(terminal, terminal);
                var finishState = FinishState<Token>.StateWithShift(new List<Token> {Terminal.Lambda});
                automation.AddTransition(startState, finishState);
            }
            
            foreach (var rule in _grammar.GetAllRules())
            {
                var selectSet = _selectSetBuilder.GetSelectSet(rule);
                foreach (var terminal in selectSet)
                {
                    var startState = new StartState<Token>(terminal, rule.Source);
                    var finishState = FinishState<Token>.StateWithNoShift(rule.RuleTokens);
                    automation.AddTransition(startState, finishState);
                }
            }

            var acceptableState = new StartState<Token>(Terminal.End, StackEndToken.Instance);
            automation.AddAcceptableState(acceptableState);
        }
    }
}