#nullable enable
using System.Collections.Generic;
using AST.Hlsl.Syntax.Expressions.Operators;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public partial record For {
        public record VariableInitializer : Initializer {
            public SeparatedList<Hlsl, AssignmentExpresion> initializers { get; init; }

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new[]
                { initializers };
        }
    }
}
