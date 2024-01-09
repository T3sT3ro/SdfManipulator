using me.tooster.sdf.AST.Syntax.CommonSyntax;

namespace me.tooster.sdf.AST.Foo.Syntax.Statements {
    [SyntaxNode] public partial record ExprStatement : Statement<foo> {
        public Expression<foo> expr { get; init; }
    }
}
