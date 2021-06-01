using System.Collections.Generic;
using SyntaxAnalyzer.Models.PushdownAutomaton.State;
using SyntaxAnalyzer.Models.Tokens;

namespace SyntaxAnalyzer.Models.PushdownAutomaton
{
    public interface IPushdownAutomation<TStackAlphabet>
    {
        void AddTransition(StartState<TStackAlphabet> startState,
            FinishState<TStackAlphabet> finishState);

        void AddAcceptableState(StartState<TStackAlphabet> startState);

        IRunningPushdownAutomation Run(
            IEnumerable<Terminal> tokens,
            List<TStackAlphabet> startStackData);
    }
}