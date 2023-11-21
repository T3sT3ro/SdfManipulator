namespace me.tooster.sdf.AST.Foo.Syntax.Statements {
    [SyntaxNode] public partial record Test : Statement {
        public TestToken       testToken       { get; init; }
        public OpenParenToken  openParenToken  { get; init; }
        public CloseParenToken closeParenToken { get; init; }
    }
}
