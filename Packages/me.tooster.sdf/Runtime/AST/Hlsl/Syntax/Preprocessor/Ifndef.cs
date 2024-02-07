using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [SyntaxNode] public partial record Ifndef : PreprocessorSyntax {
        public IfndefKeyword ifndefKeyword { get; init; } = new();
        public Identifier    id            { get; init; }
        
        public override IReadOnlyList<SyntaxOrToken<hlsl>> ChildNodesAndTokens() => new SyntaxOrToken<hlsl>[]
        {
            hashToken,
            ifndefKeyword,
            id,
            endOfDirectiveToken,
        };
    }
}
