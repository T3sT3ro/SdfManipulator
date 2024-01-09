using me.tooster.sdf.AST.Syntax;
using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    // someFunction(arguments) 
    [SyntaxNode] public partial record Call : Expression {
        public Identifier                 id      { get; init; }
        public ArgumentList<Syntax<hlsl>> argList { get; init; }
    }
}
