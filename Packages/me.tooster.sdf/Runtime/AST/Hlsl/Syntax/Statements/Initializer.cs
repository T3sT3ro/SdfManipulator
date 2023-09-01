using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    // = y
    // = {{a}, {b}}}
    public abstract partial record Initializer : Syntax<Hlsl> {
        private readonly EqualsToken /*_*/equalsToken;
        private readonly Expression  /*_*/value;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { equalsToken, value };
    }
}
