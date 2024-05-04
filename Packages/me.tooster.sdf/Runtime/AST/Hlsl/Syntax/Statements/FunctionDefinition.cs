#nullable enable
using Statement = me.tooster.sdf.AST.Syntax.CommonSyntax.Statement<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    /// function definition with body
    /// int foo(float x, row_major float y : VPOS = 7.0f) { return x + y; }
    [SyntaxNode] public partial record FunctionDefinition : Statement {
        public Type                    returnType     { get; init; }
        public Identifier              id             { get; init; }
        public ArgumentList<Parameter> paramList      { get; init; } = new();
        public Semantic?               returnSemantic { get; init; }
        public Block                   body           { get; init; }
    }
}
