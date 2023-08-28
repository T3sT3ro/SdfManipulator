#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Preprocessor {
    public record Pragma : PreprocessorSyntax {
        public PragmaKeyword pragmaKeyword { get; init; } = new();
        public TokenString?  tokenString   { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
            { hashToken, pragmaKeyword, tokenString }.FilterNotNull().ToList();
    }
}
