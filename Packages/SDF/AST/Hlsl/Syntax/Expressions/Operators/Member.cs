using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    // a.b.c
    public record Member : Expression {
        public Expression expression { get; init; }
        public DotToken   dotToken   { get; init; } = new();
        public Identifier member     { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { expression, dotToken, member };
    }
}
