#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Hlsl.Syntax.Statements;
using AST.Syntax;

namespace AST.Hlsl.Syntax {
    // float x
    // uniform row_major float4x4 M : WORLDVIEWPROJECTION, N : WORLDVIEWPROJECTION
    // float x[2][2] : VPOS = { { 1, 2 }, { 3, 4 } }
    // struct Result {float d; float3 pos;} result = {1.0f, {0.0f, 0.0f, 0.0f}};
    public partial record VariableDeclarator : Syntax<Hlsl> {
        private readonly Token<Hlsl>?                            _storageKeyword;
        private readonly Token<Hlsl>?                            _typeModifier;
        private readonly Type                                    _type;
        private readonly SeparatedList<Hlsl, VariableDefinition> _variables;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
                { storageKeyword, typeModifier, type, variables }
            .FilterNotNull().ToList();


        // x
        // x[1][2]
        // x : PSIZE = 3
        public partial record VariableDefinition : Syntax<Hlsl> {
            private readonly Identifier                   _id;
            private readonly SyntaxList<Hlsl, ArrayRank>? _arraySizes;
            private readonly Semantic?                    _semantic;
            private readonly Initializer?                 _initializer;

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new Syntax<Hlsl>?[]
                    { id, arraySizes, semantic, initializer }.FilterNotNull()
                .ToList();
        }
    }
}
