using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions {
    // {EXPR, EPXR, ...}
    public partial record StructInitializer : Expression {
        private readonly BracedList<Expression> _components;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { components };
    }
}
