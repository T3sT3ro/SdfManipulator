#nullable enable
using System.Collections.Generic;
using AST.Hlsl.Syntax.Expressions.Operators;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public partial record For {
        public partial record VariableInitializer : Initializer {
            private readonly SeparatedList<Hlsl, AssignmentExpression> _initializers;

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new[]
                { initializers };
        }
    }
}
