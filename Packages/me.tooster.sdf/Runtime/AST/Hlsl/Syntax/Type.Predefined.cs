#nullable enable
namespace me.tooster.sdf.AST.Hlsl.Syntax {
    public abstract partial record Type {
        /// scalars, vectors, matrices like int, float3 half4x4
        [SyntaxNode] public partial record Predefined : Type {
            public PredefinedTypeToken typeToken { get; init; }

            public static implicit operator Predefined(PredefinedTypeToken token) => new() { typeToken = token };
        }
    }
}
