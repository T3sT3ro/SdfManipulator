using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements.Declarations {
    // float x;
    // float x[1][2];
    // float a, b, c;
    // float x = 7.0f, y = 8.0f;
    // uniform row_major float4x4 g_mWorldViewProjection : WORLDVIEWPROJECTION;
    // struct Result {float d; float3 pos;} result = {1.0f, {0.0f, 0.0f, 0.0f}}; 
    [SyntaxNode] public partial record VariableDeclaration : Statement {
        public VariableDeclarator declarator     { get; init; }
        public SemicolonToken     semicolonToken { get; init; } = new();
    }
}
