using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Preprocessor {
    public record Include : PreprocessorSyntax {
        public IncludePreprocessorKeyword includeKeyword { get; init; }
        public QuotedStringLiteral        filepath       { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { hashToken, includeKeyword, filepath };
    }
}
