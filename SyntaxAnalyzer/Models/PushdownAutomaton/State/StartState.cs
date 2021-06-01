using System;
using SyntaxAnalyzer.Models.Tokens;

namespace SyntaxAnalyzer.Models.PushdownAutomaton.State
{
    public class StartState<TStackAlphabet>
    {
        public StartState(Terminal inputSymbol, TStackAlphabet stackSymbol)
        {
            if (Equals(stackSymbol, Terminal.End))
                throw new Exception($"Token in start state can not be {stackSymbol}");

            _inputSymbol = inputSymbol;
            _stackSymbol = stackSymbol;
        }
        
        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (!(obj is StartState<TStackAlphabet>))
                return false;

            var token = (StartState<TStackAlphabet>) obj;

            return Equals(_inputSymbol, token._inputSymbol) && Equals(_stackSymbol, token._stackSymbol);
        }

        public override int GetHashCode()
        {
            return _inputSymbol.GetHashCode() * 1039 +
                   _stackSymbol.GetHashCode();
        }

        public override string ToString()
        {
            return $"input: {_inputSymbol}, stack: {_stackSymbol}";
        }

        private readonly Terminal _inputSymbol;
        private readonly TStackAlphabet _stackSymbol;
    }
}