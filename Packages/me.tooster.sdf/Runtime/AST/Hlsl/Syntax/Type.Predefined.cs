#nullable enable
using System;
namespace me.tooster.sdf.AST.Hlsl.Syntax {
    public abstract partial record Type {
        /// scalars, vectors, matrices like int, float3 half4x4
        [SyntaxNode] public partial record Predefined : Type {
            public PredefinedTypeToken typeToken { get; init; }

            public static implicit operator Predefined(PredefinedTypeToken token) => new() { typeToken = token };

            public static implicit operator Predefined(Constants.ScalarKind kind)
                => new()
                {
                    typeToken = kind switch
                    {
                        Constants.ScalarKind.@bool   => new BoolKeyword(),
                        Constants.ScalarKind.half    => new HalfKeyword(),
                        Constants.ScalarKind.@int    => new IntKeyword(),
                        Constants.ScalarKind.@uint   => new UintKeyword(),
                        Constants.ScalarKind.@float  => new FloatKeyword(),
                        Constants.ScalarKind.@fixed  => new FixedKeyword(),
                        Constants.ScalarKind.@double => new DoubleKeyword(),
                        _                            => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
                    },
                };
        }
    }
}
