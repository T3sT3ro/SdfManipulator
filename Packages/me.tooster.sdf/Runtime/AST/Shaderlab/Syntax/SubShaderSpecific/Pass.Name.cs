using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.SubShaderSpecific {
    public partial record Pass {
        public record Name : PassStatement {
            public NameKeyword         _nameKeyword { get; init; } = new();
            public QuotedStringLiteral _name        { get; init; }

            public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
                { nameKeyword, name };
        }
    }
}
