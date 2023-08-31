using System.Collections.Generic;
using AST.Syntax;

namespace AST.Shaderlab.Syntax.SubShaderSpecific {
    public partial record Pass {
        public record Name : PassStatement {
            public NameKeyword         nameKeyword { get; init; } = new();
            public QuotedStringLiteral name        { get; init; }

            public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
                { nameKeyword, name };
        }
    }
}
