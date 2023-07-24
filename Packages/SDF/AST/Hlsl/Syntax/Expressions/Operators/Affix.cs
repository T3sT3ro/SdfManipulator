namespace AST.Hlsl.Syntax.Expressions.Operators {
    public abstract record Affix : Expression { // prefix/postfix increment/decrement
        public enum Kind { INC, DEC }

        public IdentifierName id   { get; set; }
        public Kind       kind { get; set; }

        public record Pre : Affix;

        public record Post : Affix;
    }
}