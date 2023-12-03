#nullable enable
namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record If {
        [SyntaxNode] public partial record ElseClause {
            public ElseKeyword elseKeyword { get; init; } = new();
            public Statement   statement   { get; init; }
        }
    }
}
