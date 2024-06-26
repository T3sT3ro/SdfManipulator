using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // [Gamma] Property ("_Gamma", Float) = 1
    /// <a href="https://docs.unity3d.com/2023.2/Documentation/Manual/SL-Properties.html">Shader</a>
    [SyntaxNode] public partial record Property {
        public SyntaxList<shaderlab, Attribute> attributes      { get; init; } = new();
        public IdentifierToken                  id              { get; init; }
        public OpenParenToken                   openParenToken  { get; init; } = new();
        public QuotedStringLiteral              displayName     { get; init; }
        public CommaToken                       commaToken      { get; init; } = new();
        public Type                             propertyType    { get; init; }
        public CloseParenToken                  closeParenToken { get; init; } = new();
        public Initializer                      initializer     { get; init; }
    }
}
