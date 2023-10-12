#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    // float x
    // uniform row_major float4x4 M : WORLDVIEWPROJECTION, N : WORLDVIEWPROJECTION
    // float x[2][2] : VPOS = { { 1, 2 }, { 3, 4 } }
    // struct Result {float d; float3 pos;} result = {1.0f, {0.0f, 0.0f, 0.0f}};
    [AstSyntax] public partial record VariableDeclarator : Syntax<Hlsl> {
        public        Token<Hlsl>?                            storageKeyword { get; init; }
        public        Token<Hlsl>?                            typeModifier { get; init; }
        public        Type                                    type { get; init; }
        public SeparatedList<Hlsl, VariableDefinition> variables { get; init; } = new();

        // x
        // x[1][2]
        // x : PSIZE = 3
        [AstSyntax] public partial record VariableDefinition : Syntax<Hlsl> {
            public Identifier                   id { get; init; }
            public SyntaxList<Hlsl, ArrayRank>? arraySizes { get; init; }
            public Semantic?                    semantic { get; init; }
            public For.Initializer?             initializer { get; init; }
        }
    }
}
