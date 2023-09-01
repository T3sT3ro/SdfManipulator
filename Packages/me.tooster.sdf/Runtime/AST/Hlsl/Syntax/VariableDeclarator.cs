#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    // float x
    // uniform row/*_*/major float4x4 M : WORLDVIEWPROJECTION, N : WORLDVIEWPROJECTION
    // float x[2][2] : VPOS = { { 1, 2 }, { 3, 4 } }
    // struct Result {float d; float3 pos;} result = {1.0f, {0.0f, 0.0f, 0.0f}};
    public partial record VariableDeclarator : Syntax<Hlsl> {
        private readonly Token<Hlsl>?                                               /*_*/storageKeyword;
        private readonly Token<Hlsl>?                                               /*_*/typeModifier;
        private readonly Type                                                       /*_*/type;
        private readonly SeparatedList<Hlsl, VariableDeclarator.VariableDefinition> /*_*/variables;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
                { storageKeyword, typeModifier, type, variables }
            .FilterNotNull().ToList();


        // x
        // x[1][2]
        // x : PSIZE = 3
        public partial record VariableDefinition : Syntax<Hlsl> {
            private readonly Identifier                   /*_*/id;
            private readonly SyntaxList<Hlsl, ArrayRank>? /*_*/arraySizes;
            private readonly Semantic?                    /*_*/semantic;
            private readonly For.Initializer?             /*_*/initializer;

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new Syntax<Hlsl>?[]
                    { id, arraySizes, semantic, initializer }.FilterNotNull()
                .ToList();
        }
    }
}
