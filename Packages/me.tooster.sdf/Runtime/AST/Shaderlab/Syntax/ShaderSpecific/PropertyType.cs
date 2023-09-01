using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    public abstract record PropertyType : Syntax<Shaderlab>;
    
    public record PredefinedPropertyType : PropertyType {
        // Integer, Float, Texture2D, Texture2DArray, Texture3D, Cubemap, CubemapArray, Color, Vector
        public TypeKeyword type { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new[] { type };
    }

    // Range(0.0, 1.0) is the same as Float in material properties
    public record RangePropertyType : PropertyType {
        public RangeKeyword    rangeKeyword    { get; init; } = new();
        public OpenParenToken  openParenToken  { get; init; } = new();
        public FloatLiteral    min             { get; init; }
        public CommaToken      commaToken      { get; init; } = new();
        public FloatLiteral    max             { get; init; }
        public CloseParenToken closeParenToken { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
            { rangeKeyword, openParenToken, min, commaToken, max, closeParenToken };
    }
}
