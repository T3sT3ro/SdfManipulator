using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [SyntaxNode] public partial record If : PreprocessorSyntax {
        public IfKeyword   ifKeyword { get; init; } = new();
        public TokenString condition { get; init; }

        public override IReadOnlyList<SyntaxOrToken<hlsl>> ChildNodesAndTokens() => new SyntaxOrToken<hlsl>[]
        {
            hashToken,
            ifKeyword,
            condition,
            endOfDirectiveToken,
        };
    }
}
