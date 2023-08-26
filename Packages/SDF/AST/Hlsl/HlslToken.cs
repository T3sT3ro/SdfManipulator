using System;
using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl {
    public abstract record HlslToken :
        IHlslSyntaxOrToken,
        ISyntaxToken<HlslSyntax, HlslToken, HlslTrivia, IHlslSyntaxOrToken> {
        public IReadOnlyList<HlslTrivia> LeadingTrivia  { get; init; } = Array.Empty<HlslTrivia>();
        public IReadOnlyList<HlslTrivia> TrailingTrivia { get; init; } = Array.Empty<HlslTrivia>();

        public abstract string Text { get; }
    }
}
