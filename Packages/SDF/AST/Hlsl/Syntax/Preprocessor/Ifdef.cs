using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Preprocessor {
    public record Ifdef : PreprocessorSyntax {
        public IfdefKeyword ifdefKeyword { get; init; } = new();
        public Identifier   id           { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { hashToken, ifdefKeyword, id };
    }
}
