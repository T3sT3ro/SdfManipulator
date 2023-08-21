#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax {
    public abstract record Type : HlslSyntax {
        // scalars, vectors, matrices like int, float3 half4x4
        public record Predefined : Type {
            public PredefinedTypeToken typeToken { get; init; }

            public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new[] { typeToken };

            public static implicit operator Predefined(PredefinedTypeToken token) =>
                new Predefined { typeToken = token };
        }

        // structs or types introduced by typedef
        public record UserDefined : Type {
            public Identifier id { get; init; }

            public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new[] { id };

            public static implicit operator UserDefined(string name) => new UserDefined { id = name };
        }

        // anonymous and empty: struct {}
        // regular: struct x { float pos; x other; }
        // Struct shape is a valid type and can be used in function parameter or variable declarations.  
        public record Struct : Type {
            public StructKeyword         structKeyword { get; init; } = new();
            public Identifier?           name          { get; init; }
            public OpenBraceToken        openBrace     { get; init; } = new();
            public IReadOnlyList<Member> members       { get; init; }
            public CloseBraceToken       closeBrace    { get; init; } = new();

            public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new IHlslSyntaxOrToken?[]
                    { structKeyword, name, openBrace }
                .Concat(members)
                .Append(closeBrace)
                .FilterNotNull()
                .ToList();

            public record Member : HlslSyntax {
                public HlslToken? interpolation { get; init; }
                public Type       type          { get; init; }
                public Identifier id            { get; init; }
                public Semantic?  semantic      { get; init; }
                public SemiToken  semicolon     { get; init; } = new();

                public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new IHlslSyntaxOrToken?[]
                        { interpolation, type, id, semantic, semicolon }
                    .FilterNotNull().ToList();
            }
        }

        // syntax sugar
        public static implicit operator Type(PredefinedTypeToken token) => (Predefined)token;

        public static implicit operator Type(string name) => (UserDefined)name;
    }
}
