using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [SyntaxNode] public partial record Elif : PreprocessorSyntax {
        public ElifKeyword elifKeyword { get; init; } = new();
        public TokenString condition   { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<hlsl>> ChildNodesAndTokens() => new SyntaxOrToken<hlsl>[]
        {
            hashToken,
            elifKeyword,
            condition,
            endOfDirectiveToken,
        };
    }
}
