using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    [SyntaxNode] public partial record Block : Statement {
        public OpenBraceToken              openBraceToken  { get; init; } = new();
        public SyntaxList<hlsl, Statement> statements      { get; init; } = new();
        public CloseBraceToken             closeBraceToken { get; init; } = new();
    }
}
