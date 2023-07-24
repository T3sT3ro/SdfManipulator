namespace AST.Hlsl.Syntax.Expressions.Operators {
    public record Cast : Expression {
        public Type.Type  type       { get; set; }
        public Expression expression { get; set; }
    }
}