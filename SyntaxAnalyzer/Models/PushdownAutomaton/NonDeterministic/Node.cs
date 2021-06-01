namespace SyntaxAnalyzer.Models.PushdownAutomaton.NonDeterministic
{
    public class Node<TStackAlphabet>
    {
        public readonly int OldLookahead;
        public readonly TStackAlphabet[] OldStack;
        public readonly TStackAlphabet Symbol;

        public Node(TStackAlphabet symbol, int oldLookahead,
            TStackAlphabet[] oldStack, int lastRuleIndex)
        {
            Symbol = symbol;
            OldLookahead = oldLookahead;
            OldStack = oldStack;
            LastRuleIndex = lastRuleIndex;
        }

        public int LastRuleIndex { get; private set; }

        public void Increment()
        {
            LastRuleIndex++;
        }
    }
}