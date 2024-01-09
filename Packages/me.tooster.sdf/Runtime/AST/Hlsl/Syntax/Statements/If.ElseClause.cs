#nullable enable
using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;
using Statement = me.tooster.sdf.AST.Syntax.CommonSyntax.Statement<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record If {
        [SyntaxNode] public partial record ElseClause {
            public ElseKeyword elseKeyword { get; init; } = new();
            public Statement   statement   { get; init; }
        }
    }
}
