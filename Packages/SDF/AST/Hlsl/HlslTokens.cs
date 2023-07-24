using System.Collections.Generic;
using System.Text.RegularExpressions;
using AST.Syntax;

namespace AST.Hlsl {
    public abstract record HlslToken : HlslSyntaxOrToken, ISyntaxToken<HlslSyntax, HlslTrivia, HlslSyntaxOrToken> {
        public         HlslSyntax      Parent         { get; internal set; }
        public         List<HlslTrivia> LeadingTrivia  { get; internal set; }
        public         List<HlslTrivia> TrailingTrivia { get; internal set; }
        public virtual string              Text           { get; set; }
        
    }
}
