using System.Collections.Generic;
using AST.Hlsl.Syntax.Expressions;
using AST.Syntax;

namespace AST.Hlsl.Syntax {
    public record Identifier : Expression {
        public IdentifierToken id { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new[]
            { id };

        public static implicit operator Identifier(string name) =>
            new() { id = new IdentifierToken { ValidatedText = name } };
    }
}
