#nullable enable
using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax {
    public abstract partial record Type {
        /// struct names or types introduced by typedef
        public record UserDefined : Type {
            public Identifier id { get; init; }

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new[]
                { id };

            public static implicit operator UserDefined(string name) => new() { id = name };
        }
    }
}
