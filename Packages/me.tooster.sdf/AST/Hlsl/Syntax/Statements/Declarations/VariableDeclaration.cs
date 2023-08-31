using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements.Declarations {
    // float x;
    // float x[1][2];
    // float a, b, c;
    // float x = 7.0f, y = 8.0f;
    // uniform row_major float4x4 g_mWorldViewProjection : WORLDVIEWPROJECTION;
    // struct Result {float d; float3 pos;} result = {1.0f, {0.0f, 0.0f, 0.0f}}; 
    public partial record VariableDeclaration : Statement {
        private readonly VariableDeclarator _declarator;
        private readonly SemiToken          _semiToken;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { declarator, semiToken };
    }
}
