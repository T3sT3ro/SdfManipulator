#nullable enable
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;
using Statement = me.tooster.sdf.AST.Syntax.CommonSyntax.Statement<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    // for (start conditions; conditions; increments) { body }
    // start conditions:
    //    declarations: int i, j = 0;
    //    initializers: i = 0, j = 0;
    [SyntaxNode] public partial record For : Statement {
        public ForKeyword                       forKeyword           { get; init; } = new();
        public OpenParenToken                   openParen            { get; init; } = new();
        public Initializer?                     initializer          { get; init; }
        public SemicolonToken                   firstSemicolonToken  { get; init; } = new();
        public Expression?                      condition            { get; init; }
        public SemicolonToken                   secondSemicolonToken { get; init; } = new();
        public SeparatedList<hlsl, Expression>? increments           { get; init; } = new();
        public CloseParenToken                  closeParen           { get; init; } = new();
        public Statement                        body                 { get; init; }

        [SyntaxNode] public abstract partial record Initializer;
    }
}
