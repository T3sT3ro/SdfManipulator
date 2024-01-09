using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;

namespace me.tooster.sdf.AST.Foo.Syntax {
    [SyntaxNode] public partial record EvalList<T> : Syntax<foo> where T : Statement<foo> {
        public SyntaxList<foo, T> statementList { get; init; }
    }
}
