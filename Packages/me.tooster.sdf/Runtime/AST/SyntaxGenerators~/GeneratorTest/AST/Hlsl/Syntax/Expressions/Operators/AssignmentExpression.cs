using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    // x = y
    // y <<= 2
    [Syntax] public partial record AssignmentExpression : Expression {
        private readonly Expression      _left;
        [Init(With = typeof(EqualsToken))] 
        private readonly AssignmentToken _assignmentToken;
        private readonly Expression      _right;
    }
}
