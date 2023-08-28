using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Preprocessor {
    public record Undef : PreprocessorSyntax {
        public UndefKeyword undefKeyword { get; init; } = new();
        public Identifier   id           { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { hashToken, undefKeyword, id };
    }
}
