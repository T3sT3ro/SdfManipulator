using System;
using System.Collections.Generic;
using AST.Syntax;

namespace AST.Shaderlab {
    public abstract record ShaderlabToken :
        IShaderlabSyntaxOrToken,
        ISyntaxToken<ShaderlabSyntax, ShaderlabToken, ShaderlabTrivia, IShaderlabSyntaxOrToken> {
        public         IReadOnlyList<ShaderlabTrivia> LeadingTrivia  { get; init; } = Array.Empty<ShaderlabTrivia>();
        public         IReadOnlyList<ShaderlabTrivia> TrailingTrivia { get; init; } = Array.Empty<ShaderlabTrivia>();
        public virtual string                         Text           { get; protected set;  }
    }
}
