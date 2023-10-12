#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    public abstract partial record Type {
        /// scalars, vectors, matrices like int, float3 half4x4
        [AstSyntax] public partial record Predefined : Type {
            public PredefinedTypeToken typeToken { get; init; }

            public static implicit operator Predefined(PredefinedTypeToken token) => new() { typeToken = token };
        }
    }
}
