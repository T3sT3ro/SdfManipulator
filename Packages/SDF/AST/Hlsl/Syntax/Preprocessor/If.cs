using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Preprocessor {
    public record If : PreprocessorSyntax {
        public IfKeyword   ifKeyword { get; init; } = new();
        public TokenString condition { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { hashToken, ifKeyword, condition };
    }
}
