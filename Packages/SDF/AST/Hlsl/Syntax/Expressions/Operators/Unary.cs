using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    public record Unary : Expression {
        public Token<Hlsl> operatorToken { get; init; }
        public Expression  expression    { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { operatorToken, expression };
    }
}
