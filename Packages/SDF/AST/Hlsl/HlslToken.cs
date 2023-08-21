using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl {
    public abstract record HlslToken :
        IHlslSyntaxOrToken,
        ISyntaxToken<HlslSyntax, HlslToken, HlslTrivia, IHlslSyntaxOrToken> {
        public           IReadOnlyList<HlslTrivia> LeadingTrivia  { get; set; }
        public           IReadOnlyList<HlslTrivia> TrailingTrivia { get; set; }

        public abstract string Text { get; }
    }
}
