using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.SubShaderSpecific {
    public partial record Pass {
        [SyntaxNode] public partial record Name : PassStatement {
            public NameKeyword         nameKeyword { get; init; } = new();
            public QuotedStringLiteral name        { get; init; }
        }
    }
}
