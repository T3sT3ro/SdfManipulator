using System.Collections.Generic;

namespace AST.Shaderlab.Syntax.Shader {
    // INTEGER, FLOAT, TEXTURE_2D, TEXTURE_2D_ARRAY, TEXTURE_3D, CUBEMAP, CUBEMAP_ARRAY, COLOR, VECTOR
    public abstract record PropertyType : ShaderlabSyntax;
    
    public record PredefinedPropertyType : PropertyType {
        public TypeKeyword type { get; set; }

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new[] { type };
    }

    // Range(0.0, 1.0) is the same as Float in material properties
    public record RangePropertyType : PropertyType {
        public RangeKeyword    rangeKeyword    { get; set; } = new();
        public OpenParenToken  openParenToken  { get; set; } = new();
        public FloatLiteral    min             { get; set; }
        public CommaToken      commaToken      { get; set; } = new();
        public FloatLiteral    max             { get; set; }
        public CloseParenToken closeParenToken { get; set; } = new();

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new IShaderlabSyntaxOrToken[]
            { rangeKeyword, openParenToken, min, commaToken, max, closeParenToken };
    }
}
