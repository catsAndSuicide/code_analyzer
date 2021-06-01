using System.Collections.Generic;
using SyntaxAnalyzer.Models.PushdownAutomaton.State;
using SyntaxAnalyzer.Models.Tokens;

namespace SyntaxAnalyzer.Models.PushdownAutomaton.Deterministic
{
    public class RunningDeterministicPushdownAutomaton<TStackAlphabet> : IRunningPushdownAutomation
    {
        public RunningDeterministicPushdownAutomaton(
            Dictionary<StartState<TStackAlphabet>, FinishState<TStackAlphabet>> transitions,
            Queue<Terminal> input, 
            Stack<TStackAlphabet> stack)
        {
            _transitions = transitions;
            CurrentStack = stack;
            CurrentInput = input;
            CurrentAutomatonState = AutomatonState.Running;
        }

        public bool IsAcceptableInput()
        {
            while (CurrentAutomatonState == AutomatonState.Running)
                TryNext();
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
            var inputTop = CurrentInput.Peek();
            var startState = new StartState<TStackAlphabet>(inputTop, stackTop);

            if (_transitions.TryGetValue(startState, out var goTo))
            {
                if (goTo.IsAcceptState)
                {
                    CurrentAutomatonState = AutomatonState.Accept;
                    return;
                }
                ApplyTransition(goTo);
            }
            else
                CurrentAutomatonState = AutomatonState.Error;
        }

        private void ApplyTransition(FinishState<TStackAlphabet> goTo)
        {
            if (goTo.WithShift)
                CurrentInput.Dequeue();

            for (var i = goTo.TopStackSymbols.Count - 1; i >= 0; --i)
            {
                if (!Equals(goTo.TopStackSymbols[i], Terminal.Lambda))
                    CurrentStack.Push(goTo.TopStackSymbols[i]);
            }
        }

        private readonly Dictionary<StartState<TStackAlphabet>, 
            FinishState<TStackAlphabet>> _transitions;

        private AutomatonState CurrentAutomatonState { get; set; }

        private Stack<TStackAlphabet> CurrentStack { get; }

        private Queue<Terminal> CurrentInput { get; }
    }
}