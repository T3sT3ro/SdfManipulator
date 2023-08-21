using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    // x = y
    // y <<= 2
    public record AssignmentExpresion : Expression {
        public Expression      left            { get; init; }
        public AssignmentToken assignmentToken { get; set; } = new EqualsToken();
        public Expression      right           { get; init; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens =>
            new IHlslSyntaxOrToken[] { left, assignmentToken, right };
    }
}
