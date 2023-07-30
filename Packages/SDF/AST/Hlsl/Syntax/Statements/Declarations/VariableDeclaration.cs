#nullable enable
using System.Collections.Generic;
using AST.Hlsl.Syntax.Expressions;

namespace AST.Hlsl.Syntax.Statements.Declarations {
    public record VariableDeclaration : Statement {
        public HlslToken?     storage      { get; set; }
        public HlslToken?     typeModifier { get; set; }
        public Type           type         { get; set; }
        public IdentifierName id           { get; set; }
        public uint[]?        arraySizes   { get; set; }
        public Semantic?      semantic     { get; set; }
        public Expression?    initializer  { get; set; }


        public override IReadOnlyList<HlslSyntax> ChildNodes =>
            new HlslSyntax[] { type, id, semantic, initializer };

        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens =>
            new HlslSyntaxOrToken[] { storage, typeModifier, type, id, semantic, initializer };
    }
}
