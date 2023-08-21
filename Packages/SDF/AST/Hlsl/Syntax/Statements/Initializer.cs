using System.Collections.Generic;
using AST.Hlsl.Syntax.Expressions;

namespace AST.Hlsl.Syntax.Statements {
    // = y
    // = {{a}, {b}}}
    public abstract record Initializer : HlslSyntax {
        public EqualsToken equalsToken { get; set; } = new();
        public Expression  value       { get; set; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new IHlslSyntaxOrToken[]
            { equalsToken, value };
    }
}
