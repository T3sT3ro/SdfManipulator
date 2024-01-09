using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using Statement = me.tooster.sdf.AST.Syntax.CommonSyntax.Statement<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    [SyntaxNode] public partial record Block : Statement {
        public OpenBraceToken              openBraceToken  { get; init; } = new();
        public SyntaxList<hlsl, Statement> statements      { get; init; } = new();
        public CloseBraceToken             closeBraceToken { get; init; } = new();
    }
}
