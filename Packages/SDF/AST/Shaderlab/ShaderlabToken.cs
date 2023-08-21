using System.Collections.Generic;
using AST.Syntax;

namespace AST.Shaderlab {
    public abstract record ShaderlabToken :
        IShaderlabSyntaxOrToken,
        ISyntaxToken<ShaderlabSyntax, ShaderlabToken, ShaderlabTrivia, IShaderlabSyntaxOrToken> {
        public         IReadOnlyList<ShaderlabTrivia> LeadingTrivia  { get; set; }
        public         IReadOnlyList<ShaderlabTrivia> TrailingTrivia { get; set; }
        public virtual string                         Text           { get; set;  }
    }
}
