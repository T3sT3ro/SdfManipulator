#nullable enable
namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // [Gamma]
    // [Header(header title)]
    // [Enum(One,1,SrcAlpha,5)]
    // [PowerSlider(3.0)]
    // [AnyNameForCustomDrawer]
    [SyntaxNode] public partial record Attribute {
        public OpenBracketToken     openBracketToken  { get; init; } = new();
        public IdentifierToken      id                { get; init; }
        public ArgumentList<Value>? arguments         { get; init; }
        public CloseBracketToken    closeBracketToken { get; init; } = new();

        [SyntaxNode] public partial record Value {
            public AttributeStringLiteral value { get; init; }

            public static implicit operator Value(string literal) => new Value { value = literal };
        }
    }
}
