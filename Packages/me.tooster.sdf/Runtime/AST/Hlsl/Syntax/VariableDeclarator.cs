#nullable enable
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    // float x
    // uniform row_major float4x4 M : WORLDVIEWPROJECTION, N : WORLDVIEWPROJECTION
    // float x[2][2] : VPOS = { { 1, 2 }, { 3, 4 } }
    // struct Result {float d; float3 pos;} result = {1.0f, {0.0f, 0.0f, 0.0f}};
    // volatile uniform row_major float4x4 bones[6];
    [SyntaxNode] public partial record VariableDeclarator : For.Initializer {
        public SyntaxOrTokenList<hlsl>?        storageKeywords { get; init; }
        public SyntaxOrTokenList<hlsl>?        typeModifiers   { get; init; }
        public Type                            type            { get; init; }
        public SeparatedList<hlsl, Definition> variables       { get; init; } = new();

        // x
        // x[1][2]
        // x : PSIZE = 3
        [SyntaxNode] public partial record Definition : Syntax<hlsl> {
            public Identifier                   id          { get; init; }
            public SyntaxList<hlsl, ArrayRank>? arraySizes  { get; init; }
            public Semantic?                    semantic    { get; init; }
            public Initializer?                 initializer { get; init; }
        }
    }
}
