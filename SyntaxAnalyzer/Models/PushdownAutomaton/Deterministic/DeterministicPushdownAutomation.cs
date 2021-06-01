using System.Collections.Generic;
using SyntaxAnalyzer.Models.PushdownAutomaton.State;
using SyntaxAnalyzer.Models.Tokens;

namespace SyntaxAnalyzer.Models.PushdownAutomaton.Deterministic
{
    public class DeterministicPushdownAutomaton<TStackAlphabet> : IPushdownAutomation<TStackAlphabet>
    {
        private readonly Dictionary<StartState<TStackAlphabet>, FinishState<TStackAlphabet>> _transitions;

        public DeterministicPushdownAutomaton()
        {
            _transitions = new Dictionary<StartState<TStackAlphabet>, FinishState<TStackAlphabet>>();
        }
        
        public void AddTransition(
            StartState<TStackAlphabet> startState,
            FinishState<TStackAlphabet> finishState)
        {
            _transitions[startState] = finishState;
        }

        public void AddAcceptableState(StartState<TStackAlphabet> startState)
        {
            _transitions[startState] = FinishState<TStackAlphabet>.CreateAcceptableState();
        }

        public IRunningPushdownAutomation Run(
            IEnumerable<Terminal> tokens, 
            List<TStackAlphabet> startStackData)
        {
            var inputQueue = new Queue<Terminal>(tokens);
            inputQueue.Enqueue(Terminal.End);

            var stack = new Stack<TStackAlphabet>();
            for (var i = startStackData.Count - 1; i >= 0; --i)
                stack.Push(startStackData[i]);

            return new RunningDeterministicPushdownAutomaton<TStackAlphabet>(_transitions, inputQueue, stack);
        }
    }
}