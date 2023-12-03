#nullable enable
using me.tooster.sdf.AST.Syntax;

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
