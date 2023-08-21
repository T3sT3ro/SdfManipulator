using System.Collections.Generic;
using System.Linq;
using AST.Hlsl;
using AST.Syntax;

namespace AST.Shaderlab.Syntax.Shader {
    /// <a href="https://docs.unity3d.com/2023.2/Documentation/Manual/SL-Properties.html">Shader</a>
    public partial record Property : ShaderlabSyntax {
        public IReadOnlyList<Attribute> attributes      { get; set; }
        public IdentifierToken          id              { get; set; }
        public OpenParenToken           openParenToken  { get; set; } = new();
        public QuotedStringLiteral      displayName     { get; set; }
        public CommaToken               commaToken      { get; set; } = new();
        public PropertyType             propertyType    { get; set; }
        public CloseParenToken          closeParenToken { get; set; } = new();
        public Initializer              initializer     { get; set; }

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens =>
            attributes
                .AppendAll<IShaderlabSyntaxOrToken>(id, openParenToken, displayName, commaToken, propertyType, closeParenToken, initializer)
                .ToList();
    }
}
