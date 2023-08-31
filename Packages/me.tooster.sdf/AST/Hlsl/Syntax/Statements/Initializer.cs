using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    // = y
    // = {{a}, {b}}}
    public abstract partial record Initializer : Syntax<Hlsl> {
        private readonly EqualsToken _equalsToken;
        private readonly Expression  _value;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { equalsToken, value };
    }
}
