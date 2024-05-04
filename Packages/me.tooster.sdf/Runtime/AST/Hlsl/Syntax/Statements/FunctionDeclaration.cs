#nullable enable
using Statement = me.tooster.sdf.AST.Syntax.CommonSyntax.Statement<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    /// <summary>
    /// Function signature declaration
    /// Sometimes Hlsl may require forward declarations.
    /// <code> 
    /// int foo(float x, row_major float y : VPOS = 7.0f);
    /// void foo(out float);
    /// </code>
    /// </summary>
    [SyntaxNode] public partial record FunctionDeclaration : Statement {
        public Type                    returnType     { get; init; }
        public Identifier              id             { get; init; }
        public ArgumentList<Parameter> paramList      { get; init; } = new();
        public Semantic?               returnSemantic { get; init; }
        public SemicolonToken          semicolonToken { get; init; } = new();
    }
}
