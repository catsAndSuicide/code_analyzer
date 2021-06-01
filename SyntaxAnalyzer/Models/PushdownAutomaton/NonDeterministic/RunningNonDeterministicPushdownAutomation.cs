using System.Collections.Generic;
using System.Linq;
using SyntaxAnalyzer.Models.PushdownAutomaton.State;
using SyntaxAnalyzer.Models.Tokens;

namespace SyntaxAnalyzer.Models.PushdownAutomaton.NonDeterministic
{
    public class RunningNonDeterministicPushdownAutomaton<TStackAlphabet> : IRunningPushdownAutomation
    {

        private readonly Dictionary<StartState<TStackAlphabet>,
            List<FinishState<TStackAlphabet>>> _transitions;

        private int _lookahead;

        public RunningNonDeterministicPushdownAutomaton(
            Dictionary<StartState<TStackAlphabet>, List<FinishState<TStackAlphabet>>> transitions,
            List<Terminal> input, Stack<TStackAlphabet> stack)
        {
            _transitions = transitions;
            CurrentStack = stack;
            CurrentInput = input;
            CurrentAutomatonState = AutomatonState.Running;
            History = new Stack<Node<TStackAlphabet>>();
        }

        private AutomatonState CurrentAutomatonState { get; set; }

        private Stack<TStackAlphabet> CurrentStack { get; set; }

        private List<Terminal> CurrentInput { get; }

        private Stack<Node<TStackAlphabet>> History { get; }

        public bool IsAcceptableInput()
        {
            while (CurrentAutomatonState == AutomatonState.Running) TryNext();
            return CurrentAutomatonState == AutomatonState.Accept;
        }

        private void TryNext()
        {
            if (CurrentStack.Count == 0)
            {
                CurrentAutomatonState = AutomatonState.Error;
                return;
            }

            var stackTop = CurrentStack.Pop();
            var inputTop = CurrentInput[_lookahead];
            var startState = new StartState<TStackAlphabet>(inputTop, stackTop);
            if (_transitions.ContainsKey(startState))
            {
                if (_transitions[startState].Count > 1) SaveToHistory(stackTop, 0);
                ApplyTransition(_transitions[startState][0]);
            }
            else
            {
                while (true)
                {
                    if (!History.Any())
                    {
                        CurrentAutomatonState = AutomatonState.Error;
                        return;
                    }

                    var node = History.Peek();
                    _lookahead = node.OldLookahead;
                    CurrentStack = new Stack<TStackAlphabet>(node.OldStack.Reverse());
                    stackTop = node.Symbol;
                    inputTop = CurrentInput[_lookahead];
                    startState = new StartState<TStackAlphabet>(inputTop, stackTop);
                    if (node.LastRuleIndex < _transitions[startState].Count - 1)
                    {
                        node.Increment();
                        ApplyTransition(_transitions[startState][node.LastRuleIndex]);
                        break;
                    }

                    History.Pop();
                }
            }
        }

        private void SaveToHistory(TStackAlphabet symbol, int lastTransitionIndex)
        {
            var oldStack = new TStackAlphabet[CurrentStack.Count];
            CurrentStack.CopyTo(oldStack, 0);
            var node = new Node<TStackAlphabet>(symbol, _lookahead, oldStack, lastTransitionIndex);
            History.Push(node);
        }

        private void ApplyTransition(FinishState<TStackAlphabet> goTo)
        {
            if (goTo.IsAcceptState)
            {
                CurrentAutomatonState = AutomatonState.Accept;
                return;
            }

            if (goTo.WithShift) _lookahead++;

            for (var i = goTo.TopStackSymbols.Count - 1; i >= 0; --i)
                if (!Equals(goTo.TopStackSymbols[i], Terminal.Lambda))
                    CurrentStack.Push(goTo.TopStackSymbols[i]);
        }
    }
}