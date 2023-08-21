using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Statements {
    public record Typedef : Statement {
        public TypedefKeyword typedefKeyword { get; set; } = new();
        public Type           type           { get; set; }
        public Identifier id             { get; set; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new IHlslSyntaxOrToken[]
            { typedefKeyword, type, id };
    }
}
