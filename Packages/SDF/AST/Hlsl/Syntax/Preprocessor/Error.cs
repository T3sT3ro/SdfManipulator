using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Preprocessor {
    public record Error : PreprocessorSyntax {
        public ErrorKeyword errorKeyword { get; init; } = new();
        public TokenString  tokenstring  { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { hashToken, errorKeyword, tokenstring };
    }
}
