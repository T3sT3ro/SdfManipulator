using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Foo.Syntax {
    [SyntaxNode] public partial record ProgramSyntax {
        public SyntaxList<foo, Statement> statements { get; init; }
    }
}
