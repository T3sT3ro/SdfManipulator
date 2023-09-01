#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record For {
        public partial record VariableDeclarator : Initializer {
            public VariableDeclarator declarator { get; internal init; }

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new Syntax<Hlsl>[]
                { declarator };
        }
    }
}
