#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
   public partial record For {
       [Syntax] public partial record VariableInitializer : Initializer {
            [Init] private readonly SeparatedList<Hlsl, AssignmentExpression> _initializers;
        }
    }
}
