using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Preprocessor {
    public record Elif : PreprocessorSyntax {
        public ElifKeyword elifKeyword { get; init; } = new();
        public TokenString condition   { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { hashToken, elifKeyword, condition };
    }
}
