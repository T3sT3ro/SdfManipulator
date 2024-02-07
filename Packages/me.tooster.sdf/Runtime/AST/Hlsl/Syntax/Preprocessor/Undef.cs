using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [SyntaxNode] public partial record Undef : PreprocessorSyntax {
        public UndefKeyword undefKeyword { get; init; } = new();
        public Identifier   id           { get; init; }
        
        public override IReadOnlyList<SyntaxOrToken<hlsl>> ChildNodesAndTokens() => new SyntaxOrToken<hlsl>[]
        {
            hashToken,
            undefKeyword,
            id,
            endOfDirectiveToken,
        }.Where(c => c is not null).Select(c => c!).ToList();
    }
}
