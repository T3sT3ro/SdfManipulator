using me.tooster.sdf.AST.Syntax.CommonSyntax;

namespace me.tooster.sdf.AST.Foo.Syntax.Statements {
    [SyntaxNode] public partial record Test : Statement<foo> {
        public TestKeyword     testToken       { get; init; }
        public OpenParenToken  openParenToken  { get; init; }
        public CloseParenToken closeParenToken { get; init; }
    }
}
