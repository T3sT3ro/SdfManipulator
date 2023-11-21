namespace me.tooster.sdf.AST.Foo.Syntax.Statements {
    [SyntaxNode] public partial record ExprStatement : Statement {
        public Expression expr { get; init; }
    }
}
