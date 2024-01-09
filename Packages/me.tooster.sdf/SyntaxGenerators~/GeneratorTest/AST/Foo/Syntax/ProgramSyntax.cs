using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;

namespace me.tooster.sdf.AST.Foo.Syntax {
    [SyntaxNode] public partial record ProgramSyntax {
        public SyntaxList<foo, Statement<foo>> statements { get; init; }
    }
}
