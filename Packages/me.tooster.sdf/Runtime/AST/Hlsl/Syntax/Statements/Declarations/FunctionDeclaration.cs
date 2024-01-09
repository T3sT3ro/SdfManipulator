#nullable enable
using me.tooster.sdf.AST.Syntax;
using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;
using Statement = me.tooster.sdf.AST.Syntax.CommonSyntax.Statement<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements.Declarations {
    // int foo(float x, row_major float y : VPOS = 7.0f) { return x + y; }
    [SyntaxNode] public partial record FunctionDeclaration : Statement {
        public Type                    returnType     { get; init; }
        public Identifier              id             { get; init; }
        public ArgumentList<Parameter> paramList      { get; init; } = new();
        public Semantic?               returnSemantic { get; init; }
        public Block                   body           { get; init; }

        /// <a href="https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-function-parameters">HLSL parameter syntax</a>
        [SyntaxNode] public partial record Parameter {
            public Token<hlsl>? modifier    { get; init; }
            public Type         type        { get; init; }
            public Identifier   id          { get; init; }
            public Semantic?    semantic    { get; init; }
            public Expression?  initializer { get; init; }
        }
    }
}
