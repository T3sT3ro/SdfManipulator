#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Hlsl.Syntax.Expressions;

namespace AST.Hlsl.Syntax.Statements.Declarations {
    public abstract partial record FunctionDeclaration { }

    public record Argument : HlslSyntax {
        public HlslToken?     modifier    { get; set; }
        public Type           type        { get; set; }
        public IdentifierName id          { get; set; }
        public Semantic?      semantic    { get; set; }
        public Expression?    initializer { get; set; }

        public override IReadOnlyList<HlslSyntax> ChildNodes =>
            (IReadOnlyList<HlslSyntax>)new HlslSyntax[] { type, id, semantic, initializer }
                .Where(x => x != null);

        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens =>
            (IReadOnlyList<HlslSyntaxOrToken>)new HlslSyntaxOrToken[] { modifier, type, id, semantic, initializer }
                .Where(x => x != null);
    }
}
