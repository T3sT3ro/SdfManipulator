#nullable enable
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
namespace me.tooster.sdf.AST.Hlsl.Syntax {
    /// <a href="https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-function-parameters">HLSL parameter syntax</a>
    [SyntaxNode] public partial record Parameter {
        public Token<hlsl>?      modifier    { get; init; }
        public Type              type        { get; init; }
        public Identifier        id          { get; init; }
        public Semantic?         semantic    { get; init; }
        public Expression<hlsl>? initializer { get; init; }
    }
}
