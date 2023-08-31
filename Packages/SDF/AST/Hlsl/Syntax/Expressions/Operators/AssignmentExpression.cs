using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    // x = y
    // y <<= 2
    public partial record AssignmentExpression : Expression {
        private readonly Expression      _left;
        private readonly AssignmentToken _assignmentToken;
        private readonly Expression      _right;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { left, assignmentToken, right };
    }
}
