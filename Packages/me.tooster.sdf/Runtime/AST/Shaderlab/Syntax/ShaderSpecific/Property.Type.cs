using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    public partial record Property {
        [SyntaxNode] public abstract partial record Type;

        [SyntaxNode] public partial record PredefinedType : Type {
            // Integer, Float, Texture2D, Texture2DArray, Texture3D, Cubemap, CubemapArray, Color, Vector
            public TypeKeyword type { get; init; }
        }

        // Range(0.0, 1.0) is the same as Float in material properties
        [SyntaxNode] public partial record RangeType : Type {
            public RangeKeyword    rangeKeyword    { get; init; } = new();
            public OpenParenToken  openParenToken  { get; init; } = new();
            public FloatLiteral    min             { get; init; }
            public CommaToken      commaToken      { get; init; } = new();
            public FloatLiteral    max             { get; init; }
            public CloseParenToken closeParenToken { get; init; } = new();
        }
    }
}
