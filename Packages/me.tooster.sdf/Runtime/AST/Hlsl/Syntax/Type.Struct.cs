#nullable enable
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    public abstract partial record Type {
        // anonymous and empty: struct {}
        // regular: struct x { float pos; x other; }
        // Struct shape is a valid type and can be used in function parameter or variable declarations.
        [SyntaxNode] public partial record Struct : Type {
            public StructKeyword            structKeyword { get; init; } = new();
            public Identifier?              name          { get; init; }
            public OpenBraceToken           openBrace     { get; init; } = new();
            public SyntaxList<hlsl, Member> members       { get; init; } = new();
            public CloseBraceToken          closeBrace    { get; init; } = new();

            [SyntaxNode] public partial record Member : Syntax<hlsl> {
                public Token<hlsl>?   interpolation { get; init; }
                public Type           type          { get; init; }
                public Identifier     id            { get; init; }
                public Semantic?      semantic      { get; init; }
                public SemicolonToken semicolon     { get; init; } = new();
            }
        }
    }
}
