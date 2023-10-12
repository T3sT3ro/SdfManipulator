#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record For {
        [AstSyntax] public partial record VariableInitializer : Initializer {
            public SeparatedList<Hlsl, AssignmentExpression> initializers { get; init; } = new();
        }
    }
}
