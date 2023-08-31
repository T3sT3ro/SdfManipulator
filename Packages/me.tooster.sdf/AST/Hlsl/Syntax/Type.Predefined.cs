#nullable enable
using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax {
    public abstract partial record Type {
        /// scalars, vectors, matrices like int, float3 half4x4
        public partial record Predefined : Type {
            private readonly PredefinedTypeToken _typeToken;
            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new[]
                { typeToken };


            public static implicit operator Predefined(PredefinedTypeToken token) =>
                new() { typeToken = token };
        }
    }
}
