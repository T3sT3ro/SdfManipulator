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
    public record VariableDeclarator : HlslSyntax {
        public HlslToken?                        storageKeyword { get; init; }
        public HlslToken?                        typeModifier   { get; init; }
        public Type                              type           { get; init; }
        public SeparatedList<VariableDefinition> variables      { get; init; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new IHlslSyntaxOrToken[]
                { storageKeyword, typeModifier, type, variables }
            .FilterNotNull().ToList();


        // x
        // x[1][2]
        // x : PSIZE = 3
        public record VariableDefinition : HlslSyntax {
            public Identifier                         id          { get; init; }
            public IReadOnlyList<ArrayRankSpecifier>? arraySizes  { get; init; }
            public Semantic?                          semantic    { get; init; }
            public Initializer?                       initializer { get; init; }

            public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new HlslSyntax[]
                    { id, }
                .ConcatNotNull(arraySizes)
                .AppendNotNull(semantic)
                .AppendNotNull(initializer)
                .ToList();
        }
    }
}
