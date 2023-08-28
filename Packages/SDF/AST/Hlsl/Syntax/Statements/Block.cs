using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public record Block : Statement {
        public OpenBraceToken              openBraceToken  { get; init; } = new();
        public SyntaxList<Hlsl, Statement> statements      { get; init; } = SyntaxList<Hlsl, Statement>.Empty;
        public CloseBraceToken             closeBraceToken { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { openBraceToken, statements, closeBraceToken };
    }
}
