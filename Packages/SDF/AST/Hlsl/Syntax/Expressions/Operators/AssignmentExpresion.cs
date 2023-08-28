using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    // x = y
    // y <<= 2
    public record AssignmentExpresion : Expression {
        public Expression      left            { get; init; }
        public AssignmentToken assignmentToken { get; init; } = new EqualsToken();
        public Expression      right           { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { left, assignmentToken, right };
    }
}
