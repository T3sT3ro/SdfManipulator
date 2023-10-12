#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    public abstract partial record Type {
        // anonymous and empty: struct {}
        // regular: struct x { float pos; x other; }
        // Struct shape is a valid type and can be used in function parameter or variable declarations.
        [AstSyntax] public partial record Struct : Type {
            public StructKeyword            structKeyword { get; init; } = new();
            public        Identifier?              name { get; init; }
            public OpenBraceToken           openBrace { get; init; } = new();
            public SyntaxList<Hlsl, Member> members { get; init; } = new();
            public CloseBraceToken          closeBrace { get; init; } = new();

            [AstSyntax] public partial record Member : Syntax<Hlsl> {
                public        Token<Hlsl>?   interpolation { get; init; }
                public        Type           type { get; init; }
                public        Identifier     id { get; init; }
                public        Semantic?      semantic { get; init; }
                public SemicolonToken semicolon { get; init; } = new();
            }
        }
    }
}
