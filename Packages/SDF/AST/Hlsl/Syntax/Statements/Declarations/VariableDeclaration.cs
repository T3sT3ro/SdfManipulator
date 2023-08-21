using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Statements.Declarations {
    // float x;
    // float x[1][2];
    // float a, b, c;
    // float x = 7.0f, y = 8.0f;
    // uniform row_major float4x4 g_mWorldViewProjection : WORLDVIEWPROJECTION;
    // struct Result {float d; float3 pos;} result = {1.0f, {0.0f, 0.0f, 0.0f}}; 
    public record VariableDeclaration : Statement {
        public VariableDeclarator declarator { get; init; }
        public SemiToken          semiToken  { get; init; } = new();

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens =>
            new IHlslSyntaxOrToken[] { declarator, semiToken };
    }
}
