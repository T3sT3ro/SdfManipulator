#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax {
    public abstract partial record Type {
        // anonymous and empty: struct {}
        // regular: struct x { float pos; x other; }
        // Struct shape is a valid type and can be used in function parameter or variable declarations.  
        public record Struct : Type {
            public StructKeyword            structKeyword { get; init; } = new();
            public Identifier?              name          { get; init; }
            public OpenBraceToken           openBrace     { get; init; } = new();
            public SyntaxList<Hlsl, Member> members       { get; init; }
            public CloseBraceToken          closeBrace    { get; init; } = new();

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
                    { structKeyword, name, openBrace, members, closeBrace }
                .FilterNotNull().ToList();

            public record Member : Syntax<Hlsl> {
                public Token<Hlsl>? interpolation { get; init; }
                public Type         type          { get; init; }
                public Identifier   id            { get; init; }
                public Semantic?    semantic      { get; init; }
                public SemiToken    semicolon     { get; init; } = new();

                public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
                        { interpolation, type, id, semantic, semicolon }
                    .FilterNotNull().ToList();
            }
        }
    }
}
