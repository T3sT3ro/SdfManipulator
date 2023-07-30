using System.Collections.Generic;
using System.Linq;
using AST.Hlsl.Syntax.Expressions;

namespace AST.Hlsl.Syntax.Statements.Declarations {
    public record FunctionDeclaration : Statement {
        public Type                    returnType     { get; set; }
        public IdentifierName          id             { get; set; }
        public IReadOnlyList<Argument> arguments      { get; set; }
        public Semantic?               returnSemantic { get; set; }
        public Block                   body           { get; set; }

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

        public override IReadOnlyList<HlslSyntax> ChildNodes =>
            new HlslSyntax[] { returnType, id }
                .Concat(arguments)
                .Append(returnSemantic)
                .Append(body)
                .ToList();

        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens { get; }
    }
}
