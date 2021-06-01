namespace SyntaxAnalyzer.Models.Tokens
{
    public class Token
    {
        private const string DefaultValue = "default";

        protected Token(string type, string value = "")
        {
            Type = type;
            Value = value != "" ? value : DefaultValue;
        }

        public string Value { get; }
        public string Type { get; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;

            if (!(obj is Token)) return false;

            var token = (Token) obj;

            return Value == token.Value && Type == token.Type;
        }

        public override int GetHashCode()
        {
            return $"{Type} {Value}".GetHashCode();
        }

        public override string ToString()
        {
            return $"{Type} {Value}";
        }
    }
}