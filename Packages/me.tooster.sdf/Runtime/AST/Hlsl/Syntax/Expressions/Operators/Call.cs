using me.tooster.sdf.AST.Syntax.CommonSyntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    // someFunction(arguments) 
    [SyntaxNode] public partial record Call : Expression<hlsl> {
        public NameSyntax                     calee   { get; init; }
        public ArgumentList<Expression<hlsl>> argList { get; init; }
    }
}
