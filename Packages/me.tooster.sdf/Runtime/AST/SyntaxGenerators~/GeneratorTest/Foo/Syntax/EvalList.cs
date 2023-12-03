using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Foo.Syntax {
    [SyntaxNode] public partial record EvalList<T> : Syntax<foo> where T : Statement {
        public SyntaxList<foo, T> statementList { get; init; }
    }
}
