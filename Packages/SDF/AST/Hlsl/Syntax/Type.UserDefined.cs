#nullable enable
using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax {
    public abstract partial record Type {
        /// struct names or types introduced by typedef
        public partial record UserDefined : Type {
            private readonly Identifier _id;

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new[]
                { id };

            public static implicit operator UserDefined(string name) => new() { id = name };
        }
    }
}
