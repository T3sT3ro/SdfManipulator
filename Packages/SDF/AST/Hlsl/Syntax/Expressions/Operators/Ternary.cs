using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    public record Ternary : Expression {
        public Expression    condition     { get; init; }
        public QuestionToken questionToken { get; init; } = new();
        public Expression    whenTrue      { get; init; }
        public ColonToken    colonToken    { get; init; } = new();
        public Expression    whenFalse     { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { condition, questionToken, whenTrue, colonToken, whenFalse };
    }
}
