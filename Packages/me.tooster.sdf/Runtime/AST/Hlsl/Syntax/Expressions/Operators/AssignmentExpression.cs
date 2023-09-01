using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    // x = y
    // y <<= 2
    public partial record AssignmentExpression : Expression {
        private readonly Expression      /*_*/left;
        private readonly AssignmentToken /*_*/assignmentToken;
        private readonly Expression      /*_*/right;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { left, assignmentToken, right };
    }
}
