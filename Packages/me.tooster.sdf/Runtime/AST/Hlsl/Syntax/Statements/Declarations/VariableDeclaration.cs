using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements.Declarations {
    // float x;
    // float x[1][2];
    // float a, b, c;
    // float x = 7.0f, y = 8.0f;
    // uniform row/*_*/major float4x4 g/*_*/mWorldViewProjection : WORLDVIEWPROJECTION;
    // struct Result {float d; float3 pos;} result = {1.0f, {0.0f, 0.0f, 0.0f}}; 
    public partial record VariableDeclaration : Statement {
        private readonly VariableDeclarator /*_*/declarator;
        private readonly SemiToken                           /*_*/semiToken;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { declarator, semiToken };
    }
}
