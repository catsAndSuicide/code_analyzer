namespace SyntaxAnalyzer.Models.Tokens
{
    public class StackEndToken : Token
    {
        public static readonly StackEndToken Instance = new StackEndToken();

        private StackEndToken() : base("StackEndToken", "StackEndToken")
        {
        }
    }
}