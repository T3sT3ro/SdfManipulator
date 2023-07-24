namespace AST.Hlsl.Syntax.Expressions.Operators {
    public record Member : Expression, Assignment.Left {
        public Expression expression { get; set; }
        public IdentifierName member     { get; set; }
    }
}