using System.Collections.Generic;
using System.Linq;
using SyntaxAnalyzer.Models.PushdownAutomaton.State;
using SyntaxAnalyzer.Models.Tokens;

namespace SyntaxAnalyzer.Models.PushdownAutomaton.NonDeterministic
{
    public class NonDeterministicPushdownAutomaton<TStackAlphabet> : IPushdownAutomation<TStackAlphabet>
    {
        private readonly Dictionary<StartState<TStackAlphabet>,
            List<FinishState<TStackAlphabet>>> _transitions;

        public NonDeterministicPushdownAutomaton()
        {
            _transitions = new Dictionary<StartState<TStackAlphabet>,
                List<FinishState<TStackAlphabet>>>();
        }

        public void AddTransition(
            StartState<TStackAlphabet> startState,
            FinishState<TStackAlphabet> finishState)
        {
            if (!_transitions.ContainsKey(startState)) 
                _transitions[startState] = new List<FinishState<TStackAlphabet>>();
            _transitions[startState].Add(finishState);
        }

        public void AddAcceptableState(StartState<TStackAlphabet> startState)
        {
            if (!_transitions.ContainsKey(startState)) 
                _transitions[startState] = new List<FinishState<TStackAlphabet>>();
            _transitions[startState].Add(FinishState<TStackAlphabet>.CreateAcceptableState());
        }

        public IRunningPushdownAutomation Run(
            IEnumerable<Terminal> tokens, 
            List<TStackAlphabet> startStackData)
        {
            var inputQueue = new Queue<Terminal>(tokens);
            inputQueue.Enqueue(Terminal.End);

            var stack = new Stack<TStackAlphabet>();
            for (var i = startStackData.Count - 1; i >= 0; --i) stack.Push(startStackData[i]);

            return new RunningNonDeterministicPushdownAutomaton<TStackAlphabet>(
                _transitions, 
                inputQueue.ToList(),
                stack);
        }
    }
}