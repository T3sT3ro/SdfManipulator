#nullable enable
namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // [Gamma]
    // [Header(header title)]
    // [Enum(One,1,SrcAlpha,5)]
    // [PowerSlider(3.0)]
    [SyntaxNode] public partial record Attribute {
        public OpenBracketToken              openBracketToken  { get; init; } = new();
        public AttributeToken                attributeToken    { get; init; }
        public ArgumentList<AttributeValue>? arguments         { get; init; }
        public CloseBracketToken             closeBracketToken { get; init; } = new();

        [SyntaxNode] public partial record AttributeValue {
            public AttributeStringLiteral value { get; init; }
        }
    }
}
