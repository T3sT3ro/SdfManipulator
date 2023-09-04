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
    [Syntax] public partial record VariableDeclarator : Syntax<Hlsl> {
        private readonly        Token<Hlsl>?                            _storageKeyword;
        private readonly        Token<Hlsl>?                            _typeModifier;
        private readonly        Type                                    _type;
        [Init] private readonly SeparatedList<Hlsl, VariableDefinition> _variables;

        // x
        // x[1][2]
        // x : PSIZE = 3
        [Syntax] public partial record VariableDefinition : Syntax<Hlsl> {
            private readonly Identifier                   _id;
            private readonly SyntaxList<Hlsl, ArrayRank>? _arraySizes;
            private readonly Semantic?                    _semantic;
            private readonly For.Initializer?             _initializer;
        }
    }
}
