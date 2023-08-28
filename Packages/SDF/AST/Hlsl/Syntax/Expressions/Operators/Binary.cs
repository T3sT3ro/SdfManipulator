using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    public record Binary : Expression {
        public Expression  left          { get; init; }
        public Token<Hlsl> operatorToken { get; init; }
        public Expression  right         { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { left, operatorToken, right };
    }
}
