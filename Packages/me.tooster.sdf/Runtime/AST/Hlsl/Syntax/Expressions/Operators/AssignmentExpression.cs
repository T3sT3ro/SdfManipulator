using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    // x = y
    // y <<= 2
    [Syntax] public partial record AssignmentExpression : Expression {
        private readonly Expression      _left;
        [Init(DefaultValueType = typeof(EqualsToken))] 
        private readonly AssignmentToken _assignmentToken;
        private readonly Expression      _right;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { left, assignmentToken, right };
    }
}
