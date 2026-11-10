#nullable enable
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using Statement = me.tooster.sdf.AST.Syntax.CommonSyntax.Statement<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    // switch (selector) { case 1: ... default: ... }
    [SyntaxNode] public partial record Switch : Statement {
        public SwitchKeyword          switchKeyword   { get; init; } = new();
        public OpenParenToken         openParen       { get; init; } = new();
        public Expression<hlsl>       selector        { get; init; }
        public CloseParenToken        closeParen      { get; init; } = new();
        public OpenBraceToken         openBraceToken  { get; init; } = new();
        public SyntaxList<hlsl, Case> cases           { get; init; } = new();
        public DefaultCase?           defaultCase     { get; init; }
        public CloseBraceToken        closeBraceToken { get; init; } = new();
    }
}
