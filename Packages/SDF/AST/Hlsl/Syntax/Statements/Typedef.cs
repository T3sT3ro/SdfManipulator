using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public record Typedef : Statement {
        public TypedefKeyword typedefKeyword { get; init; } = new();
        public Type           type           { get; init; }
        public Identifier     id             { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { typedefKeyword, type, id };
    }
}
