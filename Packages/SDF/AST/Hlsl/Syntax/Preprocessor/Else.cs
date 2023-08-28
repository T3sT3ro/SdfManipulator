using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Preprocessor {
    public record Else : PreprocessorSyntax {
        public ElseKeyword elseKeyword { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { hashToken, elseKeyword };
    }
}
