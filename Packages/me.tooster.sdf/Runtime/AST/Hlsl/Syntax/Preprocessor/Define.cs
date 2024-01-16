#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [SyntaxNode] public partial record Define : PreprocessorSyntax {
        public DefineKeyword             defineKeyword { get; init; } = new();
        public ArgumentList<Identifier>? argList       { get; init; } = new();
        public Identifier                id            { get; init; }
        public TokenString               tokenString   { get; init; } = new();
        
        public override IReadOnlyList<SyntaxOrToken<hlsl>> ChildNodesAndTokens => new SyntaxOrToken<hlsl>[]
        {
            hashToken,
            defineKeyword,
            argList,
            id,
            tokenString,
            endOfDirectiveToken,
        }.Where(c => c is not null).Select(c => c!).ToList();
    }
}
