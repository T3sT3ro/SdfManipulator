#nullable enable
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record For {
        [SyntaxNode] public partial record VariableInitializer : Initializer {
            public SeparatedList<hlsl, AssignmentExpression> initializers { get; init; } = new();
        }
    }
}
