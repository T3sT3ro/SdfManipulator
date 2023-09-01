using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions {
    // {EXPR, EPXR, ...}
    public partial record StructInitializer : Expression {
        private readonly BracedList<Expression> /*_*/components;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { components };
    }
}
