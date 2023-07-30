using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl {
    public abstract record HlslToken :
        HlslSyntaxOrToken,
        ISyntaxToken<HlslSyntax, HlslToken, HlslTrivia, HlslSyntaxOrToken> {
        public          HlslSyntax                Parent         { get; internal set; }
        public          IReadOnlyList<HlslTrivia> LeadingTrivia  { get; internal set; }
        public          IReadOnlyList<HlslTrivia> TrailingTrivia { get; internal set; }
        public virtual  string                    Text           { get; set; }
    }
}
