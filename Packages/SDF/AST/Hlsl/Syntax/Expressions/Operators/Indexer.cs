namespace AST.Hlsl.Syntax.Expressions.Operators {
    public record Indexer : Expression {
        public Expression expression { get; set; }
        public Expression index      { get; set; }
    }
}