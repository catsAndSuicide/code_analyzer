namespace SyntaxAnalyzer.Models.Tokens
{
    public class Terminal : Token
    {
        public static readonly Terminal Lambda = new Terminal("Lambda");

        public static readonly Terminal End = new Terminal("End");

        public Terminal(string value = "") : base("Terminal", value)
        {
        }
    }
}