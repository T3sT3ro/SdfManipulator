#nullable enable
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;
using Statement = me.tooster.sdf.AST.Syntax.CommonSyntax.Statement<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    [SyntaxNode] public partial record Switch : Statement {
        public SwitchKeyword          switchKeyword { get; init; }
        public OpenParenToken         openParen     { get; init; }
        public Identifier             selector      { get; init; }
        public CloseParenToken        closeParen    { get; init; }
        public SyntaxList<hlsl, Case> cases         { get; init; }
        public DefaultCase?           defaultCase   { get; init; }
    }
}
