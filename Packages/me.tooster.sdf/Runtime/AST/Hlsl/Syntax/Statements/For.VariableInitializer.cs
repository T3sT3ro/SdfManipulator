#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record For {
        public partial record VariableInitializer : Initializer {
            private readonly SeparatedList<Hlsl, AssignmentExpression> /*_*/initializers;

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new[]
                { initializers };
        }
    }
}
