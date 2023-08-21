using System.Collections.Generic;

namespace AST.Shaderlab.Syntax.SubShader {
    public partial record Pass {
        public record Name : PassStatement {
            public NameKeyword         nameKeyword { get; set; } = new();
            public QuotedStringLiteral name        { get; set; }

            public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new IShaderlabSyntaxOrToken[]
                { nameKeyword, name };
        }
    }
}
